using System.Collections;
/// <summary>
/// 简易异步Task,只支持一个异步自动执行,只支持null
/// </summary>
public class TaskAutoAsync : TaskBase {
    private IEnumerator enumerator;

    protected override void Start()
    {
        base.Start();
        enumerator = Steps();
    }
    protected virtual IEnumerator Steps()
    {
        yield return null;
    }
    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        if (Update()) { Complete(); return; }
        
    }
    private bool Update()
    {
        if (enumerator == null) return true;
        if (!enumerator.MoveNext()) return true;
        //yield return null
        if (enumerator.Current == null) return false;
        return true;
    }
}
