using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player {
    private int[] diceValue;
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
}
