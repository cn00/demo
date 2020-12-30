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
                var time = DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss");
                Debug.LogWarning($"{time} Id:{ag.m_EpisodeId} cmRw:{ag.m_CumulativeReward} cmEps:{ag.CompletedEpisodes} score:{ag.score}/{ag.hitCount}:{agb.score}/{agb.hitCount}");
            }
        };

        var ball = playground.ball;
        ball.CollisionEnter += (c) =>
        {
            var time = DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss");
            if(ball.lastAgentHit != TennisBall.AgentRole.O)
                Debug.Log($"{time} {ball.lastAgentHit} -> {c.gameObject.name} <- {ball.lastFloorHit} {ag.score}/{ag.hitCount}:{agb.score}/{agb.hitCount}");
        };
    }

    private void OnDrawGizmos()
    {
        Action<float, float, float> drawParabola = (p1, p2, p3) =>
        {
            /// f(x,y,z) = (
            ///     x: a1z^2 + b1z + c1
            ///     y: a2z^2 + b2z + c2
            ///     z: a3x + c3
            /// )
        };
        var ball = playground.ball;
        Action<TennisAgentA> draw = (agent) =>
        {
            var delta = agent.transform.localRotation.normalized * new Vector3(0f, 0f, -1.6f);
            var lp0 = agent.transform.localPosition + delta;
            // agent.GetVelocity(agent.transform.localPosition);
            var s = btp - lp0;
            ball.Intersect = Util.IntersectLineToPlane(lp0, s, Vector3.right, Vector3.zero);

            var p0 = agent.transform.position + delta;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(p0, playground.transform.position + agent.Pt);
            
            Gizmos.DrawLine(ball.transform.position, p0);
            
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(playground.transform.position + agent.Intersect, 0.1f);

            // Gizmos.color = Color.blue;
            Gizmos.DrawSphere(playground.transform.position + agent.Pt, 0.1f);

            var po = playground.transform.position + (new Vector3(0f, 0f, agent.Intersect.z) );
            var dir = new Vector3(0f, 100f, agent.Intersect.z);
            Gizmos.DrawRay(po, dir);
        };
        draw(playground.agentA);
        draw(playground.agentB);
    }
}
