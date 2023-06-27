using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Scenes.DOP
{
    /// <summary>
    /// Authoring for a bullet
    /// Defines how a bullet is baked into an entity
    /// </summary>
    public class BulletAuthoring : MonoBehaviour
    {
        public int pierceCount = 1;
        
        class Baker : Baker<BulletAuthoring>
        {
            public override void Bake(BulletAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                
                AddComponent<BulletTag>(entity);
                AddComponent<Velocity>(entity);
                AddComponent<Pierce>(entity, new Pierce {Value = authoring.pierceCount});
            }
        }
    }
    
    /// <summary>
    /// Tag component for a bullet
    /// </summary>
    // A tag component for ball entities.
    public struct BulletTag : IComponentData
    {
    }

    /// <summary>
    /// Component for a bullet
    /// Defines the velocity of a bullet
    /// </summary>
    // A 2d velocity vector for the ball entities.
    public struct Velocity : IComponentData
    {
        public float2 Value;
    }
    
    /// <summary>
    /// Component for a bullet
    /// Defines how many enemies a bullet can pierce
    /// </summary>
    public struct Pierce : IComponentData
    {
        public int Value;
    }

}