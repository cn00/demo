using System;
using ml.Tennis;
using UnityEngine;
using UnityEngine.UI;

public class TennisBillboard : MonoBehaviour
{
    public Text labelA, labelB;
    public TennisPlayground playground;

    [Range(0.01f, 2f)]
    public float Dt = 0.02f;
    
    [Range(0.5f, 10f)]
    public float DTime = 3f;

    private void Start()
    {
        Dt = Time.fixedDeltaTime;
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
                Debug.Log($"{time} Agent{ball.lastAgentHit} -> {c.gameObject.name} <- {ball.lastFloorHit} {ag.score}/{ag.hitCount}:{agb.score}/{agb.hitCount}");
        };
    }

    /// <summary>
    /// 预测运动轨迹
    /// </summary>
    /// <param name="p0">当前位置</param>
    /// <param name="v">当前速度</param>
    /// <param name="mass">质量</param>
    /// <param name="drag">空气阻力系数</param>
    /// <param name="dt">每步时长</param>
    public void GizmosDrawParabola(Vector3 p0, Vector3 v, float drag, float mass, float dt = 0.02f, float time = 3f)
    {
        if (dt < 0.01f) dt = 0.01f;
        if (time < 0.5f) time = 0.5f;
        if (time > 5f) time = 5f;
        
        var G = -9.81f;
        if(v.x > 0)
            Gizmos.color = Color.cyan;
        else 
            Gizmos.color = Color.red;

        var pp = p0;
        var a = new Vector3();
        var p = new Vector3();
        var tp = new Vector3();
        var bouncec = 0;
        for (float t = 0f; t < time && bouncec < 2; t += dt)
        {
            
            var tpy = tp.y;
            tp.Set(
                pp.x + v.x * dt,
                pp.y + v.y * dt,
                pp.z + v.z * dt);
            
            if (tpy * tp.y < 0f) // 反弹
            {
                ++bouncec;
                v.y = -v.y;
                // v *= 0.8f;
                tp.y = 0f;
            }
            
            a.Set(
                (drag * v.x * v.x) / mass * (v.x > 0f ? -1f:1f),
                (drag * v.y * v.y) / mass * (v.y > 0f ? -1f:1f) + G,
                (drag * v.z * v.z) / mass * (v.z > 0f ? -1f:1f));
            var pvy = v.y;
            v.Set(
                v.x + a.x * dt,
                v.y + a.y * dt,
                v.z + a.z * dt);
            
            Gizmos.DrawLine(pp, tp);
            
            pp = tp;
        }
    }
    
    private void OnDrawGizmos()
    {
        var ball = playground.ball;
        Action drawParabola = () =>
        {
            var G = playground.G;
            var pp = ball.transform.position;
            var v = ball.Velocity;
            if(v.x > 0)
                Gizmos.color = Color.blue;
            else 
                Gizmos.color = Color.magenta;

            var drag = ball.rb.drag;
            var mass = ball.rb.mass;
            GizmosDrawParabola(pp, v, drag, mass, Dt, DTime );
        };
        
        Action<TennisAgentA> draw = (agent) =>
        {
            float[] outt;
            Vector3[] btps;
            if(!agent.GetTarget(out btps, out outt)) return;
            
            var atp = btps[0];
            var lp0 = agent.transform.localPosition;
            Vector3 btp = btps[1];
            var s = btp - lp0;

            var p0 = agent.transform.position;
            for (int i = 0; i < btps.Length; i++)
            {
                if (agent.CanIReachPoint(btps[i], outt[i]))
                {
                    Gizmos.color = Color.yellow;
                    if(btps[i].y > 0)
                        Gizmos.DrawLine(p0, playground.transform.position + btps[i]);
                }
                else
                    Gizmos.color = Color.green;
                Gizmos.DrawSphere(playground.transform.position + btps[i], 0.1f);
            }

            if(agent.invertX) Gizmos.color = Color.magenta;
            else Gizmos.color = Color.blue;
            
            // Gizmos.DrawLine(p0, playground.transform.position + btp);
            // Gizmos.DrawLine(p0, ball.transform.position);

            Gizmos.DrawSphere(playground.transform.position + lp0, 0.1f);
            // Gizmos.DrawSphere(playground.transform.position + agent.Intersect, 0.1f);

            // // 过网垂线
            // var po = playground.transform.position + (new Vector3(0f, 0f, ball.Intersect.z) );
            // var dir = new Vector3(0f, 20f, ball.Intersect.z);
            // Gizmos.DrawRay(po, dir);
        };
        
        if(ball.Velocity.x < 0f)
            draw(playground.agentA);
        else
            draw(playground.agentB);
        drawParabola();
        // GizmDraw2();
    }
}
