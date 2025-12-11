using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts
{
    internal class InputSystem
    {
        private readonly InputSystem_Actions _inputs;
        private readonly Action<InputAction.CallbackContext> _jump;

        public InputSystem(Action<InputAction.CallbackContext> jump)
        {
            _jump = jump;
            _inputs = new InputSystem_Actions();
            _inputs.Player.Enable();
        }

        public Vector2 Look { get; private set; }
        public Vector2 MoveInput { get; private set; }

        public void Enable()
        {
            SubscribeInput(_inputs.Player.Move, OnMove, OnMoveCanceled);
            SubscribeInput(_inputs.Player.Look, OnMouseMove, OnMouseCanceled);
            SubscribeInput(_inputs.Player.Jump, _jump);
        }

        public void Disable()
        {
            UnsubscribeInput(_inputs.Player.Move, OnMove, OnMoveCanceled);
            UnsubscribeInput(_inputs.Player.Look, OnMouseMove, OnMouseCanceled);
            UnsubscribeInput(_inputs.Player.Jump, _jump);
        }

        private void OnMove(InputAction.CallbackContext context) =>
            MoveInput = context.ReadValue<Vector2>();

        private void OnMoveCanceled(InputAction.CallbackContext context) =>
            MoveInput = Vector2.zero;

        private void OnMouseMove(InputAction.CallbackContext context) =>
            Look = context.ReadValue<Vector2>();

        private void OnMouseCanceled(InputAction.CallbackContext context) =>
            Look = Vector2.zero;

        private void SubscribeInput(InputAction input,
            Action<InputAction.CallbackContext> performed = null,
            Action<InputAction.CallbackContext> canceled = null)
        {
            if (performed != null)
                input.performed += performed;

            if (canceled != null)
                input.canceled += canceled;
        }

        private void UnsubscribeInput(InputAction input,
            Action<InputAction.CallbackContext> performed = null,
            Action<InputAction.CallbackContext> canceled = null)
        {
            if (performed != null)
                input.performed -= performed;

            if (canceled != null)
                input.canceled -= canceled;
        }
    }
}