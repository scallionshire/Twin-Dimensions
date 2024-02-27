using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Extrudable : MonoBehaviour
{
    [HideInInspector]
    public Vector3 initScale;
    [HideInInspector]
    public Vector3 initPosition;

    private Vector3 endScale;
    private Vector3 endPosition;

    private Vector3 targetScale;
    private Vector3 targetPosition;

    private Mesh mesh;

    public Vector3 extrudeDirection;
    public float extrudeAmount;
    public bool shouldLoop = false;
    public bool isMoving = false;
    public bool isExtruding = false;
    public float extrudeSpeed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        float scaleFactor = 1.0f;

        if (GetComponent<MeshFilter>() != null) {
            // This is probably a 3D object
            mesh = GetComponent<MeshFilter>().mesh;

            // Only take the value we want
            Vector3 meshSize = Vector3.Scale(mesh.bounds.size, extrudeDirection);
            Debug.Log(gameObject.name + " meshSize: " + meshSize);

            scaleFactor = Mathf.Max(Mathf.Abs(meshSize.x), Mathf.Abs(meshSize.y), Mathf.Abs(meshSize.z));
        }
        
        initScale = transform.localScale;
        initPosition = transform.position;

        Debug.Log(gameObject.name + " scaleFactor: " + scaleFactor);

        // Gives level designers an extra option in case it's not extruding in the right direction
        if (isExtruding) {
            endScale = initScale + extrudeDirection * extrudeAmount;
        } else {
            endScale = initScale + extrudeDirection * extrudeAmount * (-1);
        }

        endPosition = initPosition + extrudeDirection * scaleFactor * extrudeAmount / 2;

        targetScale = endScale;
        targetPosition = endPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving) {
            if ((targetScale - transform.localScale).sqrMagnitude < 0.05f) {
                if (shouldLoop && targetScale == endScale) {
                    targetScale = initScale;
                    targetPosition = initPosition;
                } else if (shouldLoop && targetScale == initScale) {
                    targetScale = endScale;
                    targetPosition = endPosition;
                } else {
                    isMoving = false;
                }
            }

            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * extrudeSpeed);
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * extrudeSpeed);
        }
    }

    public void Extrude() {
        isMoving = true;
    }
}

