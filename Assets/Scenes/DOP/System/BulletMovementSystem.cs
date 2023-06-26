using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


namespace Scenes.DOP
{
    [UpdateAfter(typeof(EnemyMovementSystem))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct BulletMovementsystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BulletConfig>();
            state.RequireForUpdate<BulletTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var bulletConfig = SystemAPI.GetSingleton<BulletConfig>();
            var enemyQuery = SystemAPI.QueryBuilder().WithAll<LocalTransform, EnemyTag>().Build();
            
            var job = new BulletMovementJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
            };
            
            JobHandle handler = job.ScheduleParallel(state.Dependency);
            
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            var minDist = bulletConfig.ObstacleRadius + bulletConfig.Radius;
            
            handler.Complete();
            var pierceJob = new BulletPierceCheck()
            {
                ECB = ecb.AsParallelWriter(),
                BulletTransforms = enemyQuery.ToComponentDataArray<LocalTransform>(state.WorldUpdateAllocator),
                MinDistToObstacleSQ = minDist * minDist,
            };
            
            state.Dependency = pierceJob.ScheduleParallel(handler);
        }
    }

    // The implicit query of this IJobEntity matches all entities having LocalTransform, Velocity, and Ball components.
    [WithAll(typeof(BulletTag))]
    [BurstCompile]
    public partial struct BulletMovementJob : IJobEntity
    {
        public float DeltaTime;
        public void Execute(ref LocalTransform transform, ref Velocity speed)
        {
            if (speed.Value.Equals(float2.zero))
            {
                return;
            }
            
            transform.Position += new float3(speed.Value.x, 0, speed.Value.y) * DeltaTime;
        }
    }
    
    [WithAll(typeof(BulletTag))]
    [BurstCompile]
    public partial struct BulletPierceCheck : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ECB;
        [ReadOnly] public NativeArray<LocalTransform> BulletTransforms;
        public float MinDistToObstacleSQ;
        public void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, in LocalTransform transform, ref Pierce pierce)
        {
            // Check if the enemy is colliding with a bullet
            foreach (var bulletTransform in BulletTransforms)
            {
                var distSQ = math.distancesq(bulletTransform.Position, transform.Position);
                if (distSQ < MinDistToObstacleSQ)
                {
                    pierce.Value -= 1;
                }
            }

            if (pierce.Value <= 0)
            {
                ECB.DestroyEntity(chunkIndex, entity);
            }
        }
    }
}