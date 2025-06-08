using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class Hole : MonoBehaviour
{
    public Player Player;
    private bool _ballInside = false;
    private Coroutine _checkCoroutine;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball") && !GameManager.Instance.HasScored)
        {
            _ballInside = true;

            if (_checkCoroutine == null)
            {
                Rigidbody2D ballRb = collision.GetComponent<Rigidbody2D>();
                _checkCoroutine = StartCoroutine(CheckBallStaysInGoal(ballRb));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball"))
        {
            _ballInside = false;

            if (_checkCoroutine != null)
            {
                StopCoroutine(_checkCoroutine);
                _checkCoroutine = null;
            }
        }
    }

    private IEnumerator CheckBallStaysInGoal(Rigidbody2D ballRb)
    {
        float timeInGoal = 0f;
        float requiredStationaryTime = .5f;
        float velocityThreshold = 0.1f;

        while (_ballInside && !GameManager.Instance.HasScored)
        {
            if (ballRb.velocity.magnitude < velocityThreshold)
            {
                timeInGoal += Time.deltaTime;

                if (timeInGoal >= requiredStationaryTime)
                {
                    GameManager.Instance.ReachedGoal(Player.CurrentLevel.Strokes);
                    yield break;
                }
            }

            yield return null;
        }

        _checkCoroutine = null;
    }
}
