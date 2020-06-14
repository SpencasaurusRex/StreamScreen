public interface SignalInput
{
    SignalOutput[] Outputs { get; set; }
    float TimeLength { get; set; }
    // event TimeLengthChanged OnTimeLengthChanged;
    // bool Subscribed { get; }
}

// public delegate void TimeLengthChanged();
