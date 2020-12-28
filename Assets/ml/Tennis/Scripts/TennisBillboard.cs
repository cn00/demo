using System;
using UnityEngine;
using UnityEngine.UI;

public class TennisBillboard : MonoBehaviour
{
    public Text labelA, labelB;
    public TennisPlayground playground;

    private void Start()
    {
        var ag = playground.agentA;
        var agb = playground.agentB;
        playground.agentA.episodeBeginAction += ()=>
        {
            labelA.text = $"{ag.score}/{ag.hitCount}";
            labelB.text = $"{playground.agentB.score}/{playground.agentB.hitCount}";

            if (    ag.hitCount < 100 && ag.CompletedEpisodes % 10 == 0  
                 || ag.hitCount > 100 && ag.CompletedEpisodes % 10000 == 0)
            {
                Debug.LogWarning($"{DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss")} Id:{ag.m_EpisodeId} cmRw:{ag.m_CumulativeReward} cmEps:{ag.CompletedEpisodes} score:{ag.score}/{ag.hitCount}:{agb.score}/{agb.hitCount}");
            }
        };
    }
}
