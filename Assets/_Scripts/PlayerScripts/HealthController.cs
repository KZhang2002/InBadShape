using System;
using TMPro;
using UnityEngine;

namespace _Scripts {
    public class HealthController : MonoBehaviour {
        public entityType type = entityType.unowned;
        [SerializeField] private float health = 10f;
        private TextMeshPro _hpText;

        private void Start() {
            _hpText = GetComponentInChildren<TextMeshPro>();
            UpdateHealthText();
        }

        private void UpdateHealthText() {
            string strCulture;
            if (health % 1 - 0 <= 0.01) {
                strCulture = "0";
            }
            else {
                strCulture = "0.0";
            }
            _hpText.text = health.ToString(strCulture);
        }

        public void DoDamage(float change) {
            health += -change;
            if (health <= 0) {
                Destroy(gameObject);
                return;
            }
            UpdateHealthText();
        }
        
        public void SetHealth(float change) {
            health = change;
            UpdateHealthText();
        }
    }
}