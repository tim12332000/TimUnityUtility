using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tim.Utility
{
	public class CoroutineTasks
	{
		private Queue<IEnumerator> _tasks = new Queue<IEnumerator>();
		private bool _isDirty;

		public void Enqueue(IEnumerator task)
		{
			_tasks.Enqueue(task);
		}

		public IEnumerator Play(Action onFinish = null)
		{
			if (_isDirty)
			{
				Debug.LogWarningFormat("[CoroutineTasks][Play] This Queue is already playing.");
				yield break;
			}

			_isDirty = true;
			while (_tasks.Count > 0)
			{
				yield return _tasks.Dequeue();
			}

			_isDirty = false;
			if (onFinish != null)
				onFinish();
		}
	}
}
