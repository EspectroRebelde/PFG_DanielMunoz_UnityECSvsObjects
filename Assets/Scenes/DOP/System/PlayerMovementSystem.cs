using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Scenes.DOP
{
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct PlayerMovementSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerConfig>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        
            var config = SystemAPI.GetSingleton<PlayerConfig>();

            var horizontal = Input.GetAxis("Horizontal") + Input.GetAxis("HorizontalGamepad");
            horizontal = math.clamp(horizontal, -1, 1);
            var vertical = Input.GetAxis("Vertical") + Input.GetAxis("VerticalGamepad");
            vertical = math.clamp(vertical, -1, 1);
            
            var input = new float3(horizontal, 0, vertical) * SystemAPI.Time.DeltaTime * config.NormalSpeed;
            var rotationInput = (Input.GetKey(KeyCode.Q) ? -1 : Input.GetKey(KeyCode.E) ? 1 : 0) * SystemAPI.Time.DeltaTime * config.RotationSpeed;

            // Only move if the user has directional input.
            if (input.Equals(float3.zero) && rotationInput.Equals(0))
            {
                return;
            }

            foreach (var playerTransform in
                     SystemAPI.Query<RefRW<LocalTransform>>()
                         .WithAll<PlayerTag>())
            {
                playerTransform.ValueRW.Position = playerTransform.ValueRO.Position + input;
                playerTransform.ValueRW.Rotation = math.mul(playerTransform.ValueRO.Rotation, quaternion.RotateZ(math.radians(rotationInput)));
            }


        }
    }
}