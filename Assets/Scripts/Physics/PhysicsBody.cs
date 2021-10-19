using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LDS.GameProgramming
{
    // ------------------------------------------------------------------------
    public class PhysicsBody : MonoBehaviour
    {
        [Header("Debug")]

        [SerializeField]
        private bool _Debug;

        [SerializeField]
        private Vector2 _DebugVelocity;

        private EState _State = EState.InAir;
        private EProfile _Profile;

        private float _SkinWidthRatio;
        private float _SkinWidthRatioYFactor;
        private float _MaxSlopeAngle;
        private float _TimeGrounded;
        private float _TimeInAir;
        private float _DefaultGravityScale = 1f;
        private float _MinTerminalVelocity;

        private Rigidbody2D _RigidBody;
        private Collider2D _Collider;
        private LayerMask _LayerMask;
        private Bounds _SimulationBounds;
        private MotionRequest _SupportedMotionRequest;
        private CollisionCache.ECollisionFlags2D _CollisionFlags;
        private SurfaceInfo _SurfaceInfo;

        private Vector2 _RaycastDirection = Vector2.zero;
        private Vector2 _RaycastOrigin = Vector2.zero;
        private Vector2 _RaycastSize = Vector2.zero;
        private LayerMask _RaycastLayerMask;
        private float _RaycastSizeOffset = 0.005f;
        private float _RaycastDirectionSign;
        private float _RaycastDistance;

        private MotionResult _MotionResult;
        private Vector2 _Motion;
        private Vector2 _SimulatedMotion;
        private Vector2 _SimulationBoundOffset;
        private Vector2 _TotalSimulatedVelocity;
        private Vector2 _SimulatedVelocity;
        private Vector2 _SimulatedTeleportPosition;
        private Vector2 _SimulatedTeleportMotion;
        private Vector2 _TeleportPosition;

        private bool _SimulatedTeleportRequest;
        private bool _TeleportRequest;
        private bool _IgnoreCollisions;
        private bool _ResolveSurfaceInfo = true;
        private bool _KeepSurfaceInfo;
        private bool _KeepSpeedOnCollision;

        private readonly Stack<MotionRequest> _MotionRequests = new Stack<MotionRequest>(8);
        private readonly Vector2[] _Velocities = new Vector2[(int)EVelocityType.Max];
        private readonly RaycastHit2D[] _RaycastResults = new RaycastHit2D[4];
        private readonly List<int> _CollisionCacheIndices = new List<int>(8);

        private const float _SupportedDistanceThreshold = 0.1f;
        private const float _SlopeDistanceThreshold = 0.1f;

        #region Enums

        // ------------------------------------------------------------------------
        public enum EProfile
        {
            Solid,
            Player,
            AI,
        }

        // ------------------------------------------------------------------------
        public enum EState
        {
            Grounded,
            InAir,
        }

        // ------------------------------------------------------------------------
        public enum EVelocityType
        {
            External,
            Overlap,
            Normal,
            Max
        }

        #endregion

        #region Properties

        // ------------------------------------------------------------------------
        public EProfile Profile
        {
            get
            {
                return _Profile;
            }

            set
            {
                _Profile = value;
                InitializeProfile();
            }
        }

        // ------------------------------------------------------------------------
        public LayerMask LayerMask
        {
            get
            {
                return IgnoreCollisions ? (LayerMask)0 : _LayerMask;
            }
        }

        // ------------------------------------------------------------------------
        public List<int> CollisionCacheIndices
        {
            get
            {
                return _CollisionCacheIndices;
            }
        }

        // ------------------------------------------------------------------------
        public Vector2 Velocity
        {
            get
            {
                return _Velocities[(int)EVelocityType.Normal];
            }

            set
            {
                _Velocities[(int)EVelocityType.Normal] = value;
            }
        }

        // ------------------------------------------------------------------------
        public float SpeedX
        {
            get
            {
                return _Velocities[(int)EVelocityType.Normal].x;
            }

            set
            {
                _Velocities[(int)EVelocityType.Normal].x = value;
            }
        }

        // ------------------------------------------------------------------------
        public float SpeedXAbs
        {
            get
            {
                return Mathf.Abs(_Velocities[(int)EVelocityType.Normal].x);
            }
        }

        // ------------------------------------------------------------------------
        public float SpeedY
        {
            get
            {
                return _Velocities[(int)EVelocityType.Normal].y;
            }

            set
            {
                _Velocities[(int)EVelocityType.Normal].y = value;
            }
        }

        // ------------------------------------------------------------------------
        public Vector2 ExternalVelocity
        {
            get
            {
                return _Velocities[(int)EVelocityType.External];
            }

            set
            {
                _Velocities[(int)EVelocityType.External] = value;
            }
        }
        
        // ------------------------------------------------------------------------
        public EState State
        {
            get
            {
                return _State;
            }

            private set
            {
                if (_State == value)
                {
                    return;
                }

                _State = value;
                switch (_State)
                {
                    case EState.Grounded:
                    {
                        _TimeGrounded = Time.time;
                        break;
                    }

                    case EState.InAir:
                    {
                        _TimeInAir = Time.time;
                        break;
                    }
                }
            }
        }

        // ------------------------------------------------------------------------
        public bool IsGrounded
        {
            get
            {
                return _SurfaceInfo.State == SurfaceInfo.EState.Supported;
            }
        }

        // ------------------------------------------------------------------------
        public bool IsInAir
        {
            get
            {
                return _SurfaceInfo.State != SurfaceInfo.EState.Supported;
            }
        }
        
        // ------------------------------------------------------------------------
        public float TimeGrounded
        {
            get
            {
                return _TimeGrounded;
            }
        }

        // ------------------------------------------------------------------------
        public float TimeInAir
        {
            get
            {
                return _TimeInAir;
            }
        }

        // ------------------------------------------------------------------------
        public Bounds Bounds
        {
            get
            {
                return _Collider.bounds;
            }
        }

        // ------------------------------------------------------------------------
        public Bounds SimulationBounds
        {
            get
            {
                return _SimulationBounds;
            }
        }

        // ------------------------------------------------------------------------
        public Collider2D Collider
        {
            get
            {
                return _Collider;
            }

            set
            {
                _Collider = value;
            }
        }

        // ------------------------------------------------------------------------
        public SurfaceInfo SurfaceInfo
        {
            get
            {
                return _SurfaceInfo;
            }
        }

        // ------------------------------------------------------------------------
        public float GravityScale
        {
            get
            {
                if (_RigidBody)
                {
                    return _RigidBody.gravityScale;
                }

                return 1f;
            }

            set
            {
                _RigidBody.gravityScale = value;
            }
        }

        // ------------------------------------------------------------------------
        public float DefaultGravityScale
        {
            get
            {
                return _DefaultGravityScale;
            }

            set
            {
                _DefaultGravityScale = value;
            }
        }

        // ------------------------------------------------------------------------
        public float SkinWidth
        {
            get
            {
                return Bounds.extents.x * _SkinWidthRatio;
            }
        }

        // ------------------------------------------------------------------------
        public float SkinWidthX
        {
            get
            {
                return SkinWidth;
            }
        }

        // ------------------------------------------------------------------------
        public float SkinWidthY
        {
            get
            {
                return SkinWidth * _SkinWidthRatioYFactor;
            }
        }

        // ------------------------------------------------------------------------
        public Vector2 SimulatedMotion
        {
            get
            {
                return _SimulatedMotion;
            }
        }

        // ------------------------------------------------------------------------
        public Vector2 TotalSimulatedVelocity
        {
            get
            {
                return _TotalSimulatedVelocity;
            }
        }

        // ------------------------------------------------------------------------
        public Vector2 SimulatedVelocity
        {
            get
            {
                return _SimulatedVelocity;
            }
        }

        // ------------------------------------------------------------------------
        public CollisionCache.ECollisionFlags2D CollisionFlags
        {
            get
            {
                return _CollisionFlags;
            }
        }

        // ------------------------------------------------------------------------
        public float MaxSlopeAngle
        {
            get
            {
                return _MaxSlopeAngle;
            }
        }

        // ------------------------------------------------------------------------
        public bool IsAboveMaxSlope
        {
            get
            {
                return IsGrounded && SurfaceInfo.NormalAngle > MaxSlopeAngle;
            }
        }        

        // ------------------------------------------------------------------------
        public Vector2 Teleport
        {
            set
            {
                _TeleportRequest = true;
                _TeleportPosition = value;
            }

            get
            {
                return _TeleportPosition;
            }
        }

        // ------------------------------------------------------------------------
        public Vector2 SimulatedTeleport
        {
            set
            {
                _SimulatedTeleportPosition = value;
                _SimulatedTeleportRequest = true;
            }
        }

        // ------------------------------------------------------------------------
        public Vector2 SimulatedTeleportDelta
        {
            set
            {
                if (value == Vector2.zero)
                {
                    return;
                }

                if (!_SimulatedTeleportRequest)
                {
                    _SimulatedTeleportPosition = _RigidBody.position;
                    _SimulatedTeleportRequest = true;
                }

                _SimulatedTeleportPosition += value;
            }
        }

        // ------------------------------------------------------------------------
        public bool IgnoreCollisions
        {
            get
            {
                return _IgnoreCollisions;
            }

            set
            {
                _IgnoreCollisions = value;
            }
        }

        // ------------------------------------------------------------------------
        public Vector2 Position
        {
            get
            {
                return _RigidBody.position;
            }
        }

        // ------------------------------------------------------------------------
        public float MinTerminalVelocity
        {
            get
            {
                return _MinTerminalVelocity < 0 ? _MinTerminalVelocity : PhysicsWorldHandler.MinTerminalVelocity;
            }

            set
            {
                _MinTerminalVelocity = value;
            }
        }

        // ------------------------------------------------------------------------
        public bool ResolveSurfaceInfo
        {
            set
            {
                _ResolveSurfaceInfo = value;
            }
        }

        // ------------------------------------------------------------------------
        public bool KeepSurfaceInfo
        {
            set
            {
                _KeepSurfaceInfo = value;
            }
        }

        // ------------------------------------------------------------------------
        public bool KeepSpeedOnCollision
        {
            get
            {
                return _KeepSpeedOnCollision;
            }

            set 
            {
                _KeepSpeedOnCollision = value;
            }
        }

        #endregion

        #region Execution Flow

        // ------------------------------------------------------------------------
        private void Awake()
        {
            var physicsBodyOverrideGO = GetComponent<PhysicsBodyOverride>();
            if (physicsBodyOverrideGO != null)
            {
                _Profile = physicsBodyOverrideGO.Profile;
                _SkinWidthRatio = physicsBodyOverrideGO.SkinWidthRatio;
                _SkinWidthRatioYFactor = physicsBodyOverrideGO.SkinWidthRatioYFactor;
                _MaxSlopeAngle = physicsBodyOverrideGO.MaxSlopeAngle;
            }

            _RigidBody = GetComponentInParent<Rigidbody2D>();
            _RigidBody.bodyType = RigidbodyType2D.Kinematic;
            _RigidBody.interpolation = RigidbodyInterpolation2D.Interpolate;
            _RigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;

            _SurfaceInfo.Reset();

            InitializeProfile();
        }

        // ------------------------------------------------------------------------
        private void OnEnable()
        {
            // Some system creates collider at runtime (ex: Ferr), so we can't cache this component in Awake().
            _Collider = GetComponent<Collider2D>();
            _TeleportRequest = false;

            PhysicsWorldHandler.Register(this);
        }

        // ------------------------------------------------------------------------
        private void OnDisable()
        {
            PhysicsWorldHandler.Unregister(this);
        }

        #endregion

        #region Simulation

        // ------------------------------------------------------------------------
        public void InstantTeleport(Vector2 destination)
        {
            var teleportMotion = destination - _RigidBody.position;
            _RigidBody.transform.Translate(teleportMotion);

            Physics2D.SyncTransforms();
        }

        // ------------------------------------------------------------------------
        public void PrepareForSimulation()
        {
            _RigidBody.WakeUp();

            _CollisionCacheIndices.Clear();
            _CollisionFlags = CollisionCache.ECollisionFlags2D.None;
            _SimulationBounds = Bounds;
            _SimulationBounds.Expand(new Vector3(SkinWidthX, SkinWidthY, 0f) * -2f);
            _SimulationBoundOffset = Position - (Vector2)_SimulationBounds.center;

            if (_TeleportRequest)
            {
                _TeleportRequest = false;
                SubmitMotion(_TeleportPosition - _RigidBody.position);
            }

            var deltaTime = TimeHandler.DeltaTime;

            // Combine this frame velocities.
            _Motion = Velocity + ExternalVelocity + _DebugVelocity;
            _Motion *= deltaTime;

            // Gravity.
            if (SpeedY > 0f || !IsGrounded)
            {
                // Integrated simplified velocity verlet. (https://en.wikipedia.org/wiki/Verlet_integration)
                _Motion.y += (0.5f * Physics2D.gravity.y * GravityScale * deltaTime * deltaTime);
                SpeedY += (Physics2D.gravity.y * GravityScale * deltaTime);

                // Always clamp between min and max terminal velocity
                SpeedY = Mathf.Clamp(SpeedY, MinTerminalVelocity, PhysicsWorldHandler.MaxTerminalVelocity);
            }

            // Simulated teleport request
            if (_SimulatedTeleportRequest)
            {
                _SimulatedTeleportRequest = false;
                _SimulatedTeleportMotion = _SimulatedTeleportPosition - _RigidBody.position;
                _Motion += _SimulatedTeleportMotion;
            }
        }

        // ------------------------------------------------------------------------
        public void Simulate()
        {
            BuildMotionRequests();

            while (_MotionRequests.Count > 0)
            {
                Simulate(_MotionRequests.Pop());
            }

            var deltaTime = TimeHandler.DeltaTime;
            if (deltaTime > 0f)
            {
                _TotalSimulatedVelocity = _SimulatedMotion / deltaTime;
            }

            if (_TotalSimulatedVelocity.sqrMagnitude > PhysicsWorldHandler.EpsilonSqr)
            {
                // I know, documentation says to not do this, but I still do it because I don't care... (and we sync the transform on our side)
                _RigidBody.transform.Translate(_SimulatedMotion);
            }
        }

        // ------------------------------------------------------------------------
        private void Simulate(MotionRequest request)
        {
            var nbResults = ExecuteMotionRaycast(request);
            if (nbResults < 0)
            {
                // MotionRequest will not produce any movement.
                return;
            }

            // Execute the requested motion.
            _MotionResult.Reset();
            _MotionResult.Distance = _RaycastDistance;

            // Execute the raycasts.
            var skinWidth = request.IsHorizontal ? SkinWidthX : SkinWidthY;

            for (int i = 0; i < nbResults; ++i)
            {
                var result = _RaycastResults[i];

                if (!IsValidResult(ref result, request.IsHorizontal))
                {
                    continue;
                }

                _MotionResult.Distance = result.distance;
                _MotionResult.Body = result.collider.GetComponent<PhysicsBody>();
                _MotionResult.Normal = result.normal;
                _MotionResult.Point = result.point; 

                break;
            }

            // Submit the motion.
            if (Mathf.Abs(_MotionResult.Distance) > PhysicsWorldHandler.Epsilon)
            {
                var motion = _RaycastDirection * (_MotionResult.Distance - skinWidth);
                SubmitMotion(motion);
            }

            // Slope and collision.
            var motionRemaining = _RaycastDistance - _MotionResult.Distance;
            if (motionRemaining > PhysicsWorldHandler.Epsilon)
            {
                var isValidSlope = false;
                if (request.IsHorizontal)
                {
                    var isSlopeEntrance = IsSlopeEntrance(_MotionResult.Point);
                    isValidSlope = (isSlopeEntrance && Vector2.Angle(PhysicsWorldHandler.Up, _MotionResult.Normal) <= MaxSlopeAngle) ||
                                   (Vector2.Angle(PhysicsWorldHandler.Down, _MotionResult.Normal) <= MaxSlopeAngle);
                }

                if (!isValidSlope)
                {
                    PhysicsWorldHandler.CollisionCache.Add(this,
                                                    _MotionResult.Body,
                                                    _MotionResult.Point,
                                                    _MotionResult.Normal,
                                                    CollisionCache.GetCollisionFlags(request.RequestAxis, _RaycastDirectionSign));

                    #region Debug
                    if (_Debug)
                    {
                        DebugExtension.DebugArrow(_MotionResult.Point, _MotionResult.Normal, Color.yellow);
                    }
                    #endregion
                }
            }

            #region Debug
            if (_Debug)
            {
                var color = _RaycastDistance - _MotionResult.Distance > PhysicsWorldHandler.Epsilon ? Color.red : Color.green;
                Debug.DrawRay(_RaycastOrigin, _RaycastDirection * _MotionResult.Distance, color);
                DebugExtension.DebugLocalCube(Matrix4x4.identity, _RaycastSize, UnityEngine.Color.green, _RaycastOrigin);
                DebugExtension.DebugLocalCube(Matrix4x4.identity, _RaycastSize, color, _RaycastOrigin + _RaycastDirection * _MotionResult.Distance);
            }
            #endregion
        }

        // ------------------------------------------------------------------------
        private void SubmitMotion(Vector2 motion)
        {
            if (motion.sqrMagnitude > PhysicsWorldHandler.EpsilonSqr)
            {
                _SimulationBounds.center += (Vector3)motion;
                _SimulatedMotion += motion;
            }
        }

        // ------------------------------------------------------------------------
        public void SubmitSimulation()
        {
            UpdateSurfaceInfo();

            // Calculate this frame simulated velocity.
            _SimulatedVelocity = Velocity + _SimulatedTeleportMotion / TimeHandler.DeltaTime;

            // Build the collision flags from the collision cache data.
            for (int i = 0, count = _CollisionCacheIndices.Count; i < count; ++i)
            {
                _CollisionFlags |= PhysicsWorldHandler.CollisionCache[_CollisionCacheIndices[i]].CollisionFlags;
            }

            if (!KeepSpeedOnCollision)
            {
                if ((_CollisionFlags & CollisionCache.ECollisionFlags2D.Vertical) != 0)
                {
                    SpeedY = 0f;
                }

                if (SpeedX > PhysicsWorldHandler.Epsilon && (_CollisionFlags & CollisionCache.ECollisionFlags2D.HorizontalPos) != 0 ||
                    SpeedX < -PhysicsWorldHandler.Epsilon && (_CollisionFlags & CollisionCache.ECollisionFlags2D.HorizontalNeg) != 0)
                {
                    SpeedX = 0f;
                }
            }

            // Clear frame data
            _SimulatedTeleportMotion = Vector2.zero;
            _SimulatedMotion = Vector2.zero;
            ExternalVelocity = Vector2.zero;

            // Debug
            DebugDrawCollisionFlag();
        }

        // ------------------------------------------------------------------------        
        private void UpdateSurfaceInfo()
        {
            if (_KeepSurfaceInfo)
            {
                return;
            }

            _SurfaceInfo.Reset();
            _SurfaceInfo.TopNormal = PhysicsWorldHandler.CollisionCache.GetNormal(_CollisionCacheIndices, CollisionCache.ECollisionFlags2D.VerticalPos);

            // Skip update to the surface info
            if (!_ResolveSurfaceInfo)
            {
                return;
            }

            if (GravityScale == 0f)
            {
                return;
            }

            if (SpeedY > PhysicsWorldHandler.Epsilon)
            {
                return;
            }

            var probeDistance = 0.1f;
            var probeOffset = 0f;
            _SupportedMotionRequest = new MotionRequest(PhysicsWorldHandler.EAxes.Vertical, probeDistance * -1f);

            // Probe for potential collisions.
            var nbResults = ExecuteMotionRaycast(_SupportedMotionRequest, probeOffset);
            if (nbResults > 0)
            {
                for (int i = 0; i < nbResults; ++i)
                {
                    var result = _RaycastResults[i];

                    if (!IsValidResult(ref result, _SupportedMotionRequest.IsHorizontal))
                    {
                        continue;
                    }

                    var distance = result.distance - SkinWidthY - probeOffset;

                    _SurfaceInfo.State = distance <= _SupportedDistanceThreshold ? SurfaceInfo.EState.Supported : SurfaceInfo.EState.Unsupported;
                    _SurfaceInfo.Normal = result.normal;
                    _SurfaceInfo.ExcessDistance = distance;
                    _SurfaceInfo.Point = result.point;
                    _SurfaceInfo.Collider = result.collider;
                    _SurfaceInfo.GameObject = result.collider.gameObject;

                    // @Special Case: In an extreme situation, the boxcast may return an horizontal normal when we stand exactly at the edge of a "vertical" collider segment.
                    // @Special Case: In an extreme situation, the boxcast may return an inverted vertical normal due to some penetration in the colliders.
                    if (_SurfaceInfo.Normal.y == 0f || _SurfaceInfo.Normal.y == -1f)
                    {
                        if (_SurfaceInfo.Normal.y == 0f)
                        {
                            // Impulse to fall down...
                            //AddImpulse(new MotionImpulse(_SurfaceInfo.Normal * 0.5f, 0.25f));
                        }

                        _SurfaceInfo.Normal = PhysicsWorldHandler.Up;
                    }

                    // Reset the gravity scale when we are supported for convenience.
                    if (_SurfaceInfo.State == SurfaceInfo.EState.Supported)
                    {
                        ResetGravityScale();
                    }

                    break;
                }
            }

            // Update the state of the body.
            if (State == EState.Grounded && _SurfaceInfo.State == SurfaceInfo.EState.Unsupported)
            {
                State = EState.InAir;
            }
            else if (State == EState.InAir && _SurfaceInfo.State == SurfaceInfo.EState.Supported && SpeedY < PhysicsWorldHandler.Epsilon)
            {
                State = EState.Grounded;
            }
        }

        // ------------------------------------------------------------------------
        // Each MotionRequest will be passed in LIFO to the Simulate() method.
        // This is useful in many cases, most notably slopes (ex: Vertical +, Horizontal, Vertical -).
        // This support dynamically adding motion requets.
        private void BuildMotionRequests()
        {
            _MotionRequests.Clear();

            // Excess Distance Motion Y
            if (IsGrounded && Mathf.Abs(_SurfaceInfo.ExcessDistance) > PhysicsWorldHandler.Epsilon)
            {
                _MotionRequests.Push(new MotionRequest(PhysicsWorldHandler.EAxes.Vertical, _SurfaceInfo.ExcessDistance * -1f));
            }

            // Motion
            var motion = _Motion;
            if (motion.sqrMagnitude > PhysicsWorldHandler.EpsilonSqr)
            {
                // Motion Y
                if (Mathf.Abs(motion.y) > PhysicsWorldHandler.Epsilon)
                {
                    if (motion.y > PhysicsWorldHandler.Epsilon)
                    {
                        _MotionRequests.Push(new MotionRequest(PhysicsWorldHandler.EAxes.Vertical, motion.y));
                    }
                    else
                    {
                        _MotionRequests.Push(new MotionRequest(PhysicsWorldHandler.EAxes.Vertical, motion.y));
                    }
                }

                // Motion X
                if (Mathf.Abs(motion.x) > PhysicsWorldHandler.Epsilon)
                {
                    var motionOnSurface = TransformMotion(motion.x, _SurfaceInfo.Normal, true);

                    if (_SurfaceInfo.State == SurfaceInfo.EState.Unsupported || Mathf.Abs(motionOnSurface.y) <= PhysicsWorldHandler.Epsilon)
                    {
                        _MotionRequests.Push(new MotionRequest(PhysicsWorldHandler.EAxes.Horizontal, motion.x));
                    }
                    else if (motionOnSurface.y > float.Epsilon)
                    {
                        _MotionRequests.Push(new MotionRequest(PhysicsWorldHandler.EAxes.Horizontal, motionOnSurface.x));
                        _MotionRequests.Push(new MotionRequest(PhysicsWorldHandler.EAxes.Vertical, motionOnSurface.y));
                    }
                    else
                    {
                        _MotionRequests.Push(new MotionRequest(PhysicsWorldHandler.EAxes.Vertical, motionOnSurface.y));
                        _MotionRequests.Push(new MotionRequest(PhysicsWorldHandler.EAxes.Horizontal, motionOnSurface.x));
                    }
                }
            }
        }

        // ------------------------------------------------------------------------
        public Vector2 TransformMotion(float motion, Vector2 normal, bool isHorizontal)
        {
            if (normal == PhysicsWorldHandler.Up)
            {
                return Vector2.right * motion;
            }

            if (normal.sqrMagnitude <= float.Epsilon)
            {
                return Vector2.right * motion;
            }

            var perpendicularNormal = new Vector2(normal.y, -normal.x);

            Vector2 transformedMotion;
            if (isHorizontal)
            {
                transformedMotion = Mathf.Sign(perpendicularNormal.x) == Mathf.Sign(motion) ? perpendicularNormal : -perpendicularNormal;
            }
            else
            {
                transformedMotion = Mathf.Sign(perpendicularNormal.y) == Mathf.Sign(motion) ? perpendicularNormal : -perpendicularNormal;
            }

            transformedMotion *= Mathf.Abs(motion);

            if (Mathf.Abs(transformedMotion.y) < PhysicsWorldHandler.Epsilon)
            {
                transformedMotion.y = 0f;
            }

            return transformedMotion;
        }

        // ------------------------------------------------------------------------        
        private int ExecuteMotionRaycast(MotionRequest request, float offset = 0f)
        {
            // Clear the values to prevent stale data from previous simulation.
            _RaycastDirection = Vector2.zero;
            _RaycastOrigin = Vector2.zero;
            _RaycastSize = Vector2.zero;
            _RaycastDirectionSign = 0f;
            _RaycastDistance = 0f;

            var desiredAxisMotion = request.Motion;
            var desiredAxisMotionAbs = Mathf.Abs(desiredAxisMotion);
            if (desiredAxisMotionAbs < PhysicsWorldHandler.Epsilon)
            {
                // No motion to be processed for this axis, abort.
                return -1;
            }

            var axis = request.Axis;
            var perpendicularAxis = request.PerpendicularAxis;
            var skinWidth = request.IsHorizontal ? SkinWidthX : SkinWidthY;

            // Misc Parameters.
            _RaycastDirectionSign = Mathf.Sign(desiredAxisMotion);
            _RaycastDirection = axis * _RaycastDirectionSign;
            _RaycastDistance = desiredAxisMotionAbs + skinWidth + offset;

            // Origin.
            var boundsSize = (request.RequestAxis == PhysicsWorldHandler.EAxes.Horizontal) ? Mathf.Abs(_SimulationBounds.size.x) : Mathf.Abs(_SimulationBounds.size.y);
            _RaycastOrigin = (_RaycastDirectionSign > 0) ? (Vector2)_SimulationBounds.center + axis * 0.5f * boundsSize - axis * _RaycastSizeOffset * 0.5f:
                                                           (Vector2)_SimulationBounds.center - axis * 0.5f * boundsSize + axis * _RaycastSizeOffset * 0.5f;

            _RaycastOrigin += (_RaycastDirection * offset * -1f);

            // Size.
            boundsSize = (request.RequestAxis == PhysicsWorldHandler.EAxes.Horizontal) ? Mathf.Abs(_SimulationBounds.size.y) : Mathf.Abs(_SimulationBounds.size.x);
            _RaycastSize = perpendicularAxis * boundsSize + axis * _RaycastSizeOffset;

            // Layer Mask.
            _RaycastLayerMask = GetRaycastLayerMask(request.IsHorizontal, _RaycastDirectionSign);
            if (_RaycastLayerMask == 0)
            {
                return 0;
            }

            if (IgnoreCollisions)
            {
                return 0;
            }

            //DebugExtension.DebugLocalCube(Matrix4x4.identity, _RaycastSize, UnityEngine.Color.black, _RaycastOrigin);
            //DebugExtension.DebugLocalCube(Matrix4x4.identity, _RaycastSize, UnityEngine.Color.black, _RaycastOrigin + (_RaycastDirection * _RaycastDistance));

            return Physics2D.BoxCastNonAlloc(_RaycastOrigin,
                                             _RaycastSize,
                                             0f,
                                             _RaycastDirection,
                                             _RaycastResults,
                                             _RaycastDistance,
                                             _RaycastLayerMask);
        }

        // ------------------------------------------------------------------------
        public LayerMask GetRaycastLayerMask(bool isHorizontalMotion, float directionSign)
        {
            return _LayerMask;
        }

        // ------------------------------------------------------------------------
        public bool IsValidResult(ref RaycastHit2D result, bool isHorizontal)
        {
            var resultCollider = result.collider;

            // Ignore contacts with ourself.
            if (resultCollider == _Collider)
            {
                return false;
            }

            // Check ignore collision
            var hitBody = resultCollider.gameObject.GetComponent<PhysicsBody>();
            if (hitBody != null)
            {
                if (hitBody.IgnoreCollisions)
                {
                    return false;
                }
            }

            return true;
        }

        // ------------------------------------------------------------------------
        public bool IsSlopeEntrance(Vector2 position)
        {
            return position.y - _SimulationBounds.min.y <= _SlopeDistanceThreshold;
        }

        #endregion

        // ------------------------------------------------------------------------
        private void InitializeProfile()
        {
            switch (_Profile)
            {
                case EProfile.Solid:
                {
                    SetLayer(transform, PhysicsWorldHandler.LayerIndexSolid);
                    _LayerMask = PhysicsWorldHandler.CollisionProfileSolid;
                    _DefaultGravityScale = 1f;
                    _ResolveSurfaceInfo = false;
                    break;
                }

                case EProfile.Player:
                {
                    SetLayer(transform, PhysicsWorldHandler.LayerIndexPlayer);
                    _LayerMask = PhysicsWorldHandler.CollisionProfilePlayer;
                    _DefaultGravityScale = 1f;
                    _ResolveSurfaceInfo = true;
                    break;
                }

                case EProfile.AI:
                {
                    SetLayer(transform, PhysicsWorldHandler.LayerIndexAI);
                    _LayerMask = PhysicsWorldHandler.CollisionProfileAI;
                    _DefaultGravityScale = 1f;
                    _ResolveSurfaceInfo = true;
                    break;
                }
            }

            ResetGravityScale();
        }

        // ------------------------------------------------------------------------
        private void SetLayer(Transform transform, int layer)
        {
            transform.gameObject.layer = layer;
        }

        // ------------------------------------------------------------------------
        public void ResetGravityScale()
        {
            _RigidBody.gravityScale = _DefaultGravityScale;
        }     

        // ------------------------------------------------------------------------
        public bool Probe(PhysicsWorldHandler.EAxes axis, float motion)
        {
            var result = ExecuteMotionRaycast(new MotionRequest(axis, motion));
            return result == 0;
        }

        // ------------------------------------------------------------------------
        public float ProbeDistance(PhysicsWorldHandler.EAxes axis, float motion)
        {
            var result = ExecuteMotionRaycast(new MotionRequest(axis, motion));
            if (result > 0)
            {
                return _RaycastResults[0].distance;
            }

            return -1f;
        }

        #region Event Callbacks

        // ------------------------------------------------------------------------        
        public void OnCollisionCacheAdded(int index)
        {
            _CollisionCacheIndices.Add(index);
        }

        #endregion

        #region Debug

        // ------------------------------------------------------------------------
        private void DebugDrawCollisionFlag()
        {
            if (_Debug)
            {
                var pointA = Vector3.zero;
                var pointB = Vector3.zero;

                pointA.Set(_SimulationBounds.min.x, _SimulationBounds.max.y, 0f);
                pointB.Set(_SimulationBounds.max.x, _SimulationBounds.max.y, 0f);
                //if ((CollisionCache.ECollisionFlags2D.VerticalPos & _CollisionFlags) != 0)
                {
                    Debug.DrawLine(pointA, pointB, DebugGetCollisionFlagColor(CollisionCache.ECollisionFlags2D.VerticalPos));
                }

                pointA.Set(_SimulationBounds.max.x, _SimulationBounds.min.y, 0f);
                //if ((CollisionCache.ECollisionFlags2D.HorizontalPos & _CollisionFlags) != 0)
                {
                    Debug.DrawLine(pointA, pointB, DebugGetCollisionFlagColor(CollisionCache.ECollisionFlags2D.HorizontalPos));
                }

                pointB.Set(_SimulationBounds.min.x, _SimulationBounds.min.y, 0f);
                //if ((CollisionCache.ECollisionFlags2D.VerticalNeg & _CollisionFlags) != 0)
                {
                    Debug.DrawLine(pointA, pointB, DebugGetCollisionFlagColor(CollisionCache.ECollisionFlags2D.VerticalNeg));
                }

                pointA.Set(_SimulationBounds.min.x, _SimulationBounds.max.y, 0f);
                //if ((CollisionCache.ECollisionFlags2D.HorizontalNeg & _CollisionFlags) != 0)
                {
                    Debug.DrawLine(pointA, pointB, DebugGetCollisionFlagColor(CollisionCache.ECollisionFlags2D.HorizontalNeg));
                }
            }            
        }

        // ------------------------------------------------------------------------
        private Color DebugGetCollisionFlagColor(CollisionCache.ECollisionFlags2D flag)
        {
            if ((flag & _CollisionFlags) != 0)
            {
                return Color.red;
            }

            return Color.grey;
        }

        // ------------------------------------------------------------------------
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("--- Motion [{0:0.00}, {1:0.00}] - Velocity [{2:0.00}] - External [{3:0.00}] - Gravity Scale [{4:0.00}] ---\n{5}",
                _Motion.x,
                _Motion.y,
                Velocity,
                ExternalVelocity,
                GravityScale,
                _SurfaceInfo.ToString());

            return stringBuilder.ToString();
        }

        #endregion
    }
}
