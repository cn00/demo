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


        public Vector3 velocityMinInit = new Vector3(60f, 5f, 3f);
        public Vector3 velocityMax = new Vector3(60f, 10f, 10f); // 世界记录 196Km/h = 54m/s
        public Vector3 Velocity
            #if !UNITY_EDITOR
            => rb.velocity
            #endif
            ;
        
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
            if (CollisionEnter != null)
            {
                CollisionEnter(currentCollision);
            }
            
            if (playground.agentA.score > playground.levelOne && playground.agentB.score > playground.levelOne)
            {
            }

            var px = Random.Range(-playground.HalfSize.x, playground.HalfSize.x) > 0 ? 8f : -8f;
            var py = 1f;//Random.Range(4f, playground.HalfSize.y);//
            var pz = 0f;// Random.Range(-playground.HalfSize.z, playground.HalfSize.z);
            transform.localPosition = new Vector3(px, py, pz);

            var vx = (px > 0f ? -1f:1f) * Random.Range(velocityMinInit.x, velocityMax.x); // * 14f; // 
            var vy = Random.Range(velocityMinInit.y, velocityMax.y); // 3.5f;//
            var vz = Random.Range(velocityMinInit.z, velocityMax.z);
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

        private Collision currentCollision;


        void OnCollisionEnter(Collision collision)
        {
            currentCollision = collision;
            // if (CollisionEnter != null)
            // {
            //     CollisionEnter(collision);
            // }

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
                    else // if (lastAgentHit == AgentRole.B)
                    {
                        AgentAWins();
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