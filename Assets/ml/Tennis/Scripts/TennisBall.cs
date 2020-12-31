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

        /// <summary>
        /// 网平面交点
        /// </summary>
        public Vector3 Intersect;

        /// <summary>
        /// 落地点
        /// </summary>
        // public Vector3 Tp;

        public float Tt = 0f;

        /// <br/> - 击球点 P0, 球速 V0, 重力 G = 9.8 (N/kg), 空阻 F = kv^2
        /// <br/> - 对方场地随机取一点 Pt
        /// <br/> - 求得过网垂线 L
        /// <br/> - L 上取一点 Pl
        /// <br/> - 求得抛物线方程
        /// <br/> - 求得反射速度 Vk(x,y,x)
        /// <br/> - 求击球速度 Va, 球拍角度 ℷ
        /// <br/> 空阻 f(V) = kv^2 // https://www.zhihu.com/question/68565717
        /// <br/> Ax = Fx/M 
        /// <br/> v(X, Y, Z) = {
        /// <br/>    V(x) = Vx + tAx
        /// <br/>    V(y) = Vy + tAy
        /// <br/>    V(z) = Vz + tAz
        /// <br/> } (m/s“)
        /// <br/> Unity3D中常用的物理学公式 https://www.cnblogs.com/msxh/p/6128851.html
        /// <br/> Unity 如何计算阻力？ https://www.leadwerks.com/community/topic/4385-physics-how-does-unity-calculate-drag/
        public Vector3 GetTargetPos(float y = 0f)
        {
            //
            var G = playground.G;
            var v = Velocity;
            var drag = rb.drag; // 0.47 https://www.jianshu.com/p/9da46cf6d5f5
            var m = rb.mass;

            var ad = - Mathf.Pow(v.magnitude, 2) * drag * v.normalized;
            
            // FIXIT: 微积分方程求解更精确 https://www.zhihu.com/question/68565717
            var a1 = G + ad;
            var a2 = G - ad;

            var tp = new Vector3();
            // transform.localPosition.Set(0,0,0);
            var s = transform.localPosition;
            var t = 0f;
            if (v.y > 0f)
            {
                t = v.y / a1.y;
                var s1 = - a1.y * t * t / 2f;
                var s2 = s1 + s.y;
                if (s2 > y) s2 -= y;
                var t2 = Mathf.Sqrt(-2f * s2 / a2.y);
                t += t2;
                tp.Set(
                    s.x + a2.x * t * t / 2f + v.x * t,
                    y,
                    s.z + a2.z * t * t / 2f + v.z * t);
            }
            else
            {
                if(s.y > y) s.y -= y;
                // att/2 + vt - s = 0 ==> t = [-v ± √(vv+2as)]/a
                // ax^2 + bx + c = 0 ==> x = [-b ± √(b^2-4ac)]/(2a)
                // at^2/2 + vt - sy = 0 => t = (sqrt(v^2 -4*a/2*y)-v)/(2*a/2)
                // t = (v.y + Mathf.Sqrt(v.y * v.y - 4f * a1.y / 2f * (-s.y))) / (a1.y);
                
                t = (v.y + Mathf.Sqrt(v.y * v.y + 2f * (-a2.y) * (s.y))) / (-a2.y);
                tp.Set(
                    s.x + a1.x * t * t / 2f + v.x * t,
                    y,
                    s.z + a1.z * t * t / 2f + v.z * t);
            }
            Tt = t;
            
            return tp;
        }

        /*
        public float GetTargetT(float y)
        {
            var G = playground.G;
            var v = Velocity;
            var drag = rigidbody.drag; // 0.47 https://www.jianshu.com/p/9da46cf6d5f5
            var m = rigidbody.mass;
            var s = transform.localPosition;

            var ad = - Mathf.Pow(v.magnitude, 2) * drag * v.normalized;
            
            // FIXIT: 微积分方程求解更精确 https://www.zhihu.com/question/68565717
            var a1 = G + ad;

            var a2 = G - ad;
            var t = 0f;
            if (v.y > 0f)
            {
                t = v.y / a1.y;
                var s1 = - a1.y * t * t / 2f;
                var s2 = s1 + s.y;
                if (s2 > y) s2 -= y;
                var t2 = Mathf.Sqrt(-2f * s2 / a2.y);
                t += t2;
            }
            else
            {
                if(s.y > y) s.y -= y;
                // att/2 + vt - s = 0 ==> t = [-v ± √(vv+2as)]/a
                // ax^2 + bx + c = 0 ==> x = [-b ± √(b^2-4ac)]/(2a)
                // at^2/2 + vt - sy = 0 => t = (sqrt(v^2 -4*a/2*y)-v)/(2*a/2)
                // t = (v.y + Mathf.Sqrt(v.y * v.y - 4f * a1.y / 2f * (-s.y))) / (a1.y);
                
                t = (v.y + Mathf.Sqrt(v.y * v.y + 2f * (-a2.y) * (s.y))) / (-a2.y);
            }

            return t;
        }
        */

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
                pz = Random.Range(-playground.Size.z, playground.Size.z);
                vz = Random.Range(-1f, 1f);
            }

            var px = Random.Range(-playground.Size.x, playground.Size.x);
            var py = Random.Range(4f, playground.Size.y);
            transform.localPosition = new Vector3(px, py, pz);
            
            var vx = Random.Range(0, maxVelocity.x) * (px > 0f ? -1f:1f);
            var vy = Random.Range(-maxVelocity.y, maxVelocity.y);
            rb.velocity = new Vector3(vx, vy,vz);
            
            transform.localScale = new Vector3(.5f, .5f, .5f);

            playground.agentA.EndEpisode();
            playground.agentB.EndEpisode();
            lastFloorHit = FloorHit.Service;
            lastAgentHit = AgentRole.O;
            net = false;
            
            GetTargetPos();
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