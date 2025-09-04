using UnityEngine;

public class Bomb3D : FruitSlicer3D
{
    [HideInInspector] public Spawner3D spawner;

    public override void Slice(Vector3 swipeDirection)
    {
        // Prevent multiple slices
        if (!gameObject.activeInHierarchy) return;

        // Play slice sound if any
        if (sliceSound != null)
            AudioSource.PlayClipAtPoint(sliceSound, Camera.main.transform.position, sliceVolume);

        // Stop spawner
        if (spawner != null)
            spawner.StopSpawning();

        // Notify GameManager
        GameManager3D gm = FindObjectOfType<GameManager3D>();
        if (gm != null)
            gm.OnBombSliced();

        // Deactivate bomb for pooling
        gameObject.SetActive(false);
    }
}
