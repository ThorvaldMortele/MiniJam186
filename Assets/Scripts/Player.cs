using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GolfBall Ball;
    public LineRenderer ShootAngleRenderer;

    private Vector3 _shootAngle;
    private bool _canDetermineAngle = false;
    [SerializeField] private float _lineLength = 5f;
    public GameObject AngleChooserObject;

    private bool _canDetermineShootPower = false;
    public float MarkerSpeed = 300f;
    public float MarkerSpeedIncreaseStep = 100f;
    private bool _movingRight = true;
    public RectTransform Marker;
    public RectTransform Bar;
    public RectTransform SuccessZone;
    private bool _inputAllowed = true;
    private float _cooldownFactor = 0.4f; // 0.2 = 20% of total bar traversal time
    private float _shootPower = 5f;
    private float _shootPowerIncreaseStep = 5f;

    public GameObject PowerBar;

    //determine angle -> linerenderer from golfball to mouse pos
    //when left click -> get direction vector, normalize it
    //start spacebar power ui
    //check for spacebar input
    //when missing the mark -> shoot golf ball in set direction

    private void Start()
    {
        ResetShooting();
    }

    public void ResetShooting()
    {
        ShootAngleRenderer.positionCount = 2;
        ShootAngleRenderer.SetPosition(0, Ball.transform.position);
        ShootAngleRenderer.SetPosition(1, Ball.transform.position);

        _canDetermineAngle = true;
    }

    private void Update()
    {
        if (_canDetermineAngle) DetermineShootAngle();

        if (Input.GetMouseButtonDown(0) && _canDetermineAngle)
        {
            _canDetermineAngle = false;
            _shootAngle = ShootAngleRenderer.GetPosition(1) - ShootAngleRenderer.GetPosition(0);

            PowerBar.SetActive(true);
            _canDetermineShootPower = true;
            Marker.anchoredPosition = new Vector2(-Bar.rect.width / 2, Marker.anchoredPosition.y);
        }

        if (_canDetermineShootPower)
        {
            UpdatePowerMarker();
            DetermineShootPower();
        }
    }

    public void DetermineShootAngle()
    {
        var mouseworldpos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseworldpos.z = 0;

        Vector3 direction = (mouseworldpos - Ball.transform.position).normalized;

        Vector3 endpoint = Ball.transform.position + direction * _lineLength;

        ShootAngleRenderer.SetPosition(0, Ball.transform.position);
        ShootAngleRenderer.SetPosition(1, endpoint);
    }

    //in increments of 5
    //each time the zone gets hit +5
    //save currentpower
    //when missing, shoot with current power
    //when in the perfect zone, +10
    public void DetermineShootPower()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _inputAllowed)
        {
            CheckSuccess();

            float barWidth = Bar.rect.width;
            float totalTravelTime = barWidth / MarkerSpeed;
            float inputCooldown = totalTravelTime * _cooldownFactor;

            _inputAllowed = false;
            Invoke("ResetInput", inputCooldown);
        }
    }

    public void UpdatePowerMarker()
    {
        // Move marker
        float move = MarkerSpeed * Time.deltaTime;
        Vector2 markerPos = Marker.anchoredPosition;

        if (_movingRight)
        {
            markerPos.x += move;
            if (markerPos.x >= Bar.rect.width / 2)
            {
                markerPos.x = Bar.rect.width / 2;
                _movingRight = false;
            }
        }
        else
        {
            markerPos.x -= move;
            if (markerPos.x <= -Bar.rect.width / 2)
            {
                markerPos.x = -Bar.rect.width / 2;
                _movingRight = true;
            }
        }

        Marker.anchoredPosition = markerPos;
    }

    public void CheckSuccess()
    {
        float markerX = Marker.anchoredPosition.x;
        float successStart = SuccessZone.anchoredPosition.x - SuccessZone.rect.width / 2;
        float successEnd = SuccessZone.anchoredPosition.x + SuccessZone.rect.width / 2;

        if (markerX >= successStart && markerX <= successEnd)
        {
            MarkerSpeed += MarkerSpeedIncreaseStep;
            _shootPower += _shootPowerIncreaseStep;

            Debug.Log("Success!");
        }
        else
        {
            Ball.Shoot(_shootAngle, _shootPower);
            _canDetermineShootPower = false;
            PowerBar.SetActive(false);
            AngleChooserObject.SetActive(false);

            Debug.Log("Failed!");
        }
    }

    public void ResetInput()
    {
        _inputAllowed = true;
    }
}
