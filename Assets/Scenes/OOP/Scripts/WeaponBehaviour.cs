
using System;
using System.Collections.Generic;
using Scenes.OOP_Test;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scenes.Test
{
    /// <summary>
    /// The player has weapons that shoot BulletBehaviours
    /// </summary>
    public class WeaponBehaviour : MonoBehaviour
    {

        #region Public Variables

        public BulletSpawnParameters bulletSpawnParameters { get; set; }
        
        #endregion

        #region Private Variables
        
        List<float> angles = new List<float>();

        #endregion

        #region Properties

        public ObjectPool.Pool _bulletPool { get; set; }
        
        #endregion

        #region MonoBehaviours

        private void Start()
        {
            EnsureDefaultObjComponents();
            // Reset the default object 
            ResetDefaultObjPool();
            _bulletPool.CreateBatch(_bulletPool.defaultObject, _bulletPool.amountToPool);
            
            // Calculate the angles for the bullets
            CalculateAngles();
        }

        private void CalculateAngles()
        {
            angles.Clear();
            
            if (bulletSpawnParameters.amountDistributed > 1)
            {
                // We add the arc.x and arc.y
                angles.Add(bulletSpawnParameters.attackArcRange.x);
                angles.Add(bulletSpawnParameters.attackArcRange.y);

                int fullRange = (int) (bulletSpawnParameters.attackArcRange.y - bulletSpawnParameters.attackArcRange.x);
                for (int i = 0; i < bulletSpawnParameters.amountDistributed - 2; i++)
                {
                    // the angle between bullets should be the same from the first (arc.x) to the last (arc.y)
                    angles.Add(bulletSpawnParameters.attackArcRange.x + (fullRange / (bulletSpawnParameters.amountDistributed - 1f)) * (i + 1));
                }
            }
            else
            {
                angles.Add(0);
            }
        }

        public void EnsureDefaultObjComponents()
        {
            // Ensure the projectile has a BulletBehaviour component and a collider of some sort
            if (_bulletPool.defaultObject.GetComponent<BulletBehaviour>() == null)
            {
                _bulletPool.defaultObject.AddComponent<BulletBehaviour>();
            }
            
            if (_bulletPool.defaultObject.GetComponent<Collider>() == null)
            {
                _bulletPool.defaultObject.AddComponent<BoxCollider>();
            }
            
            // Add it to the bullet tag and layer
            _bulletPool.defaultObject.tag = "Bullet";
            _bulletPool.defaultObject.layer = LayerMask.NameToLayer("Bullet");
        }

        public void ResetDefaultObjPool()
        {
            GameObject bullet = _bulletPool.defaultObject;
            
            BulletBehaviour bulletBehaviour = bullet.GetComponent<BulletBehaviour>();
            
            // Set the bullet's parameters
            bulletBehaviour.speed = bulletSpawnParameters.speed;
            bulletBehaviour.lifeTime = bulletSpawnParameters.lifeTime;
            bulletBehaviour.damage = bulletSpawnParameters.damage;
            bulletBehaviour.pierce = bulletSpawnParameters.pierce;
            
            bulletBehaviour.SetBulletBehavior(bulletSpawnParameters.type);
            bulletBehaviour.SetBulletSpecifics(bulletSpawnParameters.specificsHolder);

            bulletBehaviour.enabled = true;
        }

        #endregion

        #region Methods
        
        public bool Shoot(Transform shootPoint)
        {
            // Get a bullet from the pool
            List<GameObject> bullets = _bulletPool.GetInactiveObject(bulletSpawnParameters.amountDistributed);
            
            // If there are no bullets in the pool, return false
            if (bullets.Count == 0)
            {
                return false;
            }
            
            // For each bullet
            for (var index = 0; index < bullets.Count; index++)
            {
                var bullet = bullets[index];
                // Set the bullet's position and rotation
                bullet.transform.position = shootPoint.position;
                // We only rotate in the y axis from the shoot point 
                bullet.transform.rotation = Quaternion.Euler(0, shootPoint.localRotation.eulerAngles.y + angles[index], 0);

                // Reset the pierce
                bullet.GetComponent<BulletBehaviour>().pierce = bulletSpawnParameters.pierce;

                bullet.SetActive(true);
            }

            return true;
        }

        #endregion
        
    }

}