using UnityEngine;
using UnityEngine.UI;

public class AdjustmentTextScript : MonoBehaviour {

    private const int MIN_VALUE = 15;
    private const int MAX_VALUE = 50;

    private Text textComponent;

    public bool isWidth;

    private void Awake()
    {
        textComponent = gameObject.GetComponent<Text>();
    }

    public void Increment()
    {
        if (isWidth)
        {
            if (GameData.NoteWidth >= MAX_VALUE)
            {
                return;
            }

            GameData.NoteWidth++;
        } else
        {
            if (GameData.NoteHeight >= MAX_VALUE)
            {
                return;
            }
            GameData.NoteHeight++;
        }
    }

    public void Decrement()
    {
        if (isWidth)
        {
            if (GameData.NoteWidth <= MIN_VALUE)
            {
                return;
            }

            GameData.NoteWidth--;
        }
        else
        {
            if (GameData.NoteHeight <= MIN_VALUE)
            {
                return;
            }
            GameData.NoteHeight--;
        }
    }

    private void Update()
    {
        textComponent.text = "" + (isWidth ? GameData.NoteWidth : GameData.NoteHeight);
    }
}
