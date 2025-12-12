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
            Rotate();
            ARotate();
        }

        private void Rotate() =>
            _normalPointer.LookAt(_goal);

        private void ARotate() =>
            _aStarPointer.LookAt(_maze.FindClosestPoint());
    }
}
