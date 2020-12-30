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

        public Vector3 maxVelocity = new Vector3(60f, 10f, 10f); // 世界记录 196Km/h = 54m/s
        public Vector3 Velocity
            #if !UNITY_EDITOR
            => rigidbody.velocity
            #endif
            ;

        /// <summary>
        /// 网平面交点
        /// </summary>
        public Vector3 Intersect;

        /// <summary>
        /// 落地点
        /// </summary>
        public Vector3 Tp;

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
        public Vector3 GetTargetPos(float y = 0f)
        {
            var s = transform.localPosition;
            if(s.y > y) s.y -= y;
            var G = playground.G;
            var v = Velocity;
            var drag = rigidbody.drag;
            var mass = rigidbody.mass;

            var a = new Vector3(
                (drag * v.x * v.x) / mass * (v.x > 0f ? -1f:1f),
                (drag * v.y * v.y) / mass * (v.y > 0f ? -1f:1f) + G,
                (drag * v.z * v.z) / mass * (v.z > 0f ? -1f:1f));
            
            // ax^2 + bx + c = 0 ==> x = [-b ± √(b^2-4ac)]/(2a)
            // at^2/2 + vt + y = 0 => t = (sqrt(v^2 -4*a/2*y)-v)/(2*a/2)
            var t = (v.y + Mathf.Sqrt(v.y * v.y - 4f * a.y / 2f * (-s.y))) / (a.y);
            Tt = t;
            
            var tp = new Vector3(
                s.x + a.x * t * t / 2f + v.x * t,
                y,
                s.z + a.z * t * t / 2f + v.z * t);
            var tpx = a.x * t * t / 2f + v.x * t;
            var tpz = a.z * t * t / 2f + v.z * t;

            return tp;
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

            var rgV = rigidbody.velocity;
            rigidbody.velocity = new Vector3(
                Mathf.Clamp(rgV.x, -maxVelocity.x, maxVelocity.z),
                Mathf.Clamp(rgV.y, -maxVelocity.y, maxVelocity.y),
                Mathf.Clamp(rgV.z, -maxVelocity.z, maxVelocity.z));
            
            #if UNITY_EDITOR
            Velocity = rigidbody.velocity;
            #endif

            // //
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
                pz = Random.Range(-playground.Size.z, playground.Size.z);
                vz = Random.Range(-1f, 1f);
            }

            var px = Random.Range(-playground.Size.x, playground.Size.x);
            var py = Random.Range(4f, playground.Size.y);
            transform.localPosition = new Vector3(px, py, pz);
            
            var vx = Random.Range(0, maxVelocity.x) * (px > 0f ? -1f:1f);
            var vy = Random.Range(-maxVelocity.y, maxVelocity.y);
            rigidbody.velocity = new Vector3(vx, vy,vz);
            
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