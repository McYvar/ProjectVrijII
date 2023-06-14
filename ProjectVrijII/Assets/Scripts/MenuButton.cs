using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    [SerializeField] private UnityEvent onButtonClick;
    [SerializeField] private float floatingDist;
    [SerializeField, Range(0f, 1f)] private float smoothTime;
    [SerializeField] private RawImage selectionIndicator;
    private float velocity;
    private float startPos;
    private bool isFloating;

    private void Start()
    {
        startPos = transform.position.x;
        if (selectionIndicator != null) selectionIndicator.enabled = false;
    }

    private void Update()
    {
        if (isFloating) transform.position = new Vector3(Mathf.SmoothDamp(transform.position.x, startPos + floatingDist, ref velocity, smoothTime), transform.position.y, transform.position.z);
        else transform.position = new Vector3(Mathf.SmoothDamp(transform.position.x, startPos, ref velocity, smoothTime), transform.position.y, transform.position.z);
    }

    public void OnButtonClick()
    {
        onButtonClick.Invoke();
    }

    public void MSG(string input)
    {
        Debug.Log(input);
    }

    public void EnableFloat()
    {
        isFloating = true;
        if (selectionIndicator != null) selectionIndicator.enabled = true;
    }

    public void DisableFloat()
    {
        isFloating = false;
        if (selectionIndicator != null) selectionIndicator.enabled = false;
    }
}
