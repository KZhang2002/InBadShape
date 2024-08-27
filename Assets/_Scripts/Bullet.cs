using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts;
using UnityEngine;

public enum entityType {
    player,
    enemy,
    unowned
}

public class Bullet : MonoBehaviour {
    public float damage = 1f;
    public float speed = 10f;
    public float lifeTime = 5f;
    public bool canPen = false;
    private entityType owner = entityType.unowned; // todo change to enums

    // private float speed = 5;
    // private Vector3 direction = Vector3.forward;

    // Start is called before the first frame update
    void Start() {
        
    }

    public void InitOwner(entityType shooter) {
        owner = shooter;
    }
    
    public void Init(entityType shooter, float damage, float speed, float lifeTime) {
        InitOwner(shooter);
        this.damage = damage;
        this.speed = speed;
        this.lifeTime = lifeTime;
    }

    void OnTriggerEnter2D(Collider2D other) {
        bool isValid = HitHelper.DoHit(other.gameObject, damage, owner);
        if (!canPen && isValid) Destroy(gameObject);
    }
    
    private void Update() {
        UpdateTimer();
    }

    private void UpdateTimer() {
        if (lifeTime > 0) {
            lifeTime -= Time.deltaTime;
        }
        else {
            Destroy(gameObject);
        }
    }
}