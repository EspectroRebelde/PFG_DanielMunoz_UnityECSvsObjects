
using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Scenes.OOP_Test
{
    
    [RequireComponent(typeof(ProjectileMover))]
    public class BulletBehaviourMono : MonoBehaviour
    {

        #region Public Variables

        #endregion

        #region Private Variables

        private float speed { get; set; } = 0;

        #endregion

        #region Properties

        private IBulletBehavior_Basic _bulletBehavior = new FixedAngle();
        private Transform _targetTransform;

        // A string to show the bullet behavior in the inspector
        [ShowInInspector]
        private string BulletBehavior
        {
            get
            {
                if (_bulletBehavior is FixedAngle)
                {
                    return "Fixed Angle";
                }
                else if (_bulletBehavior is RandomRange)
                {
                    return "Random Range";
                }
                else if (_bulletBehavior is ChangingAngle)
                {
                    return "Changing Angle";
                }
                else if (_bulletBehavior is Aimed)
                {
                    return "Aimed";
                }
                else if (_bulletBehavior is Directed)
                {
                    return "Directed";
                }
                else
                {
                    return "Unknown";
                }
            }
        }

        #endregion

        #region MonoBehaviours

        private void Start()
        {
            // Set the speed of the bullet on the projectile mover
            _bulletBehavior.speed = speed;
            _bulletBehavior?.SetUpdateBullet(this, _targetTransform);
        }

        private void FixedUpdate()
        {
            _bulletBehavior?.UpdateBullet(this, _targetTransform);
        }

        #endregion

        #region Methods

        public void SetBulletBehavior(IBulletBehavior_Basic bulletBehavior)
        {
            _bulletBehavior = bulletBehavior;
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

        #endregion

    }

    public interface IBulletBehavior_Basic
    {
        public float speed { get; set; }
        void SetUpdateBullet(BulletBehaviourMono bullet, Transform targetTransform);
        void UpdateBullet(BulletBehaviourMono bullet, Transform targetTransform);
    }

    public class FixedAngle : IBulletBehavior_Basic
    {
        public float speed { get; set; } = 0;

        public void SetUpdateBullet(BulletBehaviourMono bullet, Transform targetTransform)
        {
        }

        public void UpdateBullet(BulletBehaviourMono bullet, Transform targetTransform)
        {

        }
    }

    public class RandomRange : IBulletBehavior_Basic
    {
        public float speed { get; set; } = 0;

        public void SetUpdateBullet(BulletBehaviourMono bullet, Transform targetTransform)
        {
        }

        public void UpdateBullet(BulletBehaviourMono bullet, Transform targetTransform)
        {
        }
    }

    public class ChangingAngle : IBulletBehavior_Basic
    {
        public float speed { get; set; } = 0;

        public void SetUpdateBullet(BulletBehaviourMono bullet, Transform targetTransform)
        {
        }

        public void UpdateBullet(BulletBehaviourMono bullet, Transform targetTransform)
        {
        }
    }

    public class Aimed : IBulletBehavior_Basic
    {
        public float speed { get; set; } = 0;

        public void SetUpdateBullet(BulletBehaviourMono bullet, Transform targetTransform)
        {
            bullet.transform.LookAt(targetTransform);
        }

        public void UpdateBullet(BulletBehaviourMono bullet, Transform targetTransform)
        {
        }
    }

    public class Directed : IBulletBehavior_Basic
    {
        public float speed { get; set; } = 0;

        public void SetUpdateBullet(BulletBehaviourMono bullet, Transform targetTransform)
        {
            Transform playerTransform = GameObject.FindWithTag("Player").transform;
            bullet.SetTargetTransform(playerTransform);
        }

        public void UpdateBullet(BulletBehaviourMono bullet, Transform targetTransform)
        {
            // Lerp to the target rotation over 4 seconds
            bullet.transform.rotation = Quaternion.Lerp(bullet.transform.rotation,
                Quaternion.LookRotation(targetTransform.position - bullet.transform.position), Time.deltaTime * 4);
        }
    }
}