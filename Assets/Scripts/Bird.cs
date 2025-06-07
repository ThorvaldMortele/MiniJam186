using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    public float Speed = 2f;
    public Transform LeftBound;
    public Transform RightBound;

    private Vector3 _direction = Vector3.right;

    void Update()
    {
        transform.position += _direction * Speed * Time.deltaTime;

        if (transform.position.x >= RightBound.position.x)
        {
            _direction = Vector3.left;
            FlipSprite(true);
        }
        else if (transform.position.x <= LeftBound.position.x)
        {
            _direction = Vector3.right;
            FlipSprite(false);
        }
    }

    void FlipSprite(bool faceLeft)
    {
        Vector3 localScale = transform.localScale;
        localScale.x = Mathf.Abs(localScale.x) * (faceLeft ? -1 : 1);
        transform.localScale = localScale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball"))
        {
            Rigidbody2D ballRb = collision.GetComponent<Rigidbody2D>();
            if (ballRb != null)
            {
                Vector2 incomingVelocity = ballRb.velocity;
                Vector2 normal = -_direction.normalized; // Reflect based on current travel direction
                Vector2 reflectedVelocity = Vector2.Reflect(incomingVelocity, normal);

                ballRb.velocity = Vector2.zero;
                ballRb.AddForce(reflectedVelocity * 100f); // Adjust force multiplier as needed
            }

            Destroy(gameObject);
        }
    }
}
