using UnityEngine;
using System.Collections;

namespace HYZ
{
    public class TaskWait : TaskBase
    {
        float duration;

        public TaskWait Init(float duration)
        {
            this.duration = duration;
            return this;
        }

        protected override void Dispose()
        {
            base.Dispose();
            duration = 0;
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            if (timeElapse >= duration)
            {
                Complete();
            }
        }
    }
}