using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoundEndTally : MonoBehaviour
{
    public List<GameObject> ScoreObjs = new List<GameObject>();

    public void DisplayScores()
    {
        int total = 0;

        for (int i = 0; i < GameManager.Instance.CurrentLevel - 1; i++)
        {
            total += GameManager.Instance.Holes[i].Strokes;
            ScoreObjs[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = GameManager.Instance.Holes[i].Strokes.ToString();
        }

        ScoreObjs[ScoreObjs.Count-1].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = total.ToString();
    }

    public void NextHole()
    {
        StartCoroutine(GameManager.Instance.LoadNextLevel());
    }
}



