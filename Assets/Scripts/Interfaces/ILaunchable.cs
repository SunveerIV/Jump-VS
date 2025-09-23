namespace Interfaces {
    public interface ILaunchable {
        void Launch(UnityEngine.Vector3 directorPosition);
        bool HasStuck { set; }
        UnityEngine.Transform transform { get; }
    }
}