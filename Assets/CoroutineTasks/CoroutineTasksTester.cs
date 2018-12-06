using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
namespace Tim.Utility
{
	/// <summary>
	/// Test Evey One Sec Print Log
	/// </summary>
	public class CoroutineTasksTester : MonoBehaviour
	{
		public void Start()
		{
			CoroutineTasks coTasks = new CoroutineTasks();
			//Dynamic Set Ten IEnumerator
			for (int i = 0; i < 10; i++)
			{
				coTasks.Enqueue(WaitOneSec(i));
			}

			StartCoroutine(coTasks.Play(() =>
			{
				Debug.LogFormat("[CoroutineTasksTester][Start] All IEnumerator Done.");
			}));
		}

		private IEnumerator WaitOneSec(int i)
		{
			yield return new WaitForSeconds(1f);
			Debug.LogFormat("[CoroutineTasksTester][WaitOneSec] {0}", i);
		}
	}
}