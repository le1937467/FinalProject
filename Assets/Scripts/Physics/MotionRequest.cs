using UnityEngine;

namespace LDS.GameProgramming
{
    // ------------------------------------------------------------------------
    public struct MotionRequest
    {
        #region Properties

        // ------------------------------------------------------------------------
        // The axis on which we will apply the motion.
        public PhysicsWorldHandler.EAxes RequestAxis
        {
            get;
            set;
        }

        // ------------------------------------------------------------------------
        // The motion we want to apply of the specified axis.
        public float Motion
        {
            get;
            set;
        }

        // ------------------------------------------------------------------------
        public bool IsHorizontal
        {
            get
            {
                return RequestAxis == PhysicsWorldHandler.EAxes.Horizontal;
            }
        }

        // ------------------------------------------------------------------------
        public bool IsVertical
        {
            get
            {
                return RequestAxis == PhysicsWorldHandler.EAxes.Vertical;
            }
        }

        // ------------------------------------------------------------------------
        public Vector2 Axis
        {
            get
            {
                return IsHorizontal ? PhysicsWorldHandler.HorizontalAxis : PhysicsWorldHandler.VerticalAxis;
            }
        }

        // ------------------------------------------------------------------------
        public Vector2 PerpendicularAxis
        {
            get
            {
                return IsHorizontal ? PhysicsWorldHandler.VerticalAxis : PhysicsWorldHandler.HorizontalAxis;
            }
        }

        // ------------------------------------------------------------------------
        public Vector2 Direction
        {
            get
            {
                return Motion > 0f ? Axis : Axis * -1f;
            }
        }

        // ------------------------------------------------------------------------
        public float DirectionSign
        {
            get
            {
                return Mathf.Sign(Motion);
            }
        }

        #endregion

        // ------------------------------------------------------------------------
        public MotionRequest(PhysicsWorldHandler.EAxes eAxis, float motion)
        {
            RequestAxis = eAxis;
            Motion = motion;
        }
    }
}