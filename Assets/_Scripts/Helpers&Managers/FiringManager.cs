using System.Collections;
using System.Collections.Generic;
using _Scripts;
using UnityEngine;
using UnityEngine.Serialization;

public class FiringManager : MonoBehaviour {
    [SerializeReference] private float zOffset = 3f;
    
    private static FiringManager _instance;
    
    public static FiringManager Get {
        get {
            if (_instance == null)
                _instance = FindObjectOfType<FiringManager>();

            if (_instance == null) {
                GameObject gObj = new GameObject();
                gObj.name = "FiringManager";
                _instance = gObj.AddComponent<FiringManager>();
                //DontDestroyOnLoad(gObj); // allows tracerManager to stay persistent across scenes
            }

            return _instance;
        }
    }

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this);
        }
        else {
            _instance = this;
        }
    }
    
    public Bullet FireProjectile(Bullet bullet, entityType type, Vector2 pos, Vector2 dir, Quaternion rot) {
        Bullet projectile = Instantiate(bullet, pos, rot);
        projectile.Init(type, bullet.damage, bullet.speed, bullet.lifeTime);
        Rigidbody2D projRb = projectile.GetComponent<Rigidbody2D>();
        projRb.velocity = dir * bullet.speed;
        return projectile;
    }
    
    public Bullet FireProjectile(Bullet bullet, entityType type, Vector2 pos, Vector2 dir) {
        return FireProjectile(bullet, type, pos, dir, Quaternion.identity);
    }
    
    public RaycastHit2D FireHitscan(Vector3 pos, Vector2 dir, float dmg, int maxPenTargets, float range, List<entityType> fire2HitList) {
        RaycastHit2D[] hits = Physics2D.RaycastAll(pos, dir);
        Debug.DrawRay(pos, dir * range, Color.red, 0.1f);
        float numTargets = maxPenTargets;
        foreach (RaycastHit2D hit in hits) {
            bool hitIsValid = HitHelper.DoHit(hit.collider.gameObject, dmg, fire2HitList, entityType.player);
            Debug.DrawRay(hit.point + Vector2.left * 0.5f, Vector2.right, Color.cyan, 0.5f);
            Debug.DrawRay(hit.point + Vector2.down * 0.5f, Vector2.up, Color.cyan, 0.5f);
            numTargets--;
            if (numTargets <= 0 || !hitIsValid) {
                return hit;
            }
        }

        return new RaycastHit2D();
    }
}