// LocationManager.cs
// Singleton that starts and manages the device’s GPS location service.
using System.Collections;
using UnityEngine;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

public class LocationManager : MonoBehaviour
{
    public static LocationManager Instance { get; private set; }
    private bool isGPSEnabled = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        StartCoroutine(StartLocationService());
    }

    IEnumerator StartLocationService()
    {
        if (!Input.location.isEnabledByUser)
            yield break;

#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            yield return new WaitForSeconds(0.1f);
        }
#endif

        Input.location.Start(5f, 10f);
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait-- > 0)
            yield return new WaitForSeconds(1);

        if (maxWait <= 0 || Input.location.status == LocationServiceStatus.Failed)
            yield break;

        isGPSEnabled = true;
        Debug.Log("Location service initialized");
    }

    public bool IsGPSEnabled() => isGPSEnabled;

    public Vector2 GetCurrentLocation()
    {
        if (!isGPSEnabled)
            return Vector2.zero;
        return new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);
    }

    void OnDestroy()
    {
        if (Input.location.isEnabledByUser)
            Input.location.Stop();
    }
}
