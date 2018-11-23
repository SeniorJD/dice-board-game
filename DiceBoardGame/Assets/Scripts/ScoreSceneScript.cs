using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScoreSceneScript : MonoBehaviour, IPointerDownHandler {

    public Text scoreText;
    public GameObject sceneFadingChanger;

    private LevelFadingChangerScript levelFadingChangerScript;

    void Start () {
        int blueScore = GameData.GameController.GetPlayer(0).Score;
        int redScore = GameData.GameController.GetPlayer(1).Score;

        string s1;
        string s2;

        if (blueScore > redScore)
        {
            s1 = "Blue player wins!";
            s2 = blueScore + ":" + redScore;
            Color c = GameData.GameController.GetPlayer(0).ColorA;
            GetComponent<Image>().color = c;
        } else if (redScore > blueScore)
        {
            s1 = "Red player wins!";
            s2 = redScore + ":" + blueScore;
            Color c = GameData.GameController.GetPlayer(1).ColorA;
            GetComponent<Image>().color = c;
        } else
        {
            s1 = "Equal game!";
            s2 = blueScore + ":" + redScore;
        }

        scoreText.text = s1 + "\r\n" + s2;

        levelFadingChangerScript = sceneFadingChanger.GetComponent<LevelFadingChangerScript>();

        GameData.Clear();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        levelFadingChangerScript.FadeToLevel(0);
    }
}
