using UnityEngine;
using System.Collections;
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
        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Location services not enabled");
            yield break;
        }

        #if PLATFORM_ANDROID
        // Request fine location permission
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            yield return new WaitForSeconds(0.1f);
        }
        #endif

        // Start location service
        Input.location.Start(5f, 10f); // 5m accuracy, 10m update distance

        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // If service didn't initialize in 20 seconds
        if (maxWait <= 0)
        {
            Debug.Log("Location service initialization timed out");
            yield break;
        }

        // If service failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Location service failed");
            yield break;
        }

        isGPSEnabled = true;
        Debug.Log("Location service initialized");
    }

    public bool IsGPSEnabled()
    {
        return isGPSEnabled;
    }

    public Vector2 GetCurrentLocation()
    {
        if (!isGPSEnabled) return Vector2.zero;
        return new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);
    }

    void OnDestroy()
    {
        if (Input.location.isEnabledByUser)
            Input.location.Stop();
    }
} 