using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Scenes.DOP
{
    /// <summary>
    /// Handles the spawning of balls.
    /// <value>LocalTransform, velocity</value>
    /// </summary>
    // This UpdateBefore is necessary to ensure the balls get rendered in
    // the correct position for the frame in which they're spawned.
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct BallSpawnerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BulletConfig>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<BulletConfig>();

            if (config.ContinuousFiring)
            {
                if (!Input.GetMouseButton(0))
                {
                    return;
                }
            }
            else
            {
                if (!Input.GetMouseButtonDown(0))
                {
                    return;
                }
            }

            // For every player, spawn a ball, position it at the player's location, and give it a random velocity.
            foreach (var transform in
                     SystemAPI.Query<RefRO<LocalTransform>>()
                         .WithAll<PlayerTag>())
            {
                var amount = config.WaveAmount;
                NativeArray <float> angles = new NativeArray<float>(amount, Allocator.Temp);
                if (amount <= 1)
                {
                    angles[0] = 0;
                }
                else
                {
                    angles[0] = config.WaveArc.x;
                    angles[amount - 1] = config.WaveArc.y;
                    
                    int fullRange = config.WaveArc.y - config.WaveArc.x;
                    for (int i = 1; i < amount - 1; i++)
                    {
                        angles[i] = config.WaveArc.x + (fullRange / (amount - 1f)) * i;
                    }
                }
                
                for (int i = 0; i < amount; i++)
                {
                    // Create a ball entity from the prefab.
                    var ball = state.EntityManager.Instantiate(config.BulletPrefab);
                    
                    // Set the ball's initial position to the player's position.
                    state.EntityManager.SetComponentData(ball, new LocalTransform
                    {
                        Position = transform.ValueRO.Position + new float3(0,config.Radius/2,0),
                        Rotation = quaternion.identity,
                        Scale = config.Radius
                    });
                    
                    // Get the forward rotation of the player and make the ball go in that direction.
                    // The original rot of the player is (-90,0,0) and it rotates in the xz plane.
                    // The ball should use the same rotation as the player.
                    var velocity = math.mul(transform.ValueRO.Rotation, new float3(0, 1, 0)) * config.BulletStartVelocity;

                    // Rotate the velocity by the angle of the wave.
                    var velocity2 = math.mul(quaternion.RotateY(math.radians(angles[i])), velocity).xz;
                    
                    // Give the ball a random velocity.
                    state.EntityManager.SetComponentData(ball, new Velocity
                    {
                        Value = velocity2
                    });
                }
                
            }
        }
    }
}