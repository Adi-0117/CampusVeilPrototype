using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TriggerZone
{
    public string zoneID;
    public Vector3 simulatedPosition; // use for editor/testing
    public float radius = 5f; // trigger radius in meters
}

public class TriggerZoneManager : MonoBehaviour
{
    public static TriggerZoneManager Instance { get; private set; }

    [SerializeField]
    private List<TriggerZone> zones = new List<TriggerZone>();
    public event Action<string> OnZoneEntered;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        TriggerZoneManager.Instance.OnZoneEntered += zoneID =>
        {
            // Optional: only complete if zoneID matches current questID
            if (QuestStoryManager.Instance.GetCurrentQuest()?.questID == zoneID)
                QuestStoryManager.Instance.CompleteCurrentQuest();
        };
    }

    private void Update()
    {
        Vector3 playerPos = GetPlayerPosition();
        foreach (var zone in zones)
        {
            if (Vector3.Distance(playerPos, zone.simulatedPosition) <= zone.radius)
            {
                OnZoneEntered?.Invoke(zone.zoneID);
                // Optionally remove or disable to prevent repeated triggers
            }
        }
    }

    private Vector3 GetPlayerPosition()
    {
        // For now, use the main camera’s transform as the player stand-in
        return Camera.main.transform.position;
    }
}
