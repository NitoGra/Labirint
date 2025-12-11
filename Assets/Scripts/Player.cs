using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts
{
    [RequireComponent(typeof(Rigidbody))]
    internal class Player : MonoBehaviour
    {
        private const float MoveCameraDelta = 0.1f;
        private const float Heigth = 1.1f;

        [SerializeField] private Camera _camera;
        [SerializeField] private Rigidbody _rigidbody;
        [Space]
        [SerializeField] private float _moveSpeed = 4f;
        [SerializeField] private float _jumpForce = 4f;
        [SerializeField] private float _mouseSensitivity = 5;
        [Space]
        [SerializeField] private float _minVerticalAngle = -80f;
        [SerializeField] private float _maxVerticalAngle = 80f;

        private float _verticalLookRotation = 0f;
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
            HandleRotation();
            HandleMovement();
        }

        private void OnDisable()
        {
            _inputSystem.Disable();
        }

        private void HandleRotation()
        {
            if (Mathf.Abs(_inputSystem.Look.x) > MoveCameraDelta)
                transform.Rotate(0f, _inputSystem.Look.x * _mouseSensitivity * Time.fixedDeltaTime, 0f);

            if (Mathf.Abs(_inputSystem.Look.y) > MoveCameraDelta && _camera != null)
            {
                _verticalLookRotation -= _inputSystem.Look.y * _mouseSensitivity * Time.fixedDeltaTime;
                _verticalLookRotation = Mathf.Clamp(_verticalLookRotation, _minVerticalAngle, _maxVerticalAngle);
                _camera.transform.localRotation = Quaternion.Euler(_verticalLookRotation, 0f, 0f);
            }
        }

        private void HandleMovement()
        {
            if (_inputSystem.MoveInput == Vector2.zero)
            {
                _rigidbody.linearVelocity = new Vector3(0f, _rigidbody.linearVelocity.y, 0f);
                return;
            }

            Vector3 cameraForward = _camera.transform.forward;
            Vector3 cameraRight = _camera.transform.right;

            cameraForward.y = 0f;
            cameraRight.y = 0f;

            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 movement = (cameraForward * _inputSystem.MoveInput.y + cameraRight * _inputSystem.MoveInput.x) * _moveSpeed;
            _rigidbody.linearVelocity = new Vector3(movement.x, _rigidbody.linearVelocity.y, movement.z);
        }

        private void Jump(InputAction.CallbackContext context)
        {
            if (Physics.Raycast(transform.position, Vector3.down, Heigth) == false)
                return;

            _rigidbody.linearVelocity += _jumpForce * Vector3.up;
        }
    }
}