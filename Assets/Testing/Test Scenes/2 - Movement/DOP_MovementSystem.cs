using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Test.Movement
{
    public partial struct MyRotationSpeedSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Common.Movement>();
            state.RequireForUpdate<MovementTEntity>();
            state.RequireForUpdate<MovementREntity>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            var speed = SystemAPI.GetSingleton<MovementTEntity>().speed;
            var speedDir = SystemAPI.GetSingleton<MovementTEntity>().direction;
            var rotationSpeed = SystemAPI.GetSingleton<MovementREntity>().rotationSpeed;
            
            foreach (var transform in 
                     SystemAPI.Query<RefRW<LocalTransform>>())
            {
                transform.ValueRW.Position += speedDir * speed * deltaTime; 
                transform.ValueRW.Rotation = math.mul(transform.ValueRW.Rotation, quaternion.RotateY(rotationSpeed * deltaTime));
            }
        }
    }
    
    /*
     [RequireMatchingQueriesForUpdate]
    public partial class DOP_MovementSystem : SystemBase
    {
        EntityQuery movementQuery;
        
        private float speed;
        private float3 direction;
        private float rotationSpeed;
        private float3 rotationDirection;

        [BurstCompile]
        protected override void OnCreate()
        {
            // This makes the system not update unless at least one entity exists that has the Spawner component.
            RequireForUpdate<Common.Movement>();
            RequireForUpdate<MovementTEntity>();
            RequireForUpdate<MovementREntity>();
            
            movementQuery = GetEntityQuery(
                ComponentType.ReadWrite<LocalTransform>(),
                ComponentType.ReadOnly<CreatedEntity>());
        }

        protected override void OnStartRunning()
        {
            // Store the variables we need for the system to update
            speed = SystemAPI.GetSingleton<MovementTEntity>().speed;
            direction = SystemAPI.GetSingleton<MovementTEntity>().direction;
        
            rotationSpeed = SystemAPI.GetSingleton<MovementREntity>().rotationSpeed;
            rotationDirection = SystemAPI.GetSingleton<MovementREntity>().rotationDirection;
        }

        [BurstCompile]
        protected override void OnUpdate()
        {
            
            speed = SystemAPI.GetSingleton<MovementTEntity>().speed;
            direction = SystemAPI.GetSingleton<MovementTEntity>().direction;
        
            rotationSpeed = SystemAPI.GetSingleton<MovementREntity>().rotationSpeed;
            rotationDirection = SystemAPI.GetSingleton<MovementREntity>().rotationDirection;
                       
            
            var deltaTime = SystemAPI.Time.DeltaTime;

            new MovementJob()
            {
                deltaTime = deltaTime,
                speed = speed,
                direction = direction,
                rotationSpeed = rotationSpeed,
                rotationDirection = rotationDirection
            }.ScheduleParallel(movementQuery);

        }

        // A job to handle the movement of the entities
        /*
         * var moveJob = new MovementJob();
            
            moveJob.transformType
                = GetComponentTypeHandle<LocalTransform>(true);

            moveJob.deltaTime = deltaTime;
            moveJob.speed = speed;
            moveJob.direction = direction;
            moveJob.rotationSpeed = rotationSpeed;
            moveJob.rotationDirection = rotationDirection;
            
            this.Dependency
                = moveJob.ScheduleParallel(query, this.Dependency);
            Debug.Log("Collected transformType");
         

    }
    
    partial struct MovementJob : IJobEntity
    {
        public float deltaTime;
        public float speed;
        public float3 direction;
        public float rotationSpeed;
        public float3 rotationDirection;

        [BurstCompile]
        public void Execute(ref LocalTransform sample, in CreatedEntity tag)
        {
            // Update the entity's LocalTransform component with the new position.
            sample.Position += direction * speed * deltaTime;
            sample.Rotation = math.mul(sample.Rotation, quaternion.RotateY(rotationSpeed * deltaTime));
        }
    }
     */
}