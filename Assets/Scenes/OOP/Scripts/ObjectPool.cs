using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scenes.Test
{
    // Unity Book, Level up your code with game programming patterns 50/99
    public class ObjectPool : MonoBehaviour
    {
        public int defaultBatchSize = 10;
        [Sirenix.OdinInspector.PropertySpace(10)]
        public List<PoolDefinition> objectsToPool;
        public static ObjectPool SharedInstance { get; private set; }

        [ReadOnly]
        public Dictionary<string, Pool> pooledObjects = new Dictionary<string, Pool>();

        void Awake()
        {
            SharedInstance = this;
        }
        
        void Start()
        {
            for (var i = 0; i < objectsToPool.Count; i++)
            {
                var poolDefinition = objectsToPool[i];
                // Create a parent object for the pool
                GameObject poolParent = new GameObject(poolDefinition.name + "Pool");
                poolParent.transform.position = Vector3.zero;

                // Copy the original object so we can use it as a template
                poolDefinition.obj = Instantiate(poolDefinition.obj, poolParent.transform, true);
                poolDefinition.obj.SetActive(false);

                // Set the scale of the object
                poolDefinition.obj.transform.localScale = new Vector3(poolDefinition.scale, poolDefinition.scale, poolDefinition.scale);
                poolDefinition.obj.tag = "Bullet";
                poolDefinition.obj.layer = LayerMask.NameToLayer("Bullet");
                poolDefinition.obj.name = poolDefinition.name;

                // If the object doesn't have a BulletBehaviour, add one
                if (poolDefinition.obj.GetComponent<BulletBehaviour>() == null)
                {
                    Destroy(poolDefinition.obj.GetComponent<BulletBehaviour>());
                    poolDefinition.obj.AddComponent<BulletBehaviour>();
                }
                
                // If it has a ProjectileMover, remove it
                if (poolDefinition.obj.GetComponent<ProjectileMover>() != null)
                {
                    Destroy(poolDefinition.obj.GetComponent<ProjectileMover>());
                }

                Pool pool = new Pool
                {
                    parentHolder = poolParent,
                    defaultObject = poolDefinition.obj,
                    activeObjects = new List<GameObject>(),
                    inactiveObjects = new List<GameObject>()
                };

                pool.batchSize = poolDefinition.batchSize > 0 ? poolDefinition.batchSize : defaultBatchSize;
                pool.amountToPool = poolDefinition.amountToPool > 0 ? poolDefinition.amountToPool : pool.batchSize;

                switch (poolDefinition.expandType)
                {
                    case PoolExpandType.None:
                        pool.onExpandPool = pool.None;
                        break;
                    case PoolExpandType.OnBatch:
                        pool.onExpandPool = pool.OnBatch;
                        break;
                    case PoolExpandType.OnDemand:
                        pool.onExpandPool = pool.OnDemand;
                        break;
                    case PoolExpandType.Latest:
                        pool.onExpandPool = pool.Latest;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                pooledObjects.Add(poolDefinition.name, pool);
                
            }
        }
        
        public Pool GetPool(string name)
        {
            return pooledObjects[name];
        }
        
        public string[] GetPoolNames()
        {
            string[] poolNames = new string[pooledObjects.Count];
            int i = 0;
            foreach (var pool in pooledObjects)
            {
                poolNames[i] = pool.Key;
                i++;
            }

            return poolNames;
        }

        [Serializable]
        public struct PoolDefinition
        {
            public string name;
            public GameObject obj;
            public int amountToPool;
            public int batchSize;
            public float scale;
            public PoolExpandType expandType;

            public BulletSpawnParameters bulletSpawnParameters;
        }

        [Serializable]
        public enum PoolExpandType
        {
            None,
            OnBatch,
            OnDemand,
            Latest
        }
        
        public class Pool
        {
            public GameObject parentHolder;
            public GameObject defaultObject;
            public List<GameObject> activeObjects;
            public List<GameObject> inactiveObjects;
            public delegate bool OnExpandPool();
            public OnExpandPool onExpandPool;
            public int amountToPool;
            public int batchSize;

            // Get the first inactive object in the pool to use
            public GameObject GetInactiveObject()
            {
                if (inactiveObjects.Count > 0)
                {
                    GameObject obj = inactiveObjects[0];
                    inactiveObjects.RemoveAt(0);
                    activeObjects.Add(obj);
                    
                    return obj;
                }

                return onExpandPool() ? GetInactiveObject() : null;
            }

            public List<GameObject> GetInactiveObject(int number)
            {
                List<GameObject> objects = new List<GameObject>();
                for (int i = 0; i < number; i++)
                {
                    GameObject obj = GetInactiveObject();
                    if (obj != null)
                    {
                        objects.Add(obj);
                    }
                }

                return objects;
            }
                
            // Add an object to the pool
            private void AddObject(GameObject obj)
            {
                inactiveObjects.Add(obj);
                obj.SetActive(false);
                
                // Set parameters for the bullet
                // Speed, 
            }
            
            public void CreateBatch(GameObject obj, int amountToPool)
            {
                for (int i = 0; i < amountToPool; i++)
                {
                    // Create a new object keeping component properties
                    GameObject newObj = InstantiateObject(obj);
                    AddObject(newObj);
                    newObj.GetComponent<BulletBehaviour>().SetPool(this);
                    if (parentHolder != null)
                    {
                        newObj.transform.SetParent(parentHolder.transform);
                    }
                }
            }
            
            private GameObject InstantiateObject(GameObject obj)
            {
                GameObject newObj = Instantiate(obj, parentHolder.transform, true);
                // Set the Behaviour and Specifics to those of the original object
                BulletBehaviour bulletBehaviour = newObj.GetComponent<BulletBehaviour>();
                bulletBehaviour.SetPool(this);
                bulletBehaviour.SetBulletSpecifics(obj.GetComponent<BulletBehaviour>().GetBulletSpecifics());
                bulletBehaviour.SetBulletBehavior(obj.GetComponent<BulletBehaviour>().GetBulletBehavior());

                return newObj;
            }

            #region OnExpandPool
            
            /// <summary>
            /// OnExpandPool delegate for the None PoolExpandType
            /// </summary>
            /// <returns>Unable to expand the pool</returns>
            public bool None()
            {
                return false;
            }
            
            /// <summary>
            /// OnExpandPool delegate for the OnBatch PoolExpandType
            /// Creates a batch of objects to add to the pool
            /// </summary>
            /// <returns>True if the pool was expanded, false otherwise</returns>
            public bool OnBatch()
            {
                CreateBatch(defaultObject, batchSize);
                return true;
            }
            
            /// <summary>
            /// OnExpandPool delegate for the OnDemand PoolExpandType
            /// Creates a single object to add to the pool
            /// </summary>
            /// <returns>True if the pool was expanded, false otherwise</returns>
            public bool OnDemand()
            {
                CreateBatch(defaultObject, 1);
                return true;
            }

            /// <summary>
            /// OnExpandPool delegate for the Latest PoolExpandType
            /// Gets the earliest object in the active pool and returns it as new inactive object
            /// </summary>
            /// <returns>True if the pool was expanded, false otherwise</returns>
            public bool Latest()
            {
                // Get the earliest object in the active pool and return it as new inactive object
                if (activeObjects.Count > 0)
                {
                    GameObject obj = activeObjects[0];
                    activeObjects.RemoveAt(0);
                    inactiveObjects.Add(obj);
                    return true;
                }
                
                return false;
            }
            
            #endregion
            
            // Observer pattern for the pool
            public void ReturnObjectToPool(GameObject obj)
            {
                obj.SetActive(false);
                activeObjects.Remove(obj);
                inactiveObjects.Add(obj);
            }
        }
    }
}