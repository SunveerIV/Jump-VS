namespace Game.Interfaces {
    public interface IPlatform {
        UnityEngine.GameObject gameObject { get; }
        
        ushort Index { get; }
        
        float ScoreMultiplier { get; }
    }
}