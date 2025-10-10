namespace Game.Interfaces {
    public interface IPlatform : IStickable {
        ushort Index { get; }
        
        float ScoreMultiplier { get; }
    }
}