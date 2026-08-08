using System.Collections.Generic;
using System.Linq;

namespace KrokoshaCasualtiesMP;

public class MaxCapacityQueue<T>
{
	private readonly Queue<T> queue = new Queue<T>();

	private int _capacity;

	public int Count => queue.Count;

	public MaxCapacityQueue(int capacity)
	{
		_capacity = capacity;
	}

	public void Enqueue(T val)
	{
		queue.Enqueue(val);
		while (queue.Count > _capacity)
		{
			queue.Dequeue();
		}
	}

	public T Dequeue()
	{
		return queue.Dequeue();
	}

	public T ElementAt(int i)
	{
		return queue.ElementAt(i);
	}

	public void Clear()
	{
		queue.Clear();
	}

	public bool Contains(in T value)
	{
		return queue.Contains(value);
	}

	public Queue<T> JustGiveTheQueue()
	{
		return queue;
	}
}
