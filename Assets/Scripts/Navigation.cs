using System;
using UnityEngine;

namespace Scripts
{
    internal class Navigation : MonoBehaviour
    {
        [SerializeField] private Transform _absolutePointer;
        [SerializeField] private Transform _aStarPointer;

        private Transform _goal;
        private Func<Vector3> _closestPointFind;

        public void Init(Transform goal, Func<Vector3> findClosestPoint)
        {
            _goal = goal;
            _closestPointFind = findClosestPoint;
        }

        private void FixedUpdate()
        {
            PointerRotate();
            AStarPointerRotate();
        }

        private void PointerRotate() =>
            _absolutePointer.LookAt(_goal);

        private void AStarPointerRotate() =>
            _aStarPointer.LookAt(_closestPointFind());
    }
}