using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace task.gcyTask
{
    public class TaskSync : TaskBase
    {
        private float interval;
        private int showTimes;
        private int increaseTimes;
        private float timeIncrease;
        public TaskSync Init(float interval, int showTimes)
        {
            this.interval = interval;
            this.showTimes = showTimes;
            return this;
        }

        protected override void Start()
        {

        }

        public override void Update(float deltaTime)
        {
            timeIncrease += deltaTime;
            if (timeIncrease> interval)
            {
                increaseTimes++;
                Debug.LogError("showTimes:" + increaseTimes);
                timeIncrease = 0f;
            }

            if (increaseTimes == showTimes)
            {
                Complete();
            }
        }
    }
}

