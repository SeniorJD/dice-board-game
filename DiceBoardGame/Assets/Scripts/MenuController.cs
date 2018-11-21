using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {

    public Grid tilemapGrid;
    private GameSceneGridScript script;

    public Button skipButton;

    void Awake()
    {
        script = tilemapGrid.GetComponent<GameSceneGridScript>();
    }

    void Update()
    {
        skipButton.interactable = (GameData.Generated != null && GameData.Generated.Length > 0 && GameData.Generated[0] > 0);
    }
    public void SkipTurn()
    {
        script.SwitchPlayer();
    }
}
