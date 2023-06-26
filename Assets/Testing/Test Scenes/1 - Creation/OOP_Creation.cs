using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Random = UnityEngine.Random;

using Sirenix.OdinInspector;

namespace Test.Creation
{
    

    /// <summary>
    /// Creates a new object from a prefab given in Object Oriented Programming
    /// </summary>
    public class OOP_Creation : MonoBehaviour
    {
        [BoxGroup("Creation")]
        public GameObject prefab;
        [BoxGroup("Creation")]
        public Vector3 position = new Vector3(0, 0, 0);
        [BoxGroup("Creation")]
        public Quaternion rotation = Quaternion.identity;

        [BoxGroup("Creation")]
        public int amount = 1;
        [BoxGroup("Creation")]
        public bool randomizePosition = false;

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
                GameObject newObject = Instantiate(prefab, position, rotation);
                // We have the new object stored as a reference to directly access it
                // Change the name so we can see it in the hierarchy easier
                newObject.name = "OOP_Created Object " + i;
            }
        }
    }
}
