using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts {
    public static class HitHelper {
        public static bool DoHit(GameObject target, float damage, entityType shooterType = entityType.unowned) {
            string colObjName = target.name;
            
            HealthController hc = target.GetComponent<HealthController>();
            if (!hc) hc = target.GetComponentInChildren<HealthController>();
            if (!hc) hc = target.GetComponentInParent<HealthController>();
            if (!hc || shooterType == hc.type || hc.type == entityType.unowned) {
                //Debug.Log($"{colObjName} has no health controller! No damage done!");
                return false;
            }
            
            hc.DoDamage(damage);
            Debug.Log($"{colObjName} hit for {damage}!");
            return true;
        }
        
        public static bool DoHit(GameObject target, float damage, List<entityType> hitList, entityType shooterType = entityType.unowned) {
            var targetHC = target.GetComponent<HealthController>();
            if (targetHC && hitList.Contains(targetHC.type)) {
                return DoHit(target, damage, shooterType);
            }
            return false;
        }
    }
}