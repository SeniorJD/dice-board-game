using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player {
    private int[] diceValue = new int[] { 0, 0};
    private List<GridRectangle> playerMoves = new List<GridRectangle>();
    private int skippedTurnsLeft = GameData.MAX_SKIP_TURNS;
    private Color color;
    private Color colorA;
    private bool gaveUp;
    private bool isBot;
    private int score;
    private int playerIndex;

    public Player(int playerIndex, bool isBot, Color color)
    {
        this.playerIndex = playerIndex;
        this.isBot = isBot;
        this.color = color;
        this.colorA = new Color(color.r, color.g, color.b, 0.5f);
    }

    public int[] DiceValue
    {
        get
        {
            return diceValue;
        }

        set
        {
            diceValue = value;
        }
    }

    public Color Color
    {
        get
        {
            return color;
        }
    }

    public Color ColorA
    {
        get
        {
            return colorA;
        }
    }

    public bool GaveUp
    {
        get
        {
            return gaveUp;
        }
    }

    public bool IsBot
    {
        get
        {
            return isBot;
        }

        set
        {
            isBot = value;
        }
    }

    public int Score
    {
        get
        {
            return score;
        }

        set
        {
            score = value;
        }
    }

    public int PlayerIndex
    {
        get
        {
            return playerIndex;
        }
    }

    public bool IsFirstTurn()
    {
        return playerMoves.Count == 0;
    }

    public void AddPlayerMove(GridRectangle rect)
    {
        playerMoves.Add(rect);
    }

    public List<GridRectangle> GetPlayerMoves()
    {
        return new List<GridRectangle>(playerMoves);
    }

    public bool CanSkipTurn()
    {
        return skippedTurnsLeft > 0;
    }

    public void SkipTurn()
    {
        skippedTurnsLeft--;
    }

    public int GetSkipTurn()
    {
        return skippedTurnsLeft;
    }

    public void ResetSkippedTurns()
    {
        skippedTurnsLeft = GameData.MAX_SKIP_TURNS;
    }

    public bool WasDiceThrown()
    {
        return diceValue != null && diceValue.Length > 0 && diceValue[0] > 0 && diceValue[1] > 0;
    }

    public void GiveUp()
    {
        Debug.Log(playerIndex + " gived up");
        gaveUp = true;
    }

    public void EndTurn()
    {
        diceValue = new int[] { 0, 0 };
    }
}
