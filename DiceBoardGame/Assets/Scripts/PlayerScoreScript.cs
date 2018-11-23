using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreScript : MonoBehaviour {

    public int playerIndex;
    private Text text;
    private Animator animator;
    private Image bgImage;
    private bool isActivePlayer = false;

    private Player player;

    void Awake()
    {
        text = GetComponentInChildren<Text>();
        animator = GetComponent<Animator>();

        player = GameData.GameController.GetPlayer(playerIndex);
        bgImage = GetComponentInChildren<Image>();
        bgImage.color = player.ColorA;
    }

    // Update is called once per frame
    void Update () {
        text.text = "" + GameData.GameController.GetPlayer(playerIndex).Score;

        if (isActivePlayer)
        {
            isActivePlayer = GameData.GameController.ActivePlayerIndex == playerIndex;

            if (!isActivePlayer)
            {
                animator.SetTrigger("InactivePlayer");
            }
        } else
        {
            isActivePlayer = GameData.GameController.ActivePlayerIndex == playerIndex;

            if (isActivePlayer)
            {
                animator.SetTrigger("ActivePlayer");
            }
        }

        if (player.GaveUp)
        {
            bgImage.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }
	}
}
