using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tim.Utility.CheckerSequence
{
	/// <summary>
	/// Unity Can't Serialize Interface ,So Create this class for editor.
	/// </summary>
	public abstract class CheckerMono : MonoBehaviour, IChecker
	{
		public abstract void Check(Action<bool> isNeedPlay);
		public abstract void Play(Action done);
	}

	public interface IChecker
	{
		void Play(Action done);
		void Check(Action<bool> isNeedPlay);
	}

	public class CheckerSequence
	{
		public Action OnPlayBegin = delegate { };
		public Action OnPlayAllDone = delegate { };
		public event Action OnCheckStart = delegate { };
		public event Action OnCheckEnd = delegate { };

		private List<IChecker> _checkerSequence = new List<IChecker>();
		private int _index = 0;
		private int _playedCount = 0;

		public int PlayedCount
		{
			get
			{
				return _playedCount;
			}
		}

		public void Insert(IChecker ic, int order)
		{
			_checkerSequence.Insert(order, ic);
		}

		/// <summary>
		/// Join the last one
		/// </summary>
		/// <param name="c"></param>
		public void Add(IChecker c)
		{
			_checkerSequence.Add(c);
		}

		public void Start()
		{
			if (_index > 0)
			{
				Debug.LogError("[CheckerSequence][Start] sequence is Playing , Finish Not Yet!");
				return;
			}

			_playedCount = 0;
			OnPlayBegin.Invoke();
			CheckAndPlay();
		}

		private void CheckAndPlay()
		{
			if (_index < _checkerSequence.Count)
			{
				IChecker unit = _checkerSequence[_index];
				_index++;

				string className = unit.GetType().Name;
				Debug.LogFormat("[CheckerSequence][CheckAndPlay] className Checking {0} , _index {1}", className, _index);

				OnCheckStart.Invoke();
				unit.Check((isNeedOpen) =>
				{
					OnCheckEnd.Invoke();
					Debug.LogFormat("[CheckerSequence][CheckAndPlay] className Checked {0} isNeedOpen {1}", className, isNeedOpen);

					if (isNeedOpen)
					{
						_playedCount++;
						unit.Play(CheckAndPlay);
					}
					else
					{
						CheckAndPlay();
					}
				});
			}
			else
			{
				Debug.LogFormat("[UILobbyEnterSequence][CheckAndPlay] All Done.");
				OnPlayAllDone.Invoke();
				_index = 0;
			}
		}
	}
}
