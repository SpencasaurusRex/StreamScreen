public interface ISignalOutput
{
    int SampleRate { get; set; }
    FixedQueue<float> Buffer { get; set; }
    bool SampleRateChanged { get; }
}