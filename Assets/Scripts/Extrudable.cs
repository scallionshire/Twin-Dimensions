using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Extrudable : MonoBehaviour
{
    [HideInInspector]
    public Vector3 initScale;
    [HideInInspector]
    public Vector3 initPosition;
    public Vector3 extrudeDirection;
    public float extrudeAmount;
    public bool isExtruding = false;
    public float extrudeSpeed = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        initScale = transform.localScale;
        initPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isExtruding) {
            Vector3 targetScale = initScale + extrudeDirection * extrudeAmount;
            Vector3 targetPosition = initPosition + extrudeDirection * extrudeAmount / 2;
            
            if ((targetScale - transform.localScale).sqrMagnitude < 0.01f) {
                isExtruding = false;
            }
            
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * extrudeSpeed);
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * extrudeSpeed);
        }
    }
}
