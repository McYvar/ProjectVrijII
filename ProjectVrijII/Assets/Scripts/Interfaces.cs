using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHitable {
    void OnHit(Vector2 force, float freezeTime);
}

public interface INeedInput {
    InputHandler inputHandler { get; set; }
    void SetInputHandler(InputHandler newInputHandler);
}