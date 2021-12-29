using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class StateManager<T>
{
    public IState<T> _state;
    public string Name => _state.Name;
    public StateManager(IState<T> initialState)
    {
        _state = initialState;
    }

    public IState<T> Update()
    {
        var state = _state.Update();
        if (_state != state)
        {
            _state.Destroy();
            _state = state;
        }

        return state;
    }

    public void SetState(IState<T> state)
    {
        _state.Destroy();
        _state = state;
    }

    public void FixedUpdate()
    {
        _state.FixedUpdate();
    }

    public void OnDestroy()
    {
        _state.Destroy();
    }
}

public abstract class IState<T>
{
    public T Parent;
    protected IState<T> _nextState;
    bool _firstUpdate = true;

    public IState(T parent, bool init = true)
    {
        _nextState = this;
        Parent = parent;

        if(init)
            Init();
    }

    public virtual void OnFirstUpdate()
    {

    }

    public virtual void Destroy()
    {

    }

    public virtual IState<T> Update()
    {
        if (_firstUpdate)
        {
            OnFirstUpdate();
            _firstUpdate = false;
        }

        return _nextState;
    }
    public virtual void FixedUpdate()
    {
    }

    public virtual void Init()
    {
        _firstUpdate = true;
    }

    public abstract string Name { get; }
}
