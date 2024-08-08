using System;
using UnityEngine;

namespace _Scripts {
    public class SpriteController : MonoBehaviour {
        private Camera mainCam;
        
        private void Awake() {
            mainCam = Camera.main;
        }

        private void Update() {
            PointSprite();
        }

        private void PointSprite() {
            // Rotate obj to point at mouse
            Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            Vector3 dir = mousePos - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }
}