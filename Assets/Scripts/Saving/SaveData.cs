using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SaveData
{
    public float bestHeight = 0.0f;
    public List<Vector2> platforms = new();
    public Vector2 playerPosition = Vector2.zero, playerVelocity = Vector2.zero;
}