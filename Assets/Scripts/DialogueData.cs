// DialogueData.cs
// ScriptableObject holding an array of dialogue lines.
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/DialogueData")]
public class DialogueData : ScriptableObject
{
    public DialogueLine[] lines;
}
