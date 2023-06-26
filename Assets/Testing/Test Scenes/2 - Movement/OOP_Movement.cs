using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Random = UnityEngine.Random;

using Sirenix.OdinInspector;

namespace Test.Movement
{
    

    /// <summary>
    /// Creates a new object from a prefab given in Object Oriented Programming
    /// </summary>
    public class OOP_Movement : MonoBehaviour
    {
        [FoldoutGroup("Creation")]
        public GameObject prefab;
        [FoldoutGroup("Creation")]
        public Vector3 position = new Vector3(0, 0, 0);
        [FoldoutGroup("Creation")]
        public Quaternion rotation = Quaternion.identity;

        [FoldoutGroup("Creation")]
        public int amount = 1;
        [FoldoutGroup("Creation")]
        public bool randomizePosition = false;
        
        [BoxGroup("Movement")]
        public float speed = 1f;
        [BoxGroup("Movement")]
        public Vector3 direction = Vector3.forward;
        [BoxGroup("Movement")]
        public float rotationSpeed = 1f;
        [BoxGroup("Movement")]
        public Vector3 rotationDirection = Vector3.up;
        
        private List<GameObject> objects = new List<GameObject>();

        // Start is called before the first frame update
        void Start()
        {
            for (int i = 0; i < amount; i++)
            {
                if (randomizePosition)
                {
                    // Position +- 3 on each axis
                    var random = new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), Random.Range(-3, 3));
                    position += random;
                }

                // Create a new object from the prefab and set its main values
                var newObject = Instantiate(prefab, position, rotation);
                // We have the new object stored as a reference to directly access it
                // Change the name so we can see it in the hierarchy easier
                newObject.name = "OOP_Created Object " + i;
                
                // Add the new object to the array
                objects.Add(newObject);
            }
        }
        
        // Update is called once per frame
        void Update()
        {
            // Loop through all objects
            foreach (var obj in objects)
            {
                // Move the object on what forward direction is currently facing
                obj.transform.Translate(direction * speed * Time.deltaTime, Space.Self);
                // Rotate the object, rotationSpeed is in degrees, so we need to convert it to radians
                obj.transform.Rotate(rotationDirection, rotationSpeed * Time.deltaTime * Mathf.Rad2Deg);
            }
        }
    }
}
