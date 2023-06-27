using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Scenes.DOP
{
    /// <summary>
    /// Handles the movement of enemies.
    /// Checks for collisions with bullets.
    /// Manages health.
    /// </summary>
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct EnemyMovementSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EnemyConfig>();
            state.RequireForUpdate<BulletConfig>();
            state.RequireForUpdate<EnemyTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<EnemyConfig>();
            var bulletConfig = SystemAPI.GetSingleton<BulletConfig>();
            var enemyEntities = SystemAPI.QueryBuilder().WithAll<EnemyTag, LocalTransform, Health>().Build();
            
            var enemyQuery = SystemAPI.QueryBuilder().WithAll<LocalTransform, PlayerTag>().Build();
            var obstacleQuery = SystemAPI.QueryBuilder().WithAll<LocalTransform, BulletTag, Pierce>().Build();

            var minDist = bulletConfig.ObstacleRadius + bulletConfig.Radius;
            
            var movementJob = new EnemyMovementJob
            {
                BulletTransforms = obstacleQuery.ToComponentDataArray<LocalTransform>(state.WorldUpdateAllocator),
                BulletPierces = obstacleQuery.ToComponentDataArray<Pierce>(state.WorldUpdateAllocator),
                MinDistToObstacleSQ = minDist * minDist,
                Damage = bulletConfig.Damage,
                
                PlayerTransforms = enemyQuery.ToComponentDataArray<LocalTransform>(state.WorldUpdateAllocator),
                DeltaTime = SystemAPI.Time.DeltaTime,
                Speed = config.NormalSpeed
            };
            
            JobHandle handler = movementJob.ScheduleParallel(enemyEntities, state.Dependency);
            
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var healthJob = new EnemyHealthCheck()
            {
                ECB = ecb.AsParallelWriter()
            };
            
            state.Dependency.Complete();
            state.Dependency = healthJob.ScheduleParallel(enemyEntities, handler);
        }
    }

    /// <summary>
    /// Moves the enemy.
    /// Checks for collisions with bullets.
    /// <value>LocalTransform, health, enemyTag</value>
    /// </summary>
    // The implicit query of this IJobEntity matches all entities having LocalTransform, Velocity, and Ball components.
    [WithAll(typeof(EnemyTag))]
    [BurstCompile]
    public partial struct EnemyMovementJob : IJobEntity
    {
        [ReadOnly] public NativeArray<LocalTransform> BulletTransforms;
        public NativeArray<Pierce> BulletPierces;
        public float MinDistToObstacleSQ;
        public int Damage;
        
        [ReadOnly] public NativeArray<LocalTransform> PlayerTransforms;
        public float DeltaTime;
        public float Speed;

        public void Execute(ref LocalTransform transform, ref Health health)
        {
            if (Speed == 0)
            {
                return;
            }

            // Get the player position
            var playerPosition = PlayerTransforms[0].Position;
            
            EnemyMovement(ref transform, playerPosition);
            
            // Check if the enemy is colliding with a bullet
            for (var index = 0; index < BulletTransforms.Length; index++)
            {
                var bulletTransform = BulletTransforms[index];
                var distance = math.lengthsq(bulletTransform.Position - transform.Position);
                if (distance < MinDistToObstacleSQ)
                {
                    health.Value -= Damage;
                }
            }

        }

        private void EnemyMovement(ref LocalTransform transform, float3 playerPosition)
        {
            // Get the direction to the player
            var direction = (playerPosition - transform.Position);
            
            // Get the distance to the player
            var distance = math.length(direction);
            
            // Normalize the direction
            direction /= distance;
            
            // Get the new position
            var newPosition = transform.Position + direction * Speed * DeltaTime;
            
            // Lerp between the current position and the new position
            transform.Position = math.lerp(transform.Position, newPosition, 0.25f);
            
            // Rotate towards the player
            var rot = quaternion.LookRotation(direction, math.up());
            
            // The enemy is now on the floor facing the player so we need to rotate it 90 degrees
            rot = math.mul(rot, quaternion.RotateX(math.radians(-90)));
            
            // Lerp between the current rotation and the new rotation
            transform.Rotation = math.slerp(transform.Rotation, rot, 0.025f);
        }
    }
    
    /// <summary>
    /// Checks if the enemy's health is below 0.
    /// <value>Entity, health, enemyTag</value>
    /// </summary>
    [WithAll(typeof(EnemyTag))]
    [BurstCompile]
    public partial struct EnemyHealthCheck : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ECB;
        public void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, ref Health health)
        {
            if (health.Value <= 0)
            {
                ECB.DestroyEntity(chunkIndex, entity);
            }
        }
    }
}