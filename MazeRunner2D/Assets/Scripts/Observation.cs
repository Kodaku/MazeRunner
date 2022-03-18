using UnityEngine;
using System.IO;
public class Observation
{
    int m_startX;
    int m_startY;
    float m_completeTime;
    float m_qMax;
    float m_qMin;
    int m_numberOfSteps;
    int m_collisionWithWalls;
    float m_qValue;

    public int startX
    {
        get { return m_startX; }
        set { m_startX = value; }
    }
    public int startY
    {
        get { return m_startY; }
        set { m_startY = value; }
    }
    public float completeTime
    {
        get { return m_completeTime; }
        set { m_completeTime = value; }
    }

    public float qMax
    {
        get { return m_qMax; }
        set { m_qMax = value; }
    }

    public float qMin
    {
        get { return m_qMin; }
        set { m_qMin = value; }
    }

    public int numberOfSteps
    {
        get { return m_numberOfSteps; }
        set { m_numberOfSteps = value; }
    }

    public int collisionWithWalls
    {
        get { return m_collisionWithWalls; }
        set { m_collisionWithWalls = value; }
    }

    public float qValue
    {
        get { return m_qValue; }
        set { m_qValue = value; }
    }

    public void SaveToFile(bool append = true)
    {
        string tsvPath = Application.dataPath + "/Resources/Observations.tsv";
        string tsvData = m_startX + "\t" +
                        m_startY + "\t" +
                        m_completeTime.ToString().Replace(",", ".") + "\t" +
                        m_numberOfSteps + "\t" +
                        m_qMin.ToString().Replace(",", ".") + "\t" +
                        m_qMax.ToString().Replace(",", ".") + "\t" +
                        m_qValue.ToString().Replace(",",".") + "\t" +
                        m_collisionWithWalls;
        StreamWriter tsvWriter = new StreamWriter(tsvPath, append);
        tsvWriter.WriteLine(tsvData);
        tsvWriter.Flush();
        tsvWriter.Close();
    }
}