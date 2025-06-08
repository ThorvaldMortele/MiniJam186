using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndSceneLogic : MonoBehaviour
{
    public RoundEndTally RoundEndTally;

    void Start()
    {
        RoundEndTally.gameObject.SetActive(true);
        RoundEndTally.DisplayScores();
    }
}
