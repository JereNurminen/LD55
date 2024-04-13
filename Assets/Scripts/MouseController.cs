using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{

    public Texture2D cursorTexture;
    public Vector2 cursorHotspot;
    public CursorMode cursorMode = CursorMode.Auto;

    public GameObject summoningCirclePrefab;
    public GameObject summoningTargetPrefab;

    private bool isHeld = false;

    private Vector2 targetWorldPosition; 
    private Vector2 spawnWorldPosition;
    private GameObject summoningCircle;
    private GameObject cursor;
    private Animator cursorAnimator;

    // Start is called before the first frame update
    void Start()
    {
        cursor = Instantiate(summoningTargetPrefab, Vector2.zero, Quaternion.identity);
        cursorAnimator = cursor.GetComponent<Animator>();
        Cursor.visible = false;
    }

    void OnMouseEnter() {
        Cursor.SetCursor(cursorTexture, cursorHotspot, cursorMode);
    }

    void OnMouseExit() {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }

    void SummonCrow() {
        GetComponentInParent<PlayerController>().SummonCrow(spawnWorldPosition, targetWorldPosition);
    }

    // Update is called once per frame
    void Update()
    {
        targetWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursor.transform.position = targetWorldPosition;

        if (Input.GetMouseButtonDown(0)) {
            isHeld = true;
            spawnWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            summoningCircle = Instantiate(summoningCirclePrefab, spawnWorldPosition, Quaternion.identity);
            cursorAnimator.SetTrigger("activate");
        } else if (Input.GetMouseButtonUp(0)) {
            isHeld = false;
            summoningCircle.GetComponent<SummoningController>().Despawn();
            SummonCrow();
            cursorAnimator.SetTrigger("deactivate");
        }

        if (isHeld) {
            Debug.DrawLine(spawnWorldPosition, targetWorldPosition, Color.red);
        }
    }
}
