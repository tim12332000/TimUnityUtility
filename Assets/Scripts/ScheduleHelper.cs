using System;
using System.Collections;
using UnityEngine;

namespace Tim.Utility
{
	public interface ITask : IDisposable
	{
		void Pause();
		void Resume();
	}

	public class Tasks : ITask
	{
		private Action<Coroutine> _stopCoAction = null;
		private Func<IEnumerator, Coroutine> _startCoroutine = null;

		private Action _doSome;
		private int _count;
		private int _index;

		private Action<ITask> _onFinish;
		private Coroutine _co;
		private float _duration;
		private bool _isPause;
		private float _timer;

		public void Dispose()
		{
			if (_co != null)
			{
				_stopCoAction(_co);
				_co = null;
			}

			_doSome = null;
			_onFinish = null;
		}

		public void Pause()
		{
			_isPause = true;
		}

		public void Resume()
		{
			_isPause = false;
		}

		public Tasks(Func<IEnumerator, Coroutine> StartCo
			, Action<Coroutine> stopCo
			, Action doSomething
			, float duration
			, int count
			, Action<ITask> onFinish
			, bool firstTaskDelay = false)
		{
			_count = count;
			_doSome = doSomething;
			_stopCoAction = stopCo;
			_startCoroutine = StartCo;
			_onFinish = onFinish;

			_duration = duration;

			if (firstTaskDelay)
			{
				// Skip One Duration Time
				_index += 1;
				_count += 1;
			}

			_co = StartCo(Update());
		}

		IEnumerator Update()
		{
			while (_index < _count)
			{
				if (!_isPause)
				{
					_timer += Time.deltaTime;
					if (_timer > _index * _duration)
					{
						DoSome();
						_index++;
					}
				}

				yield return null;
			}

			if (_onFinish != null)
			{
				_onFinish(this);
			}
		}

		private void DoSome()
		{
			if (_doSome == null)
			{
				Debug.LogErrorFormat("[ScheduleHelper][Tasks] _doSome == null, Maybe Object Using This Task That is Destoryed. But Not Dipose This.");
				Dispose();
				return;
			}
			_doSome();
		}
	}

	[AddComponentMenu("")]  //  避免手動掛上去
	public class ScheduleHelper : MonoBehaviour
	{
		private static ScheduleHelper _instance;

		public static ScheduleHelper Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GameObject("[Static]ScheduleHelper").AddComponent<ScheduleHelper>();
					DontDestroyOnLoad(_instance);
				}
				return _instance;
			}
		}

		/// <summary>
		/// 加一個排程
		/// Caller 務必要在Destroy前，dispose Task
		/// </summary>
		/// <param name="doSomething">做什麼</param>
		/// <param name="duration">多久執行一次</param>
		/// <param name="count">幾次</param>
		/// <returns>Task</returns>
		public ITask NewScheule(Action doSomething, float duration, int count, Action<ITask> onFinish = null)
		{
			return new Tasks(StartCoroutine, StopCoroutine, doSomething, duration, count, onFinish);
		}

		/// <summary>
		/// Caller 務必要在Destroy前，dispose Task
		/// </summary>
		/// <param name="doSomething">做什麼</param>
		/// <param name="delayTime">多久後執行</param>
		/// <returns>Task</returns>
		public ITask DelayDo(Action doSomething, float delayTime)
		{
			return new Tasks(StartCoroutine, StopCoroutine, doSomething, delayTime, 1, null, true);
		}
	}
}