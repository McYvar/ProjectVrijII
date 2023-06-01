using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHitable {
    void TakeDamage(float damage);
    void Launch(Vector2 force, float freezeTime);
}

public interface INeedInput {
    InputHandler inputHandler { get; set; }
    int playerId { get; set; }
    void SetPlayerId(int playerId);
    void SetInputHandler(InputHandler newInputHandler);
}