using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private float speed;
    private float yRotate;
    private bool enableMove;
    private bool enableRotate;
    // Update is called once per frame
    void Update()
    {
        if (enableMove)
        {
            MoveManager();
        }
        if (enableRotate)
        {
            RotateManager();
        }
    }

    private void MoveManager()
    {
        Vector3 moveVector = new Vector3();

        if (Input.GetKey(KeyCode.W))
        {
            moveVector += transform.forward;
        }

        if (Input.GetKey(KeyCode.S))
        {
            moveVector -= transform.forward;
        }

        if (Input.GetKey(KeyCode.D))
        {
            moveVector += transform.right;
        }

        if (Input.GetKey(KeyCode.A))
        {
            moveVector -= transform.right;
        }

        moveVector.Normalize();

        moveVector *= speed;

        playerRigidbody.velocity = new Vector3(moveVector.x, playerRigidbody.velocity.y, moveVector.z);
    }

    private void RotateManager()
    {
        transform.localEulerAngles += Vector3.up * Input.GetAxis("Mouse X");
        yRotate += Input.GetAxis("Mouse Y");
        yRotate = Mathf.Max(yRotate, -90);
        yRotate = Mathf.Min(yRotate, 90);
        cameraPivot.localEulerAngles = Vector3.left * yRotate;
    }

    public void SetMovable(bool enableMove, bool enableRotate)
    {
        this.enableMove = enableMove;
        this.enableRotate = enableRotate;
        if (!enableMove)
        {
            playerRigidbody.velocity = new Vector3(0, playerRigidbody.velocity.y, 0);
        }
    }
}
