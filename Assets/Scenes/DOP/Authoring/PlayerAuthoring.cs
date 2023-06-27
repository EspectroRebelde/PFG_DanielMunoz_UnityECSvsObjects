
using Unity.Entities;
using UnityEngine;

namespace Scenes.DOP
{
    /// <summary>
    /// Authoring for a player
    /// Defines how a player is baked into an entity
    /// </summary>
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

    /// <summary>
    /// Tag component for a player
    /// </summary>
    public struct PlayerTag : IComponentData
    {
    }
}
