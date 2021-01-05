using System;
using Unity.MLAgents;
using UnityEngine;

namespace ml.Tennis
{
    public class TennisPlayground : MonoBehaviour
    {
        public TennisBall ball;
        public TennisAgentA[] agents = new TennisAgentA[4];
        public TennisAgentA agentA => agents[0];
        public TennisAgentA agentB => agents[1];
        public TennisAgentA agentA2 => agents[2];
        public TennisAgentA agentB2 => agents[3];

        public Vector3 HalfSize = new Vector3(12f, 11f, 5.5f);
        
        public Vector3 G = new Vector3(0f, -9.81f, 0f);

        public bool IsDouble = false;
        public int levelOne = 10;

        public readonly Vector3 A = new Vector3(-12f, 0f, -5f);
        public readonly Vector3 B = new Vector3(-12f, 0f, 5f);
        public readonly Vector3 C = new Vector3(12f, 0f, 5f);
        public readonly Vector3 D = new Vector3(12f, 0f, -5f);
    }
}