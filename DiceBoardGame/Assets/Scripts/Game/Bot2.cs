using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot2 {
    private const int PHASE_INIT = 0;
    private const int PHASE_GOING_TO_CENTER = 1;
    private const int PHASE_MET = 2;

    private GameController gameController;
    private Player botPlayer;

    private BotPoint[,] points;
    private int[] offsets;
    private int[] fieldSize;

    private int phase = PHASE_INIT;

    private List<List<BotPoint>> potentialZones = new List<List<BotPoint>>();

    public Bot2(GameController gameController, Player botPlayer)
    {
        this.gameController = gameController;
        this.botPlayer = botPlayer;

        Init();
    }

    private void Init()
    {
        offsets = new int[] { gameController.Field.X, gameController.Field.Y };

        Debug.Log("Field: " + gameController.Field.ToString());

        fieldSize = new int[] { gameController.Field.Width, gameController.Field.Height };

        points = new BotPoint[gameController.Field.Width, gameController.Field.Height];

        for (int i = 0; i < gameController.Field.Width; i++)
        {
            for (int j = 0; j < gameController.Field.Height; j++)
            {
                points[i, j] = new BotPoint(i + offsets[0], - j + offsets[1]);
            }
        }
    }

    public void RectPlaced(GridRectangle rect, int playerId)
    {
        Debug.Log("RectPlaced: " + rect.ToString());
        foreach (BotPoint point in GetPointsInsideRect(rect))
        {
            if (point == null)
            {
                continue;
            }

            //Debug.Log("rect placed: id: " + playerId + "point: " + point.X + ":" + point.Y);
            point.OwnerId = playerId;

            if (playerId == botPlayer.PlayerIndex)
            {
                point.AlignedToMe = false;
            }
        }

        foreach(BotPoint point in GetPointsForRect(rect))
        {
            if (point == null || point.OwnerId != -1)
            {
                continue;
            }

            if (playerId == botPlayer.PlayerIndex)
            {
                point.AlignedToMe = true;
            } else
            {
                point.AlignedToEnemy = true;
            }
        }

        if (playerId == botPlayer.PlayerIndex)
        {
            if (phase == PHASE_INIT)
            {
                phase = PHASE_GOING_TO_CENTER;
            }

            if (phase == PHASE_GOING_TO_CENTER)
            {
                bool met = BotMetPlayer();

                if (met)
                {
                    phase = PHASE_MET;
                }
            }

            Debug.Log("Phase: " + phase);
        }

        if (phase == PHASE_MET)
        {
            InitPotentialZones();
        }
    }

    private void InitPotentialZones()
    {
        ResetAddedToPotentialList();
        potentialZones.Clear();

        List<BotPoint> currentList = null;
        for (int i = 0; i < points.GetLength(0); i++)
        {
            for (int j = 0; j < points.GetLength(1); j++)
            {
                BotPoint point = GetPoint(i, j);

                if (point.OwnerId != -1 || point.AddedToPotentialList)
                {
                    continue;
                }

                currentList = new List<BotPoint>();
                FillPotentialList(point, currentList);

                Debug.Log("potential " + currentList.Count + " for point " + point.ToString());

                potentialZones.Add(currentList);
            }
        }
    }

    private void FillPotentialList(BotPoint point, List<BotPoint> list)
    {
        if (point == null || point.OwnerId != -1 || point.AddedToPotentialList || list.Contains(point))
        {
            return;
        }

        list.Add(point);
        point.AddedToPotentialList = true;

        FillPotentialList(GetLeft(point), list);
        FillPotentialList(GetTop(point), list);
        FillPotentialList(GetRight(point), list);
        FillPotentialList(GetBottom(point), list);
    }

    private bool BotMetPlayer()
    {
        foreach (GridRectangle rect in botPlayer.GetPlayerMoves())
        {
            foreach (BotPoint point in GetPointsForRect(rect))
            {
                if (point != null && point.OwnerId != -1 && point.OwnerId != botPlayer.PlayerIndex)
                {
                    Debug.Log("bot met player: " + point.ToString());
                    return true;
                }
            }
        }

        return false;
    }

    public void Analyze(GameSceneGridScript gameSceneGridScript)
    {
        ResetProcessed();

        int[] diceValue = botPlayer.DiceValue;
        Debug.Log("Phase1 : " + phase);

        if (phase == PHASE_INIT)
        {
            DoFirstTurn(gameSceneGridScript);

            return;
        }

        if (phase == PHASE_GOING_TO_CENTER)
        {
            DoGoToCenter(gameSceneGridScript);
            return;
        }

        if (phase == PHASE_MET)
        {
            DoPlaceRectByPriority(gameSceneGridScript);
            return;
        }
    
    }

    private void ResetProcessed()
    {
        for (int i = 0; i < points.GetLength(0); i++)
        {
            for (int j = 0; j < points.GetLength(1); j++)
            {
                points[i, j].ProcessedThisTurn = false;
            }
        }
    }

    private void ResetAddedToPotentialList()
    {
        for (int i = 0; i < points.GetLength(0); i++)
        {
            for (int j = 0; j < points.GetLength(1); j++)
            {
                points[i, j].AddedToPotentialList = false;
            }
        }
    }

    private void DoFirstTurn(GameSceneGridScript gameSceneGridScript)
    {
        Debug.Log("DoFirstTurn");
        int[] diceValue = botPlayer.DiceValue;

        GridRectangle tempRect = new GridRectangle(offsets[0], offsets[1], diceValue[0], diceValue[1]);

        if (CanPlaceRect(tempRect, false))
        {
            gameSceneGridScript.PlaceRect(tempRect);
        }
        else
        {
            if (botPlayer.CanSkipTurn())
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

    private void DoGoToCenter(GameSceneGridScript gameSceneGridScript)
    {
        Debug.Log("DoGoToCenter");
        float minPathToOpponent = Mathf.Sqrt(fieldSize[0] * fieldSize[0] + fieldSize[1] * fieldSize[1]);
        GridRectangle appropriateRect = null;
        int[] diceValue = botPlayer.DiceValue;

        //foreach (GridRectangle rect in botPlayer.GetPlayerMoves())
        //{
        //foreach (BotPoint point in GetPointsForRect(rect)) {
        for (int i = offsets[0]; i <= fieldSize[0] - diceValue[0] + offsets[0]; i++)
        {
            for (int j = offsets[1]; j >= -fieldSize[1] + offsets[1] + diceValue[1]; j--)
            {
                Debug.Log("Checking [" + i + ", " + j + "]");
                GridRectangle tempRect = new GridRectangle(i, j, diceValue[0], diceValue[1]);
                //Debug.Log("dgtc point: " + (point == null ? null : point.ToString()));
                //if (point == null || point.ProcessedThisTurn)
                //{
                    //continue;
                //}

                //GridRectangle tempRect = new GridRectangle(point.X, point.Y, diceValue[0], diceValue[1]);

                //point.ProcessedThisTurn = true;

                if (!CanPlaceRect(tempRect))
                {
                    continue;
                }

                float pathToOpponent = Mathf.Sqrt(Mathf.Pow(fieldSize[0] - (i + diceValue[0] - offsets[0]), 2f) + Mathf.Pow(fieldSize[1] - (-j + diceValue[1] + offsets[1]), 2f));

                if (pathToOpponent < minPathToOpponent)
                {
                    minPathToOpponent = pathToOpponent;
                    appropriateRect = tempRect;
                }
            }
        }

        Debug.Log("Appropriate rect: " + (appropriateRect == null ? null : appropriateRect.ToString()));

        if (appropriateRect != null)
        {
            gameSceneGridScript.PlaceRect(appropriateRect);
        }
        else
        {
            if (botPlayer.CanSkipTurn())
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

    private void DoPlaceRectByPriority(GameSceneGridScript gameSceneGridScript)
    {
        Debug.Log("DoPlaceRectByPriority");
        int[] diceValue = botPlayer.DiceValue;
        GridRectangle appropriateRect = null;
        int maxPotential = 0;
        float minPathToOpponent = Mathf.Sqrt(fieldSize[0] * fieldSize[0] + fieldSize[1] * fieldSize[1]);

        //foreach (GridRectangle rect in botPlayer.GetPlayerMoves())
        //{
        //    foreach (BotPoint point in GetPointsForRect(rect))
        //    {
        for (int i = offsets[0]; i <= fieldSize[0] - diceValue[0] + offsets[0]; i++)
        {
            for (int j = offsets[1]; j >= -fieldSize[1] + offsets[1] + diceValue[1]; j--)
            {
                //if (point == null || point.ProcessedThisTurn)
                //{
                //    continue;
                //}

                GridRectangle tempRect = new GridRectangle(i, j, diceValue[0], diceValue[1]);

                //point.ProcessedThisTurn = true;

                Debug.Log("processing rect: " + tempRect.ToString());
                if (!CanPlaceRect(tempRect))
                {
                    continue;
                }

                BotPoint point = GetPoint(i - offsets[0], -j + offsets[1]);
                int potential = GetPotentialOfPoint(point);

                Debug.Log("For point: " + point.ToString() + " potential " + potential + " max: " + maxPotential);

                if (potential < maxPotential)
                {
                    continue;
                }

                maxPotential = potential;

                //float pathToOpponent = Mathf.Sqrt(Mathf.Pow(fieldSize[0] - (point.X + diceValue[0] - offsets[0]), 2f) + Mathf.Pow(fieldSize[1] - (-point.Y + diceValue[1] + offsets[1]), 2f));
                float pathToOpponent = Mathf.Sqrt(Mathf.Pow(fieldSize[0] - (i + diceValue[0] - offsets[0]), 2f) + Mathf.Pow(fieldSize[1] - (-j + diceValue[1] + offsets[1]), 2f));

                Debug.Log("path: " + pathToOpponent + ", min: " + minPathToOpponent);

                if (pathToOpponent >= minPathToOpponent)
                {
                    continue;
                }

                minPathToOpponent = pathToOpponent;

                appropriateRect = tempRect;
            }
        }

        Debug.Log("Appropriate rect: " + (appropriateRect == null ? null : appropriateRect.ToString()));

        if (appropriateRect != null)
        {
            gameSceneGridScript.PlaceRect(appropriateRect);
        }
        else
        {
            if (botPlayer.CanSkipTurn())
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

    private int GetPotentialOfPoint(BotPoint point)
    {
        foreach (List<BotPoint> list in potentialZones)
        {
            if (list.Contains(point))
            {
                return list.Count;
            }
        }

        return 0;
    }

    private bool CanPlaceRect(GridRectangle tempRect)
    {
        return CanPlaceRect(tempRect, true);
    }

    private bool CanPlaceRect(GridRectangle tempRect, bool checkAligns)
    {
        if (tempRect.X < offsets[0] || tempRect.Y > offsets[1] || tempRect.X2 - offsets[0] > fieldSize[0] || (-tempRect.Y2 + offsets[1] > fieldSize[1]))
        {
            Debug.Log("Cannot place rect 0");
            return false;
        }

        for (int playerIndex = 0; playerIndex < gameController.GetPlayerCount(); playerIndex++)
        {
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

        if (!checkAligns)
        {
            return true;
        }

        foreach (GridRectangle rect in botPlayer.GetPlayerMoves())
        {
            if (rect.Aligns(tempRect))
            {
                Debug.Log("Can place rect 1");
                return true;
            }
        }

        Debug.Log("Cannot place rect 2: " + tempRect.ToString());
        return false;
    }

    private BotPoint[] GetPointsInsideRect(GridRectangle rect)
    {
        BotPoint[] result = new BotPoint[rect.Width * rect.Height];

        int index = 0;
        for (int i = 0; i < rect.Width; i++)
        {
            for (int j = 0; j < rect.Height; j++)
            {
                result[index++] = GetPoint(rect.X + i - offsets[0], - rect.Y + j + offsets[1]);
            }
        }

        return result;
    }

    private BotPoint[] GetPointsForRect(GridRectangle rect)
    {
        //Debug.Log("points for RECT: " + rect.ToString());
        BotPoint[] result = new BotPoint[rect.Width * 2 + rect.Height * 2 + 4];

        int index = 0;

        int i, j;

        // top line
        j = - rect.Y - 1 + offsets[1];
        //Debug.Log("TOP LINE Y:" + j + "; X from " + (rect.X - offsets[0]) + " to " + (rect.X2 - offsets[0]));
        for (i = rect.X - offsets[0]; i < rect.X2 - offsets[0]; i++)
        {
            result[index++] = GetPoint(i, j);
            //Debug.Log("Point got" + (result[index - 1] ?? null));
        }

        // bottom line
        j = - rect.Y2 + offsets[1];
        //Debug.Log("BOTTOM LINE Y:" + j + "; X from " + (rect.X - offsets[0]) + " to " + (rect.X2 - offsets[0]));
        for (i = rect.X - offsets[0]; i < rect.X2 - offsets[0]; i++)
        {
            result[index++] = GetPoint(i, j);
            //Debug.Log("Point got" + (result[index - 1] ?? null));
        }

        // left line
        i = rect.X - 1 - offsets[0];
        //Debug.Log("LEFT LINE X:" + i + "; Y from " + (-rect.Y + offsets[1]) + " to " + (-rect.Y2 + offsets[1]));
        for (j = -rect.Y + offsets[1]; j < -rect.Y2 + offsets[1]; j++)
        {
            result[index++] = GetPoint(i, j);
            //Debug.Log("Point got" + (result[index - 1] ?? null));
        }

        // right line
        i = rect.X2 - offsets[0];
        //Debug.Log("RIGHT LINE X:" + i + "; Y from " + (-rect.Y + offsets[1]) + " to " + (-rect.Y2 + offsets[1]));
        for (j = -rect.Y + offsets[1]; j < -rect.Y2 + offsets[1]; j++)
        {
            result[index++] = GetPoint(i, j);
            //Debug.Log("Point got" + (result[index - 1] ?? null));
        }

        // top left corner
        i = rect.X - 1 - offsets[0];
        j = - rect.Y - 1 + offsets[1];
        result[index++] = GetPoint(i, j);
        //Debug.Log("TOP LEFT " + i + ":" + j);
        //Debug.Log("Point got" + (result[index - 1] ?? null));

        // top right corner
        i = rect.X2 - offsets[0];
        j = - rect.Y - 1 + offsets[1];
        result[index++] = GetPoint(i, j);
        //Debug.Log("TOP RIGHT " + i + ":" + j);
        //Debug.Log("Point got" + (result[index - 1] ?? null));

        // bottom right corner
        i = rect.X2 - offsets[0];
        j = -rect.Y2 + offsets[1];
        result[index++] = GetPoint(i, j);
        //Debug.Log("BOTTOM RIGHT " + i + ":" + j);
        //Debug.Log("Point got" + (result[index - 1] ?? null));

        // bottom left corner
        i = rect.X - 1 - offsets[0];
        j = -rect.Y2 + offsets[1];
        result[index++] = GetPoint(i, j);
        //Debug.Log("BOTTOM LEFT " + i + ":" + j);
        //Debug.Log("Point got" + (result[index - 1] ?? null));

        return result;
    }

    private BotPoint GetPoint(int i, int j)
    {
        if (i >= 0 && j >= 0 && i < fieldSize[0] && j < fieldSize[1])
        {
            return points[i, j];
        }
        else
        {
            return null;
        }
    }

    BotPoint GetLeft(BotPoint point)
    {
        BotPoint result;
        if (point.X - offsets[0] - 1 < 0)
        {
            result = null;
        } else
        {
            result = points[point.X - offsets[0] - 1, -point.Y + offsets[1]];
        }

        return result;
    }

    BotPoint GetRight(BotPoint point)
    {
        BotPoint result;
        if (point.X - offsets[0] + 1 >= fieldSize[0])
        {
            result = null;
        } else
        {
            result = points[point.X - offsets[0] + 1, -point.Y + offsets[1]];
        }

        return result;
    }

    BotPoint GetTop(BotPoint point)
    {
        BotPoint result;
        if (- point.Y + offsets[1] - 1 < 0)
        {
            result = null;
        } else
        {
            result = points[point.X - offsets[0], -point.Y + offsets[1] - 1];
        }

        return result;
    }

    BotPoint GetBottom(BotPoint point)
    {
        BotPoint result;
        if ( - point.Y + offsets[1] + 1 >= fieldSize[1])
        {
            result = null;
        } else
        {
            result = points[point.X - offsets[0], -point.Y + offsets[1] + 1];

        }

        return result;
    }

    private class BotPoint
    {
        private int x;
        private int y;

        private int ownerId;

        private bool alignedToEnemy;
        private bool alignedToMe;

        private bool processedThisTurn;

        private bool addedToPotentialList;

        public BotPoint(int x, int y)
        {
            this.x = x;
            this.y = y;

            ownerId = -1;
            alignedToEnemy = false;
            alignedToMe = false;

            processedThisTurn = false;
            addedToPotentialList = false;
        }

        public int X
        {
            get
            {
                return x;
            }
        }

        public int Y
        {
            get
            {
                return y;
            }
        }

        public bool ProcessedThisTurn
        {
            get
            {
                return processedThisTurn;
            }

            set
            {
                processedThisTurn = value;
            }
        }

        public bool AlignedToEnemy
        {
            get
            {
                return alignedToEnemy;
            }

            set
            {
                alignedToEnemy = value;
            }
        }

        public bool AlignedToMe
        {
            get
            {
                return alignedToMe;
            }

            set
            {
                alignedToMe = value;
            }
        }

        public int OwnerId
        {
            get
            {
                return ownerId;
            }

            set
            {
                ownerId = value;
            }
        }

        public bool AddedToPotentialList
        {
            get
            {
                return addedToPotentialList;
            }

            set
            {
                addedToPotentialList = value;
            }
        }

        public override string ToString()
        {
            return base.ToString() + ": {" + X + ", " + Y + ", ownerId: " + ownerId + ", alignedToEnemy: " + alignedToEnemy + ", alignedToMe: " + alignedToMe + ", processedThisTurn: " + processedThisTurn + ", addedToPotentialList: " + addedToPotentialList;
        }
    }
}
