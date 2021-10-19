using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LDS.GameProgramming
{
    // ------------------------------------------------------------------------
    public struct CollisionData
    {
        private PhysicsBody _Body;
        private PhysicsBody _OtherBody;
        private Vector2 _Position;
        private Vector2 _Normal;
        private CollisionCache.ECollisionFlags2D _CollisionFlags;

        #region Properties

        // ------------------------------------------------------------------------
        public PhysicsBody Body
        {
            get
            {
                return _Body;
            }
        }

        // ------------------------------------------------------------------------
        public PhysicsBody OtherBody
        {
            get
            {
                return _OtherBody;
            }
        }

        // ------------------------------------------------------------------------
        public Vector2 Position
        {
            get
            {
                return _Position;
            }
        }

        // ------------------------------------------------------------------------
        public Vector2 Normal
        {
            get
            {
                return _Normal;
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

        #endregion

        // ------------------------------------------------------------------------
        public CollisionData(PhysicsBody body, PhysicsBody otherBody, Vector2 position, Vector2 normal, CollisionCache.ECollisionFlags2D collisionFlags)
        {
            _Body = body;
            _OtherBody = otherBody;
            _Position = position;
            _Normal = normal;
            _CollisionFlags = collisionFlags;
        }
    }
}