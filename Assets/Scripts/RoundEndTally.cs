using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoundEndTally : MonoBehaviour
{
    public List<GameObject> ScoreObjs = new List<GameObject>();
    public TextMeshProUGUI ButtonText;

    public void DisplayScores()
    {
        int total = 0;

        for (int i = 0; i < GameManager.Instance.CurrentLevel - 1; i++)
        {
            total += GameManager.Instance.Holes[i].Strokes;
            ScoreObjs[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = GameManager.Instance.Holes[i].Strokes.ToString();
        }

        ScoreObjs[ScoreObjs.Count-1].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = total.ToString();

        if (GameManager.Instance.CurrentLevel > 6) ButtonText.text = "Main Menu";
        else ButtonText.text = "Next Hole";
    }

    public void NextHole()
    {
        if (GameManager.Instance.CurrentLevel <= 6)
            StartCoroutine(GameManager.Instance.LoadNextLevel());
        else
        {
            GameManager.Instance.CurrentLevel = 0;
            Destroy(GameManager.Instance.gameObject);
            Destroy(AudioManager.Instance.gameObject);
            SceneManager.LoadScene("MainMenu");
        }   
    }
}



