using UnityEngine;

public class ResetToCheckpoint : MonoBehaviour
{
    public Vector3 checkpointPosition;
    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    public void Reset()
    {
        player.transform.position = checkpointPosition;
    }
}