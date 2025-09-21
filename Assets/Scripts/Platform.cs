using UnityEngine;

public class Platform : MonoBehaviour {
    
    private int index;

    public int Index => index;

    public static Platform Create(Platform prefab, Vector3 position, Quaternion rotation, int index) {
        Platform platform = Instantiate(prefab, position, rotation);
        platform.index = index;
        return platform;
    }
}