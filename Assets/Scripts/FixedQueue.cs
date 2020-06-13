using System.Collections.Generic;

public class FixedQueue<T>
{
    public readonly T[] Elements;
    public int Index { get; private set; }

    public FixedQueue(int capacity)
    {
        Elements = new T[capacity];
    }

    public void Write(T newElement)
    {
        Elements[Index++] = newElement;
        if (Index >= Elements.Length) 
            Index -= Elements.Length;
    }

    public void Write(IEnumerable<T> newElements)
    {
        var enumerator = newElements.GetEnumerator();
        while (enumerator.MoveNext())
        {
            Elements[Index++] = enumerator.Current;
        }
        enumerator.Dispose();
    }

    public T this[int i]
    {
        get
        {
            int newIndex = i + Index;
            if (newIndex >= Elements.Length)
            {
                newIndex -= Elements.Length;
            }
            return Elements[newIndex];
        }
        set
        {
            int newIndex = i + Index;
            if (newIndex >= Elements.Length)
            {
                newIndex -= Elements.Length;
            }
            Elements[newIndex] = value;
        }
    }
}