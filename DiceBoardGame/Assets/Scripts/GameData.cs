using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData {
    public static int MAX_PLAYERS = 2;
    public static int MAX_SKIP_TURNS = 3;
    public static int DEFAULT_FIELD_WIDTH = 16;
    public static int DEFAULT_FIELD_HEIGHT = 32;

    public static Color YELLOW_COLOR = new Color(0.97f, 1f, 0f);
    public static Color GREEN_COLOR = new Color(0f, 1f, 0.15f);

    private static GameController gameController;

    public static void StartGame()
    {
        gameController = new GameController(MAX_PLAYERS, NoteWidth, NoteHeight);
    }

    public static GameController GameController
    {
        get
        {
            if (gameController == null)
            {
                StartGame();
            }
            return gameController;
        }
    }

    private static int noteWidth = 16;
    private static int noteHeight = 32;
    private static bool isBotOpponent = true;

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

    public static void Clear()
    {
        //noteWidth = 16;
        //noteHeight = 32;
        //isBotOpponent = true;

        gameController = null;
    }
}
