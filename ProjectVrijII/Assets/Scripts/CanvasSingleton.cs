using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasSingleton : MonoBehaviour
{
    private Canvas canvas;

    private static CanvasSingleton instance;
    public static CanvasSingleton Instance {
        get { return instance; }
    }

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        canvas = GetComponent<Canvas>();
    }

    public Vector2 GetCanvasSize() {
        return new Vector2(canvas.pixelRect.width, canvas.pixelRect.height);
    }

    public float GetScaleFactor() {
        return canvas.scaleFactor;
    }

    public Vector2 GetScaledResolution() {
        return GetCanvasSize() * GetScaleFactor();
    }
}
