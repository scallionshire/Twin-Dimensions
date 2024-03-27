using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCollision : MonoBehaviour
{
    private TooltipManager tooltipManager;

    void Start() {
        if (GameObject.Find("TooltipCanvas") != null)
        {
            tooltipManager = GameObject.Find("TooltipCanvas").GetComponent<TooltipManager>();
        }
    }
    
    // Start is called before the first frame update
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Block")
        {
            if (tooltipManager != null)
            {
                tooltipManager.ToggleSpaceTooltip(true);
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Block")
        {
            if (tooltipManager != null)
            {
                tooltipManager.ToggleSpaceTooltip(false);
            }
        }
    }
}
