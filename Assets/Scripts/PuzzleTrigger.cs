using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzleTrigger : MonoBehaviour, IPointerClickHandler
{
    public PuzzleData puzzleData;

    public void OnPointerClick(PointerEventData e) {
        PuzzleManager.Instance.StartPuzzle(puzzleData);
    }
}
