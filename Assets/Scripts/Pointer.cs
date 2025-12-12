using UnityEngine;

namespace Scripts
{
    internal class Pointer : MonoBehaviour
    {
        [SerializeField] private Maze _maze;
        [SerializeField] private Transform _normalPointer;
        [SerializeField] private Transform _aStarPointer;
        [SerializeField] private Transform _goal;

        private void FixedUpdate()
        {
            PointerRotate();
            AStarPointerRotate();
        }

        private void PointerRotate() =>
            _normalPointer.LookAt(_goal);

        private void AStarPointerRotate() =>
            _aStarPointer.LookAt(_maze.FindClosestPoint());
    }
}