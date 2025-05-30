// ARInteractionManager.cs
// Singleton to manage AR interaction lifecycle and persist across scenes.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARInteractionManager : MonoBehaviour
{
    public static ARInteractionManager Instance { get; private set; }

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
}
