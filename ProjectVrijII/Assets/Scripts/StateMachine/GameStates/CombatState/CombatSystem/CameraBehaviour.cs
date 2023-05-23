using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : CombatBase
{
    [SerializeField] private Transform[] focussedObjects; // for now set them in manually
    [SerializeField] private float smoothTime;
    [SerializeField] private Vector2 offset;
    [SerializeField] private Vector2 scalar;
    [SerializeField] private Vector2 minimals;

    [Header("By bounds I mean invibisble walls...")]
    [SerializeField] private Transform leftBound;
    [SerializeField] private Transform rightBound;
    private Vector3 SmoothVector;

    private float centreX, distX;

    private void Start()
    {
        centreX = (leftBound.position.x + rightBound.position.x) / 2;
        distX = rightBound.position.x - leftBound.position.x;
    }

    public override void OnUpdate() {
        base.OnUpdate();
        if (focussedObjects.Length < 2) {
            Debug.Log("No objects found!");
            return;
        }

        float xAxis = 0f;
        float yAxis = 0f;

        float xMin = focussedObjects[0].position.x;
        float xMax = focussedObjects[0].position.x;

        foreach (var obj in focussedObjects) {
            xAxis += obj.position.x;
            yAxis += obj.position.y;
        }
        xAxis /= focussedObjects.Length;
        yAxis /= focussedObjects.Length;

        float invLerpX = Mathf.InverseLerp(leftBound.position.x, rightBound.position.x, xAxis);

        float hori = Mathf.Lerp(leftBound.position.x, rightBound.position.x, invLerpX) + offset.x;
        float vert = 0;

        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(hori, vert, 0), ref SmoothVector, smoothTime);
    }
}
