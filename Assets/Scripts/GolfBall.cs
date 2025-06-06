using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfBall : MonoBehaviour
{
    private Rigidbody2D _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Shoot(Vector2 direction, float force) 
    {
        _rb.AddForce(direction * force, ForceMode2D.Impulse);
    }
}
