using UnityEngine;

namespace Scripts
{
    internal class Pointer : MonoBehaviour
    {
        [SerializeField] private Maze _maze;

        [SerializeField] private Transform _normalPointer;
        [SerializeField] private Transform _aStarPointer;


        private void FixedUpdate()
        {
            Rotate();
        }

        private void Rotate()
        {
            _normalPointer.LookAt(_maze.GetGoalTransform());
        }
    }
}
