using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


/// <summary>
/// Generic stack implementation with a maximum limit
/// When something is pushed on the last item is removed from the list
/// </summary>
[System.Serializable]
public class MaxStack<T>
{
	#region Fields

	private int _limit;
	private LinkedList<T> _list;

	#endregion

	#region Constructors

	public MaxStack(int maxSize)
	{
		_limit = maxSize;
		_list = new LinkedList<T>();

	}

	#endregion

	#region Public Stack Implementation

	public void Push(T value)
	{
		if (_list.Count == _limit)
		{
			_list.RemoveLast();
		}
		_list.AddFirst(value);
	}

	public void UniquePush(T value) 
	{
		if(_list.First == null)
		{
			Push(value);
		}
		else if(!CompareValues(_list.First.Value, value))
		{

			if (_list.Count == _limit)
			{
				_list.RemoveLast();
			}
			_list.AddFirst(value);

		}
	}

	public T Pop()
	{
		if (_list.Count > 0)
		{
			T value = _list.First.Value;
			_list.RemoveFirst();
			return value;
		}
		else
		{
			throw new UnityException("The Stack is empty");
		}


	}

	public T Peek()
	{
		if (_list.Count > 0)
		{
			T value = _list.First.Value;
			return value;
		}
		else
		{
			throw new UnityException("The Stack is empty");
		}

	}

	public List<T> DeepPeek(int depth)
	{
		if(depth <= 0)
		{
			return null;
		}
		List<T> list = new List<T>();
		LinkedListNode<T> currentNode = _list.First;
		for(int i=0; i<depth; i++)
		{
			if(currentNode == null)
			{
				break;
			}
			list.Add(currentNode.Value);
			currentNode = currentNode.Next;
		}

		return list;
	}

	public void Clear()
	{
		_list.Clear();

	}

	public int Count
	{
		get { return _list.Count; }
	}

	/// <summary>
	/// Checks if the top object on the stack matches the value passed in
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	public bool IsTop(T value)
	{
		bool result = false;
		if (this.Count > 0)
		{
			result = Peek().Equals(value);
		}
		return result;
	}

	public bool Contains(T value)
	{
		bool result = false;
		if (this.Count > 0)
		{
			result = _list.Contains(value);
		}
		return result;
	}

	public IEnumerator GetEnumerator()
	{
		return _list.GetEnumerator();
	}

	public bool CompareValues(T value1, T value2)
	{
		return EqualityComparer<T>.Default.Equals(value1, value2);
	}

	#endregion

}
