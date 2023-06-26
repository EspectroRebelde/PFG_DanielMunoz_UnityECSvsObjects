using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scenes.Test
{
    /// <summary> 
    /// Handles the bullet's:
    /// <list type="bullet">
    /// <item> Set Up <see cref="IBulletBehavior.SetUpdateBullet"/> </item>
    /// <item> Movement <see cref="IBulletBehavior.UpdateBullet"/> <see cref="IBulletSpecifics.UpdateSpecifics"/> </item>
    /// <item> Collision <see cref="IBulletSpecifics.OnCollisionEnterSpecifics"/> </item>
    /// <item> Destruction <see cref="IBulletSpecifics.DestroySpecifics"/> </item>
    /// </list>
    /// </summary>
    public class BulletBehaviour : MonoBehaviour
    {

        #region Public Variables

        public float speed;
        public float lifeTime;
        public int damage;
        public int pierce;

        #endregion

        #region Private Variables

        private float timeToDestroy = 0.0f;
        
        #endregion

        #region Properties

        private IBulletBehavior _bulletBehavior;
        private IBulletSpecifics _bulletSpecifics;
        private Transform _targetTransform;
        private ObjectPool.Pool _pool;

        // A string to show the bullet behavior in the inspector
        [ShowInInspector]
        private string BulletBehavior
        {
            get
            {
                return _bulletBehavior switch
                {
                    BulletBehavior_FixedAngle => "Fixed Angle",
                    BulletBehavior_RandomRange => "Random Range",
                    BulletBehavior_ChangingAngle => "Changing Angle",
                    BulletBehavior_Aimed => "Aimed",
                    BulletBehavior_Directed => "Directed",
                    _ => "Unknown"
                };
            }
        }
        [ShowInInspector]
        private string BulletSpecifics
        {
            get
            {
                return _bulletSpecifics switch
                {
                    BulletSpecifics_None => "None",
                    BulletSpecifics_Void => "Void",
                    BulletSpecifics_BaseBullet => "Sword",
                    BulletSpecifics_Shock => "Shock",
                    _ => "Unknown"
                };
            }
        }

        #endregion

        #region MonoBehaviours

        /// <summary>
        /// Checks and sets the bullet behavior and bullet specifics as well as the life time
        /// <seealso cref="IBulletBehavior.SetUpdateBullet"/>
        /// </summary>
        private void OnEnable()
        {
            // Set the speed of the bullet on the projectile mover
            _bulletBehavior.speed = speed;
            _bulletBehavior?.SetUpdateBullet(this, _targetTransform);
            
            // Set the Time to Destroy, if it is 
            timeToDestroy = Time.time + lifeTime;
            
        }

        /// <summary>
        /// Updates the bullet's movement and specifics and checks if it should be destroyed
        /// <seealso cref="IBulletBehavior.UpdateBullet"/>
        /// <seealso cref="IBulletSpecifics.UpdateSpecifics"/>
        /// </summary>
        private void FixedUpdate()
        {
            _bulletBehavior?.UpdateBullet(this, _targetTransform);
            _bulletSpecifics?.UpdateSpecifics(this);
            
            // If the bullet has been alive for longer than its lifetime, destroy it
            if ((Time.time > timeToDestroy && lifeTime > 0.0f) || pierce <= 0)
            {
                DestroyBullet();
            }
        }

        /// <summary>
        /// Checks if the bullet has collided with anything
        /// <seealso cref="IBulletSpecifics.OnCollisionEnterSpecifics"/>
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionEnter(Collision collision)
        {
            // If the collision was with an enemy, damage it
            if (collision.gameObject.CompareTag("Enemy"))
            {
                _bulletSpecifics?.OnCollisionEnterSpecifics(this, collision);
            }
            
            // If the collision was with another bullet, ignore it.
            if (collision.gameObject.CompareTag("Bullet"))
            {
                return;
            }

            // If the pierce is less than 0, destroy the bullet
            if (pierce <= 0)
            {
                DestroyBullet();
            }
        }
        
        /// <summary>
        /// "Destroys" the bullet by returning it to the pool
        /// <seealso cref="IBulletSpecifics.DestroySpecifics"/>
        /// </summary>
        private void DestroyBullet()
        {
            _bulletSpecifics?.DestroySpecifics(this);
            ReturnToPool();
        }

        private void OnDisable()
        {
            _bulletSpecifics?.DisableSpecifics(this);
        }

        #endregion

        #region Methods

        public void SetBulletBehavior(IBulletBehavior bulletBehavior)
        {
            _bulletBehavior = bulletBehavior;
        }
        public IBulletBehavior GetBulletBehavior()
        {
            return _bulletBehavior;
        }
        public void SetBulletSpecifics(IBulletSpecifics bulletBehavior)
        {
            _bulletSpecifics = bulletBehavior;
        }
        public IBulletSpecifics GetBulletSpecifics()
        {
            return _bulletSpecifics;
        }
        public void SetTargetTransform(Transform targetTransform)
        {
            _targetTransform = targetTransform;
        }
        public void SetSpeed(float speed)
        {
            this.speed = speed;
            GetComponent<ProjectileMover>().speed = speed;
        }
        public void SetPool(ObjectPool.Pool pool)
        {
            _pool = pool;
        }

        public void ReturnToPool()
        {
            _pool.ReturnObjectToPool(gameObject);
        }

        #endregion

    }
}