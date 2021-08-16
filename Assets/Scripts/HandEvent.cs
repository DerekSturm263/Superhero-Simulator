using System;

public class HandEvent
{
    private readonly Predicate<HandInfo> _conditionL;
    private readonly Predicate<HandInfo> _conditionR;

    public Action<HandInfo, HandInfo> onStarted = delegate { };
    public Action<HandInfo, HandInfo> onUpdateTrigger = delegate { };
    public Action<HandInfo, HandInfo> onCancelled = delegate { };

    private bool _isTriggering = false;

    public static readonly System.Collections.Generic.List<HandEvent> events = new System.Collections.Generic.List<HandEvent>();

    public HandEvent(Predicate<HandInfo> condition)
    {
        this._conditionL = condition;
        this._conditionR = condition;

        events.Add(this);
    }

    public HandEvent(Predicate<HandInfo> conditionL, Predicate<HandInfo> conditionR)
    {
        this._conditionL = conditionL;
        this._conditionR = conditionR;

        events.Add(this);
    }

    public void TryEvent(HandInfo handL, HandInfo handR)
    {
        if (_conditionL.Invoke(handL) && _conditionR.Invoke(handR))
        {
            if (!_isTriggering)
            {
                _isTriggering = true;
                onStarted.Invoke(handL, handR);
            }
            else
            {
                onUpdateTrigger.Invoke(handL, handR);
            }
        }
        else if (_isTriggering)
        {
            _isTriggering = false;
            onCancelled.Invoke(handL, handR);
        }
    }
}