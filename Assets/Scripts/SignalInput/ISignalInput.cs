public interface ISignalInput
{
    ISignalOutput[] Outputs { get; set; }
    float TimeLength { get; set; }
    bool TimeLengthChanged { get; }
}