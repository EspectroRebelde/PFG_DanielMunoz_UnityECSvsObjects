
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{

    #region Public Variables

    public Transform playerTransform;
    public int health;
    public int damage;
    public float speed;
    public EnemyManager _enemyManager { get; set; }
    
    #endregion

    #region Private Variables

    private Rigidbody _rigidbody;
    
    #endregion

    #region Properties

    #endregion

    #region MonoBehaviours

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }
    
    void Update()
    {
        // Rotate towards the player on the Y axis only, lerping over time
        // Then move forward
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(playerTransform.position - transform.position, Vector3.up), Time.deltaTime * 5);
        transform.position += transform.forward * (speed * Time.deltaTime);
    }
    
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Enemy collided with " + collision.gameObject.name);
    }
    
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enemy triggered with " + other.gameObject.name);
    }
    
    #endregion

    #region Methods

    public void TakeDamage(int dmg)
    {
        health -= dmg;
        
        if (health <= 0)
        {
            DestroyEnemy();
        }
    }
    
    public void DestroyEnemy()
    {
        _enemyManager.DeactivateEnemy(gameObject);
    }
    
    #endregion


    
}
