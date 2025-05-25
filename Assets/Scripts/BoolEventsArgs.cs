using System;

public class BoolEventsArgs : EventArgs
{
    public bool Value { get; private set; }

    public BoolEventsArgs(bool value)
    {
        Value = value;
    }
}

