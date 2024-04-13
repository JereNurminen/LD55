using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{

    private bool isHeld = false;

    private Vector2 currentWorldPosition; 
    private Vector2 downWorldPosition;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void SummonCrow() {
        GetComponentInParent<PlayerController>().SummonCrow(downWorldPosition, currentWorldPosition);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            isHeld = true;
            downWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        } else if (Input.GetMouseButtonUp(0)) {
            isHeld = false;
            SummonCrow();
        }

        if (isHeld) {
            currentWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.DrawLine(downWorldPosition, currentWorldPosition, Color.red);
        }
    }
}
