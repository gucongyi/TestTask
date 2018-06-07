using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 协程调度
/// </summary>
public class TaskIEnumeratorNode
{
    public bool finished = false; 
    public int waitForFrame = -1;
    public float waitForTime = -1.0f;
    public int currentWaitFrame;
    public float currentWaitTime;
    public IEnumerator<float> iEnumerator;

    public TaskIEnumeratorNode Init(IEnumerator<float> iEnumerator)
    {
        this.iEnumerator = iEnumerator;
        return this;
    }

    public void Reset()
    {
        finished = false; 
        waitForFrame = -1;
        waitForTime = -1.0f;
        currentWaitFrame = 0;
        currentWaitTime = 0;
        iEnumerator = null;
    }

    bool CheckFinish()
    {
        if (iEnumerator == null)
        {
            finished = true;
            return true;
        }

        if ( waitForTime < 0 && waitForFrame < 0)
        {
            if (iEnumerator.MoveNext() == false||iEnumerator==null)
            {
                iEnumerator = null;
                return true;
            }
            else
            {
                if (iEnumerator.Current <= 0)
                {
                    waitForFrame = -1 * (int)iEnumerator.Current + 1;
                    currentWaitFrame = 0;
                }
                else
                {
                    waitForTime = iEnumerator.Current;
                    currentWaitTime = 0;
                }
            }
        }
        return false;
    } 

    public void Update(float deltaTime)
    {
        if (CheckFinish()) return;

        if (iEnumerator.Current <= 0)
        {
            currentWaitFrame++;
            if (currentWaitFrame >= waitForFrame)
            {
                waitForFrame = -1;
                waitForTime = -1.0f;
                CheckFinish();
            }
        }
        else
        {
            currentWaitTime += deltaTime;
            if (currentWaitTime >= waitForTime)
            {
                waitForFrame = -1;
                waitForTime = -1.0f;
                CheckFinish();
            }
        }
    }
}
