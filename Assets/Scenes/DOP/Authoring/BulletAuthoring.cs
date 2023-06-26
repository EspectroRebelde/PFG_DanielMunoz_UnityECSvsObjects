using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Scenes.DOP
{
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

    // A tag component for ball entities.
    public struct BulletTag : IComponentData
    {
    }

    // A 2d velocity vector for the ball entities.
    public struct Velocity : IComponentData
    {
        public float2 Value;
    }
    
    public struct Pierce : IComponentData
    {
        public int Value;
    }

}