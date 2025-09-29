namespace Game.Interfaces {
    public interface ILaunchable {
        void Launch(UnityEngine.Vector3 directorPosition);
        UnityEngine.Transform transform { get; }
    }
}