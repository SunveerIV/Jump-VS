namespace Game.Interfaces {
    public interface IPlatform : IStickable {
        int Index { get; }
        
        float ScoreMultiplier { get; }
    }
}