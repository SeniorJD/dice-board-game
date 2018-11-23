using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TempTilemapScript : MonoBehaviour {

    private int[] values = null;
    private bool rotated;
    private Tilemap tilemap;
    private Vector3Int position;

    //private int[] MAX_TILES = new int[] { GameData.NoteWidth, GameData.NoteWidth };

    public TileBase tile;
    // Use this for initialization
    void Start () {
        tilemap = gameObject.GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void Update() {
        int[] generated = GameData.GameController.GetActivePlayer().DiceValue;

        if (generated == null && values == null)
        {
            return;
        }

        Vector3Int currentPosition = GetCurrentPosition();
        currentPosition = GetAlignedPosition(currentPosition, values, rotated);

        if (values != null && generated != null && values.Equals(generated))
        {
            //if (rotated == GameData.Rotate)
            //{
                if (currentPosition != null && currentPosition.Equals(position))
                {
                    return;
                }
            //}
        }

        int[] oldValues = values;
        bool oldRotated = rotated;
        Vector3Int oldPosition = position == null ? new Vector3Int(0, 0, 0) : position;
        values = generated;
        //rotated = GameData.Rotate;

        position = currentPosition;

        int[] centerCoords;

        if (oldValues != null)
        {
            centerCoords = new int[] { oldValues[0] / 2, oldValues[1] / 2 };
            //if (oldValues[0] % 2 == 0)
            //{
            //    centerCoords[0]--;
            //}
            //if (oldValues[1] % 2 == 0)
            //{
            //    centerCoords[1]--;
            //}

            for (int x = 0; x < oldValues[0]; x++)
            {
                for (int y = 0; y < oldValues[1]; y++)
                {
                    Vector3Int vector;
                    if (oldRotated)
                    {
                        vector = new Vector3Int(y, -x, 0);
                        vector.x -= centerCoords[1];
                        vector.y += centerCoords[0];
                    }
                    else
                    {
                        vector = new Vector3Int(x, -y, 0);
                        vector.x -= centerCoords[0];
                        vector.y += centerCoords[1];
                    }
                    vector.x += oldPosition.x;
                    vector.y += oldPosition.y;


                    
                    tilemap.SetTile(vector, null);
                }
            }
        }

        if (values == null)
        {
            return;
        }

        centerCoords = new int[] { values[0] / 2, values[1] / 2 };
        //if (values[0] % 2 == 0)
        //{
        //    centerCoords[0]--;
        //}
        //if (values[1] % 2 == 0)
        //{
        //    centerCoords[1]--;
        //}

        for (int x = 0; x < values[0]; x++)
        {
            for (int y = 0; y < values[1]; y++)
            {
                Vector3Int vector;
                if (rotated)
                {
                    vector = new Vector3Int(y, -x, 0);
                    vector.x -= centerCoords[1];
                    vector.y += centerCoords[0];

                }
                else
                {
                    vector = new Vector3Int(x, -y, 0);
                    vector.x -= centerCoords[0];
                    vector.y += centerCoords[1];
                }
                vector.x += position.x;
                vector.y += position.y;

                
                tilemap.SetTile(vector, tile);
            }
        }
    }

    private Vector3Int GetCurrentPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 worldPoint = ray.GetPoint(-ray.origin.z / ray.direction.z);
        Vector3Int position = tilemap.WorldToCell(worldPoint);

        return position;
    }

    private Vector3Int GetAlignedPosition(Vector3Int position, int[] values, bool rotated)
    {
        //if (values == null)
        //{
        return position;
        //}

        //int minX;
        //int minY;
        //int maxX;
        //int maxY;
        //if (rotated)
        //{
        //    minX = values[1] / 2;
        //    minY = - values[0] / 2;
        //maxX = MAX_TILES[0] - (values[1] - minX);
        //maxY = - MAX_TILES[1] - (-values[0] - minY);
        //} else
        //{
        //minX = values[0] / 2;
        //minY = - values[1] / 2;
        //maxX = MAX_TILES[0] - (values[0] - minX);
        //maxY = - MAX_TILES[1] - (- values[1] - minY);
        //}

        //if (position.x < minX)
        //{
        //    position.x = minX;
        //} else if (position.x > maxX)
        //{
        //    position.x = maxX;
        //}

        //Debug.Log(position.y);
        //Debug.Log(minY);
        //Debug.Log(maxY);
        //Debug.Log("----------");

        //if (position.y > minY)
        //{
        //    position.y = minY;
        //}
        //else if (position.y < maxY)
        //{
        //    position.y = maxY;
        //}

        //return position;
    }
}
