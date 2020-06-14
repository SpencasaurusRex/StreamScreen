public interface SignalOutput
{
    int SampleRate { get; set; }
    FixedQueue<float> Buffer { get; set; }
    // event SampleRateChanged OnSampleRateChanged;
    // bool Subscribed { get; }
}
// public delegate void SampleRateChanged();