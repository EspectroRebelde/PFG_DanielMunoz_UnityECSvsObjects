using UnityEngine;

namespace Scenes.Test
{
    public interface IBulletBehavior
    {
        public float speed { get; set; }
        void SetUpdateBullet(BulletBehaviour bullet, Transform targetTransform);
        void UpdateBullet(BulletBehaviour bullet, Transform targetTransform);
    }

    public class BulletBehavior_FixedAngle : IBulletBehavior
    {
        public float speed { get; set; } = 0;

        public void SetUpdateBullet(BulletBehaviour bullet, Transform targetTransform)
        {
        }

        public void UpdateBullet(BulletBehaviour bullet, Transform targetTransform)
        {
            if (speed != 0)
            {
                //bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * speed;
                bullet.transform.position += bullet.transform.forward * (speed * Time.deltaTime);     
            }
        }
    }

    public class BulletBehavior_RandomRange : IBulletBehavior
    {
        public float speed { get; set; } = 0;

        public void SetUpdateBullet(BulletBehaviour bullet, Transform targetTransform)
        {
        }

        public void UpdateBullet(BulletBehaviour bullet, Transform targetTransform)
        {
            if (speed != 0)
            {
                //bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * speed;
                bullet.transform.position += bullet.transform.forward * (speed * Time.deltaTime);       
            }
        }
    }

    public class BulletBehavior_ChangingAngle : IBulletBehavior
    {
        public float speed { get; set; } = 0;

        public void SetUpdateBullet(BulletBehaviour bullet, Transform targetTransform)
        {
        }

        public void UpdateBullet(BulletBehaviour bullet, Transform targetTransform)
        {
            if (speed != 0)
            {
                //bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * speed;
                bullet.transform.position += bullet.transform.forward * (speed * Time.deltaTime);         
            }
        }
    }

    public class BulletBehavior_Aimed : IBulletBehavior
    {
        public float speed { get; set; } = 0;

        public void SetUpdateBullet(BulletBehaviour bullet, Transform targetTransform)
        {
            bullet.transform.LookAt(targetTransform);
        }

        public void UpdateBullet(BulletBehaviour bullet, Transform targetTransform)
        {
            if (speed != 0)
            {
                //bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * speed;
                bullet.transform.position += bullet.transform.forward * (speed * Time.deltaTime);          
            }
        }
    }

    public class BulletBehavior_Directed : IBulletBehavior
    {
        public float speed { get; set; } = 0;

        public void SetUpdateBullet(BulletBehaviour bullet, Transform targetTransform)
        {
            Transform playerTransform = GameObject.FindWithTag("Player").transform;
            bullet.SetTargetTransform(playerTransform);
        }

        public void UpdateBullet(BulletBehaviour bullet, Transform targetTransform)
        {
            // Lerp to the target rotation over 4 seconds
            bullet.transform.rotation = Quaternion.Lerp(bullet.transform.rotation,
                Quaternion.LookRotation(targetTransform.position - bullet.transform.position), Time.deltaTime * 4);
        }
    }
}