using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Scenes.OOP_Test
{
    

    public class StartManager : MonoBehaviour
    {
        public Camera mainCamera;
        public GenericUpdateManager genericUpdateManager;

        // Start is called before the first frame update
        void Awake()
        {
            // Since the camera is in orthographic mode, we need to calculate the world size
            // based on the camera's size and aspect ratio
            genericUpdateManager.worldSize = new float2(mainCamera.orthographicSize * mainCamera.aspect * 2, mainCamera.orthographicSize * 2);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}