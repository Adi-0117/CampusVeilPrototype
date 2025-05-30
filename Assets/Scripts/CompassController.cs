// CompassController.cs
// Rotates a UI arrow and updates distance text to point toward a designated target.
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompassController : MonoBehaviour
{
    public RectTransform arrow;
    public TMP_Text distanceText;

    [HideInInspector]
    public Transform target;
    private Transform playerCam;

    void Start()
    {
        playerCam = Camera.main.transform;
    }

    void Update()
    {
        if (target == null)
            return;

        Vector3 dir = target.position - playerCam.position;
        dir.y = 0;
        float angle = Vector3.SignedAngle(playerCam.forward, dir, Vector3.up);
        arrow.localEulerAngles = new Vector3(0, 0, -angle);

        float dist = dir.magnitude;
        distanceText.text = $"{dist:0.#} m";
    }
}
