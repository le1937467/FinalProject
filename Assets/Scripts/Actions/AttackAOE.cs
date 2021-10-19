using UnityEngine;

namespace LDS.GameProgramming
{
    // ------------------------------------------------------------------------
    public class AttackAOE : MonoBehaviour
    {
        [SerializeField]
        [Range(1, 10)]
        private int _Damage = 1;

        [SerializeField]
        private LayerMask _Layer;

        #region Properties

        // ------------------------------------------------------------------------
        public int Damage => _Damage;

        #endregion

        // ------------------------------------------------------------------------
        private void OnTriggerEnter2D(Collider2D collision)
        {
            var layer = collision.gameObject.layer;
            if ((_Layer & (1 << layer)) != 0)
            {
                var physicsBody = collision.attachedRigidbody?.GetComponentInChildren<PhysicsBody>();
                GameEventsHandler.OnDamageReceived?.Invoke(this, physicsBody);

                //var actionAlive = collision.attachedRigidbody?.GetComponentInChildren<ActionAlive>();
                //if (actionAlive != null)
                //{
                //    actionAlive.HP -= _Damage;
                //}
            }
        }
    }
}
