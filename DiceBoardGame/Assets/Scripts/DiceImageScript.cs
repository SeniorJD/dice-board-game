using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceImageScript : MonoBehaviour {

    public int diceIndex;

    private Image myImage;

    private void Awake()
    {
        myImage = gameObject.GetComponent<Image>();
    }

    void Update () {
        int value = GameData.GameController.GetActivePlayer().DiceValue[diceIndex];

        myImage.sprite = Resources.Load<Sprite>("Textures/Dice/Dice_" + value);
    }
}
