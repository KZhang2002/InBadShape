using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityTimer;

public class ShooterBehavior : MonoBehaviour {
    #region External References
    
    private GameObject _player;
    private Camera _mainCam;
    private FiringManager _fm;
    
    #endregion
    
    #region Internal References
    
    private NavMeshAgent _agent;
    private Rigidbody2D _rb;
    private CircleCollider2D _cc;
    [SerializeField] private Bullet bullet;
    [SerializeField] private GameObject muzzleObj;
    [SerializeField] private GameObject spriteGroup;
    private Vector3 MuzzlePoint => muzzleObj.transform.position;
    
    #endregion
    
    private Vector3 _targetPos;

    [Tooltip("Delay between shots from enemy in seconds.")] [SerializeField]
    private float shotDelay = 2f;
    
    [Tooltip("Required distance from player before this enemy shoots.")] [SerializeField]
    private float shootDistance = 10f;
    
    [Tooltip("When distance from player is above this, enemy will move towards the player.")] [SerializeField]
    private float upperMoveDistance = 15f;
    
    [Tooltip("When distance from player is below this, enemy will move away from the player.")] [SerializeField]
    private float lowerMoveDistance = 5f;

    [Tooltip("Max speed in degrees that sprite can turn.")] [SerializeField]
    private float maxTurnSpeed = 20f;

    [SerializeField] private float maxCastRange = 100f;

    private Timer _shotClock;
    private bool _shotFlag;
    //private bool;
    
    #region Debug Status Variables
    
    public bool shotAvailable;
    public float timeLeftOnShotClock;
    public bool isShotClockPaused;
    
    #endregion
    
    void Awake() {
        _player = GameObject.FindGameObjectWithTag("Player");
        _cc = GetComponent<CircleCollider2D>();
        _mainCam = Camera.main;
        _rb = GetComponent<Rigidbody2D>();
        _fm = FiringManager.Get;
        
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;

        StartShotClock();
    }

    void Update() {
        Vector3 playerPos = _player.transform.position;
        Vector3 selfPos = transform.position;
        
        float playerDistance = Vector2.Distance(playerPos, selfPos);
        bool tooClose = playerDistance < lowerMoveDistance;
        bool tooFar = playerDistance > upperMoveDistance;
        bool inRange = !tooFar && !tooClose;
        
        Vector2 dirToPlayer = playerPos - selfPos;
        dirToPlayer = dirToPlayer.normalized;
        LayerMask mask = LayerMask.GetMask("Entity", "Wall");
        RaycastHit2D hit = Physics2D.Raycast(selfPos, dirToPlayer, maxCastRange, mask);
        bool hasLineOfSight = hit && hit.collider.gameObject.CompareTag("Player");
        
        #region Line Of Sight Debug Code
        Vector3 newSelfPos = selfPos;
        newSelfPos.z = -1f;
        Vector3 endPoint = hit.collider ? hit.point : (Vector2)selfPos + dirToPlayer * maxCastRange;
        endPoint.z = -1f;
        Debug.DrawLine(newSelfPos, endPoint, Color.magenta);
        #endregion
        
        shotAvailable = hasLineOfSight && inRange; // status variable

        PointSpriteGroup(inRange, hasLineOfSight, dirToPlayer);
        
        if (!hasLineOfSight || !inRange) {
            StopShotClock();
            //todo add conditional for tooclose
            if (tooClose) {
                MoveAwayFromPlayer(dirToPlayer);
            }
            else {
                MoveTowardsPlayer();
            }
            
            Debug.DrawRay(_targetPos + Vector3.left, Vector3.right * 2, Color.red);
            Debug.DrawRay(_targetPos + Vector3.down, Vector3.up * 2, Color.red);
            
            return;
        }
        
        if (_shotClock.isCancelled) {
            StopMoving();
            StartShotClock();
        }

        timeLeftOnShotClock = _shotClock.GetTimeRemaining();
        isShotClockPaused = _shotClock.isPaused;
        
        if (_shotFlag) {
            Vector2 dirToMuzzle = MuzzlePoint - selfPos;
            Shoot(dirToMuzzle);
        }
    }

    void PointSpriteGroup(bool inRange, bool hasLineOfSight, Vector2 directionTowardsPlayer) {
        if (inRange && hasLineOfSight) {
            PointSprites(directionTowardsPlayer);
        }
        else {
            Vector2 velocityDir = _agent.velocity;
            PointSprites(velocityDir);
        }
    }

    void MoveTowardsPlayer() {
        _targetPos = _player.transform.position;
        MoveTowards(_targetPos);
    }
    
    void MoveAwayFromPlayer(Vector2 dirToPlayer) {
        Vector2 awayDir = dirToPlayer * -1;
        _targetPos = _player.transform.position + (Vector3)awayDir * lowerMoveDistance;
        MoveTowards(_targetPos);
    }

    void StopMoving() {
        _targetPos = transform.position;
        MoveTowards(_targetPos);
    }

    void MoveTowards(Vector2 position) {
        _agent.SetDestination(position);
    }

    void Shoot(Vector2 shotDirection) {
        //if (_rb.velocity.magnitude > 0f) return;
        Debug.Log("Shooter shot!");

        _fm.FireProjectile(bullet, entityType.enemy, muzzleObj.transform.position, shotDirection);
        
        _shotFlag = false;
        StartShotClock();
    }

    Timer StartShotClock() {
        _shotClock = this.AttachTimer(shotDelay, () => {
            _shotFlag = true;
        });
        
        return _shotClock;
    }
    
    void StopShotClock() {
        if (!_shotClock.isCancelled) {
            Timer.Cancel(_shotClock);
        }
    }

    void PointSprites(Vector2 dir) {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion currentAngle = spriteGroup.transform.rotation;
        Quaternion targetAngle = Quaternion.Euler(new Vector3(0, 0, angle));
        Quaternion actualAngle = Quaternion.RotateTowards(currentAngle, targetAngle, maxTurnSpeed);
        spriteGroup.transform.rotation = actualAngle;
    }
}