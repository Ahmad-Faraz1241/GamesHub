using UnityEngine;

public class MovingCube : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float movementLimit = 3f;
    public Vector3 moveDirection;
    public bool isMoving = true;

    private void Update()
    {
        if (isMoving)
        {
            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            if (moveDirection.x != 0)
            {
                if (transform.position.x > movementLimit)
                    moveDirection = Vector3.left;
                else if (transform.position.x < -movementLimit)
                    moveDirection = Vector3.right;
            }
            else if (moveDirection.z != 0)
            {
                if (transform.position.z > movementLimit)
                    moveDirection = Vector3.back;
                else if (transform.position.z < -movementLimit)
                    moveDirection = Vector3.forward;
            }
        }

        if ((Input.GetMouseButtonDown(0) || Input.touchCount > 0) && isMoving)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase != TouchPhase.Began) return;
            StopAndSlice();
        }
    }

    public void StopAndSlice()
    {
        isMoving = false;

        Transform last = SpawnManager.Instance.lastCube;

        bool moveOnX = moveDirection.x != 0;

        float delta;
        float remainingSize;
        float newSize;
        Vector3 newPosition;
        Vector3 slicePosition;
        Vector3 sliceScale;

        if (moveOnX)
        {
            delta = transform.position.x - last.position.x;
            remainingSize = last.localScale.x - Mathf.Abs(delta);

            if (remainingSize <= 0f)
            {
                GameOver();
                return;
            }

            newSize = remainingSize;
            newPosition = transform.position;
            newPosition.x -= delta / 2;

            transform.localScale = new Vector3(newSize, transform.localScale.y, transform.localScale.z);
            transform.position = newPosition;

            sliceScale = new Vector3(Mathf.Abs(delta), transform.localScale.y, transform.localScale.z);
            slicePosition = transform.position;
            slicePosition.x += delta > 0 ? newSize / 2 + sliceScale.x / 2 : -(newSize / 2 + sliceScale.x / 2);
        }
        else
        {
            delta = transform.position.z - last.position.z;
            remainingSize = last.localScale.z - Mathf.Abs(delta);

            if (remainingSize <= 0f)
            {
                GameOver();
                return;
            }

            newSize = remainingSize;
            newPosition = transform.position;
            newPosition.z -= delta / 2;

            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, newSize);
            transform.position = newPosition;

            sliceScale = new Vector3(transform.localScale.x, transform.localScale.y, Mathf.Abs(delta));
            slicePosition = transform.position;
            slicePosition.z += delta > 0 ? newSize / 2 + sliceScale.z / 2 : -(newSize / 2 + sliceScale.z / 2);
        }

        SpawnSlice(slicePosition, sliceScale);
        SpawnManager.Instance.PlaceCube(this);
    }

    private void SpawnSlice(Vector3 pos, Vector3 scale)
    {
        GameObject slice = GameObject.CreatePrimitive(PrimitiveType.Cube);
        slice.transform.localScale = scale;
        slice.transform.position = pos;
        slice.GetComponent<Renderer>().material.color = GetComponent<Renderer>().material.color;

        Rigidbody rb = slice.AddComponent<Rigidbody>();
        Destroy(slice, 2f);


        AudioManager.Instance.PlaySlice();
    }

    private void GameOver()
    {
        isMoving = false;
        AudioManager.Instance.PlayGameOver();
        GameManager2.Instance.GameOver();
    }

}
