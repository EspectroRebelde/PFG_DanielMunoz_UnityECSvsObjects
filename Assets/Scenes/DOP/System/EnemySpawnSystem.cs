using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Serialization;
using Random = Unity.Mathematics.Random;


namespace Scenes.DOP
{
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct EnemySpawnSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EnemyConfig>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<EnemyConfig>();

            if (!Input.GetKeyDown(KeyCode.Space))
            {
                return;
            }
            
            #if true
                // higher-level API
                // This "foreach query" is transformed by source-gen into code resembling the #else below.
                // For every entity having a LocalTransform and PlayerTag component, a read-only reference to
                // the LocalTransform is assigned to 'obstacleTransform'.
                foreach (var obstacleTransform in
                         SystemAPI.Query<RefRO<LocalTransform>>().
                             WithAll<PlayerTag>())
                {

                    var rand = new Random((uint)UnityEngine.Random.Range(0, int.MaxValue));

                    #if false // this is the basic OOP way of doing things
                        for (int i = 0; i < config.WaveAmount; i++)
                        {
                            // Create a enemy entity from the prefab.
                            var enemy = state.EntityManager.Instantiate(config.EnemyPrefab);
                            
                            // Get a random value
                            // We want to spawn the enemy in a circle around the player.
                            // We can use the random value to get a random x
                            // Then we calculate the corresponding z value to be on the circle.
                            var randomVal = rand.NextFloat(0, 1);
                            var xRadius = math.cos(randomVal * math.PI * 2) * rand.NextFloat(config.MinMaxRadiusToSpawn.x, config.MinMaxRadiusToSpawn.y);
                            var zRadius = math.sin(randomVal * math.PI * 2) * rand.NextFloat(config.MinMaxRadiusToSpawn.x, config.MinMaxRadiusToSpawn.y);
                            
                            quaternion rotation = quaternion.LookRotationSafe(obstacleTransform.ValueRO.Position - new float3(xRadius, 30, zRadius), new float3(0, 1, 0));
                            rotation = math.mul(rotation, quaternion.RotateX(math.radians(-90)));
                            
                            state.EntityManager.SetComponentData(enemy, new LocalTransform
                            {
                                Position = new float3
                                {
                                    x = obstacleTransform.ValueRO.Position.x + xRadius,
                                    y = obstacleTransform.ValueRO.Position.y,
                                    z = obstacleTransform.ValueRO.Position.z + zRadius
                                },
                                Scale = 1,  // If we didn't set Scale and Rotation, they would default to zero (which is bad!)
                                // We have to face the enemy towards the player (obstacle)
                                // We need to offset on the x axis by -90 degrees
                                Rotation = rotation
                            });
                            
                            state.EntityManager.SetComponentData(enemy, new Health
                            {
                                Value = config.EnemyHealth
                            });
                        }
                    
                    #else // this is the councious ECS way of doing things
                    var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
                    var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
                    
                    // state.EntityManager.SetComponentData(config.EnemyPrefab, new LocalTransform
                    // {
                    //     Position = new float3
                    //     {
                    //         x = -100,
                    //         y = 1,
                    //         z = -100
                    //     },
                    //     Scale = 1,
                    //     Rotation = quaternion.identity
                    // });
                    
                    var instances = new NativeArray<Entity>(config.WaveAmount, Allocator.TempJob);
                    state.EntityManager.Instantiate(config.EnemyPrefab, instances);

                    var job = new EnemySpawnJob
                    {
                        Instances = instances,
                        EnemyHealth = config.EnemyHealth,
                        MinMaxRadiusToSpawn = config.MinMaxRadiusToSpawn,
                        rand = rand,
                        PlayerTransform = obstacleTransform.ValueRO,
                        ECB = ecb.AsParallelWriter()
                    };
                    
                    var handle = job.Schedule(config.WaveAmount, 1);
                    
                    handle.Complete();
                    
                    instances.Dispose();
                    #endif
                }
#else
                // lower-level API
                // Get a query that matches all entities which have both a LocalTransform and Obstacle component.
                var query = SystemAPI.QueryBuilder().WithAll<LocalTransform, Obstacle>().Build();

                // Type handles are needed to access component data arrays from chunks.
                var localTransformTypeHandle = SystemAPI.GetComponentTypeHandle<LocalTransform>(true);

                // Perform the query: returns all the chunks with entities matching the query.
                var chunks = query.ToArchetypeChunkArray(Allocator.Temp);
                foreach (var chunk in chunks)
                {
                    // Use the LocalTransform type handle to get the LocalTransform component data array from the chunk.
                    // Be clear that this is not a copy! This is the actual component array stored in the chunk, so
                    // modifying its contents directly modifies the LocalTransform values of the entities.
                    // Because the array belongs to the chunk, you need not (and should not) dispose it.
                    var localTransforms = chunk.GetNativeArray(ref localTransformTypeHandle);

                    // Iterate through every entity in the chunk.
                    for (int i = 0; i < chunk.Count; i++)
                    {
                        // Directly read the component value from the component data array.
                        var obstacleTransform = localTransforms[i];

                        // Same player instantiation code as above.
                        var player = state.EntityManager.Instantiate(config.PlayerPrefab);
                        state.EntityManager.SetComponentData(player, new LocalTransform
                        {
                            Position = new float3
                            {
                                x = obstacleTransform.Position.x + config.PlayerOffset,
                                y = 1,
                                z = obstacleTransform.Position.z + config.PlayerOffset
                            },
                            Scale = 1,
                            Rotation = quaternion.identity
                        });
                    }
                }
            #endif
        }
    }
    
    // Job that instantiates an enemy entity 
    // Will be called by the EnemySpawnSystem for each enemy to spawn
    [BurstCompile]
    public struct EnemySpawnJob : IJobParallelFor
    {
        public NativeArray<Entity> Instances;
        public EntityCommandBuffer.ParallelWriter ECB;
        public LocalTransform PlayerTransform;
        public Random rand;
        public float2 MinMaxRadiusToSpawn;
        public int EnemyHealth;
        
        public void Execute(int index)
        {
            // Sets the local transform of the entity
            // We want to spawn the enemy in a circle around the player.
            // We can use the random value to get a random x
            // Then we calculate the corresponding z value to be on the circle.
            var randomVal = rand.NextFloat(0, 1);
            var xRadius = math.cos(randomVal * math.PI * 2) * rand.NextFloat(MinMaxRadiusToSpawn.x, MinMaxRadiusToSpawn.y);
            var zRadius = math.sin(randomVal * math.PI * 2) * rand.NextFloat(MinMaxRadiusToSpawn.x, MinMaxRadiusToSpawn.y);
            
            quaternion rotation = quaternion.LookRotationSafe(PlayerTransform.Position - new float3(xRadius, 30, zRadius), new float3(0, 1, 0));
            rotation = math.mul(rotation, quaternion.RotateX(math.radians(-90)));
            
            ECB.SetComponent(index, Instances[index], new LocalTransform
            {
                Position = new float3
                {
                    x = PlayerTransform.Position.x + xRadius,
                    y = PlayerTransform.Position.y,
                    z = PlayerTransform.Position.z + zRadius
                },
                Scale = 1,  // If we didn't set Scale and Rotation, they would default to zero (which is bad!)
                // We have to face the enemy towards the player (obstacle)
                // We need to offset on the x axis by -90 degrees
                Rotation = rotation
            });
            
            ECB.SetComponent(index, Instances[index], new Health
            {
                Value = EnemyHealth
            });
        }
    }
}