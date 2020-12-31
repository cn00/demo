using System;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace ml.Tennis
{
    public class TennisBall : MonoBehaviour
    {
        public TennisPlayground playground;
        public Rigidbody rb;

        public float m_velocityMax = 60f; // 216Km/h

        public bool net;

        public enum AgentRole
        {
            A,
            B,
            O,
        }

        public AgentRole lastAgentHit = AgentRole.O;

        public enum FloorHit
        {
            Service,
            FloorHitUnset,
            FloorAHit,
            FloorBHit
        }

        public FloorHit lastFloorHit;

        public Vector3 maxVelocity = new Vector3(60f, 10f, 10f); // 世界记录 196Km/h = 54m/s
        public Vector3 Velocity
            #if !UNITY_EDITOR
            => rb.velocity
            #endif
            ;

        public float[] Tt;

        /// <summary>
        /// Inspector 调试用，缓存最佳击球点
        /// </summary>
        public Vector3[] TargetPos;
        
        /// <summary>
        /// 计算最佳击球点
        /// </summary>
        /// <out>[0,1,2,3]: 落地前, 第一次落地点, 第一次弹起后, 第二次落地前</out>
        /// <returns type="System.Boolean"></returns>
        /// Unity3D中常用的物理学公式 https://www.cnblogs.com/msxh/p/6128851.html
        /// Unity 如何计算阻力？ https://www.leadwerks.com/community/topic/4385-physics-how-does-unity-calculate-drag/
        /// FIXIT: 求解微积分方程获取精确路径 https://www.zhihu.com/question/68565717
        public bool GetTarget(out Vector3[] outPos, out float[] outTimes)
        {
            // List<Quaternion> tq = new List<Quaternion>();
            // var q = new Quaternion(Vector3.back, Single.Epsilon, );
            
            var G = playground.G.y;
            var v = Velocity;
            var d = rb.drag; // 0.47 https://www.jianshu.com/p/9da46cf6d5f5
            var m = rb.mass;

            // var ad = - Mathf.Pow(v.magnitude, 2) * drag * v.normalized;

            var ops = new List<Vector3>(4);
            var ots = new List<float>(4);
            var pp = transform.localPosition; // rb.position;
            var a = new Vector3();
            var tp = pp;
            var time = 5f;
            var dt = 0.009f;//Time.deltaTime;
            // if (dt < 0.01f || dt > 0.3f) dt = 0.01f;
            var timeCount = 0f;
            var bouncec = 0;
            for (float t = 0f; t < time && bouncec < 2; t += dt )
            {
                timeCount += dt;
                var ppy = tp.y;
                tp = new Vector3(v.x * dt,v.y * dt,v.z * dt) + pp;

                if (ppy * tp.y < 0f) // 反弹
                {
                    ++ bouncec;
                    tp.y = 0f;
                    ops.Add(tp);
                    ots.Add(timeCount);
                    v.y = -v.y;
                    // v *= 0.8f; // 非刚性反弹？
                }
                pp = tp;
                
                a.Set(
                    (d * v.x * v.x) / m * (v.x > 0f ? -1f : 1f),
                    (d * v.y * v.y) / m * (v.y > 0f ? -1f : 1f) + G,
                    (d * v.z * v.z) / m * (v.z > 0f ? -1f : 1f));

                var pvy = v.y;
                v.Set(
                    v.x + a.x * dt,
                    v.y + a.y * dt,
                    v.z + a.z * dt);
                
                // 最佳击球点
                var bestHitY = v.x > 0f ? playground.agentA.BestTargetY : playground.agentB.BestTargetY;
                if (   tp.y >= bestHitY && Mathf.Abs(tp.y - bestHitY) < Mathf.Abs(v.y * dt)*1f 
                    || tp.y <  bestHitY && pvy * v.y < 0f) // 顶点
                {
                    ops.Add(tp);
                    ots.Add(timeCount);
                    if (pvy * v.y < 0f) v.y = 0f; 
                }
            }
            
            outTimes = ots.ToArray();
            outPos = ops.ToArray();
            
            TargetPos = outPos;
            Tt = outTimes;
            
            return outPos.Length > 3;
        }

        private void FixedUpdate()
        {
            // if(playground.agentA is TennisAgent)// 
            // {
            //     var rgV = rigidbody.velocity;
            //     rigidbody.velocity = new Vector3(
            //         Mathf.Clamp(rgV.x, -9f, 9f),
            //         Mathf.Clamp(rgV.y, -9f, 9f),
            //         Mathf.Clamp(rgV.z, -9f, 9f));
            // }
            // else

            // var rgV = rigidbody.velocity;
            // rigidbody.velocity = new Vector3(
            //     Mathf.Clamp(rgV.x, -maxVelocity.x, maxVelocity.z),
            //     Mathf.Clamp(rgV.y, -maxVelocity.y, maxVelocity.y),
            //     Mathf.Clamp(rgV.z, -maxVelocity.z, maxVelocity.z));
            
            #if UNITY_EDITOR
            Velocity = rb.velocity;
            #endif

            // // 无法预计算轨迹
            // var bp = transform.localPosition;
            // transform.localPosition = new Vector3(
            //     Mathf.Clamp(bp.x, -4f * playground.Size.x, 4f * playground.Size.x),
            //     Mathf.Clamp(bp.y, 0, 10f * playground.Size.y),
            //     Mathf.Clamp(bp.z, -4f * playground.Size.z, 4f * playground.Size.z));

        }

        void Reset()
        {
            var pz = 0f;
            var vz = 0f;
            if (playground.agentA.score > playground.levelOne && playground.agentB.score > playground.levelOne)
            {
                pz = Random.Range(-playground.HalfSize.z, playground.HalfSize.z);
                vz = Random.Range(-1f, 1f);
            }

            var px = Random.Range(-playground.HalfSize.x, playground.HalfSize.x);
            var py = Random.Range(4f, playground.HalfSize.y);
            transform.localPosition = new Vector3(px, py, pz);

            var vx = Random.Range(0, maxVelocity.x) * (px > 0f ? -1f:1f)/10f;
            var vy = Random.Range(-maxVelocity.y, maxVelocity.y)/10f;
            rb.velocity = new Vector3(vx, vy,vz);
            
            transform.localScale = new Vector3(.5f, .5f, .5f);

            playground.agentA.EndEpisode();
            playground.agentB.EndEpisode();
            lastFloorHit = FloorHit.Service;
            lastAgentHit = AgentRole.O;
            net = false;
        }

        void AgentAWins(float reward = 1)
        {
            playground.agentA.SetReward(reward);
            playground.agentB.SetReward(-reward);
            Reset();
        }

        void AgentBWins(float reward = 1)
        {
            playground.agentA.SetReward(-reward);
            playground.agentB.SetReward(reward);
            Reset();
        }

        public Action<Collision> CollisionEnter;

        /*
        void OnTriggerEnter(Collision collision)
        {
            if (CollisionEnter!=null)
            {
                CollisionEnter(collision);
            }
            if (collision.gameObject.name == "over")
            {
                // agent can return serve in the air
                if (lastFloorHit != FloorHit.FloorHitUnset && !net)
                {
                    net = true;
                }
        
                if (lastAgentHit == AgentRole.A)
                {
                    playground.agentA.AddReward(0.6f);
                }
                else if (lastAgentHit == AgentRole.B)
                {
                    playground.agentB.AddReward(0.6f);
                }
            }
        }
        */

        void OnCollisionEnter(Collision collision)
        {
            if (CollisionEnter != null)
            {
                CollisionEnter(collision);
            }

            if (collision.gameObject.CompareTag("iWall"))
            {
                if (collision.gameObject.name == "wallA")
                {
                    // Agent A hits into wall or agent B hit a winner
                    if (lastAgentHit == AgentRole.A || lastFloorHit == FloorHit.FloorAHit)
                    {
                        AgentBWins();
                    }
                    // Agent B hits long
                    else // if (lastAgentHit == AgentRole.B)
                    {
                        AgentAWins();
                    }

                    // else
                    // {
                    //     Reset();
                    // }
                }
                else if (collision.gameObject.name == "wallB")
                {
                    // Agent B hits into wall or agent A hit a winner
                    if (lastAgentHit == AgentRole.B || lastFloorHit == FloorHit.FloorBHit)
                    {
                        AgentAWins();
                    }
                    // Agent A hits long
                    else // if (lastAgentHit == AgentRole.A)
                    {
                        AgentBWins();
                    }

                    // else
                    // {
                    //     Reset();
                    // }
                }
                else if (collision.gameObject.name == "floorA")
                {
                    // Agent A hits into floor, double bounce or service
                    if (lastFloorHit == FloorHit.FloorAHit // double bounce
                        || lastFloorHit == FloorHit.Service)
                    {
                        AgentBWins();
                    }
                    else
                    {
                        lastFloorHit = FloorHit.FloorAHit;
                    }
                }
                else if (collision.gameObject.name == "floorB")
                {
                    // Agent B hits into floor, double bounce or service
                    if (lastFloorHit == FloorHit.FloorBHit
                        || lastFloorHit == FloorHit.Service)
                    {
                        AgentAWins();
                    }
                    else
                    {
                        lastFloorHit = FloorHit.FloorBHit;
                    }
                }
                else if (collision.gameObject.name == "net")
                {
                    if (lastAgentHit == AgentRole.A)
                    {
                        AgentBWins();
                    }
                    else if (lastAgentHit == AgentRole.B)
                    {
                        AgentAWins();
                    }
                    else
                    {
                        Reset();
                    }
                }

                // else if (collision.gameObject.name == "over")
                // {
                //     // agent can return serve in the air
                //     if (lastFloorHit != FloorHit.FloorHitUnset && !net)
                //     {
                //         net = true;
                //     }
                //
                //     if (lastAgentHit == AgentRole.A)
                //     {
                //         playground.agentA.AddReward(0.6f);
                //     }
                //     else if (lastAgentHit == AgentRole.B)
                //     {
                //         playground.agentB.AddReward(0.6f);
                //     }
                // }
            }
            else if (collision.gameObject.CompareTag("agent"))
            {
                if (collision.gameObject.name == "AgentA")
                {
                    playground.agentA.AddReward(0.6f);
                    ++playground.agentA.hitCount;

                    // Agent A double hit
                    if (lastAgentHit == AgentRole.A)
                    {
                        ++playground.agentB.score;
                        AgentBWins();
                    }
                    else
                    {
                        lastAgentHit = AgentRole.A;
                        lastFloorHit = FloorHit.FloorHitUnset;
                    }
                }
                else if (collision.gameObject.name == "AgentB")
                {
                    playground.agentB.AddReward(0.6f);
                    ++playground.agentB.hitCount;

                    // Agent B double hit
                    if (lastAgentHit == AgentRole.B)
                    {
                        ++playground.agentA.score;
                        AgentAWins();
                    }
                    else
                    {
                        lastAgentHit = AgentRole.B;
                        lastFloorHit = FloorHit.FloorHitUnset;
                    }
                }
            }
        }
    }
}