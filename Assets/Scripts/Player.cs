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

    private bool _canDetermineShootPower = false;
    public float MarkerSpeed = 300f;
    public float MarkerSpeedIncreaseStep = 100f;
    private bool _movingRight = true;
    public RectTransform Marker;
    public RectTransform Bar;
    public RectTransform SuccessZone;
    public RectTransform PerfectZone;
    private bool _inputAllowed = true;
    private float _cooldownFactor = 0.2f; // 0.2 = 20% of total bar traversal time
    private float _shootPower = 5f;
    private float _shootPowerIncreaseStep = 5f;
    private bool _hasChosenAngle = false;

    [SerializeField] private BallCameraZoom _cameraZoom;

    private float _stationaryTime = 0f;
    public float RequiredStationaryTime = 1f; // seconds before aiming is allowed
    public float VelocityThreshold = 0.05f; // how still the ball needs to be
    private bool _waitingToShoot = false;

    public GameObject PowerBar;
    public Level CurrentLevel;
    public GameObject RoundEndTallyObj;

    private void Start()
    {
        CurrentLevel.Strokes = 0;
        GameManager.Instance.Holes.Add(CurrentLevel);
        GameManager.Instance.HasScored = false;
        GameManager.Instance.RoundEndTallyObj = RoundEndTallyObj;
        PowerBar.SetActive(false);
        ResetShooting();
    }

    public void ResetShooting()
    {
        _shootPower = 5f;
        MarkerSpeed = 500f;
        _hasChosenAngle = false;

        ShootAngleRenderer.positionCount = 2;
        ShootAngleRenderer.SetPosition(0, Ball.transform.position);
        ShootAngleRenderer.SetPosition(1, Ball.transform.position);

        _canDetermineAngle = true;
        ShootAngleRenderer.enabled = true;

        Debug.Log("ResetShooting called - ready to aim");
    }

    private void Update()
    {
        if (GameManager.Instance.GameStarted)
        {
            Rigidbody2D rb = Ball.GetComponent<Rigidbody2D>();

            if (Ball.IsBallGrounded() && rb.velocity.magnitude < VelocityThreshold && !_hasChosenAngle)
            {
                _stationaryTime += Time.deltaTime;

                // After being idle long enough and the ball has been shot before
                if (_stationaryTime >= RequiredStationaryTime)
                {
                    _waitingToShoot = true;
                    ResetShooting();
                }
            }
            else
            {
                _stationaryTime = 0f;

                _canDetermineAngle = false;
                _waitingToShoot = false;
            }

            // Allow the player to aim if we're in aim state
            if (_canDetermineAngle && !_hasChosenAngle)
                DetermineShootAngle();

            if (Input.GetMouseButtonDown(0) && _waitingToShoot)
            {
                _canDetermineAngle = false;
                _waitingToShoot = false;
                _hasChosenAngle = true;

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
    }

    public void DetermineShootAngle()
    {
        if (!_canDetermineAngle)
            return;

        ShootAngleRenderer.enabled = true;

        var mouseworldpos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseworldpos.z = 0;

        Vector3 direction = (mouseworldpos - Ball.transform.position).normalized;

        Vector3 endpoint = Ball.transform.position + direction * _lineLength;

        ShootAngleRenderer.SetPosition(0, Ball.transform.position);
        ShootAngleRenderer.SetPosition(1, endpoint);
    }

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

        float perfectStart = PerfectZone.anchoredPosition.x - PerfectZone.rect.width / 2 + SuccessZone.anchoredPosition.x;
        float perfectEnd = PerfectZone.anchoredPosition.x + PerfectZone.rect.width / 2 + SuccessZone.anchoredPosition.x;

        if (markerX >= perfectStart && markerX <= perfectEnd)
        {
            MarkerSpeed += (MarkerSpeedIncreaseStep * 2);
            _shootPower += (_shootPowerIncreaseStep * 2);

            Debug.Log("Perfect Success!");
        }
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
            ShootAngleRenderer.enabled = false;

            _hasChosenAngle = false;

            CurrentLevel.Strokes += 1;

            Debug.Log("Failed!");
        }
    }

    public void ResetInput()
    {
        _inputAllowed = true;
    }
}
