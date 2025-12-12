using UnityEngine;

namespace Scripts
{
    public class TargetGoal : MonoBehaviour
    {
        [SerializeField] private Maze _maze;

        private void OnCollisionEnter(Collision collision) =>
            _maze.MoveGoalToNewPosition();
    }
}