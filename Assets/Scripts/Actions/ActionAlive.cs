using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LDS.GameProgramming
{
    // ------------------------------------------------------------------------
    public class ActionAlive : Action
    {
        [SerializeField]
        [Range(0, 100)]
        private int _HP;

        #region Properties

        // ------------------------------------------------------------------------
        public int HP
        {
            get
            {
                return _HP;
            }

            set
            {
                _HP = value;
            }
        }

        #endregion

        #region Unity Methods

        // ------------------------------------------------------------------------
        protected override void Awake()
        {
            base.Awake();

            Priority = EPriority.Status;
        }

        #endregion

        #region Class Methods

        // ------------------------------------------------------------------------
        public override bool CanExecute()
        {
            return _HP > 0;
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
        public override void StartExecution()
        {
            base.StartExecution();

            GameEventsHandler.OnDamageReceived += OnDamageReceived;
        }

        // ------------------------------------------------------------------------
        public override void StopExecution()
        {
            base.StopExecution();

            GameEventsHandler.OnDamageReceived -= OnDamageReceived;

            Destroy(transform.parent.gameObject);
        }

        // ------------------------------------------------------------------------
        public override ECanIsQueryResult Is(EIsQuery query)
        {
            switch (query)
            {
                case EIsQuery.Alive:
                {
                    return ECanIsQueryResult.Yes;
                }

                case EIsQuery.Dead:
                {
                    return ECanIsQueryResult.No;
                }
            }

            return ECanIsQueryResult.Undefined;
        }

        #endregion

        #region Event Callbacks

        // ------------------------------------------------------------------------
        private void OnDamageReceived(AttackAOE attackAOE, PhysicsBody physicsBody)
        {
            if (physicsBody != PhysicsBody)
            {
                return;
            }

            HP -= attackAOE.Damage;
        }

        #endregion
    }
}
