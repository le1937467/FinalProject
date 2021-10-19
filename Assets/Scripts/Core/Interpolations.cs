using UnityEngine;

namespace LDS.GameProgramming
{
    // ------------------------------------------------------------------------
    // http://sol.gfxile.net/interpolation/
    public abstract class Interpolations
    {
        // ------------------------------------------------------------------------
        public static float Linear(float origin, float target, float t)
        {
            return target * t + origin * (1f - t);
        }

        // ------------------------------------------------------------------------
        public static Vector2 Linear(Vector2 origin, Vector2 target, float t)
        {
            return target * t + origin * (1f - t);
        }

        // ------------------------------------------------------------------------
        public static float SmoothStep(float origin, float target, float t)
        {
            var smoothStep = t * t * (3 - 2 * t);
            return (origin * (1f - smoothStep)) + (target * smoothStep);
        }

        // ------------------------------------------------------------------------
        public static Vector2 SmoothStep(Vector2 origin, Vector2 target, float t)
        {
            var smoothStep = t * t * (3 - 2 * t);
            return  (origin * (1f - smoothStep)) + (target * smoothStep);
        }

        // ------------------------------------------------------------------------
        public static Vector3 SmoothStep(Vector3 origin, Vector3 target, float t)
        {
            var smoothStep = t * t * (3 - 2 * t);
            return (origin * (1f - smoothStep)) + (target * smoothStep);
        }

        // ------------------------------------------------------------------------
        public static Color SmoothStep(Color origin, Color target, float t)
        {
            var smoothStep = t * t * (3 - 2 * t);
            return (origin * (1f - smoothStep)) + (target * smoothStep);
        }

        // ------------------------------------------------------------------------
        public static Vector2 Squared(Vector2 origin, Vector2 target, float t)
        {
            var step = t * t;
            return (origin * (1f - step)) + (target * step);
        }

        // ------------------------------------------------------------------------
        public static float Squared(float origin, float target, float t)
        {
            var step = t * t;
            return (origin * (1f - step)) + (target * step);
        }

        // ------------------------------------------------------------------------
        public static Vector2 SquaredInv(Vector2 origin, Vector2 target, float t)
        {
            var step = 1f - (1f - t) * (1f - t);
            return (origin * (1f - step)) + (target * step);
        }

        // ------------------------------------------------------------------------
        public static float SquaredInv(float origin, float target, float t)
        {
            var step = 1f - (1f - t) * (1f - t);
            return (origin * (1f - step)) + (target * step);
        }

        // ------------------------------------------------------------------------
        public static Vector2 Cubed(Vector2 origin, Vector2 target, float t)
        {
            var step = t * t * t;
            return (origin * (1f - step)) + (target * step);
        }

        // ------------------------------------------------------------------------
        public static float Cubed(float origin, float target, float t)
        {
            var step = t * t * t;
            return (origin * (1f - step)) + (target * step);
        }

        // ------------------------------------------------------------------------
        public static Color Cubed(Color origin, Color target, float t)
        {
            var step = t * t * t;
            return (origin * (1f - step)) + (target * step);
        }

        // ------------------------------------------------------------------------
        public static Vector2 CubedInv(Vector2 origin, Vector2 target, float t)
        {
            var step = 1f - (1f - t) * (1f - t) * (1f - t);
            return (origin * (1f - step)) + (target * step);
        }

        // ------------------------------------------------------------------------
        public static float CubedInv(float origin, float target, float t)
        {
            var step = 1f - (1f - t) * (1f - t) * (1f - t);
            return (origin * (1f - step)) + (target * step);
        }

        // ------------------------------------------------------------------------
        public static Color CubedInv(Color origin, Color target, float t)
        {
            var step = 1f - (1f - t) * (1f - t) * (1f - t);
            return (origin * (1f - step)) + (target * step);
        }

        // ------------------------------------------------------------------------
        public static Vector2 WeightedAverage(Vector2 origin, Vector2 target, float slowDownFactor)
        {
            return ((origin * (slowDownFactor - 1)) + target) / slowDownFactor;
        }

        // ------------------------------------------------------------------------
        public static Vector3 WeightedAverage(Vector3 origin, Vector3 target, float slowDownFactor)
        {
            return ((origin * (slowDownFactor - 1)) + target) / slowDownFactor;
        }

        // ------------------------------------------------------------------------
        public static float WeightedAverage(float origin, float target, float slowDownFactor)
        {
            return ((origin * (slowDownFactor - 1)) + target) / slowDownFactor;
        }

        // ------------------------------------------------------------------------
        public static Color WeightedAverage(Color origin, Color target, float slowDownFactor)
        {
            return ((origin * (slowDownFactor - 1)) + target) / slowDownFactor;
        }        
    }
}