using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticFmodCaller : MonoBehaviour
{
    [SerializeField] private FModEventCaller caller;

    public static FModEventCaller staticCaller;

    private void Awake()
    {
        staticCaller = caller;
    }
}
