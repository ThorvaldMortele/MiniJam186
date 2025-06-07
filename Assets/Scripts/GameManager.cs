using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public float CurrentLevel = 1;

    public bool GameStarted = false;
    public float StartDelay = 3f;  // Wait 3 seconds before allowing aiming
    private float _startTimer = 0f;

    public float DelayBetweenLevels = 2f;
    public float MaxLevels = 6;

    public bool HasScored = false;

    public List<Level> Holes = new List<Level>();
    public GameObject RoundEndTallyObj;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        foreach(var Level in Holes)
        {
            Level.Strokes = 0;
        }
    }

    private void Update()
    {
        if (!GameStarted && !HasScored)
        {
            _startTimer += Time.deltaTime;
            if (_startTimer >= StartDelay)
            {
                GameStarted = true;
                Debug.Log("Game started! You can now aim.");
            }
            else
            {
                return;
            }
        }
    }

    public int TotalScore()
    {
        int total = 0;
        foreach(var hole in Holes)
        {
            total += hole.Strokes;
        }
        return total;
    }

    public void RecordStrokes(int holeIndex, int strokes)
    {
        if (holeIndex >= 0 && holeIndex < Holes.Count)
        {
            Holes[holeIndex].Strokes = strokes;
        }
    }

    //ball is in hole
    public void ReachedGoal(int strokes)
    {
        RecordStrokes((int)CurrentLevel - 1, strokes);

        HasScored = true;
        GameStarted = false;
        CurrentLevel += 1;

        if (CurrentLevel > MaxLevels)
        {
            StartCoroutine(DelayAfterGoalReachedEndGame());
        }
        else
        {
            StartCoroutine(DelayAfterGoalReachedShowScoreBoard());

        }
    }

    public IEnumerator DelayAfterGoalReachedEndGame()
    {
        yield return new WaitForSeconds(DelayBetweenLevels);

        SceneManager.LoadScene("EndScene");
    }

    public IEnumerator DelayAfterGoalReachedShowScoreBoard()
    {
        yield return new WaitForSeconds(DelayBetweenLevels);

        RoundEndTallyObj.SetActive(true);
        RoundEndTallyObj.GetComponent<RoundEndTally>().DisplayScores();
    }

    public IEnumerator LoadNextLevel()
    {
        AsyncOperation asyncLoaded = SceneManager.LoadSceneAsync("Level" + CurrentLevel, LoadSceneMode.Single);

        while (!asyncLoaded.isDone)
        {
            yield return null;
        }

        GameStarted = true;
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Level" + CurrentLevel, LoadSceneMode.Single);
    }
}
