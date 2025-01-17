using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts;
using UnityEngine;
using UnityEngine.Serialization;
using VInspector;

enum FiringType {
    projectile,
    hitscan
}

public class FiringController : MonoBehaviour {
    [SerializeReference] private Bullet bulletPrefab;
    [SerializeReference] private Tracer tracerPrefab;
    [SerializeReference] private GameObject muzzleObj;
    
    [Foldout("Fire 1 Stats")]
    [SerializeReference] private float fire1Dmg = 1f;
    [SerializeReference] private float roundsPerMinute;
    
    [Foldout("Fire 2 Stats")]
    [SerializeReference] private float fire2Dmg = 5f;
    [SerializeReference] private float fire2Range = 100f;
    [SerializeReference] private int fire2MaxPenTargets = 1;
    [SerializeReference] private List<entityType> fire2HitList = new List<entityType>() {
        entityType.enemy
    };
    
    private Vector3 _muzzlePoint;
    private Vector3 _origin;
    private TracerManager _tm;
    private FiringManager _fm;

    private void Awake() {
        bulletPrefab.Init(entityType.player, fire1Dmg, bulletPrefab.speed, bulletPrefab.lifeTime);
        _fm = FiringManager.Get;
    }

    private void Start() {
        _tm = TracerManager.Get;
        
        if (!_tm) {
            Debug.LogError("Error finding tracer manager!");
        }
    }

    // todo, remove constant calculation of dir
    void Update() {
        _muzzlePoint = muzzleObj.transform.position;
        _origin = transform.position;
        Vector2 dir = _muzzlePoint - _origin;
        dir = dir.normalized;
        Debug.DrawRay(_muzzlePoint, dir * 1, Color.magenta);

        if (Input.GetButtonDown("Fire1")) {
            Fire1(dir);
        }
        
        if (Input.GetButtonDown("Fire2")) {
            Fire2(dir);
        }
    }

    void Fire1(Vector2 dir) {
        _fm.FireProjectile(bulletPrefab, entityType.player, _muzzlePoint, dir, transform.rotation);
    }
    
    void Fire2(Vector2 dir) {
        RaycastHit2D hit = _fm.FireHitscan(_muzzlePoint, dir, fire2Dmg, fire2MaxPenTargets, fire2Range, fire2HitList);
        //Debug.Log(hit);
        
        // draw tracers
        if (hit.collider) {
            _tm.Trace(tracerPrefab, _muzzlePoint, hit.point);
        }
        else {
            _tm.Trace(tracerPrefab, _muzzlePoint, dir, 100);
        }
    }
}