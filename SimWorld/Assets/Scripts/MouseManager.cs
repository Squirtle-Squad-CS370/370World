using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseManager : MonoBehaviour
{
    #region Singleton

    public static MouseManager Instance { get; protected set; }

    void Awake()
    {
        // Make sure this is the only instance of AudioController
        if (Instance != null)
        {
            Debug.LogError("MouseManager - Another MM already exists.");
            return;
        }

        Instance = this;
    }

    #endregion

    [SerializeField]
    private Texture2D crosshairCursorSprite;   // cursor for aiming weapons
    [SerializeField]
    private Sprite tileSelectCursorSprite;  // cursor for selecting tiles
    private GameObject cursor_go;   // the active cursor game object
    private SpriteRenderer cursor_sr;   // active cursor's sprite renderer

    private bool isCrosshair;
    private bool isTileSelect;

    private Vector3 lastFramePosition;
    private Vector3 currFramePosition;

    void Start()
    {
        // Defaulting to crosshair for now
        // Cursor.SetCursor(crosshairCursorSprite, Vector2.zero, CursorMode.ForceSoftware);
        cursor_go = gameObject;
        cursor_sr = GetComponent<SpriteRenderer>();
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
        //Cursor.visible = true;
        cursor_sr.enabled = false;

        isCrosshair = true;
        isTileSelect = false;
    }
    public void SetTileSelectCursor()
    {
        //Cursor.visible = false;
        cursor_sr.enabled = true;
        cursor_sr.sprite = tileSelectCursorSprite;

        isTileSelect = true;
        isCrosshair = false;
    }
}
