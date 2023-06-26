using Unity.Entities;
using Unity.Transforms;

public readonly partial struct MoveToPositionAspect : IAspect
{
    private readonly RefRW<LocalTransform> _transformAspect;
    private readonly RefRO<TargetPositionComponent> _targetPosition;

    public void Move(float time)
    {
        var (pos, rot) = _transformAspect.ValueRO.Position.CalculatePosBurst(_targetPosition.ValueRO.Value.y, time);

        _transformAspect.ValueRW.Rotation = rot;
        _transformAspect.ValueRW.Position = pos;
    }
}