using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : CombatBase
{
    [SerializeField] private List<Transform> focussedObjects; // for now set them in manually 
    [SerializeField] private float smoothTime;
    [SerializeField] private Vector2 offset;
    [SerializeField] private Vector2 min;
    [SerializeField] private Vector2 max;

    [Header("By bounds I mean invibisble walls...")]
    [SerializeField] private Transform leftBound;
    [SerializeField] private Transform rightBound;
    private Vector3 smoothVector;

    private void Start() {
        focussedObjects = new List<Transform>();
    }

    public override void OnUpdate() {
        base.OnUpdate();
        if (focussedObjects.Count < 2) {
            return;
        }

        float cameraCentrePointX = 0f;

        float xMin = focussedObjects[0].position.x;
        float xMax = focussedObjects[0].position.x;

        foreach (var obj in focussedObjects) {
            cameraCentrePointX += obj.position.x;

            if (obj.position.x < xMin) xMin = obj.position.x;
            if (obj.position.x > xMax) xMax = obj.position.x;
        }
        cameraCentrePointX /= focussedObjects.Count;

        float invlerpXMin = Mathf.InverseLerp(leftBound.position.x, rightBound.position.x, xMin); // pos most left obj from 0-1
        float invlerpXMax = Mathf.InverseLerp(leftBound.position.x, rightBound.position.x, xMax); // for right from 0-1
        float dist = invlerpXMax - invlerpXMin; // distance between most left and most right, also from 0-1
        float hori = Mathf.Lerp(min.x, max.x, dist);
        float vert = Mathf.Lerp(min.y, max.y, dist);

        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(cameraCentrePointX, vert + offset.y, hori + offset.x), ref smoothVector, smoothTime);
    }

    public void AssignObjects(Transform transform) {
        focussedObjects.Add(transform);
    }
}
