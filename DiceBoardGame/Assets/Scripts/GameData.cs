using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData {
    public static int MAX_PLAYERS = 2;
    public static int MAX_SKIP_TURNS = 3;

    private static GameController gameController;

    public static void StartGame()
    {
        gameController = new GameController();
    }

    private static List<GridRectangle>[] playerMoves = new List<GridRectangle>[] { new List<GridRectangle>(), new List<GridRectangle>() };

    private static int[] generated = new int[] { 0, 0};
    private static int[] skippedTurns = new int[] { MAX_SKIP_TURNS, MAX_SKIP_TURNS };
    private static Color[] colors = new Color[] { new Color(0.3f, 0.42f, 1f), new Color(1, 0.3f, 0.33f)};
    private static bool[] userGaveUp = new bool[] { false, false };

    private static int noteWidth = 16;
    private static int noteHeight = 32;
    private static bool isBotOpponent = true;

    private static int activePlayer;

    private static int[] score = new int[2];

    public static int[] Generated
    {
        get
        {
            return generated;
        }

        set
        {
            generated = value;
        }
    }

    public static void Rotate()
    {
        int temp = generated[0];
        generated[0] = generated[1];
        generated[1] = temp;
    }

    public static void NextPlayer()
    {
        activePlayer++;

        if (activePlayer >= MAX_PLAYERS)
        {
            activePlayer = 0;
        }
    }

    public static bool CanSkipTurn()
    {
        return skippedTurns[activePlayer] > 0;
    }

    public static void SkipTurn()
    {
        skippedTurns[activePlayer]--;
    }

    public static int GetSkipTurn()
    {
        return skippedTurns[activePlayer];
    }

    public static void ResetSkippedTurns()
    {
        skippedTurns[activePlayer] = MAX_SKIP_TURNS;
    }

    public static Color GetColor()
    {
        return GetColor(activePlayer);
    }

    public static Color GetColor(int playerId)
    {
        return colors[playerId];
    }

    public static void GiveUp()
    {
        userGaveUp[activePlayer] = true;
    }

    public static bool UserGaveUp(int player)
    {
        return userGaveUp[player];
    }

    public static bool GameFinished()
    {
        return score[0] + score[1] == noteWidth * noteHeight || (userGaveUp[0] && userGaveUp[1]);
    }

    public static int NoteWidth
    {
        get
        {
            return noteWidth;
        }

        set
        {
            noteWidth = value;
        }
    }

    public static int NoteHeight
    {
        get
        {
            return noteHeight;
        }

        set
        {
            noteHeight = value;
        }
    }

    public static bool IsBotOpponent
    {
        get
        {
            return isBotOpponent;
        }

        set
        {
            isBotOpponent = value;
        }
    }

    public static int ActivePlayer
    {
        get
        {
            return activePlayer;
        }

        set
        {
            activePlayer = value;
        }
    }

    public static int[] Score
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

    public static List<GridRectangle>[] PlayerMoves
    {
        get
        {
            return playerMoves;
        }

        set
        {
            playerMoves = value;
        }
    }

    public static void Clear()
    {
        playerMoves = new List<GridRectangle>[] { new List<GridRectangle>(), new List<GridRectangle>() };
        generated = new int[] { 0, 0 };
        skippedTurns = new int[] { MAX_SKIP_TURNS, MAX_SKIP_TURNS };
        colors = new Color[] { new Color(0.3f, 0.42f, 1f), new Color(1, 0.3f, 0.33f) };
        userGaveUp = new bool[] { false, false };
        noteWidth = 16;
        noteHeight = 32;
        isBotOpponent = true;
        activePlayer = 0;
        score = new int[2];
    }
}
