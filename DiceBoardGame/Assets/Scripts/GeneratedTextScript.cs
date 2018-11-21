using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneratedTextScript : MonoBehaviour {

    private Text textComponent;
	// Use this for initialization
	void Start () {
		textComponent = gameObject.GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update () {
        int[] result = GameData.Generated;

        if (result == null || result.Length != 2)
        {
            textComponent.text = "Press Generate button";
            return;
        }

        textComponent.text = result[0] + " x " + result[1];
	}
}
