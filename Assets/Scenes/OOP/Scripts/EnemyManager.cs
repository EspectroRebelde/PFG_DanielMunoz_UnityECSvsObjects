
using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    #region Public Variables
    
    public GameObject enemyPrefab;
    public int waveSize = 10;
    public KeyCode spawnEnemyWaveKey = KeyCode.Space;
    public float minRadius = 10;
    public float maxRadius = 20;
    public float speed = 5;
    public int health = 100;
    public int damage = 10;
    
    public Transform playerPosition;
    
    #endregion

    #region Private Variables

    private GameObject parentObject;
    private List<GameObject> activeEnemies = new List<GameObject>();
    private List<GameObject> inactiveEnemies = new List<GameObject>();
    
    #endregion

    #region Properties

    #endregion

    #region MonoBehaviours

    private void Start()
    {
        // Create a parent object to store all the enemies
        parentObject = new GameObject("Enemies");

        // Instantiate an enemy to manage and edit withouth editing the original prefab
        enemyPrefab = Instantiate(enemyPrefab, Vector3.zero, Quaternion.identity, parentObject.transform);
        enemyPrefab.SetActive(false);
        enemyPrefab.name = "Enemy";
        
        // Add the enemyBehaviour script to the enemy if it doesn't have it
        var enemyBehaviour = enemyPrefab.GetComponent<EnemyBehaviour>();
        if (enemyBehaviour == null)
        {
            enemyBehaviour = enemyPrefab.AddComponent<EnemyBehaviour>();
        }
        
        // Set the player transform
        enemyBehaviour.playerTransform = playerPosition;
        
        SetDefaultEnemyBehaviour();

        InitialPoolFill(3);

    }

    private void Update()
    {
        // If the player presses the space key, spawn a wave of enemies
        if (Input.GetKeyDown(spawnEnemyWaveKey))
        {
            SpawnEnemyWave();
        }
    }

    #endregion

    #region Methods

    private void SpawnEnemyWave()
    {
        // Distribute enemies in a circle around the player between minRadius and maxRadius
        // with random angles and random radius
        // They'll be pulled from the inactiveEnemies list and added to the activeEnemies list
        // if we run out of enemies in the inactiveEnemies list, we'll instantiate new ones
        // and add them to the inactiveEnemies list
        for (var i = 0; i < waveSize; i++)
        {
            // Get a random angle
            var angle = UnityEngine.Random.Range(0, 360);
            // Get a random radius
            var radius = UnityEngine.Random.Range(minRadius, maxRadius);
            // Get the position of the enemy
            var position = new Vector3(playerPosition.position.x + radius * Mathf.Cos(angle), playerPosition.position.y, playerPosition.position.z + radius * Mathf.Sin(angle));
            // Pull an enemy from the inactiveEnemies list
            var enemy = PullObject(position);
            // Activate the enemy
            enemy.SetActive(true);
            
            // Add the enemy to the activeEnemies list
            activeEnemies.Add(enemy);
        }
    }

    private GameObject PullObject(Vector3 position)
    {
        if (inactiveEnemies.Count > 0)
        {
            // Get the first object in the inactiveEnemies list
            var obj = inactiveEnemies[0];
            // Remove it from the inactiveEnemies list
            inactiveEnemies.RemoveAt(0);
            // Add it to the activeEnemies list
            activeEnemies.Add(obj);
            // Set the position of the object
            obj.transform.position = position;
            // Return the object
            return obj;
        }
        else
        {
            // Instantiate a new object
            var obj = InstantiateEnemyBasics(position);
            // Add it to the activeEnemies list
            activeEnemies.Add(obj);
            // Return the object
            return obj;
        }
    }

    private void InitialPoolFill(int multiplier = 1)
    {
        // Fill the inactiveEnemies list with enemies equal to the waveSize * multiplier
        for (var i = 0; i < waveSize * multiplier; i++)
        {
            GameObject enemy = InstantiateEnemyBasics();
            // Add it to the inactiveEnemies list
            inactiveEnemies.Add(enemy);
        }
    }

    private GameObject InstantiateEnemyBasics(Vector3 position = default)
    {
        var enemy = Instantiate(enemyPrefab, position, Quaternion.identity, parentObject.transform);
        var enemyBehaviour = enemy.GetComponent<EnemyBehaviour>();
        enemyBehaviour._enemyManager = this;
        // enemyBehaviour.playerTransform = playerPosition;
        // enemyBehaviour.speed = speed;
        // enemyBehaviour.health = health;
        // enemyBehaviour.damage = damage;

        // Deactivate it
        enemy.SetActive(false);
        
        return enemy;
    }
    
    private void SetDefaultEnemyBehaviour()
    {
        // Set the enemy behaviour
        var enemyBehaviour = enemyPrefab.GetComponent<EnemyBehaviour>();
        enemyBehaviour.speed = speed;
        enemyBehaviour.health = health;
        enemyBehaviour.damage = damage;
    }
    
    public void DeactivateEnemy(GameObject enemy)
    {
        // Deactivate it
        enemy.SetActive(false);
        // Remove the enemy from the activeEnemies list
        activeEnemies.Remove(enemy);
        // Add it to the inactiveEnemies list
        inactiveEnemies.Add(enemy);
    }
    
    #endregion


    
}
