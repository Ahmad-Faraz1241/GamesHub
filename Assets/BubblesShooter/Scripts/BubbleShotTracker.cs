using System;
using UnityEngine;

public class BubbleShotTracker : MonoBehaviour
{
    public Action onStopped;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("GridLayer"))
        {
            onStopped?.Invoke();
            Destroy(this); // Remove after firing done
        }
    }
}
