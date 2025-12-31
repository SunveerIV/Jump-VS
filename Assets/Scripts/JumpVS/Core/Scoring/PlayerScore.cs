using System;
using Game.Constants;

namespace JumpVS.Core.Scoring {
    public sealed class PlayerScore {
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
            float xPosDifference = 1.5f - landEvent.DistanceFromCenter;
            int platformDifferential = landEvent.NewPlatformIndex - previousPlatformIndex;
            previousPlatformIndex = landEvent.NewPlatformIndex;

            if (platformDifferential > 0) {
                float bounceMultiplier = MathF.Pow(Player.BASE_POWER_FOR_BOUNCES, cachedBounces);
                float positionDifferenceMultiplier = MathF.Pow(xPosDifference, Player.EXPONENT_FOR_PLATFORM_DIFFERENCE);
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