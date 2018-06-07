using System;
using UnityEngine;
/// <summary>
/// 把task封装给协程用,monoBeheivor 的yield return 
/// </summary>
public class TaskYieldInstruction : CustomYieldInstruction
{
        private TaskBase task;
        private bool needWait= false;

        public TaskYieldInstruction(TaskBase task)
        {
            if (task == null)
            {
                throw new ArgumentNullException("task");
            }
            this.task = task;
            needWait = !task.IsFinished;
        }
        public override  bool keepWaiting
        {
            get
            {
                if(needWait==false){
                    return false;
                }
                needWait = !task.IsFinished;
                if(needWait)task.Update(Time.deltaTime);
                needWait = !task.IsFinished;
               return  needWait;
            }
        }
}
