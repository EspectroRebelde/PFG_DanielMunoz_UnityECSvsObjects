using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Test.Creation
{
    public partial struct DOP_SpawnSystem : ISystem
    {
        uint m_UpdateCounter;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            // This makes the system not update unless at least one entity exists that has the Spawner component.
            state.RequireForUpdate<SpawnerEntity>();
            state.RequireForUpdate<Common.Creation>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Only spawn cubes when no cubes currently exist.
            var prefab = SystemAPI.GetSingleton<SpawnerEntity>().Prefab;
            var amount = SystemAPI.GetSingleton<SpawnerEntity>().amount;

            // Instantiating an entity creates copy entities with the same component types and values.
            var instances = state.EntityManager.Instantiate(prefab, amount, Allocator.Temp);

            // Unlike new Random(), CreateFromIndex() hashes the random seed
            // so that similar seeds don't produce similar results.
            var random = Random.CreateFromIndex(m_UpdateCounter++);
            foreach (var entity in instances)
            {
                // Update the entity's LocalTransform component with the new position.
                var transform = SystemAPI.GetComponentRW<LocalTransform>(entity);
                if (SystemAPI.GetSingleton<SpawnerEntity>().randomizePosition)
                {
                    // Change the given position up to +- 3 units in each direction.
                    var randomPos = new float3(random.NextFloat(-3, 3), random.NextFloat(-3, 3), random.NextFloat(-3, 3));
                    transform.ValueRW.Position += randomPos;
                }
                else
                {
                    transform.ValueRW.Position = SystemAPI.GetSingleton<SpawnerEntity>().position;
                }

                transform.ValueRW.Rotation = SystemAPI.GetSingleton<SpawnerEntity>().rotation;
            }
            
            // Dispose the array of entities to avoid memory leaks.
            instances.Dispose();
            
            // Disable the system so it doesn't run again.
            state.Enabled = false;
        }
    }
}