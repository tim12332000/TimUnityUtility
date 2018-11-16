using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

namespace Tim.Utility.ObjectPoolSystem
{
	/// <summary>
	/// For Test 
	/// </summary>
	public class Bullet : MonoBehaviour
	{
		private Coroutine _flyTask;

		public Action OnTrigger = delegate { };
		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Bullet"))
			{
				return;
			}

			OnTrigger();
		}

		private void Update()
		{
			transform.Translate(0, Time.deltaTime * 3f, 0, Space.World);
		}
	}
}