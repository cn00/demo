using System;
using Unity.MLAgents;
using UnityEngine;

namespace ml.Tennis
{
    public class TennisPlayground : MonoBehaviour
    {
        public TennisAgentA agentA;
        public TennisAgentA agentB;
        public TennisBall ball;

        public Vector3 Size = new Vector3(12f, 11f, 5.5f);
        
        public Vector3 G = new Vector3(0f, -9.81f, 0f);

        public int levelOne = 10;

        public readonly Vector3 A = new Vector3(-12f, 0f, -5f);
        public readonly Vector3 B = new Vector3(-12f, 0f, 5f);
        public readonly Vector3 C = new Vector3(12f, 0f, 5f);
        public readonly Vector3 D = new Vector3(12f, 0f, -5f);
    }
}