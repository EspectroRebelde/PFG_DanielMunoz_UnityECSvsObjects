using Unity.Entities;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject PlayerPrefab;
}

public struct PlayerSpawnerComponent : IComponentData
{
    public Entity PlayerPrefab;
}


public class PlayerSpawnerBaker : Baker<PlayerSpawner>
{
    public override void Bake(PlayerSpawner authoring)
    {
        var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
        AddComponent<PlayerSpawnerComponent>(entity, new PlayerSpawnerComponent
        {
            PlayerPrefab = GetEntity(authoring.PlayerPrefab, TransformUsageFlags.Dynamic)
        });
        
    }
}