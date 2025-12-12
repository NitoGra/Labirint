using System;
using UnityEngine;

namespace Scripts
{
    internal class TargetGoal : MonoBehaviour
    {
        public event Action FoundTarget;

        private void OnCollisionEnter(Collision collision) =>
            FoundTarget.Invoke();
    }
}