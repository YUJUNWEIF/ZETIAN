using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using RVO;

public class AgentObj : MonoBehaviour, RVO.IAgentParam
{
    public Agent agent;
    public FEVector2D goal;
    FEVector2D m_targetDir;
    bool m_stop;
    public void SetPreferVelocity()
    {
        FEVector2D goalVector = goal - agent.position;

        if (goalVector.sqrMagnitude > 1.0f)
        {
            goalVector.Normalize();
        }

        float angle = Random.Range(0f, 1f) * 2.0f * Mathf.PI;
        float dist = Random.Range(0f, 1f) * 0.1f;

        m_targetDir = (goalVector + dist * new FEVector2D(Mathf.Cos(angle), Mathf.Sin(angle))) * maxSpeed;
    }

    public bool ReachGoal()
    {
        return m_stop = (agent.position - goal).sqrMagnitude < 400.0f;
    }
    public float radius { get { return 2.0f; } }
    public float maxSpeed { get { return 10.0f; } }
    public bool stop { get { return m_stop; } }
    public FEVector2D targetDir { get { return m_targetDir; } }

    public void SetPosition()
    {
        if (!agent.stop)
        {
            transform.position = new Vector3(agent.position.x, 0, agent.position.y);
        }
    }
}
