using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreScript : MonoBehaviour {

    public int playerIndex;
    private Text text;
    private Animator animator;
    private bool isActivePlayer = false;

    void Awake()
    {
        text = GetComponentInChildren<Text>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update () {
        text.text = "" + GameData.Score[playerIndex];

        if (isActivePlayer)
        {
            isActivePlayer = GameData.ActivePlayer == playerIndex;

            if (!isActivePlayer)
            {
                animator.SetTrigger("InactivePlayer");
            }
        } else
        {
            isActivePlayer = GameData.ActivePlayer == playerIndex;

            if (isActivePlayer)
            {
                animator.SetTrigger("ActivePlayer");
            }
        }
	}
}
