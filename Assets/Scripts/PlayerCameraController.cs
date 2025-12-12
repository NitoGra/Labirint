using System;
using UnityEngine;

namespace Scripts
{
    [Serializable]
    internal class PlayerCameraController
    {
        private const float MoveCameraDelta = 0.1f;
        [Space]
        [SerializeField] private float _mouseSensitivity = 5;
        [SerializeField] private float _minVerticalAngle = -80f;
        [SerializeField] private float _maxVerticalAngle = 80f;
        [SerializeField] private Camera _camera;

        private float _verticalLookRotation = 0f;

        public Transform GetCameraTransform => _camera.transform;

        public void HandleRotation(InputSystem inputSystem, Transform transform)
        {
            if (Mathf.Abs(inputSystem.Look.x) > MoveCameraDelta)
                transform.Rotate(0f, inputSystem.Look.x * _mouseSensitivity * Time.fixedDeltaTime, 0f);

            if (Mathf.Abs(inputSystem.Look.y) > MoveCameraDelta && _camera != null)
            {
                _verticalLookRotation -= inputSystem.Look.y * _mouseSensitivity * Time.fixedDeltaTime;
                _verticalLookRotation = Mathf.Clamp(_verticalLookRotation, _minVerticalAngle, _maxVerticalAngle);
                _camera.transform.localRotation = Quaternion.Euler(_verticalLookRotation, 0f, 0f);
            }
        }
    }
}