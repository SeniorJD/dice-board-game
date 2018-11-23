using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateButtonScript : MonoBehaviour {
    public void Generate()
    {
        int x = Random.Range(1, 7);
        int y = Random.Range(1, 7);

        //x = 2;
        //y = 2;

        GameData.GameController.GetActivePlayer().DiceValue = new int[] { x, y };

        //gameObject.SetActive(false);
    }
}
