using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Scenes.DOP
{
    // public class DestroyWhenOffCamera : SystemBase
    // {
    //     protected override void OnUpdate()
    //     {
    //         // When you destroy an entity, it creates a "sync point", as it's considered a "structural change".
    //         // Thus, we can do this job in a multithreaded way by instead adding these entities to a list, and then destroying them "later".
    //         // An EntityCommandBuffer is essentially a list of operations to apply later.
    //         // In this case, we use the "EndSimulation" ECB. I.e. At the end of the simulation step of this frame.
    //
    //         var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
    //         var ecb = ecbSingleton.CreateCommandBuffer(World.DefaultGameObjectInjectionWorld.Unmanaged);
    //
    //         var camera = Camera.main;
    //
    //         var obstacleQuery = SystemAPI.QueryBuilder().WithAll<LocalTransform, BulletTag>().Build();
    //         
    //         var job = new HandleOutsideCamera
    //         {
    //             ECB = ecb.AsParallelWriter(),
    //             CameraPos = camera.transform.position,
    //             CameraSize = camera.orthographicSize,
    //             CameraAspect = camera.aspect
    //         };
    //         
    //         Dependency = job.ScheduleParallel(obstacleQuery, Dependency);
    //
    //     }
    //
    // }
    
    public partial struct DestroyOffCamera : ISystemStartStop, ISystem
    {
        private float4x4 worldToCameraMatrix;
        private float4x4 projectionMatrix;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BulletConfig>();
        }
        
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var job = new HandleOutsideCamera
            {
                ECB = ecb.AsParallelWriter(),
                ViewportPos = worldToCameraMatrix,
                ProjPos = projectionMatrix
            };
            
            job.ScheduleParallel();
        }

        public void OnStartRunning(ref SystemState state)
        {
            var camera = Camera.main;
            worldToCameraMatrix = camera.worldToCameraMatrix;
            projectionMatrix = camera.projectionMatrix;
        }

        public void OnStopRunning(ref SystemState state)
        {
        }
    }
    
    [WithAll(typeof(BulletTag))]
    [BurstCompile]
    public partial struct HandleOutsideCamera : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ECB;
        public float4x4 ViewportPos;
        public float4x4 ProjPos;
        
        public void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, [ReadOnly] in LocalTransform transform)
        {
            // Calculate view space position
            float4 viewPos = math.mul(ViewportPos, new float4(transform.Position, 1));
            // Convert to projection space
            float4 projPos = math.mul(ProjPos, viewPos);
            // Calculate NDC (Normalized Device Coordinates)
            float3 ndcPos = new float3(projPos.x / projPos.w, projPos.y / projPos.w, projPos.z / projPos.w);
            // Convert to viewport space
            float3 viewportPos = new float3(ndcPos.x * 0.5f + 0.5f, ndcPos.y * 0.5f + 0.5f, -viewPos.z);
            
            if (viewportPos.x < -0.01 || viewportPos.x > 1.01 || viewportPos.y < -0.01 || viewportPos.y > 1.01)
            {
                ECB.DestroyEntity(chunkIndex, entity);
            }
        }
    }

}