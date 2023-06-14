using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    [SerializeField] private UnityEvent onButtonClick;
    [SerializeField] private float floatingDist;
    [SerializeField, Range(0f, 1f)] private float smoothTime;
    private float velocity;
    private float startPos;
    private bool isFloating;

    [SerializeField] private RawImage selectionIndicator;
    [SerializeField, Range(0f, 0.5f)] private float delayTime;
    private float rawImageVelocity;
    private float rawImageStartPos;
    private bool rawImageFloating;

    [SerializeField] private FModEventCaller caller;

    private void Start()
    {
        startPos = transform.position.x;

        if (selectionIndicator != null) rawImageStartPos = selectionIndicator.transform.position.x;
    }

    private void Update()
    {
        if (isFloating) transform.position = new Vector3(Mathf.SmoothDamp(transform.position.x, startPos + floatingDist, ref velocity, smoothTime), transform.position.y, transform.position.z);
        else transform.position = new Vector3(Mathf.SmoothDamp(transform.position.x, startPos, ref velocity, smoothTime), transform.position.y, transform.position.z);

        if (selectionIndicator == null) return;
        if (rawImageFloating) selectionIndicator.transform.position = new Vector3(Mathf.SmoothDamp(selectionIndicator.transform.position.x, rawImageStartPos + floatingDist, ref rawImageVelocity, smoothTime), selectionIndicator.transform.position.y, selectionIndicator.transform.position.z);
        else selectionIndicator.transform.position = new Vector3(Mathf.SmoothDamp(selectionIndicator.transform.position.x, rawImageStartPos, ref rawImageVelocity, smoothTime), selectionIndicator.transform.position.y, selectionIndicator.transform.position.z);

        float alpha = Mathf.InverseLerp(rawImageStartPos, rawImageStartPos + floatingDist, selectionIndicator.transform.position.x);
        selectionIndicator.color = new Color(selectionIndicator.color.r, selectionIndicator.color.g, selectionIndicator.color.b, alpha);
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
        caller.PlayFMODEvent("event:/SfxSwitch");
        StartCoroutine(EnableRawFloat());
    }

    public void DisableFloat()
    {
        isFloating = false;
        rawImageFloating = false;
    }

    private IEnumerator EnableRawFloat()
    {
        yield return new WaitForSeconds(delayTime);
        rawImageFloating = true;
    }

    private IEnumerator DisableRawFloat()
    {
        yield return new WaitForSeconds(delayTime);
    }
}
