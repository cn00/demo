using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class TennisAgent : Agent
{
    [Header("Specific to Tennis")]
    public GameObject ball;
    public bool invertX;
    // public int score;
    public GameObject myArea;
    public float angle;
    public float scale;

    public Text m_TextComponent;
    public Rigidbody m_AgentRb;
    public Rigidbody m_BallRb;
    public float m_InvertMult;
    public EnvironmentParameters m_ResetParams;

    // Looks for the scoreboard based on the name of the gameObjects.
    // Do not modify the names of the Score GameObjects
    const string k_CanvasName = "Canvas";
    const string k_ScoreBoardAName = "ScoreA";
    const string k_ScoreBoardBName = "ScoreB";

    public override void Initialize()
    {
        // m_AgentRb = GetComponent<Rigidbody>();
        // m_BallRb = ball.GetComponent<Rigidbody>();
        var canvas = GameObject.Find(k_CanvasName);
        GameObject scoreBoard;
        m_ResetParams = Academy.Instance.EnvironmentParameters;
        if (invertX)
        {
            scoreBoard = canvas.transform.Find(k_ScoreBoardBName).gameObject;
        }
        else
        {
            scoreBoard = canvas.transform.Find(k_ScoreBoardAName).gameObject;
        }
        m_TextComponent = scoreBoard.GetComponent<Text>();
        SetResetParameters();
    }

    public List<float> Observations;
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(m_InvertMult * (transform.position.x - myArea.transform.position.x));
        sensor.AddObservation(transform.position.y - myArea.transform.position.y);
        sensor.AddObservation(m_InvertMult * m_AgentRb.velocity.x);
        sensor.AddObservation(m_AgentRb.velocity.y);

        sensor.AddObservation(m_InvertMult * (ball.transform.position.x - myArea.transform.position.x));
        sensor.AddObservation(ball.transform.position.y - myArea.transform.position.y);
        sensor.AddObservation(m_InvertMult * m_BallRb.velocity.x);
        sensor.AddObservation(m_BallRb.velocity.y);

        sensor.AddObservation(m_InvertMult * gameObject.transform.rotation.z);

        #if UNITY_EDITOR
        Observations = GetObservations().ToList();
        #endif
    }

    public List<float> m_Actions;
    public override void OnActionReceived(ActionBuffers actionBuffers)

    {
        var continuousActions = actionBuffers.ContinuousActions;
        #if UNITY_EDITOR
        m_Actions = continuousActions.ToList();
        #endif
        var moveX = Mathf.Clamp(continuousActions[0], -1f, 1f) * m_InvertMult;
        var moveY = Mathf.Clamp(continuousActions[1], -1f, 1f);
        var rotateZ = Mathf.Clamp(continuousActions[2], -1f, 1f) * m_InvertMult;

        // if (moveY > 0.5 && transform.localPosition.y < -1.5f)
        // {
        //     m_AgentRb.velocity = new Vector3(m_AgentRb.velocity.x, 7f, 0f);
        // }

        m_AgentRb.velocity = new Vector3(moveX * 30f, /*moveY * 30f*/ m_AgentRb.velocity.y, 0f);

        m_AgentRb.transform.rotation = Quaternion.Euler(0f, -180f, 55f * rotateZ + m_InvertMult * 90f);

        if ( invertX && transform.localPosition.x < -m_InvertMult ||
            !invertX && transform.localPosition.x > -m_InvertMult)
        {
            transform.localPosition = new Vector3(
                -m_InvertMult * 7f,
                transform.localPosition.y,
                transform.localPosition.z);
        }

        m_TextComponent.text = score.ToString();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");    // Racket Movement
        continuousActionsOut[1] = Input.GetKey(KeyCode.Space) ? 1f : 0f;   // Racket Jumping
        continuousActionsOut[2] = Input.GetAxis("Vertical");   // Racket Rotation
    }

    public override void OnEpisodeBegin()
    {
        m_InvertMult = invertX ? -1f : 1f;

        transform.position = new Vector3(-m_InvertMult * Random.Range(6f, 8f), -1.5f, -1.8f) + transform.parent.transform.position;
        m_AgentRb.velocity = new Vector3(0f, 0f, 0f);

        SetResetParameters();
    }

    public void SetRacket()
    {
        angle = m_ResetParams.GetWithDefault("angle", 55);
        gameObject.transform.eulerAngles = new Vector3(
            gameObject.transform.eulerAngles.x,
            gameObject.transform.eulerAngles.y,
            m_InvertMult * angle
        );
    }

    public void SetBall()
    {
        scale = m_ResetParams.GetWithDefault("scale", .5f);
        ball.transform.localScale = new Vector3(scale, scale, scale);
    }

    public void SetResetParameters()
    {
        SetRacket();
        SetBall();
    }
}