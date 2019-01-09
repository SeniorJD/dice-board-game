using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredefinedButtonScript : MonoBehaviour {

    public int width = 15;
    public int height = 15;

    void Awake()
    {
        
    }

    public void Apply()
    {
        GameData.NoteWidth = width;
        GameData.NoteHeight = height;
    }
}
