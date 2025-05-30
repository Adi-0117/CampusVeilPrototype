// Billboard.cs
// Rotates a UI element so it always faces the camera.
using UnityEngine;

public class Billboard : MonoBehaviour
{
    Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        // rotate so its forward is always facing the camera
        transform.rotation = Quaternion.LookRotation(transform.position - cam.position, Vector3.up);
    }
}
