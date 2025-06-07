using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoundEndTally : MonoBehaviour
{
    public List<GameObject> ScoreObjs = new List<GameObject>();

    public void DisplayScores()
    {
        for (int i = 0; i < GameManager.Instance.CurrentLevel - 1; i++)
        {
            ScoreObjs[i].SetActive(true);
            ScoreObjs[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GameManager.Instance.Holes[i].Strokes.ToString();
        }
    }

    public void NextHole()
    {
        StartCoroutine(GameManager.Instance.LoadNextLevel());
    }
}
