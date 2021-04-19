using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private int countColliders = 0;

    void Start()
    {
        
    }

    void Update()
    {
        var move = (Input.GetKey(KeyCode.A) ? -1.0f : 0.0f) + (Input.GetKey(KeyCode.D) ? 1.0f : 0.0f);
        move *= 20.0f * Time.deltaTime;

        var jump = (Input.GetKeyDown(KeyCode.W) ? 1.0f : 0.0f);
        jump *= 5.0f;
        jump *= IsGrounded() ? 1.0f : 0.0f;

        var body = GetComponent<Rigidbody>();
        body.velocity += new Vector3(move, jump);
        //body.AddForceAtPosition(new Vector2(move, velocity), Vector2.zero);
    }

    private void OnCollisionEnter(Collision collision)
    {
        countColliders++;
    }

    private void OnCollisionExit(Collision collision)
    {
        countColliders--;
    }

    private bool IsGrounded()
    {
        return countColliders > 0;
    }
}
