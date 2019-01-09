using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GameSceneGridScript : MonoBehaviour {
    private static Vector3 NULL_POSITION = new Vector3(-100500f, -100500f, -100500f);
    private Vector3 lastMousePosition = NULL_POSITION;
    private Vector3 defaultPosition =  new Vector3();
    private bool pressedOverUI = false;
    private float k = 0.05f;

    public TileBase whiteTile;
    public TileBase gridTile;
    public TileBase yellowTile;
    public TileBase greenTile;
    public TileBase blueTile;
    public TileBase redTile;

    private TileBase[] tileBorders;

    public Button rollDicesButton;
    public Button applyMoveButton;

    public GameObject sceneFadingChanger;

    private LevelFadingChangerScript levelFadingChangerScript;

    private Tilemap gridTilemap;
    private Tilemap playerMovesTilemap;
    private Tilemap tempTilemap;

    private int[] tempTilemapDiceValues = null;
    private Vector3Int tempTilemapDrawingPosition;
    private GridRectangle tempRectangle;
    private Vector3 tempTileMapFingerPosition;

    private bool tempTilemapCanPlace = false;

    private GameController gameController;

    private void Awake()
    {
        gridTilemap = RetrieveTilemap("GridTilemap");
        playerMovesTilemap = RetrieveTilemap("PlayerMovesTilemap");
        tempTilemap = RetrieveTilemap("TempTilemap");

        gameController = GameData.GameController;

        int inset = 1;
        int xOffset = gameController.Field.X - inset;
        int yOffset = gameController.Field.Y + inset;

        for (int x = 0; x < GameData.NoteWidth + inset * 2; x++)
        {
            for (int y = 0; y < GameData.NoteHeight + inset * 2; y++)
            {
                if (x < inset || y < inset || x >= GameData.NoteWidth + inset || y >= GameData.NoteHeight + inset)
                {
                    gridTilemap.SetTile(new Vector3Int(xOffset + x, yOffset - y, 0), whiteTile);
                }
                else
                {
                    gridTilemap.SetTile(new Vector3Int(xOffset + x, yOffset - y, 0), gridTile);
                }
            }
        }

        levelFadingChangerScript = sceneFadingChanger.GetComponent<LevelFadingChangerScript>();

        tileBorders = Resources.LoadAll<TileBase>("Tiles");
    }

    private Tilemap RetrieveTilemap(string name)
    {
        Tilemap[] tilemaps = gameObject.GetComponentsInChildren<Tilemap>();
        for (int i = 0; i < tilemaps.Length; i++)
        {
            Tilemap tilemap = tilemaps[i];

            if (name.Equals(tilemap.name))
            {
                return tilemap;
            }
        }

        return null;
    }

    void Update()
    {

        applyMoveButton.gameObject.SetActive(tempTilemapCanPlace);

        if (NoDicePending())
        {
            rollDicesButton.gameObject.SetActive(true);
            return;
        }

        rollDicesButton.gameObject.SetActive(false);

        if (NewDiceRolled())
        {
            tempTileMapFingerPosition = new Vector3(0f, 0f, 0f);
            pressedOverUI = false;

            DrawTempTiles();
            return;
        }

        if (!pressedOverUI)
        {
            pressedOverUI = IsPressedOverUI(pressedOverUI);
        }

        if (Input.GetMouseButton(0))
        {
            if (pressedOverUI)
            {
                return;
            }
            ProcessMouseMoved();
        }
        else if (Input.touchCount > 0)
        {
            if (pressedOverUI)
            {
                return;
            }
            ProcessFingerTouch();
        }
        else
        {
            pressedOverUI = false;

            lastMousePosition = NULL_POSITION;
        }
    }

    private bool NewDiceRolled()
    {
        bool wasDiceThrown = GameData.GameController.GetActivePlayer().WasDiceThrown();

        return wasDiceThrown && tempTilemapDiceValues == null;
    }

    private bool IsPressedOverUI(bool currentValue)
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began && IsOverUI(touch))
            {
                return true;
            }
        }

        if (Input.GetMouseButtonDown(0) && Input.touchCount == 0)
        {
            return EventSystem.current.IsPointerOverGameObject();
        }

        return currentValue;
    }

    public static bool IsOverUI(Touch touch)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);

        pointerData.position = touch.position;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        if (results.Count > 0)
        {
            return true;
        }

        return false;
    }

    private void ProcessMouseMoved()
    {
        if (lastMousePosition == NULL_POSITION)
        {
            lastMousePosition = Input.mousePosition;
            return;
        }

        Vector3 delta = Input.mousePosition - lastMousePosition;
        Vector3 oldPosition = tempTileMapFingerPosition;
        Vector3 newPosition = new Vector3(oldPosition.x + delta.x * k, oldPosition.y + delta.y * k, oldPosition.z);

        newPosition.x = Mathf.Min(Mathf.Max(gameController.Field.X, newPosition.x), gameController.Field.X2);
        newPosition.y = Mathf.Min(Mathf.Max(gameController.Field.Y2, newPosition.y), gameController.Field.Y);

        //Debug.Log(gameController.Field.X + " " + gameController.Field.Y + " " + gameController.Field.X2 + " " + gameController.Field.Y2);

        tempTileMapFingerPosition = newPosition;

        lastMousePosition = Input.mousePosition;

        DrawTempTiles();
    }

    private void ProcessFingerTouch()
    {
        if (Input.touchCount != 1)
        {
            return;
        }

        Touch touch = Input.GetTouch(0);

        Vector2 delta = touch.deltaPosition;

        Vector3 oldPosition = tempTileMapFingerPosition;
        Vector3 newPosition = new Vector3(oldPosition.x + delta.x * k, oldPosition.y + delta.y * k, oldPosition.z);

        newPosition.x = Mathf.Min(Mathf.Max(gameController.Field.X, newPosition.x), gameController.Field.X2);
        newPosition.y = Mathf.Min(Mathf.Max(gameController.Field.Y2, newPosition.y), gameController.Field.Y);

        tempTileMapFingerPosition = newPosition;

        DrawTempTiles();
    }

    private void ClearTempTiles(int[] oldValues, Vector3Int oldPosition)
    {
        if (oldValues != null)
        {
            int[] centerCoords = new int[] { oldValues[0] / 2, oldValues[1] / 2 };

            for (int x = 0; x < oldValues[0]; x++)
            {
                for (int y = 0; y < oldValues[1]; y++)
                {
                    Vector3Int vector = new Vector3Int(x, -y, 0);
                    vector.x -= centerCoords[0];
                    vector.y += centerCoords[1];
                    vector.x += oldPosition.x;
                    vector.y += oldPosition.y;

                    tempTilemap.SetTile(vector, null);
                }
            }
        }
    }

    private void DrawTempTiles()
    {
        int[] generated = GameData.GameController.GetActivePlayer().DiceValue;

        Vector3Int currentPosition = GetCurrentPosition();

        //Debug.Log("CurrentPosition" + currentPosition);

        //if (IsCurrentPositionOutsideField(currentPosition))
        //{
        //    return;
        //}

        currentPosition = GetAlignedPosition(currentPosition, tempTilemapDiceValues);

        //Debug.Log("AlignedPosition" + currentPosition);

        if (tempTilemapDiceValues != null && generated != null && tempTilemapDiceValues.Equals(generated))
        {
            if (currentPosition != null && currentPosition.Equals(tempTilemapDrawingPosition))
            {
                return;
            }
        }

        int[] oldValues = tempTilemapDiceValues;
        Vector3Int oldPosition = tempTilemapDrawingPosition == null ? new Vector3Int(0, 0, 0) : tempTilemapDrawingPosition;
        tempTilemapDiceValues = generated;

        tempTilemapDrawingPosition = currentPosition;

        int[] centerCoords = new int[] { tempTilemapDiceValues[0] / 2, tempTilemapDiceValues[1] / 2 };

        tempRectangle = new GridRectangle(tempTilemapDrawingPosition.x - centerCoords[0], tempTilemapDrawingPosition.y + centerCoords[1], generated[0], generated[1]);

        ClearTempTiles(oldValues, oldPosition);

        if (tempTilemapDiceValues == null)
        {
            tempTilemapCanPlace = false;
            return;
        }

        tempTilemapCanPlace = CanPlaceRect(tempRectangle);
        Color color = tempTilemapCanPlace ? GameData.GREEN_COLOR : GameData.YELLOW_COLOR;

        for (int x = 0; x < tempTilemapDiceValues[0]; x++)
        {
            for (int y = 0; y < tempTilemapDiceValues[1]; y++)
            {
                Vector3Int vector = new Vector3Int(x, -y, 0);
                vector.x -= centerCoords[0];
                vector.y += centerCoords[1];
                vector.x += tempTilemapDrawingPosition.x;
                vector.y += tempTilemapDrawingPosition.y;

                int tileMapIndex = tempRectangle.GetTileIndexFor(vector.x, vector.y);

                tempTilemap.SetTile(vector, tileBorders[tileMapIndex]);
                tempTilemap.SetTileFlags(vector, TileFlags.None);
                tempTilemap.SetColor(vector, color);
                //tempTilemap.SetTile(vector, tile);
            }
        }
    }

    private bool IsCurrentPositionOutsideField(Vector3Int currentPosition)
    {
        return !gameController.Field.Contains(new GridRectangle.GridPoint(currentPosition.x, currentPosition.y));
    }

    private Vector3Int GetCurrentPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(tempTileMapFingerPosition));
        Vector3 worldPoint = ray.GetPoint(-ray.origin.z / ray.direction.z);
        Vector3Int position = tempTilemap.WorldToCell(worldPoint);

        return position;
    }

    private Vector3Int GetAlignedPosition(Vector3Int position, int[] values)
    {
        if (values == null)
        {
            return position;
        }

        int minX;
        int minY;
        int maxX;
        int maxY;
        minX = gameController.Field.X + values[0] / 2;
        minY = gameController.Field.Y - values[1] / 2;
        maxX = gameController.Field.X2 - (values[0] - values[0] / 2);
        maxY = gameController.Field.Y2 - (-values[1] + values[1] / 2);

        if (position.x < minX)
        {
            position.x = minX;
        }
        else if (position.x > maxX)
        {
            position.x = maxX;
        }

        if (position.y > minY)
        {
            position.y = minY;
        }
        else if (position.y < maxY)
        {
            position.y = maxY;
        }

        return position;
    }

    private bool NoDicePending()
    {
        return !gameController.GetActivePlayer().WasDiceThrown();
    }

    private bool CanPlaceRect(GridRectangle rect)
    {
        if (rect == null)
        {
            return false;
        }

        if (IsFirstTurn())
        {
            if (gameController.ActivePlayerIndex == 0)
            {
                return CanPlaceFirstRect0(rect);
            } else
            {
                return CanPlaceFirstRect1(rect);
            }
        }

        if (Intersects(rect))
        {
            return false;
        }

        if (Aligns(rect))
        {
            return true;
        }

        return false; // temp
    }

    private bool IsFirstTurn()
    {
        return gameController.GetActivePlayer().IsFirstTurn();
    }

    private bool CanPlaceFirstRect0(GridRectangle rect)
    {
        int maxX = gameController.Field.X2;
        int maxY = gameController.Field.Y2;

        return (rect.X2 == maxX) && (rect.Y2 == maxY);
    }

    private bool CanPlaceFirstRect1(GridRectangle rect)
    {
        int minX = gameController.Field.X;
        int minY = gameController.Field.Y;

        return (rect.X == minX) && (rect.Y == minY);
    }

    public void PlaceRect()
    {
        GridRectangle rect = tempRectangle;

        PlaceRect(rect);
    }

    public void PlaceRect(GridRectangle rect)
    {
        if (!CanPlaceRect(rect))
        {
            return;
        }

        Player activePlayer = gameController.GetActivePlayer();
        activePlayer.AddPlayerMove(rect);

        activePlayer.ResetSkippedTurns();

        activePlayer.Score += rect.GetSquare();

        if (gameController.Bot != null)
        {
            gameController.Bot.RectPlaced(rect, activePlayer.PlayerIndex);
        }

        DrawPlacedRect(rect);

        SwitchPlayer();
    }

    private void DrawPlacedRect(GridRectangle rect)
    {
        TileBase tile = gameController.ActivePlayerIndex == 0 ? blueTile : redTile;
        for (int x = rect.X; x < rect.X2; x++)
        {
            for (int y = rect.Y; y > rect.Y2; y--)
            {
                int tileMapIndex = rect.GetTileIndexFor(x, y);

                Vector3Int vector = new Vector3Int(x, y, 0);
                playerMovesTilemap.SetTile(vector, tileBorders[tileMapIndex]);
                playerMovesTilemap.SetTileFlags(vector, TileFlags.None);
                playerMovesTilemap.SetColor(vector, gameController.GetActivePlayer().Color);
                //playerMovesTilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
    }

    public void SwitchPlayer()
    {
        ClearTempTiles(tempTilemapDiceValues, tempTilemapDrawingPosition == null ? new Vector3Int(0, 0, 0) : tempTilemapDrawingPosition);

        gameController.SwitchToNextPlayer();
        //GameData.NextPlayer();

        tempTilemapCanPlace = false;
        tempRectangle = null;
        tempTilemapDrawingPosition = new Vector3Int();
        tempTilemapDiceValues = null;

        if (gameController.GameFinished())
        {
            FinishGame();
            return;
        }

        if (gameController.GetActivePlayer().GaveUp)
        {
            SwitchPlayer();
        } else
        {
            if (gameController.GetActivePlayer().IsBot)
            {
                gameController.ThrowDice();
                StartCoroutine("BotAnalyze");
            }
        }
    }

    IEnumerator BotAnalyze()
    {
        yield return null;
        if (gameController.Bot != null)
        {
            gameController.Bot.Analyze(this);
        }

        yield return null;
    }

    private void FinishGame()
    {
        levelFadingChangerScript.FadeToNextLevel();
    }

    private bool Intersects(GridRectangle tempRectangle)
    {
        for (int i = 0; i < gameController.GetPlayerCount(); i++)
        {
            foreach (GridRectangle rect in gameController.GetPlayer(i).GetPlayerMoves())
            {
                if (rect.Intersects(tempRectangle))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool Aligns(GridRectangle tempRectangle)
    {
        foreach (GridRectangle rect in gameController.GetActivePlayer().GetPlayerMoves())
        {
            if (rect.Aligns(tempRectangle))
            {
                return true;
            }
        }

        return false;
    }
}
