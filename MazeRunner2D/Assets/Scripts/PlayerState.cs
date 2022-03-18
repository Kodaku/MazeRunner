using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

using HashedState = System.Tuple<UnityEngine.Vector3, System.Tuple<CellInfo, CellInfo, CellInfo, CellInfo>>;
using NeighborsInfo = System.Tuple<CellInfo, CellInfo, CellInfo, CellInfo>;
[System.Serializable]
public class PlayerState
{
    private float[] position;
    private CellInfo[] neighborsInfo;
    private PlayerAction[] playerActions;
    private PlayerState[] nextStates;
    private Action[] actionSpace = new Action[]{Action.UP, Action.DOWN, Action.RIGHT, Action.LEFT};
    [System.NonSerialized]
    private HashedState hashedState;
    private float[] tau;
    private float epsilon = 0.1f;
    private bool m_isTerminal = false;

    public PlayerState(Vector3 _position, NeighborsInfo _neighborsInfo)
    {
        position = new float[]{_position.x, _position.y, _position.z};
        neighborsInfo = new CellInfo[]{_neighborsInfo.Item1, _neighborsInfo.Item2, _neighborsInfo.Item3, _neighborsInfo.Item4};
        GenerateHashCode();
        GenerateBasicActions();
        InitializeNextStates();
        InitializeTau();
    }

    public bool isTerminal
    {
        get { return m_isTerminal; }
        set { m_isTerminal = value; }
    }

    public void GenerateHashCode()
    {
        Vector3 statePosition = new Vector3(position[0], position[1], position[2]);
        NeighborsInfo stateNeighborsInfo = Tuple.Create(neighborsInfo[0], neighborsInfo[1], neighborsInfo[2], neighborsInfo[3]);
        hashedState = Tuple.Create(statePosition, stateNeighborsInfo);
    }

    private void GenerateBasicActions()
    {
        int totalActions = actionSpace.Length;
        playerActions = new PlayerAction[totalActions];
        foreach(Action action in actionSpace)
        {
            PlayerAction playerAction = new PlayerAction(action, 1.0f / totalActions);
            playerActions[(int)action] = playerAction;
        }
    }

    private void InitializeNextStates()
    {
        int totalActions = actionSpace.Length;
        nextStates = new PlayerState[totalActions];
        foreach(Action action in actionSpace)
        {
            nextStates[(int)action] = null;
        }
    }

    private void InitializeTau()
    {
        int totalActions = actionSpace.Length;
        tau = new float[totalActions];
        foreach(Action action in actionSpace)
        {
            tau[(int)action] = 0.0f;
        }
    }

    public void UpdateProbabilities(Action bestAction)
    {
        int totalActions = actionSpace.Length;
        foreach(PlayerAction playerAction in playerActions)
        {
            if(playerAction.name == bestAction)
            {
                playerAction.probability = 1.0f - epsilon + (epsilon / totalActions);
            }
            else
            {
                playerAction.probability = (epsilon / totalActions);
            }
        }
    }

    public Action ChooseAction()
    {
        float sum = 0.0f;
        List<float> distribution = new List<float>();
        foreach(PlayerAction playerAction in playerActions)
        {
            distribution.Add(playerAction.probability);
        }

        List<float> cumulative = distribution.Select(c => {
            var result = sum + c;
            sum += c;
            return result;
        }).ToList();

        float r = UnityEngine.Random.value;
        int idx = cumulative.BinarySearch(r);
        if(idx < 0)
        {
            idx = ~idx;
        }
        if(idx > cumulative.Count - 1)
        {
            idx = cumulative.Count - 1;
        }

        return playerActions[idx].name;
    }

    public Action GetBestAction()
    {
        Action bestAction = Action.UP;
        float maxActionValue = Mathf.NegativeInfinity;
        foreach(PlayerAction playerAction in playerActions)
        {
            float actionValue = playerAction.qValue;
            if(actionValue > maxActionValue)
            {
                maxActionValue = actionValue;
                bestAction = playerAction.name;
            }
        }
        return bestAction;
    }

    public void SetNextStateAndReward(Action currentAction, PlayerState nextState, float reward)
    {
        nextStates[(int)currentAction] = nextState;
        PlayerAction currentPlayerActionInstance = playerActions[(int)currentAction];
        currentPlayerActionInstance.reward = reward;
    }

    public Tuple<PlayerState, PlayerState, float, float> GetRandomNextStateAndReward()
    {
        List<Action> validActions = new List<Action>();
        foreach(Action action in actionSpace)
        {
            if(nextStates[(int)action] != null)
            {
                validActions.Add(action);
            }
        }
        int totalActions = validActions.Count;
        // Debug.Log(totalActions);
        if(totalActions > 0)
        {
            int index = UnityEngine.Random.Range(0, totalActions);
            Action randomAction = validActions[index];
            PlayerAction randomTakenAction = playerActions[(int)randomAction];
            // Debug.Log(randomAction + ", " + nextStates.Length);
            // Debug.Log(nextStates[0] + ", " + nextStates[1] + ", " + nextStates[2] + ", " + nextStates[3]);
            return Tuple.Create(this, nextStates[(int)randomAction], randomTakenAction.reward, tau[(int)randomAction]);
        }
        return null;
    }

    public float GetMaxQ()
    {
        float maxQ = Mathf.NegativeInfinity;
        foreach(PlayerAction playerAction in playerActions)
        {
            float qValue = playerAction.qValue;
            if(qValue > maxQ)
            {
                maxQ = qValue;
            }
        }

        return maxQ;
    }

    public float GetQ(Action currentAction)
    {
        return playerActions[(int)currentAction].qValue;
    }

    public void UpdateActionValue(Action currentAction, float maxQ, float reward, bool isEndState)
    {
        m_isTerminal = isEndState;
        PlayerAction playerAction = playerActions[(int)currentAction];
        playerActions[(int)currentAction].qValue += 0.01f * (reward + 1.0f * maxQ - playerAction.qValue);
    }

    public void IncreaseTau()
    {
        foreach(Action action in actionSpace)
        {
            tau[(int)action] += 1.0f;
        }
    }

    public void ResetTau(Action currentAction)
    {
        tau[(int)currentAction] = 0.0f;
    }

    public HashedState GetHashedState()
    {
        return hashedState;
    }
}
