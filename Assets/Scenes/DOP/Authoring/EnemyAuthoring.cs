using Scenes.DOP;
using Unity.Entities;
using UnityEngine;

namespace Scenes.DOP
{
    /// <summary>
    /// Authoring for an enemy
    /// Defines how an enemy is baked into an entity
    /// </summary>
    public class EnemyAuthoring : MonoBehaviour
    {
        class Baker : Baker<EnemyAuthoring>
        {
            public override void Bake(EnemyAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                
                AddComponent<EnemyTag>(entity);
                AddComponent<Health>(entity);
            }
        }
    }

    /// <summary>
    /// Tag component for an enemy
    /// </summary>
    public struct EnemyTag : IComponentData
    {
    }

    /// <summary>
    /// Health component for an enemy
    /// Defines how much health an enemy has
    /// </summary>
    public struct Health : IComponentData
    {
        public int Value;
    }

}