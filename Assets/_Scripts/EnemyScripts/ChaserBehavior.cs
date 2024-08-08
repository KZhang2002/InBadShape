using UnityEngine;
using UnityEngine.AI;

public class ChaserBehavior : MonoBehaviour {
    private GameObject _player;
    private Camera _mainCam;
    private Vector3 _targetPos;
    private NavMeshAgent _agent;

    void Awake() {
        _player = GameObject.FindGameObjectWithTag("Player");
        _mainCam = Camera.main;
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }

    void Update() {
        SetTargetPosition();
        SetAgentPosition();
    }

    void SetTargetPosition() {
        _targetPos = _player.transform.position;
    }

    void SetAgentPosition() {
        _agent.SetDestination(new Vector3(_targetPos.x, _targetPos.y, transform.position.z));
    }
}