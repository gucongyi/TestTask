using System;
public class TaskCallBack : TaskBase
{
    private  Action<Action> action;
    public TaskCallBack(Action<Action> action)
    {
        this.action = action;
    }
    protected override void Start()
    {
        action(OnActionEnd);
    }

    private void OnActionEnd()
    {
        Complete();
    }

    protected override void Dispose()
    {
        base.Dispose();
        action = null;
    }
}
