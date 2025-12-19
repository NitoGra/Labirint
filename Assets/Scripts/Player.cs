using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts
{
    [RequireComponent(typeof(Rigidbody))]
    internal class Player : MonoBehaviour
    {
        private const float Height = 1.1f;

        [SerializeField] private Rigidbody _rigidbody;
        [Space]
        [SerializeField] private float _moveSpeed = 4f;
        [SerializeField] private float _jumpForce = 4f;
        [SerializeField] private PlayerCameraController _playerCamera;

        private InputSystem _inputSystem;

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            _inputSystem = new(Jump);
        }

        private void OnEnable()
        {
            _inputSystem.Enable();
        }

        private void FixedUpdate()
        {
            _playerCamera.HandleRotation(_inputSystem, transform);
            HandleMovement();
        }

        private void OnDisable()
        {
            _inputSystem.Disable();
        }

        private void HandleMovement()
        {
            if (_inputSystem.MoveInput == Vector2.zero)
            {
                _rigidbody.linearVelocity = new Vector3(0f, _rigidbody.linearVelocity.y, 0f);
                return;
            }

            Vector3 cameraForward = _playerCamera.GetCameraTransform.forward.normalized;
            Vector3 cameraRight = _playerCamera.GetCameraTransform.right.normalized;

            cameraForward.y = 0f;
            cameraRight.y = 0f;

            Vector3 movement = (cameraForward * _inputSystem.MoveInput.y + cameraRight * _inputSystem.MoveInput.x) * _moveSpeed;
            _rigidbody.linearVelocity = new Vector3(movement.x, _rigidbody.linearVelocity.y, movement.z);
        }

        private void Jump(InputAction.CallbackContext context)
        {
            if (Physics.Raycast(transform.position, Vector3.down, Height) == false)
                return;

            _rigidbody.linearVelocity += _jumpForce * Vector3.up;
        }
    }
}