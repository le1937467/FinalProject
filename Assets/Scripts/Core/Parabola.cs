using UnityEngine;

namespace LDS.GameProgramming
{
    // ------------------------------------------------------------------------
    public abstract class Parabola
    {
        // ------------------------------------------------------------------------
        public static Vector2 CalculateInitialVelocity(Vector2 origin, Vector2 destination, float apexHeight, float gravityScale, ref float timeOfArrival)
        {
            var speedX = 0f;
            var speedY = 0f;

            var g = Physics2D.gravity.magnitude * gravityScale;
            var apex = (destination.y > origin.y) ? destination.y + apexHeight : origin.y + apexHeight;
            var height = apex - origin.y;

            var timeToReachApex = 0f;
            if (apex > origin.y)
            {
                speedY = Mathf.Sqrt(2.0f * g * height);
                timeToReachApex = speedY / g;
            }

            var timeToReachTarget = Mathf.Sqrt((2f * (apex - destination.y)) / g);

            timeOfArrival = timeToReachApex + timeToReachTarget;

            var direction = destination - origin;
            speedX = Mathf.Abs(direction.x) / timeOfArrival;

            var velocity = new Vector2(speedX * Mathf.Sign(direction.x), speedY);

            DebugExtension.DebugCircle2D(origin, Color.red, timeOfArrival);
            DebugExtension.DebugCircle2D(origin, Color.red, timeOfArrival);
            for (var i = 0f; i < timeOfArrival; i += 0.1f)
            {
                DebugExtension.DebugCircle2D(Parabola.PositionAtTime(origin, velocity, 1f, i), Color.red, timeOfArrival, 0.1f);
            }

            return velocity;
        }

        // ------------------------------------------------------------------------
        public static Vector2 PositionAtTime(Vector2 origin, Vector2 velocity, float gravityScale, float time)
        {
            var posX = velocity.x * time;
            var posY = velocity.y * time - (0.5f * Physics2D.gravity.magnitude * gravityScale * time * time);

            return new Vector2(posX + origin.x, posY + origin.y);
        }        
    }
}
