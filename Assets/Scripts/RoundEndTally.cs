using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoundEndTally : MonoBehaviour
{
    public List<GameObject> ScoreObjs = new List<GameObject>();

    public void DisplayScores()
    {
        foreach (var holeScoreObj in ScoreObjs)
        {
            var idx = ScoreObjs.IndexOf(holeScoreObj);

            holeScoreObj.SetActive(true);
            holeScoreObj.GetComponentInChildren<TextMeshProUGUI>().text = GameManager.Instance.Holes[idx].Strokes.ToString();
        }
    }

    public void NextHole()
    {
        StartCoroutine(GameManager.Instance.LoadNextLevel());
    }
}
