using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MenuButton : MonoBehaviour
{
    [SerializeField] private UnityEvent onButtonClick;

    public void OnButtonClick() {
        onButtonClick.Invoke();
    }

    public void MSG(string input) {
        Debug.Log(input);
    }
}
