using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    [SerializeField]
    private GameObject tileSelectCursorPrefab;
    private GameObject tileSelectCursor;

    private Vector3 lastFramePosition;
    private Vector3 currFramePosition;

    void Start()
    {
        tileSelectCursor = GameObject.Instantiate(tileSelectCursorPrefab);
    }

    void Update()
    {
        // Store mouse's current position in game world
        currFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currFramePosition.z = 0;

        UpdateCursor();

        // lastFramePos is not used yet but will be later
        lastFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lastFramePosition.z = 0;
    }

    private void UpdateCursor()
    {
        // Update the tile select cursor's position
        Tile tileUnderMouse = GetTileAtWorldCoord(currFramePosition);
        if (tileUnderMouse != null)
        {
            tileSelectCursor.SetActive(true);
            Vector3 cursorPosition = new Vector3(tileUnderMouse.X, tileUnderMouse.Y, 0);
            tileSelectCursor.transform.position = cursorPosition;
        }
        else
        {
            tileSelectCursor.SetActive(false);
        }
    }

    // Utility functions
    Tile GetTileAtWorldCoord(Vector3 coord)
    {
        int x = Mathf.RoundToInt(coord.x);
        int y = Mathf.RoundToInt(coord.y);

        return WorldController.Instance.World.GetTileAt(x, y);
    }
}
