using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

// ReSharper disable Unity.UnknownInputAxes

namespace Scenes.Test
{
    public class PlayerManager : MonoBehaviour
    {
        public float normalSpeed = 35f;
        public float accelerationSpeed = 45f;
        public Transform playerRoot;
        public Transform shootPoint;
        public float rotationSpeed = 3.0f;
        
        public Camera _camera;
        public bool relationalMovement = false;
        // Input for weapon change
        public KeyCode weaponChangeKey = KeyCode.Tab;
        public bool fullAuto = false;
        
        // Reference to a Shape object as line of sight
        public Shapes.Disc shootingRange;
        public Transform positionalRange;

        [ReadOnly]
        public bool isGamepad;
        
        private Vector3 movement;
        private Vector2 cameraGamepad;
        private float speed;
        private bool isShooting;
        private ObjectPool _objectPool;
        private WeaponBehaviour[] _weapons;
        private WeaponBehaviour _activeWeapon;

        [ValueDropdown("GetPoolNames")]
        public string poolChoice = "-";
        private string[] GetPoolNames()
        {
            if (_objectPool != null)
            {
                return _objectPool.GetPoolNames();
            }
            return new string[] {""};
        }
        
        private float timeToShoot;
        private float shootDelay;
        
        // Start is called before the first frame update
        private void Start()
        {
            // If a gamepad is connected, we set the isGamepad to true
            isGamepad = Input.GetJoystickNames().Length > 0;
            // Get ObjectPool instance
            _objectPool = ObjectPool.SharedInstance;
            
            // We need to create a weapon for each pool
            _weapons = new WeaponBehaviour[_objectPool.GetPoolNames().Length];
            for (int i = 0; i < _objectPool.GetPoolNames().Length; i++)
            {
                _weapons[i] = gameObject.AddComponent<WeaponBehaviour>();
                _weapons[i]._bulletPool = _objectPool.GetPool(_objectPool.GetPoolNames()[i]);
                _weapons[i].bulletSpawnParameters = _objectPool.objectsToPool[i].bulletSpawnParameters;
            }
            
            // Set the active weapon
            _activeWeapon = _weapons[0];
            
            UpdateShootCooldown();
            UpdateShootingRange();
        }

        private void Update()
        {
            PlayerMovementInput();
            PlayerShootInput();
            if (Input.GetKeyDown(weaponChangeKey))
            {
                ChangeWeapon();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }

            if (Input.GetKeyDown(KeyCode.F1))
            {
                relationalMovement = !relationalMovement;
            }
            if (Input.GetKeyDown(KeyCode.F2))
            {
                fullAuto = !fullAuto;
            }

        }
        
        private void ChangeWeapon()
        {
            // Get the index of the active weapon
            int index = System.Array.IndexOf(_weapons, _activeWeapon);
            // If the index is the last one, we set the index to 0
            // If not, we add 1 to the index
            index = index == _weapons.Length - 1 ? 0 : index + 1;
            // Set the active weapon
            _activeWeapon = _weapons[index];

            UpdateShootCooldown();

            UpdateShootingRange();
        }

        private void UpdateShootingRange()
        {
            // Get the correct shooting range or (-10/10 to radians) if the arc is 0
            shootingRange.AngRadiansEnd = _activeWeapon.bulletSpawnParameters.attackArcRange.y == 0 ? 10 * Mathf.Deg2Rad : _activeWeapon.bulletSpawnParameters.attackArcRange.y * Mathf.Deg2Rad;
            shootingRange.AngRadiansStart = _activeWeapon.bulletSpawnParameters.attackArcRange.x == 0 ? -10 * Mathf.Deg2Rad : _activeWeapon.bulletSpawnParameters.attackArcRange.x * Mathf.Deg2Rad;

        }

        private void UpdateShootCooldown()
        {
            // Reset the time to shoot and the shoot delay
            shootDelay = _activeWeapon.bulletSpawnParameters.cooldown;
            timeToShoot = timeToShoot > shootDelay ? shootDelay : timeToShoot;
        }

        private void PlayerMovementInput()
        {
            // If is gamepad, we get the input from the gamepad
            // If not, we get the input from the keyboard
            // movement = new Vector3(Input.GetAxis("Horizontal"), 0 ,Input.GetAxis("Vertical")).normalized;
            // movementGamepad = new Vector3(Input.GetAxis("HorizontalGamepad"), 0 ,Input.GetAxis("VerticalGamepad")).normalized;
            movement = isGamepad ? 
                new Vector3(
                    Input.GetAxis("HorizontalGamepad"), 
                    0,
                    Input.GetAxis("VerticalGamepad")
                ).normalized : 
                new Vector3(
                    Input.GetAxis("Horizontal"),
                    0,
                    Input.GetAxis("Vertical")
                ).normalized;
            
            // If we have either horizontal or vertical input we lerp the speed to the acceleration speed
            // If we don't have any input we lerp the speed to the normal speed
            if (movement.x != 0 || movement.z != 0)
            {
                speed = Mathf.Lerp(speed, accelerationSpeed, Time.fixedDeltaTime * 3);
            }
            else
            {
                speed = Mathf.Lerp(speed, normalSpeed, Time.fixedDeltaTime * 10);
            }
            
            cameraGamepad = new Vector2(Input.GetAxis("HCameraGamepad"), Input.GetAxis("VCameraGamepad"));
        }

        private void PlayerShootInput()
        {
            if (!(timeToShoot - Time.time <= 0)) return;
            
            if (!fullAuto)
            {
                // If we press the left mouse button or Space or rightTrigger in gamepad
                if (Input.GetMouseButtonDown(0) || Input.GetAxis("FireGamepad") > 0)
                {
                    isShooting = true;
                }
            }
            else
            {
                // If we press the left mouse button or Space or rightTrigger in gamepad
                if (Input.GetMouseButton(0) || Input.GetAxis("FireGamepad") > 0)
                {
                    isShooting = true;
                }
            }
        }

        private void FixedUpdate()
        {
            PlayerRotation();

            PlayerPhysicMovement();
            
            PlayerShoot();
        }

        /// <summary>
        /// The player rotates following the mouse position on the screen
        /// The player moves on the XZ plane, so we only rotate on the Y axis
        /// </summary>
        private void PlayerRotation()
        {
            Quaternion rotation;
            // If we have cameraGamepad input
            if (isGamepad)
            {
                // We look at the direction of the cameraGamepad
                // If we don't have any input we keep the current rotation
                if (cameraGamepad.x != 0 || cameraGamepad.y != 0)
                {
                    rotation = Quaternion.LookRotation(new Vector3(cameraGamepad.x, 0, cameraGamepad.y));
                }
                else
                {
                    rotation = playerRoot.rotation;
                }
            }
            else
            {
                // We need to raycast the mouse position to the world
                // We use the camera to get the mouse position
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
                // We raycast the mouse position to the world
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    // We get the direction from the ship to the mouse position
                    Vector3 direction = hit.point - playerRoot.position;
                    // We remove the Y component of the direction
                    direction.y = 0;
                    // We look at the direction
                    rotation = Quaternion.LookRotation(direction);
                }
                else
                {
                    // If we don't hit anything we keep the current rotation
                    rotation = playerRoot.rotation;
                }
                
            }
            
            // We apply the rotation to the ship
            playerRoot.rotation = Quaternion.Slerp(playerRoot.rotation, rotation, rotationSpeed * Time.fixedDeltaTime);
            
            if (relationalMovement)
            {
                // Rotate the positionalrange too
                positionalRange.transform.rotation = playerRoot.rotation;
            }
            else
            {
                // Rotate the positionalrange too
                positionalRange.transform.rotation = Quaternion.identity;
            }
        }

        /// <summary>
        /// The player moves in the XZ plane
        /// The inputs come from the InputManager (WASD or arrows + gamepad)
        /// The forward direction is the direction the player is facing
        /// </summary>
        private void PlayerPhysicMovement()
        {
            Vector3 movementDirection;
            // If we have movementGamepad
            if (isGamepad)
            {
                // The movement direction is the movementGamepad direction
                movementDirection = movement;
            }
            else
            {
                // We calculate the movement direction
                movementDirection = relationalMovement ? playerRoot.forward * movement.z + playerRoot.right * movement.x : movement;
            }
            
            // We lerp the position to the new position
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, gameObject.transform.position + movementDirection * speed, Time.fixedDeltaTime);
        }
        
        private void PlayerShoot()
        {
            if (isShooting)
            {
                shootPoint.transform.localRotation = playerRoot.transform.localRotation;
                
                _activeWeapon.Shoot(shootPoint);
                
                isShooting = false;
                timeToShoot = Time.time + shootDelay;
            }
        }
    }
}