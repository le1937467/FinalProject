using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LDS.GameProgramming
{
    // ------------------------------------------------------------------------
    public class ActionGrid : MonoBehaviour
    {
        [Header("Debug")]

        [SerializeField]
        private Transform _Debug;

        private List<Action> _AvailableActions = new List<Action>();
        private bool _GamePaused = false;
        private bool _Initialized;

        #region Properties
        #endregion

        #region Unity Methods

        // ------------------------------------------------------------------------
        private void OnEnable()
        {
            _Initialized = false;
        }

        // ------------------------------------------------------------------------
        private void Update()
        {
            if (_GamePaused)
            {
                return;
            }

            Initialize();

            // Check which actions should execute this frame.
            for (var i = 0; i < _AvailableActions.Count; ++i)
            {
                var action = _AvailableActions[i];

                if (action.IsExecuting)
                {
                    if (!action.CanContinueToExecute())
                    {
                        action.StopExecution();
                        i = -1;
                    }

                    continue;
                }

                if (!action.CanExecute())
                {
                    continue;
                }

                // Check if currently executing actions allow the action to be executed simultaneously.
                var preventExecution = false;
                for (var index = 0; index < _AvailableActions.Count; index++)
                {
                    var itAction = _AvailableActions[index];
                    if (itAction.IsExecuting && !itAction.CanExecuteSimultaneously(action))
                    {
                        preventExecution = true;
                        break;
                    }
                }

                if (preventExecution)
                {
                    continue;
                }

                // The action will now start execution.
                // Check if the action allows executing actions to be executed simultaneously.
                for (var index = 0; index < _AvailableActions.Count; index++)
                {
                    var itAction = _AvailableActions[index];
                    if (itAction.IsExecuting && itAction.Priority >= action.Priority && itAction != action && !action.CanExecuteSimultaneously(itAction))
                    {
                        itAction.StopExecution();
                    }
                }

                // Start the new action.
                action.StartExecution();
            }

            // Execute actions.
            var deltaTime = TimeHandler.DeltaTime;
            for (var index = 0; index < _AvailableActions.Count; index++)
            {
                var action = _AvailableActions[index];
                if (!action.IsExecuting)
                {
                    continue;
                }

                action.Execute();
                action.ExecutionTime += deltaTime;
            }
        }

        #endregion

        #region Class Methods

        // ------------------------------------------------------------------------
        private void Initialize()
        {
            if (!_Initialized)
            {
                _Initialized = true;

                _AvailableActions.Clear();
                _AvailableActions.AddRange(GetComponents<Action>());
                _AvailableActions.Sort((x, y) =>
                {
                    if (x.Priority == y.Priority)
                    {
                        if (x.PriorityOrder == y.PriorityOrder)
                        {
                            return 0;
                        }

                        return x.PriorityOrder > y.PriorityOrder ? 1 : -1;
                    }

                    return x.Priority > y.Priority ? 1 : -1;
                });

                for (int i = 0, count = _AvailableActions.Count; i < count; ++i)
                {
                    _AvailableActions[i].ActionGrid = this;
                }
            }
        }

        #endregion

        #region Can/Is Queries

        // ------------------------------------------------------------------------
        public Action.ECanIsQueryResult Is(Action.EIsQuery query)
        {
            foreach (var action in _AvailableActions)
            {
                if (!action.IsExecuting)
                {
                    continue;
                }

                var queryResult = action.Is(query);
                if (queryResult == Action.ECanIsQueryResult.Undefined)
                {
                    continue;
                }

                return queryResult;
            }

            // No action handled the query; this means No.
            return Action.ECanIsQueryResult.No;
        }

        // ------------------------------------------------------------------------
        public Action.ECanIsQueryResult Can(Action.ECanQuery query)
        {
            foreach (var action in _AvailableActions)
            {
                if (!action.IsExecuting)
                {
                    continue;
                }

                var queryResult = action.Can(query);
                if (queryResult == Action.ECanIsQueryResult.Undefined)
                {
                    continue;
                }

                return queryResult;
            }

            // No action handled the query; this means No.
            return Action.ECanIsQueryResult.No;
        }

        #endregion

        #region Debug

#if UNITY_EDITOR
        // ------------------------------------------------------------------------
        public void OnGUI()
        {
            if (_Debug == null)
            {
                return;
            }

            // Render the debug informations (text).
            var cam = UnityEngine.Camera.main;
            if (cam != null)
            {
                Vector3 screenPos = cam.WorldToScreenPoint(_Debug.position);
                if (screenPos.z < 0)
                {
                    return;
                }

                GUIStyle style = new GUIStyle
                {
                    fontSize = 36,
                    fontStyle = FontStyle.Normal,
                    richText = true,
                };

                GUI.Label(new Rect(screenPos.x, Screen.height - screenPos.y, 1000f, 200f), ToString(), style);
            }
        }

        // ------------------------------------------------------------------------
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            bool firstElement = true;
            for (int i = 0, count = _AvailableActions.Count; i < count; ++i)
            {
                var action = _AvailableActions[i];

                if (!action.IsExecuting)
                {
                    continue;
                }

                if (!firstElement)
                {
                    stringBuilder.Append("\n");
                }

                stringBuilder.AppendFormat("<color=white>[{0}] - [{1}]</color>", action.Priority, action.ToString());
                firstElement = false;
            }

            return firstElement ? "<color=white>--- None ---</color>" : stringBuilder.ToString();
        }
#endif

        #endregion
    }
}
