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
		button.interactable = (GameData.Generated != null && GameData.Generated.Length > 0 && GameData.Generated[0] > 0);
        button.image.sprite = GameData.CanSkipTurn() ? skipTexture : giveUpTexture;

        int skipTurnCount = GameData.GetSkipTurn();

        bgImage.color = GameData.GetColor();

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
        if (GameData.CanSkipTurn())
        {
            GameData.SkipTurn();
            script.SwitchPlayer();
        } else
        {
            GameData.GiveUp();
            script.SwitchPlayer();
        }
        
    }
}
