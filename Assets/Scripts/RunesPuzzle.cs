using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RunesPuzzle : MonoBehaviour, IPointerClickHandler
{
    [Tooltip("Image component that shows the glowing state")]
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

        // check siblings
        foreach (Transform sibling in transform.parent)
        {
            var rp = sibling.GetComponent<RunesPuzzle>();
            if (rp != null && !rp.isGlowing) return;
        }

        // all runes glowing
        PuzzleManager.Instance.OnPuzzleSolved();
    }
}
