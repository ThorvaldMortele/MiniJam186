using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class BallCameraZoom : MonoBehaviour
{
    public CinemachineVirtualCamera VirtualCam;
    public Rigidbody2D Ball;
    public float ZoomOutSize = 10f;
    public float ZoomInSize = 5f;
    public float ZoomSpeed = 2f;
    public float GroundCheckDistance = 1f;
    public LayerMask GroundLayer;

    private bool _isAirborne = false;

    private void Update()
    {
        if (Ball == null || VirtualCam == null) return;

        bool grounded = Physics2D.Raycast(Ball.position, Vector2.down, GroundCheckDistance, GroundLayer);

        // Ball is going up
        if (Ball.velocity.y > 0.5f && !grounded)
        {
            _isAirborne = true;
        }

        // Ball is falling and close to the ground
        if (_isAirborne && Ball.velocity.y < -0.5f && grounded)
        {
            _isAirborne = false;
        }

        float targetSize = _isAirborne ? ZoomOutSize : ZoomInSize;
        float currentSize = VirtualCam.m_Lens.OrthographicSize;
        VirtualCam.m_Lens.OrthographicSize = Mathf.Lerp(currentSize, targetSize, Time.deltaTime * ZoomSpeed);
    }
}
