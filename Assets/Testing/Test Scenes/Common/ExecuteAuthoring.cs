using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Test.Common
{
    public class ExecuteAuthoring : MonoBehaviour
    {
        public bool Creation;
        public bool Movement;
        public bool Controller;
        public bool FullController;
        public bool IJobChunk;
        public bool Reparenting;
        public bool EnableableComponents;
        public bool GameObjectSync;

        class Baker : Baker<ExecuteAuthoring>
        {
            public override void Bake(ExecuteAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                if (authoring.Creation) AddComponent<Creation>(entity);
                if (authoring.Movement) AddComponent<Movement>(entity);
                if (authoring.Controller) AddComponent<Controller>(entity);
                if (authoring.FullController) AddComponent<FullController>(entity);
                if (authoring.IJobChunk) AddComponent<IJobChunk>(entity);
                if (authoring.GameObjectSync) AddComponent<GameObjectSync>(entity);
                if (authoring.Reparenting) AddComponent<Reparenting>(entity);
                if (authoring.EnableableComponents) AddComponent<EnableableComponents>(entity);
            }
        }
    }

    public struct Creation : IComponentData
    {
    }

    public struct Movement : IComponentData
    {
    }

    public struct Controller : IComponentData
    {
    }

    public struct FullController : IComponentData
    {
    }

    public struct IJobChunk : IComponentData
    {
    }

    public struct GameObjectSync : IComponentData
    {
    }

    public struct Reparenting : IComponentData
    {
    }

    public struct EnableableComponents : IComponentData
    {
    }
}