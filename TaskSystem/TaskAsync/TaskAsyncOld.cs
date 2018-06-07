using System;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 结合TaskBase实现的一个异步协程
/// </summary>
public class TaskAsyncOld:TaskBase{
    private TaskList mTaskList;
    //同id只会有一个task
    private Dictionary<int, TaskBase> mTaskMap;
    protected override void Start()
    {
        mTaskList = TaskList.parallel(null, false);
        onCancelled += mTaskList.Cancel;
    }
    protected override void Dispose()
    {
        base.Dispose();
        
        if (mTaskList == null) return;
        mTaskList = null;

    }
    public TaskBase RunTask(TaskBase task)
    {
        mTaskList.PushBack(task);
        return task;
    }
    public T RunTask<T>(T task, int id)where T:TaskBase
    {
        if (mTaskMap == null) mTaskMap = new Dictionary<int, TaskBase>();
        if (mTaskMap.ContainsKey(id))
        {
            var oldTask = mTaskMap[id];
            if (oldTask != null)
            {
                oldTask.Cancel();
                //ZLog.Info("干掉了老任务 id: ",id);
            }
            mTaskMap.Remove(id);
        }
        mTaskMap.Add(id, task);
        mTaskList.PushBack(task);
        return task;
    }
    public TaskBase StartAsync(IEnumerator enumerator)
    {
        TryStart();
        var task = new TaskAwait(enumerator);
        mTaskList.PushBack(task);
        return task;
    }
    public TaskBase StartAsync(IEnumerator enumerator,int id)
    {
        TryStart();
        var task = new TaskAwait(enumerator);
        RunTask(task,id);
        return task;
    }

    private void TryStart()
    {
        //确保已经开始了,或者改为没开始就不执行
        if (!isStarted)
        {
            Update(0);
        }
    }
    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        if(mTaskList!=null)
        mTaskList.Update(deltaTime);
        
    }
    public class TaskAwait : TaskBase
    {
        private IEnumerator enumerator;
        private Action mCurrUpdate;
        private TaskBaseIEnumerator mRunningTask;

        public TaskAwait(IEnumerator enumerator)
        {
            this.enumerator = enumerator;
            
        }
        private void CancelRunningTask()
        {
            if (mRunningTask != null)
            {
                //ZLog.Info("CancelRunningTask IEnumerator ", mRunningTask.GetType().Name);

                mRunningTask.Cancel();
                mRunningTask = null;
            }
        }
        protected override void Dispose()
        {
            base.Dispose();
            //CancelRunningTask();
            enumerator = null;
            mCurrUpdate = null;
        }
        protected override void Start()
        {
            base.Start();
            //ZLog.Error("开始一个 协程taskTaskAwait ", enumerator.GetType().Name);

            onCancelled += CancelRunningTask;
            mCurrUpdate = UpdateMain;
            //mCurrUpdate();
        }
        private void UpdateMain(){
            if (UpdateReturnIsEnd(enumerator))
            {
                Complete();
            }
        }
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            //ZLog.Info("TaskAwait Update  ", enumerator.GetType().Name);
            mCurrUpdate();
        }
        private bool subEnd = false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="curr"></param>
        /// <returns>true 为结束了</returns>
        private bool UpdateReturnIsEnd(IEnumerator curr)
        {
            if (curr == null)
            {
                //ZLog.Info("当前 enumerator 结束 ", enumerator.GetType().Name);
                return true;
            }
            if (!curr.MoveNext())
            {
                //ZLog.Info("enumerator 结束 ", enumerator.GetType().Name);
                return true;
            }
           
            if (curr.Current == null)
            {
                //如果是空 ,继续下一帧. 
                //ZLog.Info("等一帧 协程:", curr.GetType().Name);
                return false;
            }
            //暂时只支持null和IEnumerator,之后考虑支持数字,x帧,时间
            if(curr.Current is IEnumerator)
            {
                //ZLog.Info("新协程", curr.Current.GetType().Name);
                subEnd = NewEnumeratorSteps(mCurrUpdate, curr.Current as IEnumerator);
                //ZLog.Info("协程 ", curr.Current.GetType().Name, " 是否结束:", subEnd);
                //子协程不等帧
                if(subEnd) return UpdateReturnIsEnd(curr);
                return false;
            }
            //if(curr.Current is UnityEngine.YieldInstruction)
            //{
            //    return (curr.Current as UnityEngine.YieldInstruction)
            //}
            throw new Exception("不支持的异步类型 "+ curr.Current.GetType().Name);
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldUpdate"></param>
        /// <param name="subIEnumerator"></param>
        /// <returns>true 为结束了</returns>
        private bool NewEnumeratorSteps(Action oldUpdate,IEnumerator subIEnumerator)
        {
            var old = oldUpdate;
            if(subIEnumerator is TaskBaseIEnumerator)
            {
                mRunningTask = subIEnumerator as TaskBaseIEnumerator;
                //ZLog.Error("新的task IEnumerator ",mRunningTask.GetTaskName());
            }
            else
            {
                //ZLog.Error("普通协程"+ curr.GetType().Name);
            }
            //mCurrUpdate = () => {
            //    UpdateSub(old,curr);
            //};
            var r = UpdateSubReturnIsEnd(old, subIEnumerator);
            if (!r)
            {
                mCurrUpdate = (
                    () =>
                    {
                        UpdateSubReturnIsEnd(old, subIEnumerator);
                    }
                    );
            }
            //mCurrUpdate();
            return r;
        }
        private bool UpdateSubReturnIsEnd(Action oldUpdate, IEnumerator subIEnumerator)
        {
            if (UpdateReturnIsEnd(subIEnumerator) )
            {
                if(mCurrUpdate!=oldUpdate)
                mCurrUpdate = oldUpdate;
                return true;
            }
            return false;
        }
    }

}

//IEnumerator
// public class TaskYieldInstruction : IEnumerator
// {
//     private TaskBase task;
//     private bool needWait= false;

//     public TaskYieldInstruction(TaskBase task)
//     {
//         if (task == null)
//         {
//             throw new ArgumentNullException("task");
//         }
//         this.task = task;
//         needWait = !task.IsFinished;
//     }
//     public  bool keepWaiting
//     {
//         get
//         {
//             if(needWait==false){
//                 return false;
//             }
//             needWait = !task.IsFinished;
//             if(needWait)task.Update(Time.deltaTime);
//            return  needWait;
//         }
//     }

//     public object Current
//     {
//         get { return null; }

//     public bool MoveNext()
//     {
//         return keepWaiting;
//     }

//     public void Reset()
//     {
//         Debug.LogError("todo释放task");
//         throw new NotImplementedException();
//     }
// }
