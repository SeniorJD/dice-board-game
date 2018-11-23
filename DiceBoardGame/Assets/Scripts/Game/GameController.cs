using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController {
    private Player[] players;
    private int activePlayerIndex;
    private GridRectangle field;

    public GameController(int playersCount, int fieldWidth, int fieldHeight)
    {
        players = new Player[playersCount];
        for (int i = 0; i < playersCount; i++)
        {
            players[i] = CreatePlayer(i);
        }

        field = new GridRectangle(-fieldWidth / 2, fieldHeight / 2, fieldWidth, fieldHeight);
    }

    private Player CreatePlayer(int index)
    {
        if (index == 0)
        {
            return new Player(0, false, new Color(0.3f, 0.42f, 1f));
        } else
        {
            return new Player(1, GameData.IsBotOpponent, new Color(1, 0.3f, 0.33f));
        }
    }

    public int GetPlayerCount()
    {
        return players.Length;
    }

    public Player GetPlayer(int playerIndex)
    {
        return players[playerIndex];
    }

    public Player GetActivePlayer()
    {
        return GetPlayer(activePlayerIndex);
    }

    public void SkipTurn()
    {
        GetActivePlayer().SkipTurn();
    }

    public void GiveUp()
    {
        GetActivePlayer().GiveUp();
    }

    public int ActivePlayerIndex
    {
        get
        {
            return activePlayerIndex;
        }

        set
        {
            activePlayerIndex = value;
        }
    }

    public GridRectangle Field
    {
        get
        {
            return field;
        }
    }

    public void SwitchToNextPlayer()
    {
        GetActivePlayer().EndTurn();

        activePlayerIndex++;

        if (activePlayerIndex >= players.Length)
        {
            activePlayerIndex = 0;
        }
    }

    public bool AllUsersGaveUp()
    {
        bool allUsersGaveUp = true;

        foreach(Player player in players)
        {
            if (!player.GaveUp)
            {
                allUsersGaveUp = false;
                break;
            }
        }

        Debug.Log("All users gave up:" + allUsersGaveUp + ", users: " + players.Length);

        return allUsersGaveUp;
    }

    public bool GameFinished()
    {
        int score = 0;

        foreach (Player player in players)
        {
            score += player.Score;
        }

        return score == field.GetSquare() || AllUsersGaveUp();
    }
}
