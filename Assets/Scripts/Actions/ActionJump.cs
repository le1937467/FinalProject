using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LDS.GameProgramming
{
    // ------------------------------------------------------------------------
    public class ActionJump : Action
    {
        [Header("Jump")]

        [SerializeField]
        private JumpData _JumpData;

        private Coroutine _CoroutineJump;

        #region Unity Methods

        // ------------------------------------------------------------------------
        protected override void Awake()
        {
            base.Awake();

            Priority = EPriority.Ability;
        }

        #endregion

        #region Class Methods

        // ------------------------------------------------------------------------
        public override bool CanExecute()
        {
            if (ActionGrid.Can(ECanQuery.Jump) != ECanIsQueryResult.Yes)
            {
                return false;
            }

            if (!PhysicsBody.IsGrounded)
            {
                return false;
            }

            return Input.GetKeyDown(KeyCode.Space);
        }

        // ------------------------------------------------------------------------
        public override bool CanContinueToExecute()
        {
            return PhysicsBody.IsGrounded;
        }

        // ------------------------------------------------------------------------
        public override bool CanExecuteSimultaneously(Action action)
        {
            return true;
        }

        // ------------------------------------------------------------------------
        public override void StartExecution()
        {
            base.StartExecution();

            _CoroutineJump = StartCoroutine(CoroutineJump());
        }

        // ------------------------------------------------------------------------
        public override void StopExecution()
        {
            base.StopExecution();

            PhysicsBody.SpeedX = 0f;
        }

        #endregion

        #region Coroutines

        // ------------------------------------------------------------------------
        private IEnumerator CoroutineJump()
        {
            // Jumping
            var jumpSpeed = _JumpData._JumpHeight / (0.5f * _JumpData._JumpTime);
            var gravity = jumpSpeed / _JumpData._JumpTime;
            PhysicsBody.GravityScale = Mathf.Abs(gravity / Physics2D.gravity.y);
            PhysicsBody.SpeedY = jumpSpeed;

            while (PhysicsBody.SpeedY > 0f)
            {
                if (!Input.GetKey(KeyCode.Space))
                {
                    PhysicsBody.SpeedY = 0f;
                    break;
                }

                yield return YieldUntilNextFrame;
            }

            // Falling
            gravity = _JumpData._JumpHeight * 2f / (_JumpData._FallTime * _JumpData._FallTime);
            PhysicsBody.GravityScale = Mathf.Abs(gravity / Physics2D.gravity.y);

            while (!PhysicsBody.IsGrounded)
            {
                yield return YieldUntilNextFrame;
            }

            _CoroutineJump = null;
        }

        #endregion

        #region Class JumpData

        // ------------------------------------------------------------------------
        [System.Serializable]
        public class JumpData
        {
            [SerializeField]
            public float _JumpHeight = 2.5f;

            [SerializeField]
            public float _JumpTime = 0.4f;

            [SerializeField]
            public float _FallTime = 0.45f;
        }

        #endregion
    }
}
