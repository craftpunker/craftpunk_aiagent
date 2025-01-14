using System;

/* 
 * T
 * 
 */
public class BinaryHeap<T> where T : IComparable<T>
{
    private readonly T[] items;
    private readonly bool IsClass;
    public int Count { get; private set; }
    public int MaxSize
    {
        get { return items == null ? -1 : items.Length; }
    }

    public BinaryHeap(int maxSize)
    {
        items = new T[maxSize];
        IsClass = typeof(T).IsClass;
        Count = 0;
    }

    public void Clear()
    {
        for (int i = 0; i < items.Length; ++i)
        {
            items[i] = default(T);
        }
        Count = 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="node"></param>
    public void Push(T node)
    {
        if (IsClass && node == null)
            return;

        if (Count >= items.Length)
            throw new IndexOutOfRangeException("Cant add node");

        //
        items[Count] = node;
        ++Count;

        //
        int current = Count - 1;
        int parent = (current - 1) / 2;

        while (items[parent].CompareTo(items[current]) > 0)
        {
            Exchange(parent, current);

            current = parent;
            parent = (current - 1) / 2;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public T Pop()
    {
        if (Count <= 0)
            throw new IndexOutOfRangeException("No element in BinaryHeap");

        var node = items[0];
        Remove(0);
        return node;
    }

    /// <summary>
    /// ，
    /// </summary>
    /// <returns></returns>
    /// <exception cref="IndexOutOfRangeException"></exception>
    public T GetRootValue()
    {
        if (Count <= 0)
            throw new IndexOutOfRangeException("No element in BinaryHeap");

        var node = items[0];
        return node;
    }

    /// <summary>
    /// （），
    /// </summary>
    /// <param name="func"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool TryGet(Func<T, bool> func, out T item)
    {
        foreach (var node in items)
        {
            if (IsClass && node == null)
                break;

            if (func(node))
            {
                item = node;
                return true;
            }
        }
        item = default(T);
        return false;
    }

    /// <summary>
    /// ，
    /// </summary>
    /// <param name="func"></param>
    /// <returns></returns>
    public bool TryRemove(Func<T, bool> func)
    {
        int idx = 0;
        foreach (var node in items)
        {
            if (IsClass && node == null)
                break;

            if (func(node))
            {
                Remove(idx);
                return true;
            }
            ++idx;
        }
        return false;
    }

    /// <summary>
    /// ，
    /// </summary>
    /// <param name="idx"></param>
    private void Remove(int idx)
    {
        if (Count <= 0)
            throw new IndexOutOfRangeException("No element in BinaryHeap");

        items[idx] = items[Count - 1];
        items[Count - 1] = default(T);
        Count--;

        int current = idx;

        while (true)
        {
            int child1 = current * 2 + 1;
            int child2 = current * 2 + 2;

            if (child1 >= Count)
                break;

            if (child2 >= Count)
            {
                if (items[child1].CompareTo(items[current]) < 0)
                    Exchange(child1, current);
                break;
            }

            int min;
            int max;
            if (items[child1].CompareTo(items[child2]) > 0)
            {
                min = child2;
                max = child1;
            }
            else
            {
                min = child1;
                max = child2;
            }

            if (items[current].CompareTo(items[min]) <= 0)
                break;

            Exchange(min, current);
            current = min;
            continue;
        }
    }

    private void Exchange(int idx1, int idx2)
    {
        var temp = items[idx1];
        items[idx1] = items[idx2];
        items[idx2] = temp;
    }
}
