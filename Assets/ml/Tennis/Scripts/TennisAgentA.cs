using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Sensors.Reflection;
using Random = UnityEngine.Random;

public class TennisAgentA : Agent
{
    public TennisPlayground playground;
    public Rigidbody rigidbody;

    public uint score;
    public uint hitCount;

    public float scale;
    public bool invertX;
    public float m_InvertMult;
    public float m_velocityMax = 9;
    public float m_rotateMax   = 180f;

    /// <summary>
    /// 目标dian
    /// </summary>
    public Vector3 Pt;

    /// <summary>
    /// 网平面交点
    /// </summary>
    public Vector3 Intersect;
    /// <summary>
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
    public Vector3 GetVelocity(Vector3 p0)
    {
        var G = -9.8f;
        var ball = playground.ball;
        
        var tx = 0f;
        if (ball.lastFloorHit == TennisBall.FloorHit.Service)
            tx = Random.Range(invertX ? playground.minPosX/2f : 0f, invertX ? 0f: playground.maxPosX/2f); // 发球线
        else
            tx = Random.Range(invertX ? playground.minPosX    : 0f, invertX ? 0f: playground.maxPosX);
        Pt = new Vector3(tx, 0f, Random.Range(playground.minPosZ, playground.maxPosZ));
        
        var s = Pt - p0;
        Intersect = IntersectLineToPlane(p0, s, Vector3.right, Vector3.zero);
        
        var pl = new Vector3(0f, Random.Range(1.2f, playground.maxPosY), Intersect.z);
        // FIXME: 
        var a = new Vector3(
            ball.rigidbody.drag*ball.rigidbody.velocity.x*ball.rigidbody.velocity.x/ball.rigidbody.mass,
            (G - ball.rigidbody.drag*ball.rigidbody.velocity.x*ball.rigidbody.velocity.x)/ball.rigidbody.mass,
            ball.rigidbody.drag*ball.rigidbody.velocity.z*ball.rigidbody.velocity.z/ball.rigidbody.mass);
        var vk = new Vector3();
        var v0 = ball.rigidbody.velocity;
        var drag = ball.rigidbody.drag;
        var f = new Vector3(drag*v0.x*v0.x, drag*v0.y*v0.y, drag*v0.z*v0.z);
        return Vector3.zero;
    }
    
    // private void Update()
    // {
    //     GetVelocity(transform.localPosition);
    // }

    /// <summary>
    /// 计算直线与平面的交点
    /// </summary>
    /// <param name="point">直线上某一点</param>
    /// <param name="direct">直线的方向</param>
    /// <param name="planeNormal">平面法向量</param>
    /// <param name="planePoint">平面内任意点</param>
    /// <returns></returns>
    public static Vector3 IntersectLineToPlane(Vector3 point, Vector3 direct, Vector3 planeNormal, Vector3 planePoint)
    {
        float d = Vector3.Dot(planePoint - point, planeNormal) / Vector3.Dot(direct.normalized, planeNormal);
        return d * direct.normalized + point;
    }
    /// <summary>
    /// 确定坐标是否在平面内
    /// </summary>
    /// <returns></returns>
    public static bool IsVecPosPlane(Vector3[] vecs, Vector3 pos)
    {
        float RadianValue = 0;
        Vector3 vecOld = Vector3.zero;
        Vector3 vecNew = Vector3.zero;
        for (int i = 0; i < vecs.Length; i++)
        {
            if (i == 0)
            {
                vecOld = vecs[i] - pos;
            }
            if (i == vecs.Length - 1)
            {
                vecNew = vecs[0] - pos;
            }
            else
            {
                vecNew = vecs[i + 1] - pos;
            }
            RadianValue += Mathf.Acos(Vector3.Dot(vecOld.normalized, vecNew.normalized)) * Mathf.Rad2Deg;
            vecOld = vecNew;
        }
        if (Mathf.Abs(RadianValue - 360) < 0.1f)
        {
            return true;
        }
        else
        {
            return false;
        }
        //vecOld = vecs[0] - pos;
        //vecNew = vecs[1] - pos;
        //RadianValue += Mathf.Acos(Vector3.Dot(vecOld.normalized, vecNew.normalized)) * Mathf.Rad2Deg;
        
        //vecOld = vecs[1] - pos;
        //vecNew = vecs[2] - pos;
        //RadianValue += Mathf.Acos(Vector3.Dot(vecOld.normalized, vecNew.normalized)) * Mathf.Rad2Deg;
        
        //vecOld = vecs[2] - pos;
        //vecNew = vecs[0] - pos;
        //RadianValue += Mathf.Acos(Vector3.Dot(vecOld.normalized, vecNew.normalized)) * Mathf.Rad2Deg;
    }

    public EnvironmentParameters m_EnvParams = null;
    public EnvironmentParameters EnvParams
    {
        get
        {
            if (m_EnvParams == null)
            {
                m_EnvParams = Academy.Instance.EnvironmentParameters;
            }
            return m_EnvParams;
        }
    }

    // [Header("i_1:p_3:r_4:v_3:l_1:lbr_4:bp_3")]
    // public List<float> Observations;
    public override void Initialize() // OnEnable
    {
        m_EnvParams = Academy.Instance.EnvironmentParameters;
        m_InvertMult = invertX ? -1f : 1f;

        Reset();
    }

    
    /// <summary>
    /// <br/>为了使代理学习，观察应包括代理完成其任务所需的所有信息。如果没有足够的相关信息，座席可能会学得不好或根本不会学。
    /// <br/>确定应包含哪些信息的合理方法是考虑计算该问题的分析解决方案所需的条件，或者您希望人类能够用来解决该问题的方法。<br/>
    /// <br/>产生观察
    /// <br/>   ML-Agents为代理提供多种观察方式：
    /// <br/>
    /// <br/>重写Agent.CollectObservations()方法并将观测值传递到提供的VectorSensor。
    /// <br/>将[Observable]属性添加到代理上的字段和属性。
    /// <br/>ISensor使用SensorComponent代理的附件创建来实现接口ISensor。
    /// <br/>Agent.CollectObservations（）
    /// <br/>Agent.CollectObservations（）最适合用于数字和非可视环境。Policy类调用CollectObservations(VectorSensor sensor)每个Agent的 方法。
    /// <br/>此函数的实现必须调用VectorSensor.AddObservation添加矢量观测值。
    /// <br/>该VectorSensor.AddObservation方法提供了许多重载，可将常见类型的数据添加到观察向量中。
    /// <br/>您可以直接添加整数和布尔值，以观测向量，以及一些常见的统一数据类型，如Vector2，Vector3和Quaternion。
    /// <br/>有关各种状态观察功能的示例，您可以查看ML-Agents SDK中包含的 示例环境。例如，3DBall示例使用平台的旋转，球的相对位置和球的速度作为状态观察。
    /// </summary>
    /// <param name="sensor" type="VectorSensor"></param>
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(m_InvertMult);                        // 角色 x1
        
        sensor.AddObservation(transform.localPosition);             // 位置 x3
        sensor.AddObservation(transform.localRotation.eulerAngles); // 角度 x3
        sensor.AddObservation(rigidbody.velocity);                  // 速度 x3

        sensor.AddObservation(playground.ball.transform.localPosition);       // 球位置 x3
        sensor.AddObservation(playground.ball.rigidbody.velocity);            // 球速度 x3
        sensor.AddObservation(playground.ball.rigidbody.angularVelocity );    // 角速度 x3
    }

    [Header("v_3:r_4")]
    public List<float> m_Actions;

    /**
     * 动作是代理执行的来自策略的指令。当学院调用代理的OnActionReceived()功能时，该操作将作为参数传递给代理。
     * 代理的动作可以采用两种形式之一，即Continuous或Discrete。
     * 当您指定矢量操作空间为Continuous时，传递给Agent的action参数是长度等于该Vector Action Space Size属性的浮点数数组。
     * 当您指定 离散向量动作空间类型时，动作参数是一个包含整数的数组。每个整数都是命令列表或命令表的索引。
     * 在离散向量操作空间类型中，操作参数是索引数组。数组中的索引数由Branches Size属性中定义的分支数确定。
     * 每个分支对应一个动作表，您可以通过修改Branches 属性来指定每个表的大小。
     * 策略和训练算法都不了解动作值本身的含义。训练算法只是为动作列表尝试不同的值，并观察随着时间的推移和许多训练事件对累积奖励的影响。
     * 因此，仅在OnActionReceived()功能中为代理定义了放置动作。
     * 例如，如果您设计了一个可以在两个维度上移动的代理，则可以使用连续或离散矢量动作。
     * 在连续的情况下，您可以将矢量操作大小设置为两个（每个维一个），并且座席的策略将创建一个具有两个浮点值的操作。
     * 在离散情况下，您将使用一个分支，其大小为四个（每个方向一个），并且策略将创建一个包含单个元素的操作数组，其值的范围为零到三。
     * 或者，您可以创建两个大小为2的分支（一个用于水平移动，一个用于垂直移动），并且Policy将创建一个包含两个元素的操作数组，其值的范围从零到一。
     * 请注意，在为代理编程动作时，使用代理Heuristic()方法测试动作逻辑通常会很有帮助，该方法可让您将键盘命令映射到动作。
     */
    
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var continuousActions = actionBuffers.ContinuousActions;
        #if UNITY_EDITOR
        m_Actions = continuousActions.ToList();
        #endif
        
        int i = 0;
        var velocityX = Mathf.Clamp(continuousActions[i++], -1f, 1f) * m_velocityMax;
        var velocityY = Mathf.Clamp(continuousActions[i++], -1f, 1f) * m_velocityMax;
        var velocityZ = Mathf.Clamp(continuousActions[i++], -1f, 1f) * m_velocityMax;
        var rotateX   = Mathf.Clamp(continuousActions[i++], -1f, 1f) * m_rotateMax;
        var rotateY   = Mathf.Clamp(continuousActions[i++], -1f, 1f) * m_rotateMax;
        var rotateZ   = Mathf.Clamp(continuousActions[i++], -1f, 1f) * m_rotateMax;
        // var rotateW     = Mathf.Clamp(continuousActions[i++], -1f, 1f);

        // // 不干预决策，在 TennisBall 中限制球的运动范围来引导
        // if (playground.agentA.score < playground.levelOne || playground.agentB.score < playground.levelOne)
        // {
        //     rotateX = invertX ? 180f : 0f;
        //     rotateY = 0f;
        //     velocityZ = 0f;
        // }

        rigidbody.velocity = new Vector3(velocityX, velocityY, velocityZ);
        
        rigidbody.rotation = Quaternion.Euler(new Vector3(rotateX, rotateY, rotateZ));// 这比使用Transform.rotation更新旋转速度更快
        // or 
        // transform.localEulerAngles = new Vector3(rotateX, rotateY, rotateZ);

    }

    private void FixedUpdate()
    {
        var p = transform.localPosition;
        var rp = rigidbody.position;
        transform.localPosition = new Vector3(
            Mathf.Clamp(p.x, invertX ? 0f : playground.minPosX, invertX ? playground.maxPosX : 0f ),
            Mathf.Clamp(p.y, 0f, 3f),
            Mathf.Clamp(p.z, playground.minPosZ, playground.maxPosZ));
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");              // moveX Racket Movement
        continuousActionsOut[1] = Input.GetKey(KeyCode.Space) ? 1f : 0f;    // moveY Racket Jumping
        continuousActionsOut[2] = Input.GetAxis("Vertical");                // moveZ
        if(SystemInfo.supportsGyroscope)
        {
            var ang = Input.gyro.attitude.eulerAngles;
            continuousActionsOut[3] = Input.gyro.attitude.x; // rotateX
            continuousActionsOut[4] = Input.gyro.attitude.y; // rotateY
            continuousActionsOut[5] = Input.gyro.attitude.z; // rotateZ
            // continuousActionsOut[6] = Input.gyro.attitude.w; // rotateW
        }
        else
        {
            continuousActionsOut[0] = Random.Range(-1f, 1f); // moveX Racket Movement
            continuousActionsOut[1] = Random.Range(-1f, 1f); // moveY Racket Jumping
            continuousActionsOut[2] = Random.Range(-1f, 1f); // moveZ
            continuousActionsOut[3] = Random.Range(-1f, 1f); // rotateX
            continuousActionsOut[4] = Random.Range(-1f, 1f); // rotateY
            continuousActionsOut[5] = Random.Range(-1f, 1f); // rotateZ
            // continuousActionsOut[6] = Random.Range(-1f, 1f); // rotateW
        }
    }

    public Action episodeBeginAction;
    public override void OnEpisodeBegin()
    {
        Reset();
        
        if(episodeBeginAction != null)
            episodeBeginAction();
    }
    
    public void Reset()
    {
        transform.localPosition = new Vector3(
            -m_InvertMult * 8,
            2f,
            m_InvertMult * 1.5f);

        rigidbody.velocity = new Vector3(0f, 0f, 0f);
        transform.eulerAngles = new Vector3(
            0f,
            invertX ? 0f : 180f,
            -55f
        );
    }
}
