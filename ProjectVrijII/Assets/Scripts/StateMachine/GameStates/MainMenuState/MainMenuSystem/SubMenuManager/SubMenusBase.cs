using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SubMenusBase : BaseState
{
    /// <summary>
    /// By: Yvar, Date: 6/1/2023
    /// Class that describes the main menu behaviour. Only controller by player 1.
    /// </summary>
    [SerializeField] private Canvas parentCanvas;
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject playerIndicator;

    [SerializeField] private Color highlightColor;
    [SerializeField] private Color unhighlightColor;

    private static List<PlayerIndicator> playerIndicators = new List<PlayerIndicator>();
    [SerializeField] TMPro.TMP_Text playerCountText;
    [SerializeField] private Image[] buttons;

    private MenuButton selectedButton = null;

    private bool activeState = false;

    private int selected = 0;
    private int selectedMax;

    public override void OnEnter()
    {
        selectedMax = buttons.Length;

        menu.SetActive(true);
        activeState = true;

        foreach (var indicator in playerIndicators)
        {
            indicator.SubscribeToConfirm(ConfirmChoice);
            indicator.SubscribeToUp(GoDown);
            indicator.SubscribeToDown(GoUp);
        }
        if (selectedButton == null) ButtonsCheck();
    }

    public override void OnExit()
    {
        StopAllCoroutines();
        menu.SetActive(false);
        activeState = false;

        foreach (var indicator in playerIndicators)
        {
            indicator.UnsubscribeFromConfirm(ConfirmChoice);
            indicator.UnsubscribeFromUp(GoDown);
            indicator.UnsubscribeFromDown(GoUp);
        }
    }

    public override void OnUpdate()
    {
        ReceiveInputHandler();
        //ButtonsCheck();
    }

    private void ReceiveInputHandler()
    {
        int playerCount = PlayerDistribution.Instance.GetAssignedPlayersCount();
        playerCountText.text = $"Player count: {playerCount}";
        int indicatorCount = playerIndicators.Count;
        if (playerCount > indicatorCount)
        {
            for (int i = indicatorCount; i < playerCount; i++)
            {
                GameObject newIndicatorObj = Instantiate(playerIndicator, parentCanvas.transform);
                PlayerIndicator newIndicator = newIndicatorObj.GetComponent<PlayerIndicator>();
                newIndicator.InitializeIndicator(PlayerDistribution.Instance.GetPlayerInputHandler(i), i, PlayerDistribution.Instance.GetPlayerColor(i));
                newIndicator.SubscribeToConfirm(ConfirmChoice);
                newIndicator.SubscribeToUp(GoDown);
                newIndicator.SubscribeToDown(GoUp);
                playerIndicators.Add(newIndicator);
            }
        }
    }

    private void ButtonsCheck()
    {
        selectedButton?.DisableFloat();
        selectedButton = buttons[selected].GetComponent<MenuButton>(); ;
        selectedButton?.EnableFloat();
    }

    private Vector2 GetBottomLeftCorner(Image image)
    {
        return new Vector2(image.rectTransform.position.x - (image.rectTransform.sizeDelta.x * CanvasSingleton.Instance.GetScaleFactor() / 2),
                           image.rectTransform.position.y - (image.rectTransform.sizeDelta.y * CanvasSingleton.Instance.GetScaleFactor() / 2));
    }
    private Vector2 GetTopRightCorner(Image image)
    {
        return new Vector2(image.rectTransform.position.x + (image.rectTransform.sizeDelta.x * CanvasSingleton.Instance.GetScaleFactor() / 2),
                           image.rectTransform.position.y + (image.rectTransform.sizeDelta.y * CanvasSingleton.Instance.GetScaleFactor() / 2));
    }
    private Vector2 GetTopLeftCorner(Image image)
    {
        return new Vector2(image.rectTransform.position.x - (image.rectTransform.sizeDelta.x * CanvasSingleton.Instance.GetScaleFactor() / 2),
                           image.rectTransform.position.y + (image.rectTransform.sizeDelta.y * CanvasSingleton.Instance.GetScaleFactor() / 2));
    }
    private Vector2 GetBottomRightCorner(Image image)
    {
        return new Vector2(image.rectTransform.position.x + (image.rectTransform.sizeDelta.x * CanvasSingleton.Instance.GetScaleFactor() / 2),
                           image.rectTransform.position.y - (image.rectTransform.sizeDelta.y * CanvasSingleton.Instance.GetScaleFactor() / 2));
    }

    private bool CheckWithinBounds(PlayerIndicator indicator, Image button)
    {
        Vector2 position = indicator.GetCentre();
        Vector2 targetBottomLeftCorner = GetBottomLeftCorner(button);
        Vector2 targetTopRightCorner = GetTopRightCorner(button);
        return (
            position.x > targetBottomLeftCorner.x && position.y > targetBottomLeftCorner.y &&
            position.x < targetTopRightCorner.x && position.y < targetTopRightCorner.y
            );
    }

    private void ConfirmChoice()
    {
        selectedButton.OnButtonClick();
    }

    public override void OnFixedUpdate()
    {
    }

    public override void OnLateUpdate()
    {
    }

    private void GoUp()
    {
        selected++;
        if (selected == selectedMax) selected = 0;
        ButtonsCheck();
    }

    private void GoDown()
    {
        selected--;
        if (selected == -1) selected = selectedMax - 1;
        ButtonsCheck();
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (EditorApplication.isPlaying && activeState)
        {
            Gizmos.color = Color.red;
            foreach (var button in buttons)
            {
                Gizmos.DrawLine(GetBottomLeftCorner(button), GetTopLeftCorner(button));
                Gizmos.DrawLine(GetBottomLeftCorner(button), GetBottomRightCorner(button));
                Gizmos.DrawLine(GetTopRightCorner(button), GetTopLeftCorner(button));
                Gizmos.DrawLine(GetTopRightCorner(button), GetBottomRightCorner(button));
            }
        }
#endif
    }
}




