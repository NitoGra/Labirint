using UnityEngine;

namespace Scripts
{
    internal class Bootstrap : MonoBehaviour
    {
        [SerializeField] private Player _player;
        [SerializeField] private TargetGoal _goal;
        [SerializeField] private Navigation _pointer;
        [SerializeField] private Maze _maze;

        private void Awake()
        {
            _pointer.Init(_goal.transform, _maze.FindClosestPoint);
            _maze.Init(_player, _goal);
        }
    }
}