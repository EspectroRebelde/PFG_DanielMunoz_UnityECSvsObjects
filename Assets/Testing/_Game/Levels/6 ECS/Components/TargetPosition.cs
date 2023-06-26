using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class TargetPosition : MonoBehaviour
{
    public float3 Value;
}

public struct TargetPositionComponent : IComponentData
{
    public float3 Value;
}

public class TargetPositionBaker : Baker<TargetPosition>
{
    public override void Bake(TargetPosition authoring)
    {
        var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
        AddComponent<TargetPositionComponent>(entity, new TargetPositionComponent
        {
            Value = authoring.Value
        });
    }
}