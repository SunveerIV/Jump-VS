using UnityEngine;
using Game.Utility.Prefab;

public class Platform : MonoBehaviour {
    
    private int index;

    public int Index => index;

    public static Platform Create(Vector3 position, Quaternion rotation, int index) {
        Platform platform = Instantiate(PrefabContainer.PLATFORM, position, rotation);
        platform.index = index;
        return platform;
    }
}