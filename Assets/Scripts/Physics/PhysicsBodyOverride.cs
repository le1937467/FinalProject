using UnityEngine;

namespace LDS.GameProgramming
{
    // ------------------------------------------------------------------------
    public class PhysicsBodyOverride : MonoBehaviour
    {
        [SerializeField]
        private PhysicsBody.EProfile _Profile;

        [SerializeField]
        [Range(0.1f, 0.95f)]
        private float _SkinWidthRatio = 0.1f;

        [SerializeField]
        [Range(1f, 2f)]
        private float _SkinWidthRatioYFactor = 1f;

        [SerializeField]
        private float _MaxSlopeAngle = 45f;

        #region Properties

        // ------------------------------------------------------------------------
        public PhysicsBody.EProfile Profile => _Profile;

        // ------------------------------------------------------------------------
        public float SkinWidthRatio => _SkinWidthRatio;

        // ------------------------------------------------------------------------
        public float SkinWidthRatioYFactor => _SkinWidthRatioYFactor;

        // ------------------------------------------------------------------------
        public float MaxSlopeAngle => _MaxSlopeAngle;

        #endregion
    }
}