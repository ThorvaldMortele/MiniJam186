using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfBall : MonoBehaviour
{
    private Rigidbody2D _rb;

    public LayerMask GroundLayer; // Assign to "Ground" layer in Inspector
    public float GroundCheckRadius = 0.1f;
    public Sprite BallTexture;

    public PhysicsMaterial2D GrassMaterial;
    public PhysicsMaterial2D SandMaterial;
    public PhysicsMaterial2D MudMaterial;

    public float RaycastDistance = 1f;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        GetComponent<SpriteRenderer>().sprite = BallTexture;
    }

    public void Shoot(Vector2 direction, float force) 
    {
        _rb.AddForce(direction * force, ForceMode2D.Impulse);
        _rb.angularVelocity = -720f;
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

    private void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, RaycastDistance, GroundLayer);

        if (hit.collider != null)
        {
            string tag = hit.collider.tag;

            if (tag == "Mud")
            {
                _rb.sharedMaterial = MudMaterial;
            }
            else if (tag == "Sand")
            {
                _rb.sharedMaterial = SandMaterial;
            }
            else
            {
                _rb.sharedMaterial = GrassMaterial;
            }
        }
    }
}
