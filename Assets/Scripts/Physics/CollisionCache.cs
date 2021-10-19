using System;
using System.Collections.Generic;
using UnityEngine;

namespace LDS.GameProgramming
{
    // ------------------------------------------------------------------------
    public class CollisionCache
    {
        private const int _Size = 128;
        private List<CollisionData> _Collisions = new List<CollisionData>(_Size);

        #region Enums

        // Bitfield used to represent the dynamic body collision state before/after the simulation.
        public enum ECollisionFlags2D
        {
            None = 0,
            HorizontalPos = 0x1,
            HorizontalNeg = 0x2,
            VerticalPos = 0x4,
            VerticalNeg = 0x8,

            Horizontal = HorizontalPos | HorizontalNeg,
            Vertical = VerticalPos | VerticalNeg,
            All = Horizontal | Vertical,
        };

        #endregion

        // ------------------------------------------------------------------------
        public CollisionData this[int index]
        {
            get
            {
                return _Collisions[index];
            }
        }

        // ------------------------------------------------------------------------
        public void Add(PhysicsBody body, PhysicsBody otherBody, Vector2 position, Vector2 normal, ECollisionFlags2D collisionFlag)
        {
            if (_Collisions.Count  >= _Size)
            {
                Debug.AssertFormat(false, "Number of collisions goes above the collision cache initial limit!");
            }

            _Collisions.Add(new CollisionData(body, otherBody, position, normal, collisionFlag));

            body.OnCollisionCacheAdded(_Collisions.Count - 1);
        }

        // ------------------------------------------------------------------------
        public void Clear()
        {
            _Collisions.Clear();
        }

        // ------------------------------------------------------------------------
        public Vector2 GetNormal(List<int> indices, ECollisionFlags2D collisionFlag)
        {
            var normal = Vector2.zero;
            var normalCount = 0;

            for (int i = 0, count = indices.Count; i < count; ++i)
            {
                var collision = _Collisions[indices[i]];
                if ((collision.CollisionFlags & collisionFlag) != 0)
                {
                    normal += collision.Normal;
                    normalCount++;
                }
            }

            return normalCount > 0 ? normal / normalCount : normal;
        }

        // ------------------------------------------------------------------------
        static public ECollisionFlags2D GetCollisionFlags(PhysicsWorldHandler.EAxes eAxis, float motionSign)
        {
            switch (eAxis)
            {
                case PhysicsWorldHandler.EAxes.Horizontal:
                {
                    return (motionSign > 0) ? ECollisionFlags2D.HorizontalPos : ECollisionFlags2D.HorizontalNeg;
                }

                case PhysicsWorldHandler.EAxes.Vertical:
                {
                    return (motionSign > 0) ? ECollisionFlags2D.VerticalPos : ECollisionFlags2D.VerticalNeg;
                }

                default:
                {
                    return ECollisionFlags2D.None;
                }
            }
        }
    }
}