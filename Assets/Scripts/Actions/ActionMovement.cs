using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LDS.GameProgramming
{
    // ------------------------------------------------------------------------
    public class ActionMovement : Action
    {
        [SerializeField]
        [Range(1f, 10f)]
        private float _HorizontalSpeed;

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
            return !Mathf.Approximately(HorizontalAxis, 0f);
        }

        // ------------------------------------------------------------------------
        public override bool CanContinueToExecute()
        {
            return CanExecute();
        }

        // ------------------------------------------------------------------------
        public override bool CanExecuteSimultaneously(Action action)
        {
            if (action is ActionIdle)
            {
                return false;
            }

            return true;
        }

        // ------------------------------------------------------------------------
        public override void Execute()
        {
            base.Execute();

            PhysicsBody.SpeedX = HorizontalAxis * _HorizontalSpeed;
        }

        // ------------------------------------------------------------------------
        public override void StopExecution()
        {
            base.StopExecution();

            PhysicsBody.SpeedX = 0f;
        }

        // ------------------------------------------------------------------------
        public override ECanIsQueryResult Can(ECanQuery query)
        {
            if (query == ECanQuery.Jump)
            {
                return ECanIsQueryResult.Yes;
            }

            if (query == ECanQuery.Attack)
            {
                return ECanIsQueryResult.Yes;
            }

            return ECanIsQueryResult.Undefined;
        }

        #endregion
    }
}
