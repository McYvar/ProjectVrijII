using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : CombatBase
{
    [SerializeField] private Transform[] focussedObjects; // for now set them in manually
    [SerializeField] private float smoothTime;
    [SerializeField] private Vector2 offset;
    private Vector3 SmoothVector;
    
    public override void OnUpdate() {
        base.OnUpdate();
        if (focussedObjects.Length < 2) {
            Debug.Log("No objects found!");
            return;
        }

        float xAxis = 0f;
        float leftBound = focussedObjects[0].position.x;
        float rightBound = focussedObjects[0].position.x;

        foreach (var obj in focussedObjects) {
            xAxis += obj.position.x;
            if (transform.position.x > rightBound) rightBound = transform.position.x;
            if (transform.position.x < leftBound) leftBound = transform.position.x;
        }
        xAxis /= focussedObjects.Length;
        float boundDistance = rightBound - leftBound;
        Debug.Log(boundDistance);


        transform.position = Vector3.SmoothDamp(
            transform.position,
            new Vector3(
            xAxis,
            boundDistance + offset.x,
            -boundDistance - offset.y),
            ref SmoothVector,
            smoothTime);
    }
}
