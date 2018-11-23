using UnityEngine;
using UnityEngine.UI;

public class AdjustmentTextScript : MonoBehaviour {

    private const int MIN_VALUE = 15;
    private const int MAX_VALUE = 50;

    private Text textComponent;

    public bool isWidth;
    private int value;

    private void Awake()
    {
        textComponent = gameObject.GetComponent<Text>();

        if (value < MIN_VALUE)
        {
            value = MIN_VALUE;
        } else if (value > MAX_VALUE)
        {
            value = MAX_VALUE;
        }

        value = isWidth ? GameData.DEFAULT_FIELD_WIDTH : GameData.DEFAULT_FIELD_HEIGHT;
    }

    public void Increment()
    {
        if (value == MAX_VALUE)
        {
            return;
        }
        value++;

        Save();
    }

    public void Decrement()
    {
        if (value == MIN_VALUE)
        {
            return;
        }
        value--;

        Save();
    }

    private void Save()
    {
        if (isWidth)
        {
            GameData.NoteWidth = value;
        }
        else
        {
            GameData.NoteHeight = value;
        }
    }

    private void Update()
    {
        textComponent.text = "" + value;
    }
}
