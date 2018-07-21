using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeDrag : MonoBehaviour {
    public float Value = 1.0f;
    private bool _initialized;
    private Vector3 startPos;
    public Vector3 TargetPos;

    // Use this for initialization
    void Start() {
        Init();
    }

    void Awake() {
        Init();
    }

    private void Init() {
        if(_initialized) {
            return;
        }

        startPos = transform.position;
    }

    // Update is called once per frame
    void Update() {

        // The step size is equal to speed times frame time.
        float step = 1 * Time.deltaTime;

        // Move our position a step closer to the target.
        transform.position = Vector3.MoveTowards(transform.position, TargetPos, step);

        if (Vector3.Distance(transform.position, TargetPos) < 0.1f) {
            transform.position = startPos;
        }

    }
}
