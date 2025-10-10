using Game.Behaviours.Platforms;

namespace Game.Interfaces {
    public interface ILevelMultiplayer : ILevel {
        PlatformMultiplayer GetPlatformAtIndex(int index);
    }
}