using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager
{
    private Observation observation;
    private int startX = 0;
    private int startY = 0;
    private float episodeDuration = 0.0f;
    private int numberOfSteps = 0;
    private float minQValue = Mathf.Infinity;
    private float maxQValue = Mathf.NegativeInfinity;
    private float qValue = 0.0f;
    private int collisionWithWalls;
    public DataManager()
    {
        observation = new Observation();
    }

    public void UpdateTimer(float timer)
    {
        episodeDuration += timer;
        numberOfSteps++;
    }

    public void SetStartValues(int startX, int startY)
    {
        this.startX = startX;
        this.startY = startY;
    }

    public void SetMinMaxQValues(float qValue)
    {
        if(qValue > maxQValue)
        {
            maxQValue = qValue;
        }
        if(qValue < minQValue)
        {
            minQValue = qValue;
        }
    }

    public float GetMinQValue()
    {
        return minQValue;
    }

    public float GetMaxQValue()
    {
        return maxQValue;
    }

    public void SetQValue(float q)
    {
        qValue = q;
    }

    public void CollidedWithWalls()
    {
        collisionWithWalls++;
    }

    public void RegisterObservation()
    {
        observation.startX = startX;
        observation.startY = startY;
        observation.completeTime = episodeDuration;
        observation.numberOfSteps = numberOfSteps;
        observation.qMin = minQValue;
        observation.qMax = maxQValue;
        observation.qValue = qValue;
        observation.collisionWithWalls = collisionWithWalls;

        observation.SaveToFile(append:true);
    }

    public void Reset()
    {
        episodeDuration = 0.0f;
        numberOfSteps = 0;
        collisionWithWalls = 0;
    }
}
