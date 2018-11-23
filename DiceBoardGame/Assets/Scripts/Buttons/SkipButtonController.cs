using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkipButtonController : MonoBehaviour {

    public Sprite skipTexture;
    public Sprite giveUpTexture;

    public Grid tilemapGrid;
    private GameSceneGridScript script;

    private Button button;
    private Image buttonImage;
    private Text text;
    private Image bgImage;

    void Awake()
    {
        script = tilemapGrid.GetComponent<GameSceneGridScript>();
        button = GetComponent<Button>();
        text = GetComponentInChildren<Text>();
        buttonImage = GetComponent<Image>();

        Image[] images = GetComponentsInChildren<Image>();

        foreach(Image i in images)
        {
            if (i.gameObject.name == "BlueImage")
            {
                bgImage = i;
                break;
            }
        }
    }

    // Update is called once per frame
    void Update () {
        Player activePlayer = GameData.GameController.GetActivePlayer();

        button.interactable = (activePlayer.WasDiceThrown());
        button.image.sprite = activePlayer.CanSkipTurn() ? skipTexture : giveUpTexture;

        int skipTurnCount = activePlayer.GetSkipTurn();

        bgImage.color = activePlayer.Color;

        if (skipTurnCount <= 0)
        {
            text.gameObject.SetActive(false);
            bgImage.gameObject.SetActive(false);
        } else
        {
            text.gameObject.SetActive(true);
            bgImage.gameObject.SetActive(true);
            text.text = "" + skipTurnCount;
        }
    }

    public void SkipTurn()
    {
        Player activePlayer = GameData.GameController.GetActivePlayer();
        if (activePlayer.CanSkipTurn())
        {
            GameData.GameController.SkipTurn();
            script.SwitchPlayer(); // TODO
        } else
        {
            GameData.GameController.GiveUp();
            script.SwitchPlayer(); // TODO
        }
    }
}
