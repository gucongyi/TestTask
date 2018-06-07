using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace task.gcyTask
{
    public class TaskIEnumator : TaskIEnumerator
    {

        protected override void Start()
        {
            base.Start();
            //TryConnect();
            StartIEnumerator(DelayStart());
        }

        IEnumerator<float> DelayStart()
        {
            int increaseTimes=1;
            
            while (increaseTimes<4)
            {
                yield return 1f;
                Debug.LogError("showTimes:" + increaseTimes);
                increaseTimes++;
            }
            Complete();
        }
    }
}


