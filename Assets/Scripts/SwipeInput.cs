using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class SwipeInput : MonoBehaviour
{
    public static Vector2 SwipeDirection { get; private set; }
    public event Action<Vector2> OnMove;

    private Vector2 startTouch;
    private bool isSwiping;

    public float minSwipeDistance = 50f;

    void Update()
    {
        SwipeDirection = Vector2.zero;

        if (Touchscreen.current == null)
            return;

        if (Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            startTouch = Touchscreen.current.primaryTouch.position.ReadValue();
            isSwiping = true;
        }

        if (isSwiping)
        {
            Vector2 endTouch = Touchscreen.current.primaryTouch.position.ReadValue();

            Vector2 delta = endTouch - startTouch;

            if (delta.magnitude > minSwipeDistance)
            {
                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                {
                    SwipeDirection = delta.x > 0 ? Vector2.right : Vector2.left;
                }
                else
                {
                    SwipeDirection = delta.y > 0 ? Vector2.up : Vector2.down;
                }

                OnMove?.Invoke(SwipeDirection);
                isSwiping = false;
            }

            if (Touchscreen.current.primaryTouch.press.wasReleasedThisFrame)
                isSwiping = false;
        }
    }
}