using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitPartyButtonScript : MonoBehaviour {
    public GameObject sceneFadingChanger;

    private LevelFadingChangerScript levelFadingChangerScript;

    void Awake()
    {
        levelFadingChangerScript = sceneFadingChanger.GetComponent<LevelFadingChangerScript>();
    }

    public void ExitParty()
    {
        levelFadingChangerScript.FadeToLevel(0);
    }
}
