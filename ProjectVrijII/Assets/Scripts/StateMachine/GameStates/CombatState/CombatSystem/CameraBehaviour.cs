using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : CombatBase
{
    [SerializeField] private Transform[] focussedObjects; // for now set them in manually
    [SerializeField] private float smoothTime;
    [SerializeField] private Vector2 offset;
    [SerializeField] private Vector2 min;
    [SerializeField] private Vector2 max;

    [Header("By bounds I mean invibisble walls...")]
    [SerializeField] private Transform leftBound;
    [SerializeField] private Transform rightBound;
    private Vector3 smoothVector;

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

            if (obj.position.x < xMin) xMin = obj.position.x;
            if (obj.position.x > xMax) xMax = obj.position.x;
        }
        xAxis /= focussedObjects.Length;
        yAxis /= focussedObjects.Length;

        float invlerpXMin = Mathf.InverseLerp(leftBound.position.x, rightBound.position.x, xMin); // pos most left obj from 0-1
        float invlerpXMax = Mathf.InverseLerp(leftBound.position.x, rightBound.position.x, xMax); // for right from 0-1
        float dist = invlerpXMax - invlerpXMin; // distance between most left and most right, also from 0-1
        float hori = Mathf.Lerp(min.x, max.x, dist);
        float vert = Mathf.Lerp(min.y, max.y, dist);

        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(xAxis, vert + offset.y, hori + offset.x), ref smoothVector, smoothTime);
    }
}
