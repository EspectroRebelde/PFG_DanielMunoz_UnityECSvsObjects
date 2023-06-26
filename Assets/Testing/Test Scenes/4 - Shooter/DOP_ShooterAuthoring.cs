using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Transforms;

using Sirenix.OdinInspector;

// An authoring component is just a normal MonoBehavior that has a Baker<T> class.

namespace Test.Shooter
{
    /// <summary>
    /// Creates a new object from a prefab given in Data Oriented Programming (ECS style)
    /// This will be the same as the OOP_Creation script, but with ECS
    /// It implements an auth
    /// </summary>
    public class DOP_ShooterAuthoring : MonoBehaviour
    {
        [FoldoutGroup("Creation")]
        public GameObject Prefab;
        [FoldoutGroup("Creation")]
        public float3 position = new float3(0, 0, 0);
        [FoldoutGroup("Creation")]
        public quaternion rotation = quaternion.identity;

        [FoldoutGroup("Creation")]
        public int amount = 1;
        [FoldoutGroup("Creation")]
        public bool randomizePosition = false;
        
        [BoxGroup("2D-Controller")]
        public float speed = 1f;
        [BoxGroup("2D-Controller")]
        
        public KeyCode forward = KeyCode.W;
        [BoxGroup("2D-Controller")]
        public KeyCode backward = KeyCode.S;
        [BoxGroup("2D-Controller")]
        public KeyCode left = KeyCode.A;
        [BoxGroup("2D-Controller")]
        public KeyCode right = KeyCode.D;



        // In baking, this Baker will run once for every SpawnerAuthoring instance in a subscene.
        // (Note that nesting an authoring component's Baker class inside the authoring MonoBehaviour class
        // is simply an optional matter of style.)
        class Baker : Baker<DOP_ShooterAuthoring>
        {
            public override void Bake(DOP_ShooterAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new SpawnerEntity
                {
                    Prefab = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic),
                    position = authoring.position,
                    rotation = authoring.rotation,
                    amount = authoring.amount,
                    randomizePosition = authoring.randomizePosition
                });
                AddComponent(entity, new MovementComponentData
                {
                    speed = authoring.speed,
                });
                AddComponent(entity, new BindingsComponentData
                {
                    Forward = authoring.forward, 
                    Backward = authoring.backward, 
                    Left = authoring.left, 
                    Right = authoring.right
                });
            }
        }
    }

    public struct BindingsComponentData : IComponentData
    {
        public KeyCode Forward;
        public KeyCode Backward;
        public KeyCode Left;
        public KeyCode Right;
    }

    struct MovementComponentData : IComponentData
    {
        public float speed;
    }
    
    struct RotationComponentData : IComponentData
    {
        public float rotationSpeed;
    }

    struct SpawnerEntity : IComponentData
    {
        public Entity Prefab;
        public float3 position;
        public quaternion rotation;

        public int amount;
        public bool randomizePosition;
    }

    struct ControlledEntity : IComponentData
    {
    }

}

