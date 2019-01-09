using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameCameraScript : MonoBehaviour {
    private static Vector3 NULL_POSITION = new Vector3(-100500f, -100500f, -100500f);
    public int minCameraZoom = 20;
    public float k = 0.05f;
    private Camera myCamera;
    private Vector3 lastMousePosition = NULL_POSITION;
    private Vector3 defaultPosition;

    private int minCameraX;
    private int minCameraY;
    private int maxCameraX;
    private int maxCameraY;

    private bool pressedOverUI = false;

    private bool canceled = true;

    // Use this for initialization
    void Start () {
        myCamera = gameObject.GetComponent<Camera>();

        int halfZoom = 0;
        minCameraX = - GameData.NoteWidth / 2 - halfZoom;
        minCameraY = GameData.NoteHeight / 2 + halfZoom;
        maxCameraX = -minCameraX;
        maxCameraY = -minCameraY;

        defaultPosition = transform.position;

        myCamera.orthographicSize = Mathf.Max(GameData.NoteWidth, GameData.NoteHeight);
    }

    // Update is called once per frame
    void Update() {
        if (canceled)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            pressedOverUI = EventSystem.current.IsPointerOverGameObject();
        } else {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    pressedOverUI = true;
                    break;
                }
            }
        }

        if (!GameData.GameController.GetActivePlayer().WasDiceThrown())
        {
            return;
        }

        if (Input.touchCount > 0)
        {
            if (pressedOverUI)
            {
                return;
            }

            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
            {
                return;
            }

            if (lastMousePosition == NULL_POSITION)
            {
                lastMousePosition = touch.position;
                return;
            }
            myCamera.orthographicSize = minCameraZoom;

            Vector2 delta = touch.deltaPosition;

            Vector3 oldPosition = transform.position;
            Vector3 newPosition = new Vector3(transform.position.x + delta.x * k, transform.position.y + delta.y * k, transform.position.z);

            newPosition.x = Mathf.Min(Mathf.Max(minCameraX, newPosition.x), maxCameraX);
            newPosition.y = Mathf.Min(Mathf.Max(maxCameraY, newPosition.y), minCameraY);

            transform.position = newPosition;

            lastMousePosition = touch.position;
        } else if (Input.GetMouseButton(0))
        {
            if (pressedOverUI)
            {
                return;
            }

            if (lastMousePosition == NULL_POSITION)
            {
                lastMousePosition = Input.mousePosition;
                return;
            }
            myCamera.orthographicSize = minCameraZoom;

            Vector3 delta = Input.mousePosition - lastMousePosition;
            Vector3 oldPosition = transform.position;
            Vector3 newPosition = new Vector3(transform.position.x + delta.x * k, transform.position.y + delta.y * k, transform.position.z);

            newPosition.x = Mathf.Min(Mathf.Max(minCameraX, newPosition.x), maxCameraX);
            newPosition.y = Mathf.Min(Mathf.Max(maxCameraY, newPosition.y), minCameraY);

            transform.position = newPosition;

            lastMousePosition = Input.mousePosition;

        } else 
        {
            if (pressedOverUI)
            {
                return;
            }

            lastMousePosition = NULL_POSITION;

            transform.position = defaultPosition;
            myCamera.orthographicSize = Mathf.Max(GameData.NoteWidth, GameData.NoteHeight);
        }
	}

    public static bool IsOverUI()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);

        pointerData.position = Input.GetTouch(0).position;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        if (results.Count > 0)
        {
            return true;
        }

        return false;
    }
}
