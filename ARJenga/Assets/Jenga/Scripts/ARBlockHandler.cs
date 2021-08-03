using System.Collections;
using System.Collections.Generic;
using UnityEngine;

////////////////////////////////////////////////////////////////////////////////
/// <summary>
/// 
/// Class to manage Jenga blocks using a weapon moved by an AR marker.
///
/// </summary>
////////////////////////////////////////////////////////////////////////////////

public class ARBlockHandler : MonoBehaviour
{
    //== Members ===============================================================

    public JengaStateMachine stateMachine;

    //== Properties ============================================================

    public GameObject prefabBall;

    private GameObject currentBall;

    private LineRenderer laserLine;

    private bool shotBall;

    //== Methods ===============================================================

    // ---- Unity events ----

    void Start()
    {
        shotBall = false;
        laserLine = GetComponent<LineRenderer>();
    }

    void Update()
    {
        Vector3 rayOrigin = transform.position;

        laserLine.SetPosition(0, rayOrigin);

        // When there is no ball fired.

        if (!shotBall)
        {
            RaycastHit hit;

            if (Physics.Raycast(rayOrigin, transform.forward * 20, out hit))
            {
                // If the ray collides with the tower, the laser won't pass through it.

                laserLine.SetPosition(1, hit.point);

                // The ball is fired when pressing 'A'.

                if (Input.GetKeyDown(KeyCode.A))
                {
                    Debug.Log(shotBall);
                    shotBall = true;
                    fire(hit);
                }
            }
            else
            {
                laserLine.SetPosition(1, rayOrigin + transform.forward * 20);
            }
        }
        else
        {
            // The laser points the shot ball.

            laserLine.SetPosition(1, currentBall.transform.position);

            // The ball is deleted when pressing 'D'.

            if (Input.GetKeyDown(KeyCode.D))
            {
                shotBall = false;
                laserLine.material.color = Color.red;
                Destroy(currentBall);
                if (stateMachine != null)
                    stateMachine.releaseBlock();
            }

            // Pull the ball when pressing 'S'.

            if (Input.GetKey(KeyCode.S))
            {
                currentBall.transform.position += Vector3.Normalize(rayOrigin - currentBall.transform.position) * 0.002f;
            }
        }
    }

    // ---- Behaviour of handler with blocks ----

    void fire(RaycastHit hit)
    {
        // Create a new ball.

        currentBall = Instantiate(prefabBall);
        currentBall.transform.position = hit.point;

        // Make a joint between the ball and the block.

        currentBall.GetComponent<SpringJoint>().connectedBody = hit.collider.GetComponent<Rigidbody>();
        currentBall.GetComponent<SpringJoint>().anchor = new Vector3(0.0f, 0.0f, 0.0f);
        currentBall.GetComponent<SpringJoint>().connectedAnchor = hit.transform.InverseTransformPoint(hit.point);

        // The laser now is green.

        laserLine.material.color = Color.green;

        // Notify the state machine that a block has been grabbed.

        if (stateMachine != null)
            stateMachine.setBlockGrabbed(hit.rigidbody);
    }
}
