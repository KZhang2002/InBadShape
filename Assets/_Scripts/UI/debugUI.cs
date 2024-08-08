using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class debugUI : MonoBehaviour {
    private GameObject _player;
    private Rigidbody2D _rb;

    private MovementController _mc;

    private ShooterBehavior _sb;
    private GameObject _shooter;

    private readonly GUIStyle _debugGuiStyle = new();

    void Awake() {
        _player = GameObject.FindWithTag("Player");
        _mc = _player.GetComponent<MovementController>();
        _rb = _player.GetComponent<Rigidbody2D>();
        
        //todo refactor to make component type variable
        _shooter = FindEnemy("Shooter");
        if (_shooter) {
            _sb = _shooter.GetComponent<ShooterBehavior>();
        } else {
            _sb = null;
            _shooter = null;
        }
        
    }

    private GameObject FindEnemy(string tag) {
        return GameObject.FindGameObjectWithTag(tag);
    }

    private void OnGUI() {
        _debugGuiStyle.fontSize = 12;
        _debugGuiStyle.normal.textColor = Color.black;
        float x = 10f;
        float y = 0f;

        List<string> debugStrings = new List<string>();
        // debugStrings.Add($"Player Velocity: {_mc.Velocity}");
        // debugStrings.Add($"RigidBody Velocity: {_rb.velocity}");
        if (_sb) {
            Vector3 shooterPos = _shooter.transform.position;
            shooterPos.z = -0.2f;
            
            Debug.DrawRay(shooterPos + Vector3.left, Vector3.right * 2, Color.black);
            Debug.DrawRay(shooterPos + Vector3.down, Vector3.up * 2, Color.black);
            
            debugStrings.Add($"ShotAvailable: {_sb.shotAvailable}");
            debugStrings.Add($"Distance from shooter: {Vector2.Distance(shooterPos, _player.transform.position)}");
            debugStrings.Add($"Time left on shot clock: {_sb.timeLeftOnShotClock}");
            debugStrings.Add($"Shot clock cancelled?: {_sb.isShotClockPaused}");
            debugStrings.Add($"Has line of sight?: {_sb.hasLOSofPlayer}");
            debugStrings.Add($"In range of player?: {_sb.playerInRange}");
            debugStrings.Add($"Current action: {_sb.currentAction}");
        }
        else {
            _shooter = FindEnemy("Shooter");
            if (_shooter) { _sb = _shooter.GetComponent<ShooterBehavior>(); }
            else { debugStrings.Add($"No Shooters in level."); }
        }

        for (int i = 0; i < debugStrings.Count; i++) {
            GUI.Label(new Rect(x, y + (12 * (i + 1)), 200, 50), debugStrings[i], _debugGuiStyle);
        }
    }
}