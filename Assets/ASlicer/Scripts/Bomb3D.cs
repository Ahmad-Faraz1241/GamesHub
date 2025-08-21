using UnityEngine;

public class Bomb3D : FruitSlicer3D
{
    [HideInInspector] public Spawner3D spawner;

    public override void Slice(Vector3 swipeDirection)
    {
        // Play bomb sound
        if (sliceSound != null)
            AudioSource.PlayClipAtPoint(sliceSound, Camera.main.transform.position, sliceVolume);

        // Stop spawning
        if (spawner != null)
            spawner.StopSpawning();

        // Optional: explosion effect here

        // Disable bomb
        gameObject.SetActive(false);
    }
}
