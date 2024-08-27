using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts {
    public static class HitHelper {
        /// <summary>
        /// Checks if hit object has a health controller and sends damage. Returns false
        /// if the target shares the same entityType as the shooter.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="damage"></param>
        /// <param name="shooterType"></param>
        /// <returns></returns>
        public static bool DoHit(GameObject target, float damage, entityType shooterType = entityType.unowned) {
            string colObjName = target.name;
            
            HealthController hc = target.GetComponent<HealthController>();
            if (!hc) hc = target.GetComponentInChildren<HealthController>();
            if (!hc) hc = target.GetComponentInParent<HealthController>();
            if (!hc) {
                //Debug.Log($"{colObjName} has no health controller! No damage done!");
                return true;
            }
            
            if (hc && shooterType == hc.type) {
                return false;
            }
            
            if (hc) {
                hc.DoDamage(damage);
                Debug.Log($"{colObjName} hit for {damage}!");
            }
            
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