using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CameraController : MonoBehaviour {
    private float zOffset = -10f;
    [Range(0,1)] [SerializeField] private float cameraLerpVal = 0.5f;
    [Range(0,50)] [SerializeField] private float maxXOffset = 8f;
    [Range(0,40)] [SerializeField] private float maxYOffset = 4f;
    private Camera _mainCam;
    private GameObject _player;

    void Awake() {
        _mainCam = Camera.main;
        _player = GameObject.FindGameObjectWithTag("Player");
        zOffset = _mainCam.transform.position.z;
    }
    
    void Update() {
        if (!_player) {
            // todo add code for game over camera
            Destroy(gameObject);
        }
        MoveCam();
    }

    void MoveCam() {
        Vector3 playerPos = _player.transform.position;
        Vector3 mousePos = _mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 newPos = Vector3.Lerp(playerPos, mousePos, 0.5f);
        
        // Bounds of max offset from player position
        float upperBoundYOffset = playerPos.y + maxYOffset;
        float lowerBoundYOffset = playerPos.y - maxYOffset;
        float upperBoundXOffset = playerPos.x + maxXOffset;
        float lowerBoundXOffset = playerPos.x - maxXOffset;
        
        // Hard limiter
        float yDist = newPos.y - playerPos.y;
        float xDist = newPos.x - playerPos.x;
        if (math.abs(yDist) > maxYOffset) newPos.y = yDist > 0 ? upperBoundYOffset : lowerBoundYOffset;
        if (math.abs(xDist) > maxXOffset) newPos.x = xDist > 0 ? upperBoundXOffset : lowerBoundXOffset;
        
        newPos.z = zOffset;
        transform.position = newPos;
    }
}