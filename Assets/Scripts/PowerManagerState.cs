using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PowerManagerState<T> : IState<T> where T : MonoBehaviour
{
    public delegate IState<T> StateActionDelegate();
    public override string Name => _state.Name;

    IState<T> _state;
    PowerConsumer _powerConsumer;
    bool _isOutofPower = false;
    StateActionDelegate _onOutofPower;
    StateActionDelegate _onPowerRestored;

    public PowerManagerState(T parent, IState<T> initialState, bool init = true) : base(parent, init)
    {
        _state = initialState;

        _powerConsumer = Parent.GetComponent<PowerConsumer>();
        _powerConsumer.OnPowerChanged.AddListener(OnPowerChanged);
    }

    public void OnPowerChanged(float current, float max)
    {
        if (current == 0 && _isOutofPower == false)
        {
            _isOutofPower = true;
            _state = _onOutofPower.Invoke();
        }

        if (current > 0 && _isOutofPower)
        {
            _isOutofPower = false;
            _state = _onPowerRestored.Invoke();
        }
    }

    public PowerManagerState<T> OnOutPowerPower(StateActionDelegate callback)
    {
        _onOutofPower = callback;
        return this;
    }
    public PowerManagerState<T> OnPowerRestored(StateActionDelegate callback)
    {
        _onPowerRestored = callback;
        return this;
    }

    public override void FixedUpdate()
    {
        _state.FixedUpdate();
    }

    public override void Init()
    {
    }

    public override void OnFirstUpdate()
    {
        OnPowerChanged(_powerConsumer.CurrentPower, _powerConsumer.MaxPower);
    }

    public override IState<T> Update()
    {
        base.Update();

        var state = _state.Update();
        if(state != _state)
        {
            _state.Destroy();
            _state = state;
        }
        return this;
    }
}
