using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController {
    private Player[] players = new Player[GameData.MAX_PLAYERS];

    public GameController()
    {
        for (int i = 0; i < GameData.MAX_PLAYERS; i++)
        {
            players[i] = CreatePlayer(i);
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

    public Player GetPlayer(int playerIndex)
    {
        return players[playerIndex];
    }
}
