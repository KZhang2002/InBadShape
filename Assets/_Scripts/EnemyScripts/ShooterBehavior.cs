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
    private Vector3 SelfPos => transform.position;
    private Vector3 PlayerPos => _player.transform.position;

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
    
    #region Debug Status Variables
    
    public bool shotAvailable;
    public float timeLeftOnShotClock;
    public bool isShotClockPaused;
    public bool playerInRange;
    public bool hasLOSofPlayer;
    public string currentAction;
    
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
        float playerDistance = Vector2.Distance(PlayerPos, SelfPos);
        bool tooClose = playerDistance < lowerMoveDistance;
        bool tooFar = playerDistance > upperMoveDistance;
        bool inRange = !tooFar && !tooClose;
        playerInRange = inRange;
        
        Vector2 dirToPlayer = PlayerPos - SelfPos;
        dirToPlayer = dirToPlayer.normalized;
        LayerMask mask = LayerMask.GetMask("Wall");
        RaycastHit2D hit = Physics2D.Raycast(SelfPos, dirToPlayer, playerDistance, mask);
        
        //bool hasLineOfSight = hit && hit.collider.gameObject.CompareTag("Player");
        bool hasLineOfSight = !hit.collider;
        hasLOSofPlayer = hasLineOfSight;
        
        #region Line Of Sight Debug Code
        Vector3 newSelfPos = SelfPos;
        newSelfPos.z = -1f;
        Vector3 endPoint = hit.collider ? hit.point : (Vector2)newSelfPos + dirToPlayer * playerDistance;
        Debug.DrawLine(newSelfPos, endPoint, Color.magenta);
        #endregion
        
        shotAvailable = hasLineOfSight && inRange; // status variable

        PointSpriteGroup(tooFar, hasLineOfSight, dirToPlayer);
        
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
        isShotClockPaused = _shotClock.isCancelled;
        
        if (_shotFlag) {
            Vector2 dirToMuzzle = MuzzlePoint - SelfPos;
            Shoot(dirToMuzzle);
        }
    }

    void PointSpriteGroup(bool tooFar, bool hasLineOfSight, Vector2 directionTowardsPlayer) {
        if (!tooFar && hasLineOfSight) {
            PointSprites(directionTowardsPlayer);
        }
        else {
            Vector2 velocityDir = _agent.velocity;
            if (velocityDir == Vector2.zero) return;
            PointSprites(velocityDir);
        }
    }

    void MoveTowardsPlayer() {
        _targetPos = _player.transform.position;
        MoveTowards(_targetPos);
        
        currentAction = "Moving to player";
    }
    
    void MoveAwayFromPlayer(Vector2 dirToPlayer) {
        Vector2 awayDir = dirToPlayer * -1;
        _targetPos = SelfPos + (Vector3)awayDir * (lowerMoveDistance);
        MoveTowards(_targetPos);

        currentAction = "Moving from player";
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
        currentAction = "Shooting!";

        _fm.FireProjectile(bullet, entityType.enemy, muzzleObj.transform.position, shotDirection);
        
        _shotFlag = false;
        StartShotClock();
    }

    Timer StartShotClock() {
        currentAction = "Aiming at player";
        
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