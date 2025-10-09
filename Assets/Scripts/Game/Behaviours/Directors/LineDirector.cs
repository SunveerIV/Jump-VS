using System;
using Game.Behaviours.Players;
using UnityEngine;
using Game.Interfaces;

namespace Game.Behaviours.Directors {
    public class LineDirector : MonoBehaviour {

        private const float GROW_RATE = 0.04f;
        private const float MAX_DISTANCE = 3f;
        private const float TIME_STEP = 0.01f;

        [SerializeField] private LineRenderer line;
        
        private Camera mainCamera;
        private DateTime timeCreated;
        private ILaunchable launchable;
        
        public static LineDirector Create(LineDirector prefab, ILaunchable launchable) {
            LineDirector director = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            director.launchable = launchable;
            director.transform.SetParent(launchable.transform);
            director.mainCamera = Camera.main;
            director.timeCreated = DateTime.Now;
            director.Move();
            director.DrawTrajectory();
            return director;
        }

        public void Update() {
            Move();
            DrawTrajectory();
            Delete();
        }

        private void DrawTrajectory() {
            Vector2 startPos = launchable.transform.position;
            Vector2 startVel = (transform.position - launchable.transform.position) * PlayerSingleplayer.VELOCITY_AMPLIFIER;
            int resolution = (int)((DateTime.Now - timeCreated).TotalMilliseconds * GROW_RATE);
            line.positionCount = resolution;
            Vector3[] points = new Vector3[resolution];
            Debug.Log(points.Length);

            for (int i = 0; i < resolution; i++) {
                float t = i * TIME_STEP;
                Vector2 pos = startPos + startVel * t + 0.5f * Physics2D.gravity * t * t;
                points[i] = pos;
            }

            line.SetPositions(points);
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
            float inputWorldPosX = Mathf.Clamp(mainCamera.ScreenToWorldPoint(inputPosition).x,
                CirclePos.x - MAX_DISTANCE, CirclePos.x + MAX_DISTANCE);
            float inputWorldPosY = Mathf.Clamp(mainCamera.ScreenToWorldPoint(inputPosition).y,
                CirclePos.y - MAX_DISTANCE, CirclePos.y + MAX_DISTANCE);
            transform.position = new Vector2(inputWorldPosX, inputWorldPosY);
        }
        
        /// <summary>
        /// If the player releases MouseButton1 or stops touching the screen,
        /// launchable is launched and Director is destroyed
        /// </summary>
        private void Delete() {
            if (Input.GetMouseButtonUp(0)) {
                launchable.Launch(transform.position);
                Destroy(gameObject);
            }
        }
    }
}