using UnityEngine;

namespace LDS.GameProgramming
{
    // ------------------------------------------------------------------------
    public abstract class Action : MonoBehaviour
    {
        private static RaycastHit2D[] _RaycastHits = new RaycastHit2D[4];
        protected static Quaternion _QuaternionIdentity = Quaternion.identity;
        protected static Quaternion _QuaternionHorizontal180 = Quaternion.Euler(0f, 180f, 0f);
        protected static Quaternion _QuaternionVerticla180 = Quaternion.Euler(180f, 0f, 0f);

        private ActionGrid _ActionGrid;
        private PhysicsBody _PhysicsBody;
        private WaitUntil _WaitUntilGameNotPaused;
        private WaitForSeconds _WaitForPhysicsUpdate;
        private EState _State;
        private EPriority _Priority;
        private int _PriorityOrder;
        private int _ExecutionFrame;
        private float _ExecutionTime;
        private bool _Terminated;
        private bool _GamePaused;

        #region Enums

        // ------------------------------------------------------------------------
        public enum EPriority
        {
            Status,
            Interaction,
            Ability,
            Movement,
        }

        // ------------------------------------------------------------------------
        public enum EState
        {
            Inactive,
            Executing,
        }

        // ------------------------------------------------------------------------
        public enum ECanIsQueryResult
        {
            Yes,
            No,
            Undefined, // Does not answer to this query, check next executing action. Last Undefined means No.
        }

        // ------------------------------------------------------------------------
        public enum EIsQuery
        {
            Alive,
            Dead,
            UsingWeapon,
            UsingItem,
        };

        // ------------------------------------------------------------------------
        public enum ECanQuery
        {
            Jump,
            Attack,
        };

        #endregion

        #region Properties

        // ------------------------------------------------------------------------
        public ActionGrid ActionGrid
        {
            get
            {
                return _ActionGrid;
            }

            set
            {
                _ActionGrid = value;
            }
        }

        // ------------------------------------------------------------------------
        public EPriority Priority
        {
            get
            {
                return _Priority;
            }

            set
            {
                _Priority = value;
            }
        }

        // ------------------------------------------------------------------------
        public int PriorityOrder
        {
            get
            {
                return _PriorityOrder;
            }

            protected set
            {
                _PriorityOrder = value;
            }
        }

        // ------------------------------------------------------------------------
        public EState State
        {
            get
            {
                return _State;
            }
        }

        // ------------------------------------------------------------------------
        public bool IsExecuting
        {
            get
            {
                return State == EState.Executing;
            }
        }

        // ------------------------------------------------------------------------
        public int ExecutionFrame
        {
            get
            {
                return _ExecutionFrame;
            }

            set
            {
                _ExecutionFrame = value;
            }
        }

        // ------------------------------------------------------------------------
        public float ExecutionTime
        {
            get
            {
                return _ExecutionTime;
            }

            set
            {
                _ExecutionTime = value;
            }
        }

        // ------------------------------------------------------------------------
        public bool Terminated
        {
            get
            {
                return _Terminated;
            }

            set
            {
                _Terminated = value;
            }
        }

        // ------------------------------------------------------------------------
        public PhysicsBody PhysicsBody
        {
            get
            {
                return _PhysicsBody;
            }
        }

        // ------------------------------------------------------------------------
        protected CustomYieldInstruction YieldUntilNextFrame
        {
            get
            {
                if (_GamePaused)
                {
                    return _WaitUntilGameNotPaused;
                }

                return null;
            }
        }

        // ------------------------------------------------------------------------
        protected WaitForSeconds WaitForOnePhysicsUpdate
        {
            get
            {
                return _WaitForPhysicsUpdate;
            }
        }

        // ------------------------------------------------------------------------
        public float HorizontalAxis
        {
            get
            {
                var leftArrow = Input.GetKey(KeyCode.LeftArrow);
                var rightArrow = Input.GetKey(KeyCode.RightArrow);

                if (!leftArrow && !rightArrow)
                {
                    return 0f;
                }

                if (leftArrow && rightArrow)
                {
                    return 0f;
                }

                if (leftArrow)
                {
                    return -1f;
                }

                if (rightArrow)
                {
                    return 1f;
                }

                return 0f;
            }
        }


        #endregion

        #region Unity Methods

        // ------------------------------------------------------------------------
        protected virtual void Awake()
        {
            _PhysicsBody = transform.parent.GetComponentInChildren<PhysicsBody>();
            _WaitUntilGameNotPaused = new WaitUntil(() => !_GamePaused);
            _WaitForPhysicsUpdate = new WaitForSeconds(Time.fixedDeltaTime * 2f);
        }

        // ------------------------------------------------------------------------
        protected virtual void OnEnable()
        {
            _State = EState.Inactive;
        }

        // ------------------------------------------------------------------------
        protected virtual void OnDisable()
        {
            if (IsExecuting)
            {
                StopExecution();
            }

            _State = EState.Inactive;
        }

        #endregion

        #region Class Methods

        // ------------------------------------------------------------------------
        // This is the place where you perform action related tests to know if it can become active.
        public abstract bool CanExecute();

        // ------------------------------------------------------------------------
        // This is the method where an action check if it can continue to execute. If false returned, the action execution is stopped.
        public abstract bool CanContinueToExecute();

        // ------------------------------------------------------------------------
        // This is the place where an action state if the action can be executed simultaneously on a different layer.
        public abstract bool CanExecuteSimultaneously(Action action);

        // ------------------------------------------------------------------------
        // Called once when the action start execution.
        public virtual void StartExecution()
        {
            _State = EState.Executing;
            _ExecutionTime = 0f;
            _Terminated = false;
        }

        // ------------------------------------------------------------------------
        // Called once per frame. This is where you put the action execution code. 
        // A good patern is to use Coroutine for latent actions.
        public virtual void Execute()
        {
        }

        // ------------------------------------------------------------------------
        // Called once when the action stop execution.
        public virtual void StopExecution()
        {
            _ExecutionTime = 0f;
            _State = EState.Inactive;
        }

        #endregion

        #region Can/Is Queries

        // ------------------------------------------------------------------------
        public virtual ECanIsQueryResult Is(EIsQuery query)
        {
            return ECanIsQueryResult.Undefined;
        }

        // ------------------------------------------------------------------------
        public virtual ECanIsQueryResult Can(ECanQuery query)
        {
            return ECanIsQueryResult.Undefined;
        }

        #endregion
    }
}
