using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using TMPro;
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

    public int CurrentCombo = 0;
    public TextMeshProUGUI ComboText;
    private float _comboScaleStep = .15f;

    private float _currentBaseScale = 1f;

    private float _shakeTimer;
    [SerializeField] private CinemachineVirtualCamera _cam;
    private CinemachineBasicMultiChannelPerlin _noise;

    private void Start()
    {
        CurrentLevel.Strokes = 0;
        GameManager.Instance.Holes.Add(CurrentLevel);
        GameManager.Instance.HasScored = false;
        GameManager.Instance.RoundEndTallyObj = RoundEndTallyObj;
        PowerBar.SetActive(false);
        ResetShooting();

        _noise = _cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
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

            IncreaseCombo("perfect");

            Debug.Log("Perfect Success!");
        }
        else if (markerX >= successStart && markerX <= successEnd)
        {
            MarkerSpeed += MarkerSpeedIncreaseStep;
            _shootPower += _shootPowerIncreaseStep;

            IncreaseCombo("success");

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

            IncreaseCombo("failure");

            Debug.Log("Failed!");
        }
    }

    public void ResetInput()
    {
        _inputAllowed = true;
    }

    public void IncreaseCombo(string spacebarPressActionType)
    {
        ComboText.rectTransform.DOKill();

        switch (spacebarPressActionType)
        {
            case "success":
                {
                    CurrentCombo += 1;
                    ComboText.text = "x" + CurrentCombo;
                    ComboText.color = Color.white;

                    _currentBaseScale += _comboScaleStep;
                    PlayPop(_currentBaseScale, 0.15f, false);
                    Shake(1, 0.2f);
                    PopPowerBar();
                    break;
                }

            case "perfect":
                {
                    CurrentCombo += 2;
                    ComboText.text = "x" + CurrentCombo;
                    ComboText.color = Color.yellow;

                    _currentBaseScale += _comboScaleStep * 2;
                    PlayPop(_currentBaseScale, 0.15f, true); // Shake enabled
                    Shake(2.5f, 0.3f);
                    PopPowerBar(1.25f, 0.3f);
                    break;
                }

            case "failure":
                {
                    CurrentCombo = 0;
                    ComboText.text = "";
                    _currentBaseScale = 1f;

                    ComboText.rectTransform.DOKill();
                    ComboText.rectTransform.localScale = Vector3.one;
                    ComboText.rectTransform.localRotation = Quaternion.identity;
                    break;
                }
        }
    }

    private void PlayPop(float targetScale, float duration, bool doShake)
    {
        RectTransform rt = ComboText.rectTransform;
        rt.localScale = Vector3.one * targetScale;

        float popAmount = 0.2f * targetScale;
        float randomZ = Random.Range(10f, 25f);
        if (rt.localEulerAngles.z > 0) randomZ = -randomZ;

        Sequence pop = DOTween.Sequence();
        pop.Append(rt.DOScale(targetScale + popAmount, duration * 0.5f).SetEase(Ease.OutBack));
        pop.Join(rt.DORotate(new Vector3(0, 0, randomZ), duration * 0.5f));
        pop.Append(rt.DOScale(targetScale, duration * 0.3f).SetEase(Ease.InBack));
        pop.Append(rt.DORotate(Vector3.zero, duration * 0.2f));

        // Add shake at the end only if it's a perfect
        if (doShake)
        {
            pop.Append(rt.DOShakeRotation(
                duration: 0.25f,
                strength: 10f,   // degrees
                vibrato: 10,
                randomness: 90,
                fadeOut: true
            ));
        }
    }

    private void PopPowerBar(float scaleUp = 1.1f, float duration = 0.2f)
    {
        Bar.DOKill(); // Cancel any ongoing tweens on this panel

        Bar.localScale = Vector3.one; // Reset scale in case it was mid-tween

        Sequence pop = DOTween.Sequence();
        pop.Append(Bar.DOScale(scaleUp, duration * 0.5f).SetEase(Ease.OutBack));
        pop.Append(Bar.DOScale(1f, duration * 0.5f).SetEase(Ease.InOutQuad));
    }

    public void Shake(float intensity, float time)
    {
        if (_noise == null) return;

        _noise.m_AmplitudeGain = intensity;
        _noise.m_FrequencyGain = 2f; // You can tweak this for snappier or smoother shake
        _shakeTimer = time;

        CancelInvoke(nameof(StopShake));
        Invoke(nameof(StopShake), time);
    }

    private void StopShake()
    {
        if (_noise == null) return;
        _noise.m_AmplitudeGain = 0f;
    }
}
