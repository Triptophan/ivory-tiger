using UnityEngine;
using System.Collections;
using System;

public class Heap<T> where T : IHeapItem<T>
{
    T[] items;
    int currentItemCount;

    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }

    public void Add(T item)
    {
        item.Heapindex = currentItemCount;
        items[currentItemCount] = item;
        SortUp(item);
        currentItemCount++;
    }

    public T RemoveFirst()
    {
        T firstItem = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].Heapindex = 0;
        SortDown(items[0]);

        return firstItem;
    }

    public bool Contains(T item)
    {
        return Equals(items[item.Heapindex], item);
    }

    public int Count
    {
        get { return currentItemCount; }
    }

    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    void SortDown(T item)
    {
        while(true)
        {
            int childIndexLeft = item.Heapindex * 2 + 1;
            int childIndexRight = item.Heapindex * 2 + 2;
            int swapIndex = 0;

            if (childIndexLeft < currentItemCount)
            {
                swapIndex = childIndexLeft;
                if (childIndexRight < currentItemCount)
                {
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                        swapIndex = childIndexRight;
                }
                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    Swap(item, items[swapIndex]);
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }
    private void SortUp(T item)
    {
        int parentIndex = (item.Heapindex - 1) / 2;

        while (true)
        {
            T parentItem = items[parentIndex];
            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }
            else
            {
                break;
            }
            parentIndex = (item.Heapindex - 1) / 2;
        }
    }

    void Swap (T itemA, T itemB)
    {
        items[itemA.Heapindex] = itemB;
        items[itemB.Heapindex] = itemA;

        int itemAHeapindex = itemA.Heapindex;
        itemA.Heapindex = itemB.Heapindex;
        itemB.Heapindex = itemAHeapindex;
    }
}

public interface IHeapItem<T> : IComparable<T>
{
    int Heapindex { get; set; }

}
