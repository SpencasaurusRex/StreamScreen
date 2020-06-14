using System;

public interface ISignal
{ 
    float Evaluate(float t);
}

[Serializable]
public class ISignalContainer : IUnifiedContainer<ISignal>
{
} 