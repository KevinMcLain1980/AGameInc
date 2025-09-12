using UnityEngine;

public enum MovementState
{
    Idle = 0,
    Walking = 1,
    Sprinting = 2,
    Crouching = 3,
    Sliding = 4,
    Climbing = 5,
    Jumping = 6
}

public class PlayerStateManager : MonoBehaviour
{
    public MovementState currentState { get; private set; }

    /// <summary>
    /// Sets the current movement state and optionally logs the change.
    /// </summary>
    public void SetState(MovementState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
            // Optional: Debug.Log($"Movement state changed to: {currentState}");
        }
    }

    /// <summary>
    /// Checks if the player is currently in the specified state.
    /// </summary>
    public bool Is(MovementState state)
    {
        return currentState == state;
    }
}
