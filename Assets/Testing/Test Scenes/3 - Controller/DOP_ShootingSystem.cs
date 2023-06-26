using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Test.Controller
{
    [BurstCompile]
    // ISystem and ISystemStartStop
    public partial struct DOP_ShootingSystem : ISystem, ISystemStartStop
    {
        private EntityQuery controllerQuery;
        
        private float speed;
        
        // Key bindings for movement
        private KeyCode forwardKey;
        private KeyCode backwardKey;
        private KeyCode leftKey;
        private KeyCode rightKey;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            // This makes the system not update unless at least one entity exists that has the Spawner component.
            state.RequireForUpdate<Common.Controller>();
            state.RequireForUpdate<MovementComponentData>();
            state.RequireForUpdate<BindingsComponentData>();
            
        }

        [BurstCompile]
        void ISystemStartStop.OnStartRunning(ref SystemState state)
        {
            // Store the variables we need for the system to update
            speed = SystemAPI.GetSingleton<MovementComponentData>().speed;

            forwardKey = SystemAPI.GetSingleton<BindingsComponentData>().Forward;
            backwardKey = SystemAPI.GetSingleton<BindingsComponentData>().Backward;
            leftKey = SystemAPI.GetSingleton<BindingsComponentData>().Left;
            rightKey = SystemAPI.GetSingleton<BindingsComponentData>().Right;
            
        }
        

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;

            // Check if the keys are pressed 
            var movementFlags = MovementFlags.None;
            if (Input.GetKey(forwardKey))
            {
                movementFlags |= MovementFlags.Forward;
            }
            if (Input.GetKey(backwardKey))
            {
                movementFlags |= MovementFlags.Backward;
            }
            if (Input.GetKey(leftKey))
            {
                movementFlags |= MovementFlags.Left;
            }
            if (Input.GetKey(rightKey))
            {
                movementFlags |= MovementFlags.Right;
            }
            
            // Schedule the job
            state.Dependency = new ControllerMoveJob
            {
                deltaTime = deltaTime,
                speed = speed,
                direction = movementFlags
            }.ScheduleParallel(state.Dependency);
            
        }
        
        [BurstCompile]
        void ISystemStartStop.OnStopRunning(ref SystemState state)
        {
        }
    }

    [BurstCompile]
    internal partial struct ControllerMoveJob : IJobEntity
    {
        public float deltaTime;
        public float speed;
        
        public MovementFlags direction;

        [BurstCompile]
        public void Execute(ref LocalTransform transform, in ControlledEntity tag)
        {
            if ((direction & MovementFlags.IsDiagonal) == MovementFlags.IsDiagonal)
            {
                // Diagonal movement (|_ is speed*deltatime but diagonal will be speed*deltatime*0.707)
                switch (direction)
                {
                    case MovementFlags.ForwardLeft:
                        transform.Position += new float3(-speed * deltaTime * 0.707f, 0, speed * deltaTime * 0.707f);
                        break;
                    case MovementFlags.BackwardLeft:
                        transform.Position += new float3(-speed * deltaTime * 0.707f, 0, -speed * deltaTime * 0.707f);
                        break;
                    case MovementFlags.ForwardRight:
                        transform.Position += new float3(speed * deltaTime * 0.707f, 0, speed * deltaTime * 0.707f);
                        break;
                    case MovementFlags.BackwardRight:
                        transform.Position += new float3(speed * deltaTime * 0.707f, 0, -speed * deltaTime * 0.707f);
                        break;
                }
            }
            else
            {
                // Normal movement (|_ is speed*deltatime)
                switch (direction)
                {
                    case MovementFlags.Forward:
                        transform.Position += new float3(0, 0, speed * deltaTime);
                        break;
                    case MovementFlags.Backward:
                        transform.Position += new float3(0, 0, -speed * deltaTime);
                        break;
                    case MovementFlags.Left:
                        transform.Position += new float3(-speed * deltaTime, 0, 0);
                        break;
                    case MovementFlags.Right:
                        transform.Position += new float3(speed * deltaTime, 0, 0);
                        break;
                }
            }
        }
    }
    
    // 4 bits as flags for movement
    // 0000 = None
    // 0100 = Forward
    // 0101 = Backward
    // 1000 = Left
    // 1010 = Right
    // 1100 = Forward + Left
    // 1101 = Backward + Left
    // 1110 = Forward + Right
    // 1111 = Backward + Right
    [Flags]
    public enum MovementFlags
    {
        None = 0b0000,
        Forward = 0b0100,
        Backward = 0b0101,
        Left = 0b1000,
        Right = 0b1010,
        ForwardLeft = MovementFlags.Forward | MovementFlags.Left,
        BackwardLeft = MovementFlags.Backward | MovementFlags.Left,
        ForwardRight = MovementFlags.Forward | MovementFlags.Right,
        BackwardRight = MovementFlags.Backward | MovementFlags.Right,
            
        // Diagonal check (11**) with * = 0 or 1
        IsDiagonal = 0b1100,
    }
}