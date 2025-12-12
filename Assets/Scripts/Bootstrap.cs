using UnityEngine;

namespace Scripts
{
    internal class Bootstrap : MonoBehaviour
    {
        [SerializeField] private Player _player;
        [SerializeField] private TargetGoal _goal;
        [SerializeField] private Pointer _pointer;
        [SerializeField] private Maze _maze;

        private void Awake()
        {
            _pointer.Boosttrap(_goal.transform, _maze.FindClosestPoint);
            _maze.Boosttrap(_player, _goal);
        }
    }
}