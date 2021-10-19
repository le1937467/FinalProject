using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LDS.GameProgramming
{
    // ------------------------------------------------------------------------
    public class ActionAttack : Action
    {
        [SerializeField]
        private GameObject _AttackAOE;

        [SerializeField]
        [Range(0f, 1f)]
        private float _AttackTime = 0.25f;

        #region Unity Methods
        
        // ------------------------------------------------------------------------
        protected override void Awake()
        {
            base.Awake();

            Priority = EPriority.Ability;
            _AttackAOE.SetActive(false);
        }

        #endregion

        #region Class Methods

        // ------------------------------------------------------------------------
        public override bool CanExecute()
        {
            if (ActionGrid.Can(ECanQuery.Attack) != ECanIsQueryResult.Yes)
            {
                return false;
            }

            return Input.GetKeyDown(KeyCode.A);
        }

        // ------------------------------------------------------------------------
        public override bool CanContinueToExecute()
        {
            return Terminated == false;
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

            _AttackAOE.SetActive(true);
        }

        // ------------------------------------------------------------------------
        public override void Execute()
        {
            base.Execute();
            Terminated = ExecutionTime >= _AttackTime;
        }

        // ------------------------------------------------------------------------
        public override void StopExecution()
        {
            base.StopExecution();

            _AttackAOE.SetActive(false);
        }

        #endregion
    }
}
