using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TracerManager : MonoBehaviour {
    [SerializeReference] private float zOffset = 3f;
    
    private static TracerManager _instance;
    
    public static TracerManager Get {
        get {
            if (_instance == null)
                _instance = FindObjectOfType<TracerManager>();

            if (_instance == null) {
                GameObject gObj = new GameObject();
                gObj.name = "TracerManager";
                _instance = gObj.AddComponent<TracerManager>();
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
    
    public void Trace(Tracer tracer, Vector3 origin, Vector2 direction, float length) {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
        Trace(tracer, origin, rotation, length);
    }

    public void Trace(Tracer tracer, Vector3 origin, Quaternion rotation, float length) {
        Tracer tr = Instantiate(tracer, origin + Vector3.forward * zOffset, rotation);
        Transform t = tr.transform;

        Vector3 scaleVector = t.localScale;
        scaleVector.x = length + 0.2f;
        t.localScale = scaleVector;
    }

    public void Trace(Tracer tracer, Vector3 origin, Vector3 end) {
        origin.z = 0;
        end.z = 0;
        float length = Vector3.Distance(origin, end);
        Vector3 direction = end - origin;
        Trace(tracer, origin, direction, length);
    }
}