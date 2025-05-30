// RunesPuzzle.cs
// Rune-activation puzzle that completes when all runes have been clicked/glow.
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RunesPuzzle : MonoBehaviour, IPointerClickHandler
{
    public Image runeImage;
    public Sprite defaultSprite;
    public Sprite glowingSprite;
    private bool isGlowing;

    void Start()
    {
        isGlowing = false;
    }

    public void OnPointerClick(PointerEventData e)
    {
        isGlowing = true;
        runeImage.sprite = glowingSprite;

        foreach (Transform sibling in transform.parent)
        {
            var rp = sibling.GetComponent<RunesPuzzle>();
            if (rp != null && !rp.isGlowing)
                return;
        }

        PuzzleManager.Instance.OnPuzzleSolved();
    }
}
