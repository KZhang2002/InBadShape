using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using VInspector;

public class Tracer : MonoBehaviour {
    public float lifeTime = 0f; // -1f to last forever, 0 to auto calculate lifeTime
    
    [Foldout("Fade Settings")]
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float fadeDelaySeconds = 0.3f;
    [SerializeField] private float fadeOpacityCutoff = 0.7f;
    
    [Foldout("Expansion Settings")]
    [SerializeField] private float expansionDelaySeconds = 0.4f;
    [SerializeField] private float expansionDuration = 1f;
    [SerializeField] private float expansionStartValue = 1f;
    [SerializeField] private float expansionEndValue = 3f;

    private void Start() {
        if (Mathf.Abs(lifeTime - 0f) < 0.001f) {
            float fadeLifetime = fadeDuration * fadeOpacityCutoff + fadeDelaySeconds;
            float expandLifetime = expansionDuration + expansionDelaySeconds;
            if (fadeLifetime < expandLifetime) {
                lifeTime = fadeDuration * fadeOpacityCutoff + fadeDelaySeconds;
            }
            else {
                lifeTime = expansionDuration + expansionDelaySeconds;
            }
        }
        
        StartCoroutine(Fade());
        StartCoroutine(Expand());
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

    private IEnumerator Fade() {
        SpriteRenderer spriteRend = gameObject.GetComponentInChildren<SpriteRenderer>();
        Color initialColor = spriteRend.color;

        float elapsedTime = 0f;

        yield return new WaitForSeconds(fadeDelaySeconds);

        while (elapsedTime < fadeDuration) {
            elapsedTime += Time.deltaTime;

            float smStepValue = elapsedTime / fadeDuration;
            spriteRend.color = new Color(initialColor.r, initialColor.g, initialColor.b,
                Mathf.SmoothStep(1f, 0f, smStepValue));

            if (smStepValue >= fadeOpacityCutoff) yield break;
            yield return null;
        }
    }
    
    private IEnumerator Expand() {
        Vector3 initialScale = transform.localScale;
        float elapsedTime = 0f;
        float expStartVal = initialScale.y;
        if (expansionStartValue >= 0) {
            expStartVal = expansionStartValue;
            transform.localScale = new Vector3(initialScale.x, expansionStartValue, initialScale.z);
        }

        yield return new WaitForSeconds(expansionDelaySeconds);

        while (elapsedTime < expansionDuration) {
            elapsedTime += Time.deltaTime;

            float smStepValue = elapsedTime / expansionDuration;
            transform.localScale = new Vector3(initialScale.x, Mathf.Lerp(expStartVal, expansionEndValue, smStepValue), initialScale.z);
            
            yield return null;
        }
    }
}