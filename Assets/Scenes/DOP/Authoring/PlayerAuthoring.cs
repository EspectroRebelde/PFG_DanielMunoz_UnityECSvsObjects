
using Unity.Entities;
using UnityEngine;

namespace Scenes.DOP
{
    public class PlayerAuthoring : MonoBehaviour
    {

        public class PlayerAuthoringBaker : Baker<PlayerAuthoring>
        {
            public override void Bake(PlayerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new PlayerTag());
            }
        }
    }

    public struct PlayerTag : IComponentData
    {
    }
}
