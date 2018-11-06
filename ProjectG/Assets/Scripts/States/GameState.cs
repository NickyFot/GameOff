using UnityEngine;
using System.Collections;

public abstract class GameState
{
    //-----------------------------------------------------------------

    public GameState() { }

    //-----------------------------------------------------------------

    public void Start()
    {
        OnStart();
    }
    public void Update()
    {
        OnUpdate();
    }
    public void FixedUpdate()
    {
        OnFixedUpdate();
    }
    public void End()
    {
        OnEnd();
    }

    //-----------------------------------------------------------------

    protected virtual void OnStart()
    {
        throw new System.NotImplementedException();
    }
    protected virtual void OnUpdate()
    {
        throw new System.NotImplementedException();
    }
    protected virtual void OnFixedUpdate()
    {
        throw new System.NotImplementedException();
    }
    protected virtual void OnEnd()
    {
        throw new System.NotImplementedException();
    }

    //-----------------------------------------------------------------
}
