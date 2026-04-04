using UnityEngine;

public interface IPileQuery
{
    CorpsePile.PileState GetStateAtPosition(Vector3 position);
    float GetSlowMultiplierAtPosition(Vector3 position);
}
