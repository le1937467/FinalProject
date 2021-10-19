using System;

namespace LDS.GameProgramming
{
    // ------------------------------------------------------------------------
    public class GameEventsHandler
    {
        public static Action<AttackAOE, PhysicsBody> OnDamageReceived;
    }
}
