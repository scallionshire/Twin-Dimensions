using UnityEngine;

public class ComputerLabMonitorTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("The player has entered the detection zone.");

            GetComponent<ResetToCheckpoint>().Reset();

            showRedHint();
            showBlueHint();
        }
    }

    private void showRedHint()
    {
        GameObject red32 = GameObject.Find("red32");
        red32.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
    }

    private void showBlueHint()
    {
        GameObject blue32 = GameObject.Find("blue32");
        blue32.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
    }

}