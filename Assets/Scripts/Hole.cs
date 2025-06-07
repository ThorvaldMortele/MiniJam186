using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    public Player Player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball") && !GameManager.Instance.HasScored)
        {
            GameManager.Instance.ReachedGoal(Player.CurrentLevel.Strokes);
        }
    }
}
