using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Serialization;
using Sirenix.OdinInspector;
using Random = Unity.Mathematics.Random;

namespace Scenes.OOP_Test
{
    

    public class ProjectileManager : MonoBehaviour
    {

        #region Public Variables

        [LabelText("Attack Arc")] public int2 bulletAttackArcRange = new int2(-45, 45);
        [LabelText("Amount")] public int bulletAmountDistributed = 1; // How many bullets are distributed in the attack arc
        [LabelText("Amount Per Wave")] public int bulletAmmountInEach; // How many attack arcs are created (if any)

        [LabelText("Amount Group, Arc")]
        public int2 bulletAmmountInGroup; // How many bullets are spawned in a group (if any) and the arc variation between them

        [LabelText("Speed Range")] public float2 bulletSpeedRange = (new float2(1, 2));
        [LabelText("Speed Increment")] public float bulletSpeedIncrement = 0;
        [LabelText("Delay Waves")] public float2 bulletDelayBetweenWaves;
        [LabelText("Delay Attacks")] public float2 bulletDelayBetweenAttacks;

        #endregion

        #region Private Variables

        private const float waveGrowth_bulletSpeed = 0.5f;

        // A function that determines a minimun expected number of bullets to be spawned by wave number
        // It should increase exponentially up to wave 100
        // It should start at 10 bullets and end with 100 at wave 100
        private int GetMinExpectedBulletsByWave(int waveNumber)
        {
            return (int)math.round(math.pow(waveNumber, 2) / 10);
        }

        // A pool (list) of bullets that are currently active
        private List<GameObject> activeBullets = new List<GameObject>();

        // A pool (list) of bullets that are currently inactive
        private List<GameObject> inactiveBullets = new List<GameObject>();

        #endregion

        #region Properties

        #endregion

        #region MonoBehaviours

        private void Awake()
        {
            // Get the AbilityGOManager instance
            var bulletPrefabs = AbilityGOManager.Instance.bullets;

            // Instantiate the first bullet prefab and add it to the inactive bullets pool (x10)
            for (int i = 0; i < 100; i++)
            {
                var bullet = Instantiate(bulletPrefabs[2]);
                bullet.SetActive(false);
                inactiveBullets.Add(bullet);
            }

        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns a BulletSpawnParameters object with the parameters for the bullet spawn
        /// </summary>
        /// <param name="enemyTypeData"> The enemy type data to use (<see cref="EnemyTypeData"/>) </param>
        /// <param name="waveNumber"> The current wave number </param>
        /// <param name="bulletType"> The bullet type to use (<see cref="BulletType"/>) </param>
        /// <param name="bulletSpawnBehaviour"> The bullet behaviour to use (<see cref="BulletSpawnBehaviour"/>) </param>
        /// <param name="bulletSpawnOffset"> The offset from the enemy's position where the bullets are spawned </param>
        /// <param name="bulletSpawnRadius"> The radius of the circle where the bullets are spawned </param>
        /// <param name="bulletAttackArcRange"> The range of the attack arc </param>
        /// <param name="bulletAmountDistributed"> How many bullets are distributed in the attack arc </param>
        /// <param name="bulletAmmountInEach"> How many attack arcs are created (if any) </param>
        /// <param name="bulletAmmountInGroup"> How many bullets are spawned in a group (if any) and the arc variation between them </param>
        /// <param name="bulletDelayBetweenWaves"> The delay between each wave of bullets </param>
        /// <param name="bulletDelayBetweenAttacks"> The delay between each attack </param>
        /// <returns></returns>
        public BulletSpawnParameters GetBulletSpawnParameters(EnemyTypeData enemyTypeData, int waveNumber,
            BulletType bulletType, BulletSpawnBehaviour bulletSpawnBehaviour, BulletBehaviour bulletBehaviour,
            float2 bulletSpawnOffset = new float2(), float bulletSpawnRadius = 0f,
            int2 bulletAttackArcRange = new int2(), int bulletAmountDistributed = 0, int bulletAmmountInEach = 0,
            int2 bulletAmmountInGroup = new int2(), float2 bulletDelayBetweenWaves = new float2(),
            float2 bulletDelayBetweenAttacks = new float2())
        {
            BulletSpawnParameters parameters = new BulletSpawnParameters();

            // Calculate and assign parameters based on enemy type and wave number
            parameters.type = bulletType;
            parameters.spawnBehaviour = bulletSpawnBehaviour;
            parameters.bulletBehaviour = bulletBehaviour;

            parameters.spawnOffset = bulletSpawnOffset is { x: 0, y: 0 }
                ? new float2(0, 0)
                : bulletSpawnOffset;

            parameters.radius = bulletSpawnRadius == 0f
                ? 0f
                : bulletSpawnRadius;

            parameters.attackArcRange = bulletAttackArcRange is { x: 0, y: 0 }
                ? this.bulletAttackArcRange
                : bulletAttackArcRange;

            parameters.amountDistributed = bulletAmountDistributed == 0
                ? this.bulletAmountDistributed
                : bulletAmountDistributed;

            parameters.amountInEach = bulletAmmountInEach == 0
                ? this.bulletAmmountInEach
                : bulletAmmountInEach;

            parameters.amountInGroup = bulletAmmountInGroup is { x: 0, y: 0 }
                ? this.bulletAmmountInGroup
                : bulletAmmountInGroup;

            parameters.delayBetweenWaves = bulletDelayBetweenWaves is { x: 0, y: 0 }
                ? this.bulletDelayBetweenWaves
                : bulletDelayBetweenWaves;

            parameters.delayBetweenAttacks = bulletDelayBetweenAttacks is { x: 0, y: 0 }
                ? this.bulletDelayBetweenAttacks
                : bulletDelayBetweenAttacks;

            // Speed range should be calculated based on the wave number
            // Min (x) should be the base speed
            // Max (y) should be the base speed + wave number * speed increase per wave
            parameters.speedRange = new float2(bulletSpeedRange.x, (bulletSpeedRange.x + bulletSpeedRange.y + bulletSpeedIncrement) / 2);


            // Adjust parameters based on the enemy type
            if (enemyTypeData.enemyType != EnemyType.NONE)
            {
                parameters = ApplyEnemyTypeParameters(parameters, enemyTypeData);
            }

            return parameters;
        }

        public BulletSpawnParameters GetBulletSpawnParameters(EnemyTypeData enemyTypeData, int waveNumber, BulletSpawnParameters parameters)
        {
            return GetBulletSpawnParameters(enemyTypeData, waveNumber, parameters.type, parameters.spawnBehaviour, parameters.bulletBehaviour,
                parameters.spawnOffset, parameters.radius, parameters.attackArcRange, parameters.amountDistributed,
                parameters.amountInEach, parameters.amountInGroup, parameters.delayBetweenWaves,
                parameters.delayBetweenAttacks);
        }

        public BulletSpawnParameters GetDefaultParameters()
        {
            BulletSpawnParameters parameters = new BulletSpawnParameters();

            parameters.type = BulletType.Normal;
            parameters.spawnBehaviour = BulletSpawnBehaviour.FixedAngle;
            parameters.bulletBehaviour = BulletBehaviour.NONE;

            parameters.spawnOffset = new float2(0, 0);
            parameters.radius = 0f;

            parameters.attackArcRange = this.bulletAttackArcRange;
            parameters.amountDistributed = 0;
            parameters.amountInEach = 0;
            parameters.amountInGroup = new int2(0, 0);

            parameters.delayBetweenWaves = new float2(0, 0);
            parameters.delayBetweenAttacks = new float2(0, 0);

            parameters.speedRange = this.bulletSpeedRange;

            return parameters;
        }

        private BulletSpawnParameters ApplyEnemyTypeParameters(BulletSpawnParameters parameters, EnemyTypeData enemyTypeData)
        {
            // Initialize unity.mathematics random
            Random randomGenerator = new((uint)DateTime.Now.Ticks);

            parameters.type = enemyTypeData.bulletType;

            // With the enemy type and bullet type, calculate the parameters
            switch (enemyTypeData.enemyType)
            {
                case EnemyType.NONE:
                    break;
                case EnemyType.Basic:
                    break;
                case EnemyType.Fast:
                    break;
                case EnemyType.Tank:
                    break;
                case EnemyType.Boss:
                    break;
                default:
                    break;
            }

            return parameters;
        }

        public float CalculateBulletsInAttack(BulletSpawnParameters parameters)
        {
            return parameters.amountDistributed * parameters.amountInEach * parameters.amountInGroup.x;
        }

        public void UpdateDefaultParameters(int waveNumber)
        {
            // Update the default parameters based on the current wave number
            // This should be called at the start of each wave
            // This takes into account that the wave number is 1-based
            // Max wave number is 100 (for now)

            // Min 1 -> Wave 1, Max 12 -> Wave 100
            bulletAmountDistributed = (int)math.lerp(1, 12, waveNumber / 100f);
            // Min 1 -> Wave 1, Max 5 -> Wave 100
            bulletAmmountInEach = (int)math.lerp(1, 5, waveNumber / 100f);
            // x = Min 1 -> Wave 1, Max 4 -> Wave 100
            // y = Min 0 -> Wave 1, Max 10 -> Wave 100 (degrees)
            bulletAmmountInGroup = new int2((int)math.lerp(1, 4, waveNumber / 100f), (int)math.lerp(0, 10, waveNumber / 100f));

            // x = 1 -> Wave 1, 5 -> Wave 100
            // y = 1 -> Wave 1, 10 -> Wave 100
            // Breakpoints at 20, 40, 60, 80 and 100
            if (waveNumber % 20 == 0)
            {
                bulletSpeedRange = new float2(1 * (waveNumber / 20f), 2 * (waveNumber / 20f));
            }

            bulletSpeedIncrement = math.pow(waveNumber / 100f, waveGrowth_bulletSpeed) * 12f;

            // x = 0.5 -> Wave 1, 0.1 -> Wave 100
            // y = 1 -> Wave 1, 0.5 -> Wave 100
            bulletDelayBetweenWaves = new float2(math.lerp(0.5f, 0.1f, waveNumber / 100f), math.lerp(1f, 0.5f, waveNumber / 100f));

            // x = 0 -> Wave 1, 0.25 -> Wave 100
            // y = 0.1 -> Wave 1, 1 -> Wave 100
            bulletDelayBetweenAttacks = new float2(math.lerp(0f, 0.25f, waveNumber / 100f), math.lerp(0.1f, 1f, waveNumber / 100f));

        }

        public void SpawnProjectile(BulletSpawnParameters parameters)
        {
            // Search the player ship for the bullet spawn point
            Transform bulletSpawnPoint = GameObject.FindGameObjectWithTag("Player").transform;

            // With the forward vector, set the arc attack range
            int2 arcAttackRange = parameters.attackArcRange;
            int ammountOfBullets = parameters.amountDistributed;

            // Define the angles
            List<float> angles = new List<float>();
            if (ammountOfBullets > 1)
            {
                // We add the arc.x and arc.y
                angles.Add(arcAttackRange.x);
                angles.Add(arcAttackRange.y);

                int fullRange = arcAttackRange.y - arcAttackRange.x;
                for (int i = 0; i < ammountOfBullets - 2; i++)
                {
                    // the angle between bullets should be the same from the first (arc.x) to the last (arc.y)
                    angles.Add(arcAttackRange.x + (fullRange / (ammountOfBullets - 1f)) * (i + 1));
                }
            }
            else
            {
                angles.Add(0);
            }

            // Spawn the bullets
            for (int i = 0; i < ammountOfBullets; i++)
            {
                // We move a pool bullet to the spawn point
                // Its rotation should be the forward vector of the player ship
                // Rotated by the angle we want (angles[i])
                // And the position should be the bullet spawn point

                // Get the bullet from the pool
                if (inactiveBullets.Count > 0)
                {
                    // Get the first bullet in the list
                    GameObject bullet = inactiveBullets[0];
                    // Remove it from the list
                    inactiveBullets.RemoveAt(0);
                    // Add it to the active bullets list
                    activeBullets.Add(bullet);

                    // Set the bullet position
                    bullet.transform.position = bulletSpawnPoint.position + new Vector3(parameters.spawnOffset.x, 0, parameters.spawnOffset.y);
                    // Set the bullet rotation
                    bullet.transform.rotation = Quaternion.Euler(0, angles[i], 0);
                    // Set the bullet speed
                    // Get a random speed between the range
                    var speed = 0f;

                    // Set a new seed for the random
                    UnityEngine.Random.InitState((int)DateTime.Now.Ticks);

                    switch (parameters.type)
                    {
                        case BulletType.Normal:
                            speed = UnityEngine.Random.Range(parameters.speedRange.x, parameters.speedRange.y) * 10;
                            break;
                        case BulletType.Exploding:
                            speed = UnityEngine.Random.Range(parameters.speedRange.x, parameters.speedRange.y) * 5;
                            bullet.GetComponent<Rigidbody>().mass *= 3;
                            break;
                        case BulletType.Homing:
                            speed = UnityEngine.Random.Range(parameters.speedRange.x, parameters.speedRange.y) * 10;
                            bullet.GetComponent<Rigidbody>().mass *= 10f;
                            break;
                        default:
                            speed = UnityEngine.Random.Range(parameters.speedRange.x, parameters.speedRange.y) * 5;
                            break;
                    }

                    Debug.Log("Speed range: " + parameters.speedRange.x + " - " + parameters.speedRange.y + " | Speed: " + speed);

                    // Add the component to the bullet depending on the behaviour
                    switch (parameters.bulletBehaviour)
                    {
                        case BulletBehaviour.NONE:
                            break;
                        case BulletBehaviour.Directed:
                            bullet.GetComponent<BulletBehaviourMono>().SetBulletBehavior(new Directed());
                            break;
                    }

                    bullet.GetComponent<BulletBehaviourMono>().SetSpeed(speed);

                    // Set it active
                    bullet.SetActive(true);
                }
            }

        }

        #endregion



    }

    /// <summary> 
    /// Struct that contains all the parameters needed to spawn a bullet. 
    /// <param name="type">The type of the bullet (<see cref="BulletType"/>)</param>
    /// <param name="behaviour">The behaviour of the bullet (<see cref="BulletSpawnBehaviour"/>)</param>
    /// <param name="spawnOffset">The offset from the enemy's position where the bullets are spawned</param>
    /// <param name="radius">The radius of the circle where the bullets are spawned</param>
    /// <param name="attackArcRange">The range of the attack arc</param>
    /// <param name="amountDistributed">How many bullets are distributed in the attack arc</param>
    /// <param name="amountInEach">How many attack arcs are created (if any)</param>
    /// <param name="amountInGroup">How many bullets are spawned in a group (if any) and the arc variation between them</param>
    /// <param name="speedRange">The range of speeds the bullet can have</param>
    /// <param name="delayBetweenWaves">The delay between each wave of bullets</param>
    /// <param name="delayBetweenAttacks">The delay between each attack</param> 
    /// </summary>
    public struct BulletSpawnParameters
    {
        public BulletType type;
        public BulletSpawnBehaviour spawnBehaviour;
        public BulletBehaviour bulletBehaviour;

        public float2 spawnOffset; // X,Y
        public float radius; // Radius of the circle where the bullets are spawned

        public int2 attackArcRange;
        public int amountDistributed; // How many bullets are distributed in the attack arc
        public int amountInEach; // How many attack arcs are created (if any)
        public int2 amountInGroup; // How many bullets are spawned in a group (if any) and the arc variation between them

        public float2 speedRange;
        public float2 delayBetweenWaves;
        public float2 delayBetweenAttacks;

        // A ToString method to print the parameters
        public override string ToString()
        {
            return "BulletSpawnParameters:\n" +
                   "type: " + type + "\n" +
                   "behaviour: " + spawnBehaviour + "\n" +
                   "spawnOffset: " + spawnOffset + "\n" +
                   "radius: " + radius + "\n" +
                   "attackArcRange: " + attackArcRange + "\n" +
                   "amountDistributed: " + amountDistributed + "\n" +
                   "amountInEach: " + amountInEach + "\n" +
                   "amountInGroup: " + amountInGroup + "\n" +
                   "speedRange: " + speedRange + "\n" +
                   "delayBetweenWaves: " + delayBetweenWaves + "\n" +
                   "delayBetweenAttacks: " + delayBetweenAttacks + "\n";
        }
    }

    /// <summary>
    /// Enum that contains all the bullet types.
    /// Explanation of each bullet type:
    /// <list type="bulletType">
    /// <item><description><see cref="Normal"/>: A normal bullet that goes straight forward</description></item>
    /// <item><description><see cref="Homing"/>: A bullet that follows the player</description></item>
    /// <item><description><see cref="Exploding"/>: A bullet that explodes when it hits the player</description></item>
    /// </list>
    /// </summary>
    public enum BulletType
    {
        // One for each GO
        Normal,
        Homing,
        Exploding,
        // Add more bullet types as needed
    }

    /// <summary>
    /// Enum that contains all the bullet behaviours.
    /// Explanation of each bullet behaviour:
    /// <list type="bulletBehaviour">
    /// <item><description><see cref="FixedAngle"/>: The bullet is spawned with a fixed angle</description></item>
    /// <item><description><see cref="RandomRange"/>: The bullet is spawned with a random angle within a range</description></item>
    /// <item><description><see cref="ChangingAngle"/>: The bullet is spawned with a random angle within a range and the angle changes over time.<br/>
    /// This is done by changing the spawner's forward point</description></item>
    /// <item><description><see cref="Aimed"/>: The bullet is spawned with an angle aimed at the player.<br/>
    /// This is done by aiming the spawner's forward to the player</description></item>
    /// <item><description><see cref="Directed"/>: The bullet homes towards the player</description></item>
    /// </list>
    /// </summary>
    public enum BulletSpawnBehaviour
    {
        FixedAngle,
        RandomRange,
        ChangingAngle, // Rotate the forward 
        Aimed, // Forward is aimed at the player
        Directed
    }

    public enum BulletBehaviour
    {
        NONE,
        RandomRange,
        ChangingAngle, // Rotate the forward 
        Aimed, // Forward is aimed at the player
        Directed
    }

    /// <summary>
    /// Struct that contains all the parameters needed to spawn an enemy.
    /// <param name="enemyType">The type of the enemy (<see cref="EnemyType"/>)</param>
    /// <param name="bulletType">The type of the bullet (<see cref="BulletType"/>)</param>
    /// <param name="attackArcRange">The range of the attack arc (forward is 0 degrees)</param>
    /// <param name="bulletAmountRange">The range of the amount of bullets spawned in each attack</param>
    /// <param name="bulletSpeedRange">The range of the speed of the bullets</param>
    /// <param name="bulletDamageRange">The range of the damage of the bullets</param>
    /// </summary>
    public struct EnemyTypeData
    {
        public EnemyType enemyType;
        public BulletType bulletType;
        public int2 attackArcRange;
        public int2 bulletAmountRange;
        public float2 bulletSpeedRange;
        public float2 bulletDamageRange;

        public EnemyTypeData(EnemyType enemyType, BulletType bulletType,
            int2 attackArcRange, int2 bulletAmountRange, float2 bulletSpeedRange,
            float2 bulletDamageRange)
        {
            this.enemyType = enemyType;
            this.bulletType = bulletType;
            this.attackArcRange = attackArcRange;
            this.bulletAmountRange = bulletAmountRange;
            this.bulletSpeedRange = bulletSpeedRange;
            this.bulletDamageRange = bulletDamageRange;
        }
    }

    public enum EnemyType
    {
        NONE,
        Basic,
        Fast,
        Tank,

        Boss
        // Add more enemy types as needed
    }

}