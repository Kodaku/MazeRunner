using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

using NeighborsInfo = System.Tuple<CellInfo, CellInfo, CellInfo, CellInfo>;
using State = System.Tuple<UnityEngine.Vector3, System.Tuple<CellInfo, CellInfo, CellInfo, CellInfo>>;
public class Player : MonoBehaviour
{
    private State currentState = null;
    private State nextState = null;
    private ActionValue q;
    private Policy policy;
    private Model model;
    private Action currentAction;
    private bool m_hasFoundGoal = false;
    private bool m_hasMoved = false;
    private float kappa = 0.001f;

    private State SetState(Dictionary<Tuple<int, int>, List<string>> world)
    {
        Vector3 playerPositon = transform.position;
        Tuple<int, int> position = Tuple.Create(Mathf.RoundToInt(playerPositon.x), Mathf.RoundToInt(playerPositon.y));
        List<string> cellsState = world[position];
        List<CellInfo> cellInfos = new List<CellInfo>();
        if(cellsState.Contains("NorthBlocked"))
        {
            cellInfos.Add(CellInfo.NORTH_BLOCKED);
        }
        else
        {
            cellInfos.Add(CellInfo.NORTH_FREE);
        }
        if(cellsState.Contains("SouthBlocked"))
        {
            cellInfos.Add(CellInfo.SOUTH_BLOCKED);
        }
        else
        {
            cellInfos.Add(CellInfo.SOUTH_FREE);
        }
        if(cellsState.Contains("EastBlocked"))
        {
            cellInfos.Add(CellInfo.EAST_BLOCKED);
        }
        else
        {
            cellInfos.Add(CellInfo.EAST_FREE);
        }
        if(cellsState.Contains("WestBlocked"))
        {
            cellInfos.Add(CellInfo.WEST_BLOCKED);
        }
        else
        {
            cellInfos.Add(CellInfo.WEST_FREE);
        }
        NeighborsInfo neighborsInfo = Tuple.Create(cellInfos[0], cellInfos[1], cellInfos[2], cellInfos[3]);
        return Tuple.Create(playerPositon, neighborsInfo);
    }

    public void SetCurrentState(Dictionary<Tuple<int, int>, List<string>> world)
    {
        currentState = SetState(world);
        // print(currentState);
    }

    public void SetNextState(Dictionary<Tuple<int, int>, List<string>> world)
    {
        nextState = SetState(world);
        // print(nextState);
    }

    public void SelectAction()
    {
        Action bestAction = GetBestAction();
        policy.EpsilonGreedyUpdate(currentState, bestAction);
        currentAction = policy.ChooseAction(currentState);
    }

    private Action GetBestAction()
    {
        if(q.ContainsState(currentState))
        {
            return q.GetBestAction(currentState);
        }
        else
        {
            PlayerState playerState = new PlayerState(currentState.Item1, currentState.Item2);
            q.AddPlayerState(playerState);
            policy.AddPlayerState(playerState);
            return GetBestAction();
        }
    }
    public void Move(Dictionary<Tuple<int, int>, List<string>> world)
    {
        m_hasMoved = false;
        Tuple<int, int> position = Tuple.Create(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        List<string> cellsState = world[position];
        Vector3 tmpPosition = transform.position;
        foreach(string cellState in cellsState)
        {
            // print(cellState);
        }
        if(currentAction == Action.UP && !cellsState.Contains("NorthBlocked"))
        {
            Tuple<int, int> newPosition = Tuple.Create(position.Item1, position.Item2 + 1);
            if(world.ContainsKey(newPosition))
            {
                m_hasMoved = true;
                tmpPosition.y += 1.0f;
            }
        }
        if(currentAction == Action.DOWN && !cellsState.Contains("SouthBlocked"))
        {
            Tuple<int, int> newPosition = Tuple.Create(position.Item1, position.Item2 - 1);
            if(world.ContainsKey(newPosition))
            {
                m_hasMoved = true;
                tmpPosition.y -= 1.0f;
            }
        }
        if(currentAction == Action.RIGHT && !cellsState.Contains("EastBlocked"))
        {
            Tuple<int, int> newPosition = Tuple.Create(position.Item1 + 1, position.Item2);
            if(world.ContainsKey(newPosition))
            {
                m_hasMoved = true;
                tmpPosition.x += 1.0f;
            }
        }
        if(currentAction == Action.LEFT && !cellsState.Contains("WestBlocked"))
        {
            Tuple<int, int> newPosition = Tuple.Create(position.Item1 - 1, position.Item2);
            if(world.ContainsKey(newPosition))
            {
                m_hasMoved = true;
                tmpPosition.x -= 1.0f;
            }
        }
        transform.position = tmpPosition;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.CompareTag("Goal"))
        {
            // print("Collison");
            m_hasFoundGoal = true;
        }
    }

    public bool hasFoundGoal
    {
        get { return m_hasFoundGoal; }
        set { m_hasFoundGoal = value; }
    }

    public bool hasMoved
    {
        get { return m_hasMoved; }
        set { m_hasMoved = value; }
    }

    public bool IsStuck()
    {
        return currentState == nextState;
    }

    public void UpdateAllTau()
    {
        model.IncreaseAllTau();
        model.ResetTau(currentState, currentAction);
    }

    public void QUpdate(float reward, bool isEndState)
    {
        if(!isEndState)
        {
            float maxQ = GetMaxQ(nextState);
            // Q[currentState][currentAction] += 0.01f * (reward + 1.0f * maxQ - Q[currentState][currentAction]);
            q.Update(currentState, currentAction, maxQ, reward, isEndState);
        }
        else
            // Q[currentState][currentAction] += 0.01f * (reward - Q[currentState][currentAction]);
            q.Update(currentState, currentAction, 0.0f, reward, isEndState);
        // currentState = nextState;
    }

    public void UpdateModel(float reward)
    {
        model.Update(currentState, currentAction, nextState, reward);
        currentState = nextState;
    }

    public void RunSimulation()
    {
        Tuple<PlayerState, PlayerState, float, float> simulationResult = model.GetRandomPlayerStateAndReward();
        if(simulationResult != null)
        {
            State currentSimulatedState = simulationResult.Item1.GetHashedState();
            State nextSimulatedState = simulationResult.Item2.GetHashedState();
            // print(currentSimulatedState);
            // print(nextSimulatedState);
            float simulationTau = simulationResult.Item4;
            float simulationReward = simulationResult.Item3 + kappa * Mathf.Sqrt(simulationTau);
            if(!simulationResult.Item1.isTerminal)
            {
                float maxQ = GetMaxQ(nextSimulatedState);
                q.Update(currentSimulatedState, currentAction, maxQ, simulationReward, simulationResult.Item1.isTerminal);
            }
            else
                q.Update(currentSimulatedState, currentAction, 0.0f, simulationReward, simulationResult.Item1.isTerminal);
        }
    }

    private float GetMaxQ(State nextState)
    {
        if(q.ContainsState(nextState))
        {
            return q.GetMaxQ(nextState);
        }
        else
        {
            PlayerState playerState = new PlayerState(nextState.Item1, nextState.Item2);
            policy.AddPlayerState(playerState);
            q.AddPlayerState(playerState);
            return GetMaxQ(nextState);
        }
    }

    public float GetQ()
    {
        return q.GetQ(currentState, currentAction);
    }

    public Action GetCurrentAction()
    {
        return currentAction;
    }

    public void SaveData()
    {
        Serializer.WriteToBinaryFile<Policy>("Assets/Resources/policy.txt", policy);
        Serializer.WriteToBinaryFile<ActionValue>("Assets/Resources/Q.txt", q);
        Serializer.WriteToBinaryFile<Model>("Assets/Resources/model.txt", model);
    }

    public void LoadData()
    {
        if(System.IO.File.Exists("Assets/Resources/Q.txt"))
        {
            //Load Q and Policy and initialize the StateDictionary
            policy = Serializer.ReadFromBinaryFile<Policy>("Assets/Resources/policy.txt");
            q = Serializer.ReadFromBinaryFile<ActionValue>("Assets/Resources/Q.txt");
            model = Serializer.ReadFromBinaryFile<Model>("Assets/Resources/model.txt");
            policy.InitializeStateDictionary();
            q.InitializeStateDictionary();
            model.InitializeStateDictionary();
        }
        else
        {
            policy = new Policy();
            q = new ActionValue();
            model = new Model();
        }
    }
}
