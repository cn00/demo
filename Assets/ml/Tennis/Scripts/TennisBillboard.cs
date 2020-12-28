using System;
using UnityEngine;
using UnityEngine.UI;

public class TennisBillboard : MonoBehaviour
{
    public Text labelA, labelB;
    public TennisPlayground playground;

    private void FixedUpdate()
    {
        labelA.text = $"{playground.agentA.score}/{playground.agentA.hitCount}";
        labelB.text = $"{playground.agentB.score}/{playground.agentB.hitCount}";
    }
}
