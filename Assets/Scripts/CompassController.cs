using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CompassController : MonoBehaviour
{
    [Tooltip("The UI arrow that will rotate")]
    public RectTransform arrow;
    [Tooltip("UI Text to display distance")]
    public TMP_Text distanceText;
    [Tooltip("Transform of the target, e.g. the current quest marker")]
    [HideInInspector] public Transform target;
    private Transform playerCam;

    void Start()
    {
        playerCam = Camera.main.transform;
    }

    void Update()
    {
        if (target == null) return;

        // 1) Rotate arrow to face target horizontally
        Vector3 dir = (target.position - playerCam.position);
        dir.y = 0; // ignore vertical
        float angle = Vector3.SignedAngle(playerCam.forward, dir, Vector3.up);
        arrow.localEulerAngles = new Vector3(0, 0, -angle);

        // 2) Update distance text
        float dist = dir.magnitude;
        distanceText.text = $"{dist:0.#} m";
    }
}
