using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

namespace Scenes.OOP_Test
{
    

    public class WaveManager : MonoBehaviour
    {

        #region Public Variables

        public ProjectileManager projectileManager;
        public GenericUpdateManager genericUpdateManager;
        public AbilityGOManager abilityManager;

        // Button that calls the SpawnProjectileTest method
        [Button("Spawn Projectile Test")]
        public void SpawnProjectileTestButton()
        {
            SpawnProjectileTest();
        }

        // Button to increment the wave
        [Button("Increment Wave")]
        public void IncrementWave()
        {
            genericUpdateManager.wave++;
            projectileManager.UpdateDefaultParameters(genericUpdateManager.wave);
        }

        #endregion

        #region Private Variables

        #endregion

        #region Properties

        #endregion

        #region MonoBehaviours

        #endregion

        #region Methods

        public void SpawnProjectileTest()
        {
            var projectileParam = projectileManager.GetDefaultParameters();
            projectileParam.bulletBehaviour = BulletBehaviour.Directed;
            projectileParam.type = BulletType.Homing;
            projectileParam.spawnOffset = new float2(20, 20);

            var enemyData = new EnemyTypeData(
                EnemyType.Basic,
                BulletType.Homing,
                new int2(-45, 45),
                new int2(5, 10),
                projectileParam.speedRange,
                new float2(0, 0));

            var projectile = projectileManager.GetBulletSpawnParameters(enemyData, 1, projectileParam);

            Debug.Log(projectile);

            projectileManager.SpawnProjectile(projectile);
        }

        #endregion

    }
}
