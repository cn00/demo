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

        public int levelOne = 10;

        public float minPosX = -12f;
        public float maxPosX = 12f;
        public float minPosY = 0f;
        public float maxPosY = 12f;
        public float minPosZ = -5.5f;
        public float maxPosZ = 5.5f;

        public readonly Vector3 A = new Vector3(-12f, 0f, -5f);
        public readonly Vector3 B = new Vector3(-12f, 0f, 5f);
        public readonly Vector3 C = new Vector3(12f, 0f, 5f);
        public readonly Vector3 D = new Vector3(12f, 0f, -5f);
    }
}