using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

namespace Scenes.OOP_Test
{
    

    public class ShipManager : MonoBehaviour
    {
        // Holds the references to all the ships the player has access to
        [TabGroup("Basic")] public List<GameObject> basicShips = new List<GameObject>();

        [TabGroup("Stealth")] public List<GameObject> stealthShips = new List<GameObject>();

        [TabGroup("Bomber")] public List<GameObject> bomberShips = new List<GameObject>();

        [TabGroup("Fighter")] public List<GameObject> fighterShips = new List<GameObject>();

        [TabGroup("Fighter Heavy")] public List<GameObject> fighterHeavyShips = new List<GameObject>();

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}