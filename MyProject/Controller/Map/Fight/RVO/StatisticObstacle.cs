using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using RVO;

public class StatisticObstacle : MonoBehaviour
{
    public GameObject prefab;
    public GameObject go;
    RVO.Simulator simulator;

    public Agent NewAgent(FEVector2D position, FEVector2D goal)
    {
        GameObject g = GameObject.Instantiate(prefab);
        g.SetActive(true);
        var script = g.AddComponent<AgentObj>();
        g.transform.localScale = new Vector3(script.radius * 2f, 1f, script.radius * 2f);
        script.agent = new Agent();
        script.agent.Initialize(script, 10, position);
        script.goal = goal;
        return script.agent;
    }
    public void SetAgents()
    {
        FEVector2D pos;
        FEVector2D goal;
        for (int i = 0; i < 5; ++i)
        {
            for (int j = 0; j < 5; ++j)
            {
                pos = new FEVector2D(55.0f + i * 10.0f, 55.0f + j * 10.0f);
                goal = new FEVector2D(-75.0f, -75.0f);
                simulator.AddAgent(NewAgent(pos, goal));

                pos = new FEVector2D(-55.0f - i * 10.0f, 55.0f + j * 10.0f);
                goal = new FEVector2D(75.0f, -75.0f);
                simulator.AddAgent(NewAgent(pos, goal));

                pos = new FEVector2D(55.0f + i * 10.0f, -55.0f - j * 10.0f);
                goal = new FEVector2D(-75.0f, 75.0f);
                simulator.AddAgent(NewAgent(pos, goal));

                pos = new FEVector2D(-55.0f - i * 10.0f, -55.0f - j * 10.0f);
                goal = new FEVector2D(75.0f, 75.0f);
                simulator.AddAgent(NewAgent(pos, goal));
            }
        }
    }
    public void setPreferredVelocities()
    {
        var no = simulator.agents_.First;
        while (no != null)
        {
            var agent = no.Value;
            agent.As<AgentObj>().SetPreferVelocity();
            no = no.Next;
        }
    }

    public bool reachedGoal()
    {
        var no = simulator.agents_.First;
        while (no != null)
        {
            var agent = no.Value;
            if( !agent.As<AgentObj>().ReachGoal()) { return false; }
            no = no.Next;
        }
        return true;
    }

    void DrawObstacleMap(RVO.ObstacleMap obsMap)
    {
        for (int index = 0; index < obsMap.obstacles.Count; ++index)
        {
            DrawObstacle(obsMap, obsMap.obstacles[index]);
        }
    }
    void DrawObstacle(RVO.ObstacleMap obsMap, RVO.ObstacleMap.Obs obstacle)
    {
        var vs = new Vector3[obstacle.vss.Count + 1];
        for (int index = 0; index < obstacle.vss.Count; ++index)
        {
            var v = obstacle.vss[index];
            vs[index] = new Vector3(v.x, 0f, v.y);
        }
        vs[obstacle.vss.Count] = vs[0];

        var line = new GameObject().AddComponent<LineRenderer>();
        line.positionCount = vs.Length;
        line.SetPositions(vs);
    }

    void SetObs()
    {
        var data = File.ReadAllBytes("Assets/StreamingAssets/Test.bin");
        var ms = new MemoryStream(data);
        var obsMap = new ObstacleMap();
        obsMap.Unmarsh(new Util.InStream(ms));

        for (int index = 0; index < obsMap.obstacles.Count; ++index)
        {
            var obs = obsMap.obstacles[index];
            simulator.addObstacle(obs.vss);
        }
        simulator.processObstacles();
        DrawObstacleMap(obsMap);
    }
    void Start()
    {
        simulator = new RVO.Simulator();        
        SetAgents();
        SetObs();        
        LateUpdate();
    }
    float time = 0f;
    void Update()
    {
        if (!reachedGoal())
        {
            time += Time.deltaTime;
            if (time > 0.2f)
            {
                setPreferredVelocities();
                simulator.doStep(0.2f);
                time -= 0.2f;
            }
        }
    }

    void LateUpdate()
    {
        var no = simulator.agents_.First;
        while (no != null)
        {
            var agent = no.Value;
            no = no.Next;
            agent.As<AgentObj>().SetPosition();
        }        
    }
}