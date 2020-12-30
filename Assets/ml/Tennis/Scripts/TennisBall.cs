using System;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace ml.Tennis
{
    public class TennisBall : MonoBehaviour
    {
        public TennisPlayground playground;
        public Rigidbody rigidbody;

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


        /// <summary>
        /// 落地点
        /// </summary>
        public Vector3 Tp;

        /// <summary>
        /// 网平面交点
        /// </summary>
        public Vector3 Intersect;

        public float Tt = 0f;

        /// - 击球点 P0, 球速 V0, 重力 G = 9.8 (N/kg), 空阻 F = kv^2
        /// - 对方场地随机取一点 Pt
        /// - 求得过网垂线 L
        /// - L 上取一点 Pl
        /// - 求得抛物线方程
        /// - 求得反射速度 Vk(x,y,x)
        /// - 求击球速度 Va, 球拍角度 ℷ
        /// 空阻 f(V) = kv^2
        /// Ax = Fx/M 
        /// v(X, Y, Z) = {
        ///    V(x) = Vx + tAx
        ///    V(y) = Vy + tAy
        ///    V(z) = Vz + tAz
        /// } (m/s“)
        /// </summary>
        public Vector3 GetTargetPos()
        {
            var G = 9.8f;
            var p = transform.localPosition;
            var v = rigidbody.velocity;
            var ay = G - (rigidbody.drag * v.y * v.y) / rigidbody.mass;
            // ax^2 + bx + c = 0 ==> x = [-b ± √(b^2-4ac)]/(2a)
            // at^2/2 + vt + y = 0 => t = (sqrt(v^2 -4*a/2*y)-v)/(2*a/2)
            var t = (v.y + Mathf.Sqrt(v.y * v.y - 4f * ay / 2f * (-p.y))) / (ay);
            Tt = t;

            var ax = -(rigidbody.drag * v.x * v.x) / rigidbody.mass;
            var tpx = ax * t * t / 2f + v.x * t;
            var az = -(rigidbody.drag * v.z * v.z) / rigidbody.mass;
            var tpz = az * t * t / 2f + v.z * t;

            Tp = new Vector3(p.x + tpx, 0f, p.z + tpz);
            return Tp;
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
            {
                var rgV = rigidbody.velocity;
                rigidbody.velocity = new Vector3(
                    Mathf.Clamp(rgV.x, -m_velocityMax, m_velocityMax),
                    Mathf.Clamp(rgV.y, -m_velocityMax, m_velocityMax),
                    Mathf.Clamp(rgV.z, -m_velocityMax, m_velocityMax));
            }

            var bp = transform.localPosition;
            transform.localPosition = new Vector3(
                Mathf.Clamp(bp.x, 2f * playground.minPosX, 2f * playground.maxPosX),
                Mathf.Clamp(bp.y, playground.minPosY, 2f * playground.maxPosY),
                Mathf.Clamp(bp.z, 2f * playground.minPosZ, 2f * playground.maxPosZ));

        }

        void Reset()
        {
            var pz = 0f;
            var vz = 0f;
            if (playground.agentA.score > playground.levelOne && playground.agentB.score > playground.levelOne)
            {
                pz = Random.Range(playground.minPosZ, playground.maxPosZ);
                vz = Random.Range(-1f, 1f);
            }

            transform.localPosition = new Vector3(
                Random.Range(playground.minPosX, playground.maxPosX),
                Random.Range(4f, playground.maxPosY),
                pz);
            rigidbody.velocity = new Vector3(
                Random.Range(0.01f, 0.2f),
                Random.Range(0.01f, 0.3f),
                vz);
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