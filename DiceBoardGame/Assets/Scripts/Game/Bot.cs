using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot {

    private GameController gameController;

    private int[,] field;

    private int[] offsets;
    private int[] fieldSize;

    public Bot(GameController gameController)
    {
        this.gameController = gameController;

        Init();
    }

    private void Init()
    {
        field = new int[gameController.Field.Width, gameController.Field.Height];

        for (int i = 0; i < gameController.Field.Width; i++)
        {
            for (int j = 0; j < gameController.Field.Height; j++)
            {
                field[i, j] = 1;
            }
        }

        offsets = new int[] { gameController.Field.X, gameController.Field.Y };

        fieldSize = new int[] { gameController.Field.Width, gameController.Field.Height };
    }

    public void ApplyTurn(GridRectangle rect, bool opponent)
    {
        for (int i = rect.X; i < rect.X2; i++)
        {
            for (int j = rect.Y; j > rect.Y2; j--)
            {
                field[i - offsets[0], j - offsets[1]] = 0;
            }
        }

        if (!opponent)
        {
            return;
        }

        bool left = rect.X > offsets[0];
        bool top = rect.Y < offsets[1];
        bool right = rect.X2 < fieldSize[0] + offsets[0];
        bool bottom = rect.Y2 > fieldSize[1] + offsets[1];

        if (left)
        {
            int i = rect.X - offsets[0] - 1;
            if (top)
            {
                MarkPriorIfClean(i, rect.Y - offsets[1] - 1);
            }

            for (int j = rect.Y - offsets[1]; j > rect.Y2 - offsets[1]; j--)
            {
                MarkPriorIfClean(i, j);
            }

            if (bottom)
            {
                MarkPriorIfClean(i, rect.Y2 - offsets[1]);
            }
        }
        if (right)
        {
            int i = rect.X2 - offsets[0];
            if (top)
            {
                MarkPriorIfClean(i, rect.Y - offsets[1] - 1);
            }

            for (int j = rect.Y - offsets[1]; j > rect.Y2 - offsets[1]; j--)
            {
                MarkPriorIfClean(i, j);
            }

            if (bottom)
            {
                MarkPriorIfClean(i, rect.Y2 - offsets[1]);
            }
        }
        if (top)
        {
            int j = rect.Y - offsets[1] - 1;

            for (int i = rect.X - offsets[0]; i < rect.X2 - offsets[0]; i++)
            {
                MarkPriorIfClean(i, j);
            }
        }

        if (bottom)
        {
            int j = rect.Y2 - offsets[1];

            for (int i = rect.X - offsets[0]; i < rect.X2 - offsets[0]; i++)
            {
                MarkPriorIfClean(i, j);
            }
        }
    }

    private void MarkPriorIfClean(int i, int j)
    {
        if (field[i, j] == 1)
        {
            field[i, j] = 2;
        }
    }

    public void Clear()
    {

    }

    public void Analyze(GameSceneGridScript gameSceneGridScript)
    {
        Player activePlayer = gameController.GetActivePlayer();

        int[] diceValue = activePlayer.DiceValue;

        if (activePlayer.IsFirstTurn())
        {
            GridRectangle tempRect = new GridRectangle(offsets[0], offsets[1], diceValue[0], diceValue[1]);

            if (CanPlaceRect(tempRect))
            {
                gameSceneGridScript.PlaceRect(tempRect);
            } else
            {
                if (activePlayer.CanSkipTurn())
                {
                    gameController.SkipTurn();
                    gameSceneGridScript.SwitchPlayer(); // TODO
                }
                else
                {
                    gameController.GiveUp();
                    gameSceneGridScript.SwitchPlayer(); // TODO
                }
            }

            return;
        }

        Debug.Log("Offsets: " + offsets[0] + "x" + offsets[1]);
        Debug.Log("FieldSize: " + fieldSize[0] + "x" + fieldSize[1]);

        float minPathToOpponent = Mathf.Sqrt(fieldSize[0] * fieldSize[0] + fieldSize[1] * fieldSize[1]);
        GridRectangle appropriateRect = null;

        for (int i = offsets[0]; i <= fieldSize[0] - diceValue[0] + offsets[0]; i++)
        {
            for (int j = offsets[1]; j >= -fieldSize[1] + offsets[1] + diceValue[1]; j--)
            {
                Debug.Log("Checking [" + i + ", " + j + "]");
                GridRectangle tempRect = new GridRectangle(i, j, diceValue[0], diceValue[1]);
                if (CanPlaceRect(tempRect))
                {
                    float pathToOpponent = Mathf.Sqrt(Mathf.Pow(fieldSize[0] - (i + diceValue[0] - offsets[0]), 2f) + Mathf.Pow(fieldSize[1] - (-j + diceValue[1] + offsets[1]), 2f));

                    Debug.Log("min path: " + minPathToOpponent + ", cur path: " + pathToOpponent);

                    if (pathToOpponent < minPathToOpponent)
                    {
                        minPathToOpponent = pathToOpponent;
                        appropriateRect = tempRect;
                    }
                }
            }
        }

        Debug.Log("Appropriate rect: " + (appropriateRect == null ? null : appropriateRect.ToString()));

        if (appropriateRect != null)
        {
            gameSceneGridScript.PlaceRect(appropriateRect);
        } else
        {
            if (activePlayer.CanSkipTurn())
            {
                gameController.SkipTurn();
                gameSceneGridScript.SwitchPlayer(); // TODO
            }
            else
            {
                gameController.GiveUp();
                gameSceneGridScript.SwitchPlayer(); // TODO
            }
        }
    }

    private bool CanPlaceRect(GridRectangle tempRect)
    {
        for (int playerIndex = 0; playerIndex < gameController.GetPlayerCount(); playerIndex++) {
            Player player = gameController.GetPlayer(playerIndex);

            foreach (GridRectangle rect in player.GetPlayerMoves())
            {
                if (rect.Intersects(tempRect))
                {
                    Debug.Log("Cannot place rect 1");
                    return false;
                }
            }
        }

        foreach (GridRectangle rect in gameController.GetActivePlayer().GetPlayerMoves())
        {
            if (rect.Aligns(tempRect))
            {
                Debug.Log("Can place rect 1");
                return true;
            }
        }

        Debug.Log("Cannot place rect 2");
        return false;
    }
}
