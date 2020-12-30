using System;
using ml.Tennis;
using UnityEngine;
using UnityEngine.UI;

public class TennisBillboard : MonoBehaviour
{
    public Text labelA, labelB;
    public TennisPlayground playground;

    [Range(0.01f, 2f)]
    public float Dt = Time.fixedDeltaTime;
    
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
                Debug.Log($"{time} {ball.lastAgentHit} -> {c.gameObject.name} <- {ball.lastFloorHit} {ag.score}/{ag.hitCount}:{agb.score}/{agb.hitCount}");
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="p0"></param>
    /// <param name="v"></param>
    /// <param name="mass"></param>
    /// <param name="drag"></param>
    /// <param name="dmt"></param>
    public static void GizmosDrawParabola(Vector3 p0, Vector3 v, Vector3 a, float drag, float mass, float dt = 0.02f, float time = 3f)
    {
        if (dt < 0.01) dt = 0.01f;
        if (time < 0.5) time = 0.5f;
        
        var G = -9.8f;
        if(v.x > 0)
            Gizmos.color = Color.cyan;
        else 
            Gizmos.color = Color.red;

        var pp = p0;
        var p = new Vector3();
        var tp = new Vector3();
        for (float t = 0f; t < time; t += dt)
        {
            
            var tpy = tp.y;
            
            // // 不考虑空阻
            // tp.Set(
            //     p0.x + a.x * t * t / 2f + v.x * t, 
            //     p0.y + a.y * t * t / 2f + v.y * t, 
            //     p0.z + a.z * t * t / 2f + v.z * t);
            
            tp.Set(
                pp.x + v.x * dt,
                pp.y + v.y * dt,
                pp.z + v.z * dt);
            
            if (tpy * tp.y < 0f) // 反弹
            {
                v.y = -v.y;
                v *= 0.8f;
                tp.y = 0f;
            }
            
            a.Set(
                (drag * v.x * v.x) / mass * (v.x > 0f ? -1f:1f),
                (drag * v.y * v.y) / mass * (v.y > 0f ? -1f:1f) + G,
                (drag * v.z * v.z) / mass * (v.z > 0f ? -1f:1f));
            v.Set(
                v.x + a.x * dt,
                v.y + a.y * dt,
                v.z + a.z * dt);

            Gizmos.DrawLine(pp, tp);
            pp = tp;
        }
    }


    void GizmDraw2()
    {
        var G = playground.G;
        var ball = playground.ball;
        var pp = ball.transform.position;
        var v = ball.Velocity;
        if(v.x > 0)
            Gizmos.color = Color.cyan;
        else 
            Gizmos.color = Color.red;

        var drag = ball.rigidbody.drag;
        var mass = ball.rigidbody.mass;
        var a  = new Vector3(
            -(drag * v.x * v.x) / mass,
            G + (drag * v.y * v.y) / mass,
            -(drag * v.z * v.z) / mass);
        // var ps = Dweiss.RigidbodyExtension.CalculateMovement(pp, ball.Velocity, a, 100, 0.03f, Vector3.zero, Vector3.zero, mass, drag);
        var ps = Dweiss.RigidbodyExtension.CalculateMovement(ball.rigidbody, 100, 0.01f);
        pp = ps[0];
        foreach (var p in ps)
        {
            Gizmos.DrawLine(pp, p);
            pp = p;
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
                Gizmos.color = Color.cyan;
            else 
                Gizmos.color = Color.red;

            var drag = ball.rigidbody.drag;
            var mass = ball.rigidbody.mass;
            var a  = new Vector3(
                   -(drag * v.x * v.x) / mass,
                G + (drag * v.y * v.y) / mass,
                   -(drag * v.z * v.z) / mass);
            GizmosDrawParabola(pp, v, a, drag, mass, Dt, DTime );

            // var dt = 0.1f;
            // for (float t = 0f; t < ball.Tt + dt/2f; t += dt)
            // {
            //     var tp = new Vector3(
            //         a.x * t * t / 2f + v.x * t,
            //         a.y * t * t / 2f + v.y * t,
            //         a.z * t * t / 2f + v.z * t);
            //     var p = ball.transform.position + tp;
            //     Gizmos.color = Color.yellow;
            //     Gizmos.DrawLine(pp, p);
            //     pp = p;
            //     // v += a * dt;
            // }
        };
        
        Action<TennisAgentA> draw = (agent) =>
        {
            if(agent.invertX)
                Gizmos.color = Color.magenta;
            else
                Gizmos.color = Color.blue;
            
            var btp = ball.GetTargetPos();
            var atp = ball.GetTargetPos(0.5f);
            var delta = agent.transform.localRotation.normalized * new Vector3(0f, 0f, -1.6f);
            var lp0 = agent.transform.localPosition + delta;
            // agent.GetVelocity(agent.transform.localPosition);
            var s = btp - lp0;
            ball.Intersect = Util.IntersectLineToPlane(lp0, s, Vector3.right, Vector3.zero);

            var p0 = agent.transform.position + delta;
            Gizmos.DrawLine(p0, playground.transform.position + ball.Tp);
            Gizmos.DrawLine(p0, ball.transform.position);
            Gizmos.DrawLine(p0, playground.transform.position + atp);
            
            Gizmos.DrawSphere(playground.transform.position + ball.Intersect, 0.1f);

            var po = playground.transform.position + (new Vector3(0f, 0f, ball.Intersect.z) );
            var dir = new Vector3(0f, 20f, ball.Intersect.z);
            Gizmos.DrawRay(po, dir);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(playground.transform.position + btp, 0.1f);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(playground.transform.position + atp, 0.1f);
        };
        
        draw(playground.agentA);
        draw(playground.agentB);
        drawParabola();
        // GizmDraw2();
    }
}
