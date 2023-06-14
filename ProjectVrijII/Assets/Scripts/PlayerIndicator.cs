using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerIndicator : MonoBehaviour
{
    [SerializeField] float indicatorMoveSpeed;
    private Image indicatorImage;
    private bool isHidden = false;

    private int playerId;
    private InputHandler inputHandler;

    private void Awake() {
        indicatorImage = GetComponent<Image>();
    }

    private void OnDisable()
    {
        if (inputHandler != null)
        {

        }
    }

    private void Update() {
        if (isHidden || inputHandler == null) return;
        float scalar = CanvasSingleton.Instance.GetScaleFactor();
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, indicatorImage.rectTransform.sizeDelta.x * scalar / 2, Screen.width - indicatorImage.rectTransform.sizeDelta.x * scalar / 2),
            Mathf.Clamp(transform.position.y, indicatorImage.rectTransform.sizeDelta.y * scalar / 2, Screen.height - indicatorImage.rectTransform.sizeDelta.y * scalar / 2),
            transform.position.z
            );

        //MoveIndicator();
    }

    private void MoveIndicator() {
        transform.position += new Vector3(inputHandler.leftDirection.x, inputHandler.leftDirection.y) * (CanvasSingleton.Instance.GetScaleFactor() * indicatorMoveSpeed);
    }

    public void InitializeIndicator(InputHandler inputHandler, int playerId, Color color) {
        this.playerId = playerId;
        this.inputHandler = inputHandler;
        indicatorImage.color = color;
    }

    public void HideIndicator() {
        isHidden = false;
        indicatorImage.enabled = false;
    }

    public void ShowIndicator() {
        isHidden = true;
        indicatorImage.enabled = true;
    }

    // the pivot is on the lower right so when screen casting we should calculate the centre point first
    public Vector2 GetCentre() {
        return new Vector2(transform.position.x, transform.position.y);
    }

    public void SetInputHandler(InputHandler inputHandler) {
        this.inputHandler = inputHandler;
    }

    public int GetPlayerId() {
        return playerId;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(GetCentre(), 10);
    }

    public void SubscribeToConfirm(Action callback) {
        inputHandler.southFirst += callback;
    }

    public void UnsubscribeFromConfirm(Action callback) {
        inputHandler.southFirst -= callback;
    }


    public void SubscribeToUp(Action callback)
    {
        inputHandler.UpFirst += callback;
    }
    public void UnsubscribeFromUp(Action callback)
    {
        inputHandler.UpFirst -= callback;
    }
    public void SubscribeToDown(Action callback)
    {
        inputHandler.DownFirst += callback;
    }
    public void UnsubscribeFromDown(Action callback)
    {
        inputHandler.DownFirst -= callback;
    }
}
 