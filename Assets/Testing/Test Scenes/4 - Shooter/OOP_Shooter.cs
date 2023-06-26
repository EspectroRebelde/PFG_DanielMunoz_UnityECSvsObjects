using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Random = UnityEngine.Random;


using Sirenix.OdinInspector;


namespace Test.Shooter
{
    

    /// <summary>
    /// Creates a new object from a prefab given in Object Oriented Programming
    /// </summary>
    public class OOP_Shooter : MonoBehaviour
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
        
        private List<GameObject> objects = new List<GameObject>();

        // Start is called before the first frame update
        void Awake()
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
            MovementFlags direction = MovementFlags.None;
            if (Input.GetKey(forward))
            {
                direction |= MovementFlags.Forward;
            }
            if (Input.GetKey(backward))
            {
                direction |= MovementFlags.Backward;
            }
            if (Input.GetKey(left))
            {
                direction |= MovementFlags.Left;
            }
            if (Input.GetKey(right))
            {
                direction |= MovementFlags.Right;
            }

            // If there is at least one bit set
            if (direction != MovementFlags.None)
            {
                var posVector = Vector3.zero;
                if ((direction & MovementFlags.IsDiagonal) == MovementFlags.IsDiagonal)
                {
                    // Diagonal movement (|_ is speed*deltatime but diagonal will be speed*deltatime*0.707)
                    switch (direction)
                    {
                        case MovementFlags.ForwardLeft:
                            posVector = new Vector3(-speed * Time.deltaTime * 0.707f, 0, speed * Time.deltaTime * 0.707f);
                            break;
                        case MovementFlags.BackwardLeft:
                            posVector = new Vector3(-speed * Time.deltaTime * 0.707f, 0, -speed * Time.deltaTime * 0.707f);
                            break;
                        case MovementFlags.ForwardRight:
                            posVector = new Vector3(speed * Time.deltaTime * 0.707f, 0, speed * Time.deltaTime * 0.707f);
                            break;
                        case MovementFlags.BackwardRight:
                            posVector = new Vector3(speed * Time.deltaTime * 0.707f, 0, -speed * Time.deltaTime * 0.707f);
                            break;
                    }
                }
                else
                {
                    // Normal movement (|_ is speed*deltatime)
                    switch (direction)
                    {
                        case MovementFlags.Forward:
                            posVector = new Vector3(0, 0, speed * Time.deltaTime);
                            break;
                        case MovementFlags.Backward:
                            posVector = new Vector3(0, 0, -speed * Time.deltaTime);
                            break;
                        case MovementFlags.Left:
                            posVector = new Vector3(-speed * Time.deltaTime, 0, 0);
                            break;
                        case MovementFlags.Right:
                            posVector = new Vector3(speed * Time.deltaTime, 0, 0);
                            break;
                    }
                }
                
                // Move all objects
                foreach (var obj in objects)
                {
                    obj.transform.position += posVector;
                }
            }
        }
    }
}
