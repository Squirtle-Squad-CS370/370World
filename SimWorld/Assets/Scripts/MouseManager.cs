using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    [SerializeField]
    private Sprite crosshairCursorSprite;   // cursor for aiming weapons
    [SerializeField]
    private Sprite tileSelectCursorSprite;  // cursor for selecting tiles
    [SerializeField]
    private GameObject cursor_go;   // the active cursor game object
    private SpriteRenderer cursor_sr;   // active cursor's sprite renderer

    private bool isCrosshair;
    private bool isTileSelect;

    private Vector3 lastFramePosition;
    private Vector3 currFramePosition;

    void Start()
    {
        // Defaulting to crosshair for now
        cursor_sr = cursor_go.GetComponent<SpriteRenderer>();
        SetCrosshairCursor();
    }

    void Update()
    {
        // Store mouse's current position in game world
        currFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currFramePosition.z = 0;

        UpdateCursor();

        // lastFramePos is not used yet but will be later
        /*
        lastFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lastFramePosition.z = 0;
        */
    }

    private void UpdateCursor()
    {
        if( isCrosshair )
        {
            // Simply stay underneath the mouse
            cursor_go.SetActive(true);
            cursor_go.transform.position = new Vector3(currFramePosition.x, currFramePosition.y, 0);
        }

        if( isTileSelect )
        {
            // Update the tile select cursor's position
            Tile tileUnderMouse = WorldController.Instance.GetTileAtWorldCoord(currFramePosition);
            if (tileUnderMouse != null)
            {
                cursor_go.SetActive(true);
                Vector3 cursorPosition = new Vector3(tileUnderMouse.X, tileUnderMouse.Y, 0);
                cursor_go.transform.position = cursorPosition;
            }
            else
            {
                cursor_go.SetActive(false);
            }
        }
    }

    public void SetCrosshairCursor()
    {
        cursor_sr.sprite = crosshairCursorSprite;

        isCrosshair = true;
        isTileSelect = false;
    }
    public void SetTileSelectCursor()
    {
        cursor_sr.sprite = tileSelectCursorSprite;

        isTileSelect = true;
        isCrosshair = false;
    }
}
