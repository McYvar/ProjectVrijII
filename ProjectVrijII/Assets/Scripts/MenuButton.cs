using UnityEngine;
using UnityEngine.Events;

public class MenuButton : MonoBehaviour
{
    [SerializeField] private UnityEvent onButtonClick;
    [SerializeField] private float floatingDist;
    [SerializeField, Range(0f, 1f)] private float smoothTime;
    private float velocity;
    private float startPos;
    private bool isFloating;

    private void Start()
    {
        startPos = transform.position.x;
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
    }

    public void DisableFloat()
    {
        isFloating = false;
    }
}
