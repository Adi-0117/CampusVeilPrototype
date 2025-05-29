using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MistPuzzle : MonoBehaviour, IPointerClickHandler
{
    [Tooltip("How many taps to clear")]
    public int tapsToClear = 10;
    private int currentTaps;

    // If your mist is a UI Image, drag it here; else you can fade the entire panel
    public Image mistImage;

    void Start()
    {
        currentTaps = 0;
    }

    public void OnPointerClick(PointerEventData e)
    {
        currentTaps++;
        float alpha = Mathf.Lerp(1f, 0f, (float)currentTaps / tapsToClear);
        if (mistImage != null) mistImage.color = new Color(1,1,1, alpha);

        if (currentTaps >= tapsToClear)
        {
            PuzzleManager.Instance.OnPuzzleSolved();
        }
    }
} 