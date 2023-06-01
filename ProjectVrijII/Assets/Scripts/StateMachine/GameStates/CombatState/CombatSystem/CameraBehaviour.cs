using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : CombatBase
{
    private List<Transform> focussedObjects;
    [SerializeField] private float smoothTime;
    [SerializeField] private Vector2 offset;
    [SerializeField] private Vector2 min;
    [SerializeField] private Vector2 max;
    [SerializeField] private float minHeight;

    [Header("By bounds I mean invibisble walls...")]
    [SerializeField] private Transform leftBound;
    [SerializeField] private Transform rightBound;
    private Vector3 smoothVector;

    private Camera thisCam;

    private void Awake() {
        thisCam = GetComponent<Camera>();
    }

    private void Start() {
        focussedObjects = new List<Transform>();
    }

    public override void OnLateUpdate() {
        base.OnLateUpdate();
        if (focussedObjects.Count < 2) {
            return;
        }

        float cameraCentrePointX = 0f;

        float xMin = focussedObjects[0].position.x;
        float xMax = focussedObjects[0].position.x;

        float yMin = focussedObjects[0].position.y;
        float yMax = focussedObjects[0].position.y;

        foreach (var obj in focussedObjects) {
            cameraCentrePointX += obj.position.x;

            if (obj.position.x < xMin) xMin = obj.position.x;
            if (obj.position.x > xMax) xMax = obj.position.x;
            if (obj.position.y < yMin) yMin = obj.position.y;
            if (obj.position.y > yMax) yMax = obj.position.y;
        }
        cameraCentrePointX /= focussedObjects.Count;

        float invlerpXMin = Mathf.InverseLerp(leftBound.position.x, rightBound.position.x, xMin); // pos most left obj from 0-1
        float invlerpXMax = Mathf.InverseLerp(leftBound.position.x, rightBound.position.x, xMax); // for right from 0-1
        float horiDist = invlerpXMax - invlerpXMin; // distance between most left and most right, also from 0-1

        float hori = Mathf.Lerp(min.x, max.x, horiDist);
        float vert = Mathf.Lerp(min.y, max.y, horiDist);

        float vertDist = yMax - yMin;
        Vector3 topOfScreenPoint = thisCam.ScreenToWorldPoint(new Vector3(transform.position.x, Screen.height, focussedObjects[0].position.z - transform.position.z));
        //Debug.Log(topOfScreenPoint);
        Debug.DrawLine(Vector3.zero, topOfScreenPoint);
        if (vertDist > topOfScreenPoint.y - minHeight) vert += vertDist - (topOfScreenPoint.y - minHeight);


        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(cameraCentrePointX, vert + offset.y, hori + offset.x), ref smoothVector, smoothTime);
    }

    public void AssignObjects(Transform transform) {
        focussedObjects.Add(transform);
    }
}
