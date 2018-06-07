using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HYZ;
public class TaskDelayCall : TaskBase
{
    float duration;
    Action action;

    public TaskDelayCall Init(Action action, float duration)
    {
        this.action = action;
        this.duration = duration;
        return this;
    }

    protected override void Dispose()
    {
        base.Dispose();
        duration = 0;
        action = null;
    }
    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        if (timeElapse >= duration)
        {
            action();
            Complete();
        }
    }
}
