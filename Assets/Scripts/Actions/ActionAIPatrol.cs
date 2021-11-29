using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LDS.GameProgramming
{
    // ------------------------------------------------------------------------
    public class ActionAIPatrol : Action
    {
        [SerializeField]
        private List<Transform> _Path = new List<Transform>();

        //[SerializeField]
        //[Range(1f, 10f)]
        //private float _PatrolSpeed = 1f;

        [SerializeField]
        private float _WaitTime;

        private Coroutine _CoroutineFollowPath;
        private int _PathIndex;

        #region Properties

        // ------------------------------------------------------------------------
        public Vector2 NextPathDestination
        {
            get
            {
                _PathIndex = (_PathIndex + 1) % _Path.Count;
                return _Path[_PathIndex].position;
            }
        }

        #endregion

        #region Unity Methods

        // ------------------------------------------------------------------------
        protected override void Awake()
        {
            base.Awake();

            Priority = EPriority.Movement;
        }

        #endregion

        #region Class Methods

        // ------------------------------------------------------------------------
        public override bool CanExecute()
        {
            return true;
        }

        // ------------------------------------------------------------------------
        public override bool CanContinueToExecute()
        {
            return CanExecute();
        }

        // ------------------------------------------------------------------------
        public override bool CanExecuteSimultaneously(Action action)
        {
            return false;
        }

        // ------------------------------------------------------------------------
        public override void StartExecution()
        {
            base.StartExecution();

            _CoroutineFollowPath = StartCoroutine(CoroutineFollowPath());
        }

        // ------------------------------------------------------------------------
        public override void StopExecution()
        {
            base.StopExecution();

            StopCoroutine(_CoroutineFollowPath);
            _CoroutineFollowPath = null;
        }

        #endregion

        #region Coroutines

        // ------------------------------------------------------------------------
        private IEnumerator CoroutineFollowPath()
        {
            while (true)
            {
                yield return new WaitForSeconds(_WaitTime);

                var origin = PhysicsBody.Position;
                var destination = NextPathDestination;

                var timeOfArrival = 0f;
                PhysicsBody.Velocity = Parabola.CalculateInitialVelocity(origin, destination, 2f, 1f, ref timeOfArrival);
                yield return new WaitForSeconds(timeOfArrival);
                PhysicsBody.InstantTeleport(destination);
                PhysicsBody.Velocity = Vector2.zero;

                //var duration = (destination - origin).magnitude / _PatrolSpeed;
                //var alpha = 0f;
                //while (alpha < 1f)
                //{
                //    alpha += TimeHandler.DeltaTime / duration;
                //    alpha = Mathf.Min(alpha, 1f);

                //    PhysicsBody.SimulatedTeleport = Interpolations.Linear(origin, destination, alpha);
                //    yield return YieldUntilNextFrame;
                //}
            }
        }

        #endregion
    }
}
