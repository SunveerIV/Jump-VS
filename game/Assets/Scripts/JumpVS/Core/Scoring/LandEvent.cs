namespace JumpVS.Core.Scoring {
    public readonly struct LandEvent {
        
        public ushort NewPlatformIndex { get; }
        public float DistanceFromCenter { get; }
        public float NewPlatformScoreMultiplier { get; }

        public LandEvent(ushort newPlatformIndex, float distanceFromCenter, float newPlatformScoreMultiplier) {
            NewPlatformIndex = newPlatformIndex;
            DistanceFromCenter = distanceFromCenter;
            NewPlatformScoreMultiplier = newPlatformScoreMultiplier;
        }
    }
}