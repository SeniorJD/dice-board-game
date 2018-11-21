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
        int blueScore = GameData.Score[0];
        int redScore = GameData.Score[1];

        string s1;
        string s2;

        if (blueScore > redScore)
        {
            s1 = "Blue player wins!";
            s2 = blueScore + ":" + redScore;
            Color c = GameData.GetColor(0);
            GetComponent<Image>().color = new Color(c.r, c.g, c.b, 0.5f);
        } else if (redScore > blueScore)
        {
            s1 = "Red player wins!";
            s2 = redScore + ":" + blueScore;
            Color c = GameData.GetColor(1);
            GetComponent<Image>().color = new Color(c.r, c.g, c.b, 0.5f);
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
