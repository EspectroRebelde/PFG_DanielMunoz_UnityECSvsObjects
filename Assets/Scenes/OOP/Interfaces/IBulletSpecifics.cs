using UnityEngine;

namespace Scenes.Test
{
    /// <summary>
    /// Handles the specifics of bullets:
    /// Update specifics
    /// Collision specifics
    /// Destroy specifics
    /// </summary>
    public interface IBulletSpecifics
    {
        void UpdateSpecifics(BulletBehaviour bulletBehaviour);
        void OnCollisionEnterSpecifics(BulletBehaviour bulletBehaviour, Collision collision);
        void DestroySpecifics(BulletBehaviour bulletBehaviour);
        void DisableSpecifics(BulletBehaviour bulletBehaviour);
    }
    
    public class BulletSpecifics_None : IBulletSpecifics
    {
        public void UpdateSpecifics(BulletBehaviour bulletBehaviour)
        {
            
        }

        public void OnCollisionEnterSpecifics(BulletBehaviour bulletBehaviour, Collision collision)
        {
            
        }

        public void DestroySpecifics(BulletBehaviour bulletBehaviour)
        {
            
        }

        public void DisableSpecifics(BulletBehaviour bulletBehaviour)
        {
            
        }
    }
    
    /// <summary>
    /// Specifics for a bullet of type Void
    /// </summary>
    public class BulletSpecifics_Void : IBulletSpecifics
    {
        public void UpdateSpecifics(BulletBehaviour bulletBehaviour)
        {
        }

        public void OnCollisionEnterSpecifics(BulletBehaviour bulletBehaviour, Collision collision)
        {
            collision.gameObject.GetComponent<EnemyBehaviour>().TakeDamage(bulletBehaviour.damage);
            bulletBehaviour.pierce--;
        }

        public void DestroySpecifics(BulletBehaviour bulletBehaviour)
        {
        }

        public void DisableSpecifics(BulletBehaviour bulletBehaviour)
        {
            
        }
    }
    
    /// <summary>
    /// Specifics for a bullet of type Sword
    /// </summary>
    public class BulletSpecifics_BaseBullet : IBulletSpecifics
    {
        public void UpdateSpecifics(BulletBehaviour bulletBehaviour)
        {
        }

        public void OnCollisionEnterSpecifics(BulletBehaviour bulletBehaviour, Collision collision)
        {
            collision.gameObject.GetComponent<EnemyBehaviour>().TakeDamage(bulletBehaviour.damage);
            bulletBehaviour.pierce--;
        }

        public void DestroySpecifics(BulletBehaviour bulletBehaviour)
        {
        }

        public void DisableSpecifics(BulletBehaviour bulletBehaviour)
        {
            
        }
    }
    
    /// <summary>
    /// Specifics for a bullet of type Shock
    /// </summary>
    public class BulletSpecifics_Shock : IBulletSpecifics
    {
        public void UpdateSpecifics(BulletBehaviour bulletBehaviour)
        {
        }

        public void OnCollisionEnterSpecifics(BulletBehaviour bulletBehaviour, Collision collision)
        {
            collision.gameObject.GetComponent<EnemyBehaviour>().TakeDamage(bulletBehaviour.damage);
            bulletBehaviour.pierce--;
        }

        public void DestroySpecifics(BulletBehaviour bulletBehaviour)
        {
            float explosionRadius = 10.0f;
            int explosionDamage = 5;
            // transparent red (RGBA) to unity.color
            Color explosionColor = new Color(1.0f, 0.0f, 0.0f, 0.5f);
            
            // A explosionn occurs when the bullet is destroyed
            // It damages all enemies in a radius
            Collider[] colliders = Physics.OverlapSphere(bulletBehaviour.transform.position, explosionRadius);

            // Damage all enemies in the radius
            foreach (Collider collider in colliders)
            {
                // Check the tag
                if (collider.gameObject.CompareTag("Enemy"))
                {
                    // Damage the enemy
                    collider.GetComponent<EnemyBehaviour>().TakeDamage(explosionDamage);
                }
            }
            
            // Draw a shape to show the explosion radius with the Shapes package
            Shapes.Draw.Sphere(bulletBehaviour.transform.position, explosionRadius, explosionColor);
            
        }

        public void DisableSpecifics(BulletBehaviour bulletBehaviour)
        {
            
        }
    }
    
}