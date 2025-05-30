// DialogueLine.cs
// Data container for a single line of dialogue.
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string speakerName;
    public Sprite portrait;

    [TextArea(2, 5)]
    public string text;
    public Choice[] choices;
}
