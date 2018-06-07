using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TaskExtensions
{
    public static TaskBaseIEnumerator AsIEnumerator(this TaskBase task)
    {
        if (task == null)
        {
            throw new NullReferenceException();
        }
        return new TaskBaseIEnumerator(task);
    }
    /// <summary>
    /// 把task封装给协程用,monoBeheivor 的yield return 
    /// </summary>
    public static TaskYieldInstruction AsCoroutine(this TaskBase task)
    {
        if (task == null)
        {
            throw new NullReferenceException();
        }

        return new TaskYieldInstruction(task);
    }
    public static T Run<T>(this T task, bool updateImmediately = false) where T : TaskBase
    {
        TaskManager.SceneInstnce.AddTask(task);
        if (updateImmediately)
            task.Update(Time.deltaTime);
        return task;
    }

    public static T RunGlobal<T>(this T task,bool updateImmediately=false) where T : TaskBase
    {
        TaskManager.GlolabInstnce.AddTask(task);
        if (updateImmediately)
            task.Update(Time.deltaTime);
        return task;
    }

    public static T RunWith<T>(this T task,TaskList taskList, bool updateImmediately = false) where T : TaskBase
    {
        taskList.PushBack(task);
        if (updateImmediately)
            task.Update(Time.deltaTime);
        return task;
    }
}