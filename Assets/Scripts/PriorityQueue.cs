using System.Collections.Generic;
using System;

public class PriorityQueue<TElement, TPriority>
{
    private readonly List<(TElement Element, TPriority Priority)> _heap = new();
    private readonly Comparer<TPriority> _comparer = Comparer<TPriority>.Default;

    public int Count { get; private set; }

    public void Enqueue(TElement element, TPriority priority)
    {
        _heap.Add((element, priority));
        Count++;
        HeapifyUp(_heap.Count - 1);
    }

    public TElement Dequeue()
    {
        if (_heap.Count == 0)
            throw new InvalidOperationException("Queue is empty.");

        TElement top = _heap[0].Element;
        int last = _heap.Count - 1;
        _heap[0] = _heap[last];
        _heap.RemoveAt(last);
        Count--;
        if (_heap.Count > 0)
            HeapifyDown(0);
        return top;
    }

    public TElement Peek()
    {
        if (_heap.Count == 0)
            throw new InvalidOperationException("Queue is empty.");
        return _heap[0].Element;
    }

    public void Clear()
    {
        _heap.Clear();
        Count = 0;
    }

    private void HeapifyUp(int index)
    {
        while (index > 0)
        {
            int parent = (index - 1) / 2;
            if (_comparer.Compare(_heap[index].Priority, _heap[parent].Priority) >= 0)
                break;
            Swap(index, parent);
            index = parent;
        }
    }

    private void HeapifyDown(int index)
    {
        int count = _heap.Count;
        while (true)
        {
            int smallest = index;
            int left = 2 * index + 1;
            int right = 2 * index + 2;

            if (left < count && _comparer.Compare(_heap[left].Priority, _heap[smallest].Priority) < 0)
                smallest = left;
            if (right < count && _comparer.Compare(_heap[right].Priority, _heap[smallest].Priority) < 0)
                smallest = right;

            if (smallest == index)
                break;

            Swap(index, smallest);
            index = smallest;
        }
    }

    private void Swap(int a, int b)
    {
        var temp = _heap[a];
        _heap[a] = _heap[b];
        _heap[b] = temp;
    }
}
