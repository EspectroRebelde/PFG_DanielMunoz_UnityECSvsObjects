using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Transforms;

using Sirenix.OdinInspector;

// An authoring component is just a normal MonoBehavior that has a Baker<T> class.

namespace Test.Creation
{
    /// <summary>
    /// Creates a new object from a prefab given in Data Oriented Programming (ECS style)
    /// This will be the same as the OOP_Creation script, but with ECS
    /// It implements an auth
    /// </summary>
    public class DOP_CreationAuthoring : MonoBehaviour
    {
        [BoxGroup("Creation")]
        public GameObject Prefab;
        [BoxGroup("Creation")]
        public float3 position = new float3(0, 0, 0);
        [BoxGroup("Creation")]
        public quaternion rotation = quaternion.identity;

        [BoxGroup("Creation")]
        public int amount = 1;
        [BoxGroup("Creation")]
        public bool randomizePosition = false;

        // In baking, this Baker will run once for every SpawnerAuthoring instance in a subscene.
        // (Note that nesting an authoring component's Baker class inside the authoring MonoBehaviour class
        // is simply an optional matter of style.)
        class Baker : Baker<DOP_CreationAuthoring>
        {
            public override void Bake(DOP_CreationAuthoring authoring)
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
            }
        }
    }

    struct SpawnerEntity : IComponentData
    {
        public Entity Prefab;
        public float3 position;
        public quaternion rotation;

        public int amount;
        public bool randomizePosition;
    }
}

