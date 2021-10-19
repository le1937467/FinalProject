using UnityEngine;
using System.Collections.Generic;

namespace LDS.GameProgramming
{
    // ------------------------------------------------------------------------
    // Nice way to create a singleton in Unity...
    public class PhysicsWorldHandler : MonoBehaviour
    {
        private static PhysicsWorldHandler _Instance;
        public static readonly float Epsilon = 0.00001f;
        public static readonly float EpsilonSqr = 0.00001f * 0.00001f;
        public static readonly float MinTerminalVelocity = -20f;
        public static readonly float MaxTerminalVelocity = 100f;
        public static readonly Vector2 Up = Vector2.up;
        public static readonly Vector2 Down = Vector2.down;

        private List<PhysicsBody> _PhysicsBodies = new List<PhysicsBody>();
        private CollisionCache _CollisionCache = new CollisionCache();
        private bool _GamePaused = false;

        #region Enums

        // Enum for Horizontal/Vertical axes.
        // @Note: Simulation axes are sometime used directly as the Vector2 indices. 
        // @Note: DO NOT CHANGE THIS ENUM.
        public enum EAxes
        {
            Horizontal,
            Vertical,
            Count,
        }

        #endregion

        #region Properties

        // ------------------------------------------------------------------------
        private static PhysicsWorldHandler Instance
        {
            set
            {
                if (_Instance == null)
                {
                    _Instance = value;
                }
            }
        }

        // ------------------------------------------------------------------------
        public static CollisionCache CollisionCache => _Instance._CollisionCache;

        // ------------------------------------------------------------------------
        public static Vector2 HorizontalAxis => Vector2.right;

        // ------------------------------------------------------------------------
        public static Vector2 VerticalAxis => Vector2.up;

        #endregion

        #region LayerMask

        public static int LayerIndexSolid = 0;
        public static int LayerIndexPlayer = 0;
        public static int LayerIndexAI = 0;

        public static int CollisionProfileSolid = 0;
        public static int CollisionProfilePlayer = 0;
        public static int CollisionProfileAI = 0;

        #endregion

        // ------------------------------------------------------------------------
        public void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LayerIndexSolid = LayerMask.NameToLayer("Solid");
            LayerIndexPlayer = LayerMask.NameToLayer("Player");
            LayerIndexAI = LayerMask.NameToLayer("AI");

            CollisionProfileSolid = (1 << LayerIndexSolid) | (1 << LayerIndexPlayer);
            CollisionProfilePlayer = (1 << LayerIndexSolid);
            CollisionProfileAI = (1 << LayerIndexSolid);
        }

        // ------------------------------------------------------------------------
        // Movement of ALL dynamic objects is managed from this method.
        // Note: I chosen to use a sequential motion simulation instead of a simultaneous one.
        //       Reading the chapter "Sequential Versus Simultaneous Motion" of Real Time Collision Detection convinced me.
        public void Update()
        {
            if (_GamePaused)
            {
                return;
            }

            _CollisionCache.Clear();

            // Prepare simulation.
            foreach (var itPhysicsBody in _PhysicsBodies)
            {
                itPhysicsBody.PrepareForSimulation();
            }

            // Simulate.
            foreach (var itPhysicsBody in _PhysicsBodies)
            {
                itPhysicsBody.Simulate();
            }

            // Submit simulation.
            Physics2D.SyncTransforms();
            foreach (var itPhysicsBody in _PhysicsBodies)
            {
                itPhysicsBody.SubmitSimulation();
            }
        }

        // ------------------------------------------------------------------------
        public static void Register(PhysicsBody physicsBody)
        {
            if (!_Instance._PhysicsBodies.Contains(physicsBody))
            {
                _Instance._PhysicsBodies.Add(physicsBody);
            }
        }

        // ------------------------------------------------------------------------
        public static void Unregister(PhysicsBody physicsBody)
        {
            if (_Instance._PhysicsBodies.Contains(physicsBody))
            {
                _Instance._PhysicsBodies.Remove(physicsBody);
            }
        }
    }
}
