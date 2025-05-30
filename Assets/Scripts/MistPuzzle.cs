// MistPuzzle.cs
// Puzzle where tapping a mist image gradually clears it until solved.
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MistPuzzle : MonoBehaviour, IPointerClickHandler
{
    public int tapsToClear = 10;
    private int currentTaps;
    public Image mistImage;

    void Start()
    {
        currentTaps = 0;
    }

    public void OnPointerClick(PointerEventData e)
    {
        currentTaps++;
        float alpha = Mathf.Lerp(1f, 0f, (float)currentTaps / tapsToClear);
        if (mistImage != null)
            mistImage.color = new Color(1, 1, 1, alpha);

        if (currentTaps >= tapsToClear)
            PuzzleManager.Instance.OnPuzzleSolved();
    }
}
