using System.Collections;
using System.Collections.Generic;
using task.gcyTask;
using UnityEngine;

public class TaskStartUp : MonoBehaviour
{
    private TaskList parallelTaskList;
    private TaskList serialTaskList;
    // Use this for initialization
    void Start ()
	{
        //new TaskSync().Init(1f, 3).Run(true);//1 同步输出
        //new TaskIEnumator().Run(true);//2异步输出
        //parallelTaskList=TaskList.parallel(null,true);//3并行输出
        //parallelTaskList.PushBack(new TaskIEnumator());//3
        //parallelTaskList.PushBack(new TaskSync().Init(1f,6));//3

        serialTaskList = TaskList.serial(null,true);//4串行输出
	    serialTaskList.PushBack(new TaskIEnumator());//4
        serialTaskList.PushBack(new TaskSync().Init(1f, 8));//4
    }
	
	// Update is called once per frame
	void Update () {
        //parallelTaskList.Update(Time.unscaledDeltaTime);//3并行输出
        serialTaskList.Update(Time.unscaledDeltaTime);//4串行输出
    }
}
