using UnityEngine;

public class FruitRecycle : MonoBehaviour
{
    [HideInInspector] public float despawnY = -7f;

    void Update()
    {
        if (transform.position.y < despawnY)
            gameObject.SetActive(false);
    }
}
