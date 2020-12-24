using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class TennisArea : MonoBehaviour
{
    public GameObject ball;
    public GameObject agentA;
    public GameObject agentB;
    Rigidbody m_BallRb;

    public float minPosX = -12f;
    public float maxPosX =  12f;
    public float minPosY = -7f;
    public float maxPosY =  3f;
    public float minPosZ = -5.5f;
    public float maxPosZ =  5.5f;

    // Use this for initialization
    void Start()
    {
        m_BallRb = ball.GetComponent<Rigidbody>();
        MatchReset();
    }

    public void MatchReset()
    {
        // var ballOut = Random.Range(minPosX, maxPosX);
        // var flip = Random.Range(0, 2);
        // if (flip == 0)
        // {
        //     ball.transform.localPosition = new Vector3(-ballOut, Random.Range(minPosY, maxPosY), Random.Range(minPosZ, maxPosZ));
        // }
        // else
        // {
        //     ball.transform.localPosition = new Vector3(ballOut, Random.Range(minPosY, maxPosY), Random.Range(minPosZ, maxPosZ));
        // }
        
        ball.transform.localPosition = new Vector3(
            Random.Range(minPosX, maxPosX), 
            Random.Range(minPosY, maxPosY), 
            Random.Range(minPosZ, maxPosZ));
        m_BallRb.velocity = new Vector3(Random.Range(0.01f, 0.2f), Random.Range(0.01f, 0.3f), Random.Range(-0.1f, 0.1f));
        ball.transform.localScale = new Vector3(.5f, .5f, .5f);
    }

    void FixedUpdate()
    {
        // var rgV = m_BallRb.velocity;
        // m_BallRb.velocity = new Vector3(
        //     Mathf.Clamp(rgV.x, -9f, 9f),
        //     Mathf.Clamp(rgV.y, -9f, 9f),
        //     Mathf.Clamp(rgV.z, -9f, 9f));
    }
}
