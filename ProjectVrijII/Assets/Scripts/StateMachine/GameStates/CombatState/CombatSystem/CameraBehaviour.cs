using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : CombatBase
{
    [SerializeField] private Transform[] focussedObjects; // for now set them in manually
    [SerializeField] private float smoothTime;
    [SerializeField] private Vector2 offset;
    [SerializeField] private Vector2 scalar;
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

        float upperBound = focussedObjects[0].position.y;
        float lowerBound = focussedObjects[0].position.y;

        foreach (var obj in focussedObjects) {
            xAxis += obj.position.x;
            if (obj.transform.position.x > rightBound) rightBound = obj.transform.position.x;
            if (obj.transform.position.x < leftBound) leftBound = obj.transform.position.x;

            if (obj.transform.position.y > upperBound) upperBound = obj.transform.position.y;
            if (obj.transform.position.y < lowerBound) lowerBound = obj.transform.position.y;
        }
        xAxis /= focussedObjects.Length;
        float horizontalBoundDistance = rightBound - leftBound;
        float verticalBoundDistance = upperBound - lowerBound;


        transform.position = Vector3.SmoothDamp(
            transform.position,
            new Vector3(
            xAxis,
            (verticalBoundDistance * scalar.x) - offset.x,
            (-horizontalBoundDistance * scalar.y) - offset.y),
            ref SmoothVector,
            smoothTime);
    }
}
