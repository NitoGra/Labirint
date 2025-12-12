using System;
using UnityEngine;

namespace Scripts
{
    internal class Pointer : MonoBehaviour
    {
        [SerializeField] private Transform _normalPointer;
        [SerializeField] private Transform _aStarPointer;

        private Transform _goal;
        private Func<Vector3> _findClosestPoint;

        public void Boosttrap(Transform goal, Func<Vector3> findClosestPoint)
        {
            _goal = goal;
            _findClosestPoint = findClosestPoint;
        }

        private void FixedUpdate()
        {
            PointerRotate();
            AStarPointerRotate();
        }

        private void PointerRotate() =>
            _normalPointer.LookAt(_goal);

        private void AStarPointerRotate() =>
            _aStarPointer.LookAt(_findClosestPoint());
    }
}