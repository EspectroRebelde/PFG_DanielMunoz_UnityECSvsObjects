
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scenes.Test
{
    public class BulletCollector : MonoBehaviour
    {

        #region Public Variables

        public Camera cam;
        public bool isRunning = true;
        
        public float innerWaitTime = 0.1f;
        public float outerWaitTime = 0.1f;
        
        public float3x3 boundWidth = new float3x3(new float3(0, 1.01f, 0), new float3(-0.01f,0, 1.01f), new float3(0, -0.01f, 0));
        
        #endregion

        #region Private Variables

        #endregion

        #region Properties

        #endregion

        #region MonoBehaviours

        // Start will activate a coroutine that will check for bullets that have gone out of bounds
        void Start()
        {
            StartCoroutine(CheckOutOfBounds());
        }
        
        // OnDestroy will stop the coroutine
        private void OnDestroy()
        {
            StopCoroutine(CheckOutOfBounds());
        }
        
        public void StopBulletCollector()
        {
            StopCoroutine(CheckOutOfBounds());
        }

        #endregion

        #region Methods

        // This coroutine will check for bullets that have gone out of bounds and return them to the pool
        private IEnumerator CheckOutOfBounds()
        {
            while (isRunning)
            {
                // Get all the bullets in the Bullet Layer
                GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
                int tenPercent = bullets.Length / 10;
                
                // For each bullet
                for (var index = 0; index < bullets.Length; index++)
                {
                    var bullet = bullets[index];
                    // If the bullet is outside the camera's view
                    Vector3 screenPoint = cam.WorldToViewportPoint(bullet.transform.position);
                    if (screenPoint.x < boundWidth.c1.x || 
                        screenPoint.x > boundWidth.c1.z || 
                        screenPoint.y < boundWidth.c2.y || 
                        screenPoint.y > boundWidth.c0.y || 
                        screenPoint.z < 0)
                    {
                        // Return the bullet to the pool
                        bullet.GetComponent<BulletBehaviour>().ReturnToPool();
                    }

                    if (tenPercent > 0 && index % tenPercent == 0)
                    {
                        yield return new WaitForSeconds(innerWaitTime);
                    }
                }
                
                yield return new WaitForSeconds(outerWaitTime);
            }
        }

        #endregion



    }
}
