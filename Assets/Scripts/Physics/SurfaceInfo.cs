using UnityEngine;
using System.Text;

namespace LDS.GameProgramming
{
    // ------------------------------------------------------------------------
    public struct SurfaceInfo
    {
        private GameObject _GameObject;
        private PhysicsBody _PhysicsBody;
        private float _ExcessDistance;

        #region Enums

        public enum EState
        {
            // We are supported by this surface.
            Supported,

            // We are not supported by this surface.
            Unsupported,
        }

        #endregion

        #region Properties

        // ------------------------------------------------------------------------
        public EState State
        {
            get;
            set;
        }

        // ------------------------------------------------------------------------
        public Vector2 Normal
        {
            get;
            set;
        }

        // ------------------------------------------------------------------------
        public float NormalAngle
        {
            get
            {
                if (Normal == Vector2.zero)
                {
                    return 0f;
                }

                return Vector2.Angle(PhysicsWorldHandler.Up, Normal);
            }
        }

        // ------------------------------------------------------------------------
        public float NormalSignedAngle
        {
            get
            {
                if (Normal == Vector2.zero)
                {
                    return 0f;
                }

                return Vector2.SignedAngle(PhysicsWorldHandler.Up, Normal);
            }
        }

        // ------------------------------------------------------------------------
        public Vector2 TopNormal
        {
            get;
            set;
        }

        // ------------------------------------------------------------------------
        public float TopNormalAngle
        {
            get
            {
                if (TopNormal == Vector2.zero)
                {
                    return 0f;
                }

                return Vector2.Angle(PhysicsWorldHandler.Down, TopNormal);
            }
        }
        
        // ------------------------------------------------------------------------
        public Vector2 Velocity
        {
            get
            {
                if (PhysicsBody)
                {
                    return PhysicsBody.TotalSimulatedVelocity;
                }

                // For RigidBody2D, we return the body's velocity.
                if (Collider != null && Collider.attachedRigidbody != null)
                {
                    return Collider.attachedRigidbody.velocity;
                }

                return Vector2.zero;
            }
        }

        // ------------------------------------------------------------------------
        public Vector2 Motion
        {
            get
            {
                if (PhysicsBody)
                {
                    return PhysicsBody.TotalSimulatedVelocity * TimeHandler.DeltaTime;
                }

                // For RigidBody2D, we return the body's velocity.
                if (Collider != null && Collider.attachedRigidbody != null)
                {
                    return Collider.attachedRigidbody.velocity * TimeHandler.DeltaTime;
                }

                return Vector2.zero;
            }
        }

        // ------------------------------------------------------------------------
        public float ExcessDistance
        {
            get
            {
                return _ExcessDistance;
            }

            set
            {
                _ExcessDistance = value;
            }
        }

        // ------------------------------------------------------------------------
        public Vector2 Point
        {
            get;
            set;
        }

        // ------------------------------------------------------------------------
        public Collider2D Collider
        {
            get;
            set;
        }

        // ------------------------------------------------------------------------
        public GameObject GameObject
        {
            get
            {
                return _GameObject;
            }

            set
            {
                if (value != null)
                {
                    // Cache associated PhysicsBody
                    _PhysicsBody = value.GetComponent<PhysicsBody>();
                }

                _GameObject = value;
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
        public int SortingOrder
        {
            get;
            set;
        }

        // ------------------------------------------------------------------------
        public bool IsSolid
        {
            get
            {
                return (State == EState.Supported && GameObject != null) ? GameObject.layer == PhysicsWorldHandler.LayerIndexSolid : false;
            }
        }

        #endregion

        // ------------------------------------------------------------------------
        public void Reset()
        {
            State = EState.Unsupported;
            Normal = Vector3.up;
            TopNormal = Vector3.down;
            ExcessDistance = float.MaxValue;
            Point = Vector3.zero;
            Collider = null;
            GameObject = null;
            SortingOrder = 0;
        }

        // ------------------------------------------------------------------------
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("--- SurfaceInfo - {0} - Normal [{1} {5:0.0}] - TopNormal [{7} {8:0.00}] - DynamicBody [{3}] - Velocity [{4}] - GameObject [{6}] - ExcessDistance [{2:0.00}]  ---",
                                       State,
                                       Normal,
                                       ExcessDistance,
                                       string.Empty,
                                       Velocity,
                                       NormalAngle,
                                       GameObject ? GameObject.name : "None",
                                       TopNormal,
                                       TopNormalAngle);

            return stringBuilder.ToString();
        }
    }

};

