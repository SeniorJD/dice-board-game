using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateButtonScript : MonoBehaviour {
    public void Generate()
    {
        GameData.GameController.ThrowDice();
    }
}
