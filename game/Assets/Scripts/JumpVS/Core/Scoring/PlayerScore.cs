using System;

namespace JumpVS.Core.Scoring {
    public sealed class PlayerScore {
        private const float HALF_PLATFORM_WIDTH = 1.5f;
        private const float BASE_POWER_FOR_BOUNCES = 1.3f;
        private const float EXPONENT_FOR_PLATFORM_DIFFERENCE = 12f;
        
        private int cachedBounces;
        private int previousPlatformIndex;
        private float previousScore;

        public float Value { get; private set; }
        
        public PlayerScore() {
            cachedBounces = 0;
            previousPlatformIndex = 0;
            previousScore = 0f;
            Value = 0f;
        }

        public void Bounce() {
            cachedBounces++;
        }

        public void LandOnPlatform(LandEvent landEvent) {
            float xPosDifference = HALF_PLATFORM_WIDTH - landEvent.DistanceFromCenter;
            int platformDifferential = landEvent.NewPlatformIndex - previousPlatformIndex;
            previousPlatformIndex = landEvent.NewPlatformIndex;

            if (platformDifferential > 0) {
                float bounceMultiplier = MathF.Pow(BASE_POWER_FOR_BOUNCES, cachedBounces);
                float positionDifferenceMultiplier = MathF.Pow(xPosDifference, EXPONENT_FOR_PLATFORM_DIFFERENCE);
                previousScore = landEvent.NewPlatformScoreMultiplier * platformDifferential * bounceMultiplier *
                                positionDifferenceMultiplier;
                Value += previousScore;
            }
            else if (platformDifferential < 0) {
                Value -= previousScore;
                previousScore = 0;
            }

            cachedBounces = 0;
        }
    }
}