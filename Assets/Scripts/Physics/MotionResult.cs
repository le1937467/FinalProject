using UnityEngine;

namespace LDS.GameProgramming
{
    // ------------------------------------------------------------------------
    public struct MotionResult
    {
        private PhysicsBody _Body;
        private PhysicsBody _OtherBody;
        private Vector2 _Point;
        private Vector2 _Normal;
        private float _Distance;

        #region Properties

        // ------------------------------------------------------------------------
        public PhysicsBody Body
        {
            get
            {
                return _Body;
            }

            set
            {
                _Body = value;
            }
        }

        // ------------------------------------------------------------------------
        public PhysicsBody OtherBody
        {
            get
            {
                return _OtherBody;
            }

            set
            {
                _OtherBody = value;
            }
        }

        // ------------------------------------------------------------------------
        public float Distance
        {
            get
            {
                return _Distance;
            }

            set
            {
                _Distance = value;
            }
        }

        // ------------------------------------------------------------------------
        public Vector2 Point
        {
            get
            {
                return _Point;
            }

            set
            {
                _Point = value;
            }
        }

        // ------------------------------------------------------------------------
        public Vector2 Normal
        {
            get
            {
                return _Normal;
            }

            set
            {
                _Normal = value;
            }
        }

        #endregion

        // ------------------------------------------------------------------------
        public void Reset()
        {
            _Body = null;
            _OtherBody = null;
            _Distance = 0f;
            _Normal = Vector2.up;
            _Point = Vector2.zero;
        }
    }

}
