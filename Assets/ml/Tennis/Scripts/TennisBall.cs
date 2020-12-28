using System;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class TennisBall : MonoBehaviour
{
    public TennisPlayground playground;
    public Rigidbody rigidbody;

    public float minPosX = -12f;
    public float maxPosX =  12f;
    public float minPosY = 0f;
    public float maxPosY =  12f;
    public float minPosZ = -5.5f;
    public float maxPosZ =  5.5f;
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
            Mathf.Clamp(bp.x, minPosX, maxPosX),
            Mathf.Clamp(bp.y, minPosY, maxPosY),
            Mathf.Clamp(bp.z, minPosZ, maxPosZ));

    }

    void Reset()
    {
        var pz = 0f;
        var vz = 0f;
        if (playground.agentA.score > playground.levelOne && playground.agentB.score > playground.levelOne )
        {
            pz = Random.Range(minPosZ, maxPosZ);
            vz = Random.Range(-1f, 1f);
        }

        transform.localPosition = new Vector3(
            Random.Range(minPosX, maxPosX), 
            Random.Range(4f, maxPosY), 
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
        playground.agentA.SetReward( reward);
        playground.agentB.SetReward(-reward);
        Reset();
    }

    void AgentBWins(float reward = 1)
    {
        playground.agentA.SetReward(-reward);
        playground.agentB.SetReward( reward);
        Reset();
    }

    void OnCollisionEnter(Collision collision)
    {
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
                else if (lastAgentHit == AgentRole.B)
                {
                    AgentAWins();
                }
                else
                {
                    Reset();
                }
            }
            else if (collision.gameObject.name == "wallB")
            {
                // Agent B hits into wall or agent A hit a winner
                if (lastAgentHit == AgentRole.B || lastFloorHit == FloorHit.FloorBHit)
                {
                    AgentAWins();
                }
                // Agent A hits long
                else if (lastAgentHit == AgentRole.A)
                {
                    AgentBWins();
                }
                else
                {
                    Reset();
                }
            }
            else if (collision.gameObject.name == "floorA")
            {
                // Agent A hits into floor, double bounce or service
                if (lastFloorHit == FloorHit.FloorAHit
                    || lastFloorHit == FloorHit.Service)
                {
                    if (lastAgentHit == AgentRole.A)
                        AgentBWins();
                    else
                    {
                        Reset();
                    }
                }
                else
                {
                    lastFloorHit = FloorHit.FloorAHit;
                    //successful serve 过网？
                    if (lastAgentHit == AgentRole.B && !net)
                    {
                        net = true;
                    }
                }
            }
            else if (collision.gameObject.name == "floorB")
            {
                // Agent B hits into floor, double bounce or service
                if (lastFloorHit == FloorHit.FloorBHit
                    || lastFloorHit == FloorHit.Service)
                {
                    if (lastAgentHit == AgentRole.B)
                        AgentAWins();
                    else
                    {
                        Reset();
                    }
                }
                else
                {
                    lastFloorHit = FloorHit.FloorBHit;
                    //successful serve
                    if (lastAgentHit == AgentRole.A && !net)
                    {
                        net = true;
                    }
                }
            }
            else if (collision.gameObject.name == "net" && !net)
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
        }
        else if (collision.gameObject.CompareTag("agent"))
        {
            if (collision.gameObject.name == "AgentA")
            {
                playground.agentA.AddReward(0.6f);
                ++playground.agentA.hitCount;
                // Debug.LogWarning($"congratulations AgentA hit the ball. score:{playground.agentA.score:0.000000}:{playground.agentB.score:0.000000}");

                // Agent A double hit
                if (lastAgentHit == AgentRole.A)
                {
                    ++playground.agentB.score;
                    AgentBWins();
                }
                else
                {
                    // agent can return serve in the air
                    if (lastFloorHit != FloorHit.Service && !net)
                    {
                        net = true;
                    }

                    lastAgentHit = AgentRole.A;
                    lastFloorHit = FloorHit.FloorHitUnset;
                }
            }
            else if (collision.gameObject.name == "AgentB")
            {
                playground.agentB.AddReward(0.6f);
                ++playground.agentB.hitCount;
                // Debug.LogWarning($"congratulations AgentB hit the ball. score:{playground.agentA.score:0.000000}:{playground.agentB.score:0.000000}");

                // Agent B double hit
                if (lastAgentHit == AgentRole.B)
                {
                    ++playground.agentA.score;
                    AgentAWins();
                }
                else
                {
                    if (lastFloorHit != FloorHit.Service && !net)
                    {
                        net = true;
                    }

                    lastAgentHit = AgentRole.B;
                    lastFloorHit = FloorHit.FloorHitUnset;
                }
            }
        }
    }
}
