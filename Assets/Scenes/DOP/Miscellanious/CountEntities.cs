using System;
using System.Collections;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes.DOP
{
    /// <summary>
    /// Links to the text objects in the scene and updates them with the current entity count.
    /// </summary>
    public class CountEntities : MonoBehaviour
    {
        public float updateInterval = 0.25f;
        public bool isRunning = true;
        public Text bulletEntityCountText;
        public Text enemyEntityCountText;
        private EntityManager _entityManager;
        
        private void Start()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            StartCoroutine(RefreshTexts());
        }

        private IEnumerator RefreshTexts()
        {
            while (isRunning)
            {
                bulletEntityCountText.text = _entityManager.CreateEntityQuery(typeof(BulletTag)).CalculateEntityCount().ToString();
                enemyEntityCountText.text = _entityManager.CreateEntityQuery(typeof(EnemyTag)).CalculateEntityCount().ToString();
                yield return new WaitForSeconds(updateInterval);
            }
            
            yield return null;
        }
        
        private void OnDestroy()
        {
            StopCoroutine(RefreshTexts());
        }
    }
}