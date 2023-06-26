
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scenes.DOP
{
    public class ConfigAuthoring : MonoBehaviour
    {
        // Player
        [Header("Player")]
        public float playerNormalSpeed = 10f;
        public float playerRotationSpeed = 10f;
        
        // Bullet
        [Space]
        [Header("Bullet")]
        public float bulletSpeed = 10f;
        public float bulletObstacleRadius = 0.5f;
        public float radiusToSpawnBullet = 4f;
        public GameObject bulletPrefab;
        public int bulletDamage = 10;
        public int2 waveArc = new int2(-45, 45);
        public int bulletsPerWave = 3;
        public bool isContinuousFiring = false;
        
        // Enemy
        [Space]
        [Header("Enemy")]
        public float2 minMaxRadiusToSpawn = new float2(100f, 150f);
        public int waveAmount = 100;
        public int enemyHealth = 100;
        public float enemySpeed = 10f;
        public GameObject enemyPrefab;
        
    
        public class ConfigAuthoringBaker : Baker<ConfigAuthoring>
        {
            public override void Bake(ConfigAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity,
                    new PlayerConfig
                    {
                        NormalSpeed = authoring.playerNormalSpeed,
                        RotationSpeed = authoring.playerRotationSpeed,
                    });
                AddComponent(entity,
                    new BulletConfig
                    {
                        BulletStartVelocity = authoring.bulletSpeed,
                        ObstacleRadius = authoring.bulletObstacleRadius,
                        Radius = authoring.radiusToSpawnBullet,
                        BulletPrefab = GetEntity(authoring.bulletPrefab, TransformUsageFlags.Dynamic),
                        Damage = authoring.bulletDamage,
                        ContinuousFiring = authoring.isContinuousFiring,
                        WaveArc = authoring.waveArc,
                        WaveAmount = authoring.bulletsPerWave
                    });
                AddComponent(entity,
                    new EnemyConfig
                    {
                        MinMaxRadiusToSpawn = authoring.minMaxRadiusToSpawn,
                        WaveAmount = authoring.waveAmount,
                        EnemyHealth = authoring.enemyHealth,
                        NormalSpeed = authoring.enemySpeed,
                        EnemyPrefab = GetEntity(authoring.enemyPrefab, TransformUsageFlags.Dynamic)
                    });
            }
        }
    }
    
    public struct PlayerConfig : IComponentData
    {
        public float NormalSpeed;
        public float RotationSpeed;
    }
    
    public struct BulletConfig : IComponentData
    {
        public float BulletStartVelocity;
        public float ObstacleRadius;
        public float Radius;
        public Entity BulletPrefab;
        public int Damage;
        public int2 WaveArc;
        public int WaveAmount;
        public bool ContinuousFiring;
    }
    
    public struct EnemyConfig : IComponentData
    {
        public float2 MinMaxRadiusToSpawn;
        public int WaveAmount;
        public float NormalSpeed;
        public int EnemyHealth;
        public Entity EnemyPrefab;
    }
}