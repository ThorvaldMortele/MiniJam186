using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfBall : MonoBehaviour
{
    private Rigidbody2D _rb;

    public LayerMask GroundLayer; // Assign to "Ground" layer in Inspector
    public float GroundCheckRadius = 0.1f;
    public Sprite BallTexture;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        GetComponent<SpriteRenderer>().sprite = BallTexture;
    }

    public void Shoot(Vector2 direction, float force) 
    {
        _rb.AddForce(direction * force, ForceMode2D.Impulse);
    }

    public bool IsBallGrounded()
    {
        Vector3 groundCheckPos = transform.position + Vector3.up * -0.2871468f; //manual offset so it doesnt rotate with
        bool isGrounded = Physics2D.OverlapCircle(groundCheckPos, GroundCheckRadius, GroundLayer);

        // Optional: Debug draw the circle in Scene view
        Debug.DrawRay(groundCheckPos, Vector3.right * GroundCheckRadius, isGrounded ? Color.green : Color.red);
        Debug.DrawRay(groundCheckPos, Vector3.left * GroundCheckRadius, isGrounded ? Color.green : Color.red);

        return isGrounded;
    }
}
