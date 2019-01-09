using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController {
    private Player[] players;
    private int activePlayerIndex;
    private GridRectangle field;

    private Bot2 bot;

    public GameController(int playersCount, int fieldWidth, int fieldHeight)
    {
        field = new GridRectangle(-fieldWidth / 2, fieldHeight / 2, fieldWidth, fieldHeight);

        players = new Player[playersCount];
        for (int i = 0; i < playersCount; i++)
        {
            players[i] = CreatePlayer(i);
            if (players[i].IsBot)
            {
                bot = new Bot2(this, players[i]);
            }
        }
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

    public void ThrowDice()
    {
        int x = Random.Range(1, 7);
        int y = Random.Range(1, 7);

        GetActivePlayer().DiceValue = new int[] { x, y };
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

    public Bot2 Bot
    {
        get
        {
            return bot;
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

        //if (GetActivePlayer().IsBot)
        //{
        //    ThrowDice();

            
        //}
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

    public bool OtherUsersGaveUpAndLost()
    {
        int nonGaveUpCount = 0;
        int nonGaveUpScore = 0;
        int maxScore = 0;
        for (int i = 0; i < players.Length; i++)
        {
            Player player = players[i];

            if (!player.GaveUp)
            {
                if (nonGaveUpCount > 0)
                {
                    return false;
                }

                nonGaveUpCount++;
                nonGaveUpScore = player.Score;
            } else
            {
                maxScore = Mathf.Max(maxScore, player.Score);
            }
        }

        return nonGaveUpCount > 0 && nonGaveUpScore > maxScore;
    }

    public bool GameFinished()
    {
        int halfPoints = (field.GetSquare() / 2);
        bool halfPointsPresent = false;
        int score = 0;

        foreach (Player player in players)
        {
            halfPointsPresent = halfPointsPresent || (player.Score > halfPoints);
            score += player.Score;
        }

        return halfPointsPresent || score == field.GetSquare() || AllUsersGaveUp() || OtherUsersGaveUpAndLost();
    }
}
