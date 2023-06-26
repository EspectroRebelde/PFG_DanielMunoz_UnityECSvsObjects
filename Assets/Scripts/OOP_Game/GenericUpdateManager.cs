using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

using Sirenix.OdinInspector;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine.Jobs;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Scenes.OOP_Test
{
    public class GenericUpdateManager : MonoBehaviour
    {
        [BoxGroup("Game Settings")]
        // A value droopdown that allows multiple tags to be selected
        [ValueDropdown("GetTags", IsUniqueList = true)]
        public List<string> tagToSearchFor;

        [BoxGroup("Game Settings")] public bool usesController;

        [BoxGroup("Game Settings")] public GameObject playerShip;
        [BoxGroup("Game Settings")] public BasicPlayerController playerController;
        [BoxGroup("Game Settings")] public ProjectileManager projectileManager;

        [BoxGroup("Wave Settings")] public int wave = 1;

        [BoxGroup("Wave Settings")] public float waveMaxTime = 60;

        // [BoxGroup("Wave Settings")]
        // private float waveTime = 0;
        [BoxGroup("Wave Settings")] [ValueDropdown("GetTags", IsUniqueList = true)]
        public List<string> enemyTag = new List<string>() { "Enemy" };

        [BoxGroup("Wave Settings")] public WaveManager waveManager;

        [BoxGroup("World Settings")] [ReadOnly]
        public float2 worldSize = new(-1, -1);

        private float2 playableArea = new(-1, -1);

        [BoxGroup("World Settings")] [ReadOnly]
        public float2 borderThreshold = new(-1, -1);

        [BoxGroup("Skybox Settings")] public Camera skyboxCamera;

        [BoxGroup("Skybox Settings")] public float skyboxRotationSpeed = 0.1f;

        [BoxGroup("Skybox Settings")] public float3 skyboxRotationAxis = new(0, 1, 0);

        [BoxGroup("Skybox Settings")] public List<Material> skyboxes;

        [BoxGroup("Skybox Settings")] public float cooldownToChangeSkybox = 50;

        [BoxGroup("Skybox Settings")] public float cooldown = 10;

        //private string[] GetTags() => UnityEditorInternal.InternalEditorUtility.tags;
        private TransformAccessArray objects;

        private WorldState worldState = WorldState.Refresh;

        [Flags]
        public enum WorldState
        {
            None = 0,
            Refresh = 1 << 0,
            Update = 1 << 1,
            Running = 1 << 2,
            UpdatePlayer = 1 << 3,
            UpdateController = 1 << 4,
        }

        // An odin button to change the worldState
        [Button]
        public void UpdateController()
        {
            worldState |= WorldState.UpdateController;
        }

        // Start is called before the first frame update
        void Start()
        {
            if (worldSize.x < 0 || worldSize.y < 0)
            {
                Debug.LogError("World size is not set!");
            }

            // The borderThreshold is used to determine the playable area
            // It should depend on the player's ship model bounds
            borderThreshold = new float2(playerShip.GetComponent<Renderer>().bounds.size.x, playerShip.GetComponent<Renderer>().bounds.size.z);

            playableArea = new float2((worldSize.x + borderThreshold.x) / 2, (worldSize.y + borderThreshold.y) / 2);
            RefreshList();

            // Set all the skyboxes' exposure to 1
            foreach (var skybox in skyboxes)
            {
                skybox.SetFloat("_Exposure", 1);
            }
        }

        // Update is called once per frame
        void Update()
        {
            // Refresh the list of objects to manage the bounds of
            if ((worldState & WorldState.Refresh) != 0)
            {
                RefreshList();
            }

            // The player's ship has been updated so we need to update the playable area
            if ((worldState & WorldState.UpdatePlayer) != 0)
            {
                borderThreshold = new float2(playerShip.GetComponent<Renderer>().bounds.size.x, playerShip.GetComponent<Renderer>().bounds.size.z);
                playableArea = new float2((worldSize.x + borderThreshold.x) / 2, (worldSize.y + borderThreshold.y) / 2);
            }

            if ((worldState & WorldState.UpdateController) != 0)
            {
                playerController.isGamepad = usesController;
                worldState &= ~WorldState.UpdateController;
            }

            // Something is in need of update so we don't run the main script
            if ((worldState & WorldState.Update) != 0)
            {
                return;
            }

            worldState |= WorldState.Running;

            // Create an IJobParallelForTransform to query all the objects and manage them
            var job = new BoundaryWrap
            {
                playableAreaSizeX = playableArea.x,
                playableAreaSizeY = playableArea.y,
            };

            // Schedule the job
            JobHandle jobHandle = job.Schedule(objects);

            SkyboxMovement();
            WaveManagement();

            jobHandle.Complete();
        }

        // IJobParallelForTransform 
        // It takes a list of objects and a float2 that represents the world size (already divided by 2)
        // If the object is outside the world size, it will wrap it around to the other side
        // If not, it will leave it be
        [BurstCompile(FloatPrecision.Low, FloatMode.Fast, CompileSynchronously = true, Debug = true)]
        private struct BoundaryWrap : IJobParallelForTransform
        {
            public float playableAreaSizeX;
            public float playableAreaSizeY;

            public void Execute(int index, TransformAccess transform)
            {
                // Get the position of the object
                float3 position = transform.position;

                // If the object is outside the world size, wrap it around to the other side
                position.x = Mod(position.x + playableAreaSizeX, playableAreaSizeX * 2) - playableAreaSizeX;
                position.z = Mod(position.z + playableAreaSizeY, playableAreaSizeY * 2) - playableAreaSizeY;

                // Set the position of the object
                transform.position = position;
            }

            // Module operation using the Unity.Mathematics library
            // It's faster than the % operator
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static float Mod(float a, float m) => a - m * math.floor(a / m);
        }

        private void RefreshList()
        {
            List<GameObject> objectsInScene = new List<GameObject>();
            // Get all the objects in the scene that have the tag we're looking for
            foreach (var tagToSearch in tagToSearchFor.Where(tagToSearch => tagToSearch != null))
            {
                objectsInScene.AddRange(GameObject.FindGameObjectsWithTag(tagToSearch));
            }

            // Add the objects to the TransformAccessArray
            objects = new TransformAccessArray(objectsInScene.Count);
            foreach (GameObject obj in objectsInScene)
            {
                objects.Add(obj.transform);
            }

            // Set the Refresh flag to false
            worldState &= ~WorldState.Refresh;
        }

        public void AddToList(List<GameObject> addedGameObjects)
        {
            // We process the new list and add it to the TransformAccessArray
            foreach (GameObject obj in addedGameObjects)
            {
                objects.Add(obj.transform);
            }
        }

        public void RemoveFromList(List<GameObject> removedGameObjects)
        {
            // We have to process the new list and remove it from the TransformAccessArray with RemoveAtSwapBack

        }

        public void RemoveFromList(GameObject removedGameObject)
        {
            // Find the index
        }

        /// <summary>
        /// Rotated the skybox camera as to give the illusion of movement
        /// It reduces the cooldown to change the skybox
        /// If the cooldown reaches 0, it'll start a coroutine to change the skybox
        /// </summary>
        private void SkyboxMovement()
        {
            // Rotate the skybox camera
            skyboxCamera.transform.Rotate(Vector3.up, skyboxRotationSpeed * Time.deltaTime);

            // Reduce the cooldown to change the skybox
            cooldown -= Time.deltaTime;

            // If the cooldown reaches 0, start it
            if (cooldown <= 0)
            {
                cooldown = cooldownToChangeSkybox;
                StartCoroutine(ChangeSkybox());
            }
        }

        // ChangeSkybox coroutine
        private IEnumerator ChangeSkybox()
        {

            // Get a random skybox
            Material newSkybox = skyboxes[Random.Range(0, skyboxes.Count)];
            Material oldSkybox = RenderSettings.skybox;
            // Set the cooldown to change the skybox
            cooldownToChangeSkybox = Random.Range(60, 120);

            // Fade the skybox to black (exposure = 0)
            for (float i = 1; i > 0; i -= 0.025f)
            {
                RenderSettings.skybox.SetFloat("_Exposure", i);
                // TODO: Rays along the edges of the screen to make it look like the skybox is being pulled
                yield return new WaitForSeconds(0.1f);
            }

            // Set the new skybox's exposure to match the old one
            newSkybox.SetFloat("_Exposure", 0);

            // Set the new skybox
            RenderSettings.skybox = newSkybox;

            // Reset the old skybox's exposure
            oldSkybox.SetFloat("_Exposure", 1);

            // Fade the skybox to normal (exposure = 1)
            for (float i = 0; i < 1; i += 0.025f)
            {
                RenderSettings.skybox.SetFloat("_Exposure", i);
                // TODO: Slowly fade the rays
                yield return new WaitForSeconds(0.1f);
            }
        }

        // Implementation of ChangeSkybox using Jobs
        private struct ChangeSkyboxJob : IJob
        {
            public Material newSkybox;
            public float changeRation;

            public void Execute()
            {
                // Fade the skybox to black
                for (float i = 0; i < 1; i += changeRation)
                {
                    RenderSettings.skybox.SetFloat("_Exposure", i);
                }

                // Set the new skybox's exposure to match the old one
                newSkybox.SetFloat("_Exposure", 1);

                // Set the new skybox
                RenderSettings.skybox = newSkybox;

                // Fade the skybox to normal
                for (float i = 0; i < 1; i += changeRation)
                {
                    RenderSettings.skybox.SetFloat("_Exposure", 1 - i);
                }
            }
        }

        private void WaveManagement()
        {
            // If the wave is over, start a new one
            // The wave is over when all the enemies with tag = enemyTag are destroyed
            foreach (var tagToSearch in enemyTag.Where(tagToSearch => tagToSearch != null))
            {
                //TODO == 0
                if (GameObject.FindGameObjectsWithTag(tagToSearch).Length != 0)
                {
                    wave++;

                    projectileManager.UpdateDefaultParameters(wave);

                    // TODO: Start a new wave
                }
            }

        }

    }
}