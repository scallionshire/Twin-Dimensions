using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Extrudable : MonoBehaviour
{
    public int extrudableId;
    private bool gameStateUpdated = false;
    
    [HideInInspector]
    public Vector3 initScale;
    [HideInInspector]
    public Vector3 initPosition;

    private Vector3 endScale;
    private Vector3 endPosition;

    private Vector3 targetScale;
    private Vector3 targetPosition;

    private Mesh mesh;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider2D;
    private bool is2D = false;
    private GameManager gameManager;

    public Vector3 extrudeDirection;
    public float extrudeAmount;
    public bool shouldLoop = false;
    public bool isMoving = false;
    public bool isExtruding = false;
    public float extrudeSpeed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager")?.GetComponent<GameManager>();

        float scaleFactor = 1.0f;

        if (GetComponent<MeshFilter>() != null) {
            // This is probably a 3D object
            mesh = GetComponent<MeshFilter>().mesh;

            // Only take the value we want
            Vector3 meshSize = Vector3.Scale(mesh.bounds.size, extrudeDirection);

            scaleFactor = Mathf.Max(Mathf.Abs(meshSize.x), Mathf.Abs(meshSize.y), Mathf.Abs(meshSize.z));
        } else {
            // This is probably a 2D object
            is2D = true;
            spriteRenderer = GetComponent<SpriteRenderer>();
            boxCollider2D = GetComponent<BoxCollider2D>();
        }
        
        if (is2D) {
            // This is a 2D object
            initScale = new Vector3(spriteRenderer.size.x, spriteRenderer.size.y, 1.0f);
        } else {
            initScale = transform.localScale;
        }

        initPosition = transform.position;

        Vector3 scaleDirection = transform.rotation * extrudeDirection;
        Debug.Log("Extrudable " + extrudableId + "Scale direction: " + scaleDirection);
        if (isExtruding) {
            endScale = initScale + scaleDirection * extrudeAmount;
        } else {
            // Gives level designers an extra option in case it's not extruding in the right direction
            endScale = initScale + scaleDirection * extrudeAmount * (-1);
        }

        endPosition = initPosition + extrudeDirection * scaleFactor * extrudeAmount * 0.5f;

        targetScale = endScale;
        targetPosition = endPosition;

        if (gameManager != null) {
            if (extrudableId < gameManager.gameState.Extrudables.Count && gameManager.gameState.Extrudables[extrudableId]) {
                MakeAlreadyExtruded();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving) {
            if (!gameStateUpdated) {
                gameStateUpdated = true;
                gameManager?.UpdateExtrudables(extrudableId);
            }

            if (is2D) {
                if ((targetScale - new Vector3(spriteRenderer.size.x, spriteRenderer.size.y, 1.0f)).sqrMagnitude < 0.05f) {
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
                spriteRenderer.size = Vector2.Lerp(spriteRenderer.size, new Vector2(targetScale.x, targetScale.y), Time.deltaTime * extrudeSpeed);
                boxCollider2D.size = Vector2.Lerp(boxCollider2D.size, new Vector2(targetScale.x, targetScale.y), Time.deltaTime * extrudeSpeed);
            } else {
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
    }

    public void Extrude() {
        isMoving = true;
    }

    public void MakeAlreadyExtruded() {
        if (is2D) {
            spriteRenderer.size = new Vector2(endScale.x, endScale.y);
            boxCollider2D.size = new Vector2(endScale.x, endScale.y);
            transform.position = endPosition;
        } else {
            transform.localScale = endScale;
            transform.position = endPosition;
        }
    }
}

