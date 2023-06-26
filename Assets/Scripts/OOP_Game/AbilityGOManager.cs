using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

namespace Scenes.OOP_Test
{
    

    public class AbilityGOManager : MonoBehaviour
    {
        // Holds the references to all the ships the player has access to
        [TabGroup("Drone Beacon")] public List<GameObject> droneBeacon = new List<GameObject>();

        [TabGroup("Drone Attack")] public List<GameObject> droneAttack = new List<GameObject>();

        [TabGroup("Drone Repair")] public List<GameObject> droneRepair = new List<GameObject>();

        [TabGroup("Missile")] public List<GameObject> missile = new List<GameObject>();

        [TabGroup("Bullets")] public List<GameObject> bullets = new List<GameObject>();

        public static AbilityGOManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

    }

}