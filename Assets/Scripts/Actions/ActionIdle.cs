using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LDS.GameProgramming
{
    // ------------------------------------------------------------------------
    public class ActionIdle : Action
    {
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
            return PhysicsBody.SpeedX <= PhysicsWorldHandler.EpsilonSqr;
        }

        // ------------------------------------------------------------------------
        public override bool CanContinueToExecute()
        {
            return CanExecute();
        }

        // ------------------------------------------------------------------------
        public override bool CanExecuteSimultaneously(Action action)
        {
            return true;
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
