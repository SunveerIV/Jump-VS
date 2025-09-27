using Interfaces;
using UnityEngine;

public class Director : MonoBehaviour {
    
    private const float SCALE_SCALE = 5f;
    private const float MAX_DISTANCE = 3f;

    private Camera mainCamera;
    private ILaunchable launchable;

    private void Start() {
        mainCamera = Camera.main;
    }

    private void Update() {
        Move();
        Rotate();
        Scale();
        CheckDelete();
    }

    /// <summary>
    /// Moves the object within +/- MAX_DISTANCE around the player's position based on touch input or mouse position.
    /// </summary>
    private void Move() {
        Vector2 inputPosition;
        #if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            inputPosition = Input.mousePosition;
        #elif UNITY_IOS || UNITY_ANDROID
            if (Input.touchCount > 0) {
                inputPosition = Input.GetTouch(0).position;
            }
            else {
                inputPosition = Vector2.zero;
            }
        #endif
        Vector2 CirclePos = launchable.transform.position;
        float inputWorldPosX = Mathf.Clamp(mainCamera.ScreenToWorldPoint(inputPosition).x, CirclePos.x - MAX_DISTANCE, CirclePos.x + MAX_DISTANCE);
        float inputWorldPosY = Mathf.Clamp(mainCamera.ScreenToWorldPoint(inputPosition).y, CirclePos.y - MAX_DISTANCE, CirclePos.y + MAX_DISTANCE);
        transform.position = new Vector2(inputWorldPosX, inputWorldPosY);
    }

    /// <summary>
    /// Rotates the director to always face away from the player's position.
    /// </summary>
    private void Rotate() {
        float distanceX = transform.position.x - launchable.transform.position.x;
        float distanceY = transform.position.y - launchable.transform.position.y;
        float rotationAngle = Mathf.Atan2(distanceY, distanceX) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotationAngle);
    }

    /// <summary>
    /// Scales the director based on its distance from the player.
    /// </summary>
    private void Scale() {
        float distance = Vector2.Distance(launchable.transform.position, transform.position);
        transform.localScale = new Vector2(distance * SCALE_SCALE, distance * SCALE_SCALE);
    }

    /// <summary>
    /// If the player releases the mouse button or finger, the director gets destroyed.
    /// Additionally, the player's parent is removed, and velocity is set to
    /// the difference between the player's position and the director's position.
    /// </summary>
    private void CheckDelete() {
        if (Input.GetMouseButtonUp(0)) {
            launchable.Launch(transform.position);
            Destroy(gameObject);
        }
    }

    public static Director Create(Director prefab, Vector3 position, Quaternion rotation, ILaunchable launchable) {
        Director director = Instantiate(prefab, position, rotation);
        director.launchable = launchable;
        director.transform.SetParent(launchable.transform);
        return director;
    }
}