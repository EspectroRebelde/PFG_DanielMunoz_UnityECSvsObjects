using Unity.Collections;
using UnityEngine;
// ReSharper disable Unity.UnknownInputAxes
namespace Scenes.OOP_Test
{
    
        [RequireComponent(typeof(Rigidbody))]

    public class BasicPlayerController : MonoBehaviour
    {
        public float normalSpeed = 35f;
        public float accelerationSpeed = 55f;
        public Transform spaceshipRoot;
        public float rotationSpeed = 3.0f;

        [ReadOnly] public bool isGamepad;

        private Vector3 movement;
        private Vector2 cameraGamepad;
        private float speed;
        private Rigidbody rigidbodyComponent;
        private Camera _camera;

        // Start is called before the first frame update
        private void Start()
        {
            _camera = Camera.main;
            rigidbodyComponent = GetComponent<Rigidbody>();
            rigidbodyComponent.useGravity = false;
        }

        private void Update()
        {
            // If is gamepad, we get the input from the gamepad
            // If not, we get the input from the keyboard
            // movement = new Vector3(Input.GetAxis("Horizontal"), 0 ,Input.GetAxis("Vertical")).normalized;
            // movementGamepad = new Vector3(Input.GetAxis("HorizontalGamepad"), 0 ,Input.GetAxis("VerticalGamepad")).normalized;

            movement = isGamepad
                ? new Vector3(0, 0, Input.GetAxis("VerticalGamepad")).normalized
                : new Vector3(0, 0, Input.GetAxis("Vertical")).normalized;

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

        private void FixedUpdate()
        {
            SpaceShipRotation();

            SpaceShipPhysicMovement();
        }

        /// <summary>
        /// The ship rotates following the mouse position on the screen
        /// The ship moves on the XZ plane, so we only rotate on the Y axis
        /// </summary>
        private void SpaceShipRotation()
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
                    rotation = spaceshipRoot.rotation;
                }
            }
            else
            {
                // We get the mouse position on the screen
                Vector3 mousePos = Input.mousePosition;

                // We convert the mouse position to world coordinates
                Vector3 mouseWorldPos = _camera.ScreenToWorldPoint(mousePos);

                // We get the direction from the ship to the mouse
                Vector3 direction = mouseWorldPos - spaceshipRoot.position;

                // We get the angle between the ship and the mouse
                float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

                // We create a new rotation with the angle we just calculated
                rotation = Quaternion.AngleAxis(angle, Vector3.up);
            }

            // We apply the rotation to the ship
            spaceshipRoot.rotation = Quaternion.Slerp(spaceshipRoot.rotation, rotation, rotationSpeed * Time.fixedDeltaTime);
        }

        /// <summary>
        /// The ship moves in the XZ plane
        /// The inputs come from the InputManager (WASD or arrows + gamepad)
        /// The forward direction is the direction the ship is facing
        /// All movement is applied with forces
        /// </summary>
        private void SpaceShipPhysicMovement()
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
                // We get the forward direction of the ship
                Vector3 forward = spaceshipRoot.forward;

                // We calculate the movement direction
                movementDirection = (forward * movement.z);
            }

            // We apply the movement direction to the rigidbody
            rigidbodyComponent.AddForce(movementDirection * speed, ForceMode.Force);
        }
    }
}