using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GameSceneGridScript : MonoBehaviour {

    public TileBase whiteTile;
    public TileBase gridTile;
    public TileBase yellowTile;
    public TileBase greenTile;
    public TileBase blueTile;
    public TileBase redTile;

    public Button rollDicesButton;
    public Button applyMoveButton;

    public GameObject sceneFadingChanger;

    private LevelFadingChangerScript levelFadingChangerScript;

    private Tilemap gridTilemap;
    private Tilemap playerMovesTilemap;
    private Tilemap tempTilemap;

    private int inset = 2;

    private int xOffset;
    private int yOffset;

    private int[] tempTilemapDiceValues = null;
    private Vector3Int tempTilemapDrawingPosition;
    private GridRectangle tempRectangle;

    private bool tempTilemapCanPlace = false;

    private void Awake()
    {
        gridTilemap = RetrieveTilemap("GridTilemap");
        playerMovesTilemap = RetrieveTilemap("PlayerMovesTilemap");
        tempTilemap = RetrieveTilemap("TempTilemap");

        xOffset = -GameData.NoteWidth / 2 - inset;
        yOffset = GameData.NoteHeight / 2 + inset;

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

        if (Input.GetMouseButton(0))
        {
            DrawTempTiles();
        }
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
        int[] generated = GameData.Generated;

        Vector3Int currentPosition = GetCurrentPosition();

        Debug.Log("CurrentPosition" + currentPosition);

        if (IsCurrentPositionOutsideField(currentPosition))
        {
            return;
        }

        currentPosition = GetAlignedPosition(currentPosition, tempTilemapDiceValues);

        Debug.Log("AlignedPosition" + currentPosition);

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
        TileBase tile = tempTilemapCanPlace ? greenTile : yellowTile;

        for (int x = 0; x < tempTilemapDiceValues[0]; x++)
        {
            for (int y = 0; y < tempTilemapDiceValues[1]; y++)
            {
                Vector3Int vector = new Vector3Int(x, -y, 0);
                vector.x -= centerCoords[0];
                vector.y += centerCoords[1];
                vector.x += tempTilemapDrawingPosition.x;
                vector.y += tempTilemapDrawingPosition.y;


                tempTilemap.SetTile(vector, tile);
            }
        }
    }

    private bool IsCurrentPositionOutsideField(Vector3Int currentPosition)
    {
        if (currentPosition.x < -GameData.NoteWidth / 2)
        {
            Debug.Log("exit1: " + currentPosition.x + " < " + -GameData.NoteWidth / 2);
            return true;
        }

        if (currentPosition.x >= GameData.NoteWidth - GameData.NoteWidth / 2)
        {
            Debug.Log("exit2: " + currentPosition.x + " >= " + (GameData.NoteWidth - GameData.NoteWidth / 2));
            return true;
        }

        if (currentPosition.y > GameData.NoteHeight / 2)
        {
            Debug.Log("exit3: " + currentPosition.y + " > " + GameData.NoteHeight / 2);
            return true;
        }

        if (currentPosition.y <= -GameData.NoteHeight + GameData.NoteHeight / 2)
        {
            Debug.Log("exit4: " + currentPosition.y + " <= " + (-GameData.NoteHeight + GameData.NoteHeight / 2));
            return true;
        }

        return false;
    }

    private Vector3Int GetCurrentPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
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

        int[] MAX_TILES = new int[] { GameData.NoteWidth, GameData.NoteHeight };

        int minX;
        int minY;
        int maxX;
        int maxY;
        minX = -MAX_TILES[0] / 2 + values[0] / 2;
        minY = MAX_TILES[1] / 2 - values[1] / 2;
        maxX = MAX_TILES[0] - MAX_TILES[0] / 2 - (values[0] - values[0] / 2);
        maxY = -MAX_TILES[1] + MAX_TILES[1] / 2 - (-values[1] + values[1] / 2);

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
        int[] generated = GameData.Generated;

        return generated == null || generated.Length == 0 || generated[0] == 0 || generated[1] == 0;
    }

    private bool CanPlaceRect(GridRectangle rect)
    {
        if (rect == null)
        {
            return false;
        }

        if (IsFirstTurn())
        {
            if (GameData.ActivePlayer == 0)
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
        int activePlayer = GameData.ActivePlayer;

        return GameData.PlayerMoves[activePlayer].Count == 0;
    }

    private bool CanPlaceFirstRect0(GridRectangle rect)
    {
        int maxX = GameData.NoteWidth - GameData.NoteWidth / 2;
        int maxY = -GameData.NoteHeight + GameData.NoteHeight / 2;

        return (rect.X2 == maxX) && (rect.Y2 == maxY);
    }

    private bool CanPlaceFirstRect1(GridRectangle rect)
    {
        int minX = - GameData.NoteWidth / 2;
        int minY = GameData.NoteHeight / 2;

        return (rect.X == minX) && (rect.Y == minY);
    }

    public void PlaceRect()
    {
        int activePlayer = GameData.ActivePlayer;

        GameData.PlayerMoves[activePlayer].Add(tempRectangle);

        GameData.ResetSkippedTurns();

        GameData.Score[activePlayer] += tempRectangle.GetSquare();

        DrawPlacedRect();

        SwitchPlayer();
    }

    private void DrawPlacedRect()
    {
        TileBase tile = GameData.ActivePlayer == 0 ? blueTile : redTile;
        for (int x = tempRectangle.X; x < tempRectangle.X2; x++)
        {
            for (int y = tempRectangle.Y; y > tempRectangle.Y2; y--)
            {
                playerMovesTilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
    }

    public void SwitchPlayer()
    {
        ClearTempTiles(tempTilemapDiceValues, tempTilemapDrawingPosition == null ? new Vector3Int(0, 0, 0) : tempTilemapDrawingPosition);

        GameData.NextPlayer();
        GameData.Generated = new int[] { 0, 0 };

        tempTilemapCanPlace = false;
        tempRectangle = null;
        tempTilemapDrawingPosition = new Vector3Int();
        tempTilemapDiceValues = null;

        if (GameData.GameFinished())
        {
            FinishGame();
            return;
        }

        if (GameData.UserGaveUp(GameData.ActivePlayer))
        {
            SwitchPlayer();
        }
    }

    private void FinishGame()
    {
        levelFadingChangerScript.FadeToNextLevel();
    }

    private bool Intersects(GridRectangle tempRectangle)
    {
        foreach(List<GridRectangle> list in GameData.PlayerMoves)
        {
            foreach(GridRectangle rect in list)
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
        foreach (GridRectangle rect in GameData.PlayerMoves[GameData.ActivePlayer])
        {
            if (rect.Aligns(tempRectangle))
            {
                return true;
            }
        }

        return false;
    }
}
