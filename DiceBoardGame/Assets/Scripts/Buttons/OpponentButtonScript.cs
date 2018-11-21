using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpponentButtonScript : MonoBehaviour {

    private Text textComponent;

    private void Awake()
    {
        textComponent = gameObject.GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update () {
        textComponent.text = GameData.IsBotOpponent ? "Bot" : "Human";
    }

    public void Switch()
    {
        GameData.IsBotOpponent = !GameData.IsBotOpponent;
    }
}
