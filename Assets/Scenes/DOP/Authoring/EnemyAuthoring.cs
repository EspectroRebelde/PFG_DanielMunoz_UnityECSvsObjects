using Scenes.DOP;
using Unity.Entities;
using UnityEngine;

namespace Scenes.DOP
{
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

    public struct EnemyTag : IComponentData
    {
    }

    public struct Health : IComponentData
    {
        public int Value;
    }

}