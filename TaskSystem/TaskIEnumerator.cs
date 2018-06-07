using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 可使用协程的Task. 
/// yield return floatValue : 当floatValue小于等于0时，等待(floatValue+1)帧； 当floatValue大于0时，等待floatValue秒；
/// 例：
/// yield return 0; 等待一帧；
/// yield return -3; 等待4帧；
/// yield return 2.5f; 等待2.5秒；
/// </summary> 

public class TaskIEnumerator : TaskBase
{
    List<TaskIEnumeratorNode> listUpdateNode;
    List<TaskIEnumeratorNode> listLateUpdateNode;

    

    void Init()
    {
        if (listUpdateNode == null)
            listUpdateNode = new List<TaskIEnumeratorNode>();

        if (HasLateUpate && listLateUpdateNode == null)
            listLateUpdateNode = new List<TaskIEnumeratorNode>();
    }

    protected override void Start()
    {
        base.Start();
        Init();
    }

    protected override void Dispose()
    {
        base.Dispose();

        ClearListNode(listUpdateNode);
        ClearListNode(listLateUpdateNode);
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        UpdateListNode(listUpdateNode);
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
        if(IsStarted)
            UpdateListNode(listLateUpdateNode);
    }

    void ClearListNode(List<TaskIEnumeratorNode> list)
    {
        if (list == null)
            return;
        foreach (var item in list)
        {
            ReturnTaskIEnumeratorNode(item);
        }
        list.Clear();
    }

    void UpdateListNode(List<TaskIEnumeratorNode> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            list[i].Update(deltaTime);
        }

        for (int i = 0; i < list.Count; i++)
        {
            TaskIEnumeratorNode node = list[i];
            if (node.finished)
            {
                list.RemoveAt(i);
                ReturnTaskIEnumeratorNode(node);
                i--;
            }
        }
    }

    /// <summary>
    /// 开启协程
    /// </summary>
    /// <param name="iEnumerator"></param>
    /// <param name="useLateUpdate">是否在LateUpdate中执行</param>
    /// <returns>返回传入的iEnumerator</returns>
    public IEnumerator<float> StartIEnumerator(IEnumerator<float> iEnumerator,bool useLateUpdate=false)
    {
        Init();

#if UNITY_EDITOR
        if (useLateUpdate && hasLateUpate == false)
        {
            throw new System.Exception("StartIEnumerator useLateUpdate, but hasLateUpate==false");
        }
#endif
        List<TaskIEnumeratorNode> list = useLateUpdate ? listLateUpdateNode : listUpdateNode;
        TaskIEnumeratorNode taskIEnumeratorNode = GetTaskIEnumeratorNode().Init(iEnumerator); 
        list.Add(taskIEnumeratorNode);
        return iEnumerator;
    }

    /// <summary>
    /// 结束协程
    /// </summary>
    /// <param name="iEnumerator"></param>
    /// <param name="useLateUpdate">是否在LateUpdate中执行,与开启协程时的参数对应</param>
    /// <returns>返回null</returns>
    public IEnumerator<float> StopIEnumerator(IEnumerator<float> iEnumerator, bool useLateUpdate = false)
    {
        if (iEnumerator == null)
            return null;

        List<TaskIEnumeratorNode> list = useLateUpdate ? listLateUpdateNode:listUpdateNode ;
        for (int i = 0; i < list.Count; i++)
        {
            if(list[i].iEnumerator==iEnumerator)
            {
                TaskIEnumeratorNode node = list[i];
                list.RemoveAt(i);
                ReturnTaskIEnumeratorNode(node);
                i--;
            }
        } 
        return null;
    } 

#region node pool
    static Queue<TaskIEnumeratorNode> queueTaskIEnumeratorNode=new Queue<TaskIEnumeratorNode>();
    static TaskIEnumeratorNode GetTaskIEnumeratorNode()
    {
        if( queueTaskIEnumeratorNode.Count>0)
        {
            return queueTaskIEnumeratorNode.Dequeue();
        } 
        return new TaskIEnumeratorNode();
    }

    static void ReturnTaskIEnumeratorNode(TaskIEnumeratorNode taskIEnumeratorNode)
    {
        taskIEnumeratorNode.Reset();
        queueTaskIEnumeratorNode.Enqueue(taskIEnumeratorNode);
    }
#endregion

}
