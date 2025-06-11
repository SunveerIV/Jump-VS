using UnityEngine;
using UnityEngine.Serialization;

public class Director : MonoBehaviour {

    private static readonly Vector2 SCALE_SCALE = new Vector2(5, 5);
    private static readonly Vector2 VELOCITY_AMPLIFIER = new Vector2(4, 4);
    public Camera mainCamera;
    public Circle player;

    private void Start() {
        mainCamera = Camera.main;
    }
    
    private void Update() {
        Move();
        Rotate();
        Scale();
        CheckDelete();
    }

    private void Move() {
        Vector3 InputPosition;
        if (Input.touchCount > 0) {
            InputPosition = Input.GetTouch(0).position;
        } else {
            InputPosition = Input.mousePosition;
        }
        Vector2 CirclePos = player.transform.position;
        transform.position = new Vector2(Mathf.Clamp(mainCamera.ScreenToWorldPoint(InputPosition).x, CirclePos.x - 3f, CirclePos.x + 3f), Mathf.Clamp(mainCamera.ScreenToWorldPoint(InputPosition).y, CirclePos.y - 3f, CirclePos.y + 3f));

    }
    
    private void Rotate() {
        float angle = 180f + Mathf.Atan2(player.transform.position.y - transform.position.y, player.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void Scale() {
        float distance = Vector2.Distance(player.transform.position, transform.position);
        transform.localScale = new Vector2(distance, distance) * SCALE_SCALE;
    }

    private void CheckDelete() {
        if (Input.GetMouseButtonUp(0)) {
            player.transform.SetParent(null);
            player.hasStuck = false;
            player.RB.linearVelocity = (transform.position - player.transform.position) * VELOCITY_AMPLIFIER;
            Destroy(gameObject);
        }
    }
}