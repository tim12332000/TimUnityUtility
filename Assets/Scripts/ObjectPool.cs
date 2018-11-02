using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tim.Utility
{
	public class ObjectPool<T> : IDisposable where T : Component
	{
		public Action<T> BeforeSpawnAction;

		public Transform _parent = null;

		public List<T> SpawnedPool
		{
			get
			{
				return _spawnedPool;
			}
		}

		private List<T> _spawnedPool = new List<T>();
		private Queue<T> _pool = new Queue<T>();
		private T _prefab = null;

		public int Count
		{
			get
			{
				return _pool.Count;
			}
		}

		public ObjectPool(T prefab, Transform parent, int preloadCount)
		{
			if (parent != null)
				_parent = parent;

			_prefab = prefab;

			NewBuffer(preloadCount);
		}

		public ObjectPool(T prefab, Transform parent) : this(prefab, parent, 0)
		{
		}

		public ObjectPool(T prefab) : this(prefab, null)
		{
		}

		public T Spawn()
		{
			T result = null;

			if (_pool.Count > 0)
			{
				result = _pool.Dequeue();
			}
			else
			{
				// asyc
				result = Clone();
			}

			if (BeforeSpawnAction != null)
				BeforeSpawnAction(result);

			result.gameObject.SetActive(true);

			if (_parent)
				result.transform.SetParent(_parent, false);

			_spawnedPool.Add(result);

			return result;
		}

		public T Spawn(Transform parent)
		{
			T t = Spawn();
			t.transform.SetParent(parent, false);
			return t;
		}

		public void Despwan(T go)
		{
			_pool.Enqueue(go);
			_spawnedPool.Remove(go);
			go.gameObject.SetActive(false);
			if (_parent != null)
			{
				go.transform.SetParent(_parent);
			}
		}

		/// <summary>
		/// Despwan All form ObjectPool Spwan()
		/// </summary>
		public void DespwanAll()
		{
			foreach (T go in _spawnedPool.ToList())
			{
				Despwan(go);
			}
		}

		public void Dispose()
		{
			foreach (var s in _spawnedPool.ToList())
			{
				UnityEngine.Object.Destroy(s.gameObject);
			}

			foreach (var s in _pool.ToList())
			{
				UnityEngine.Object.Destroy(s.gameObject);
			}
		}

		public IEnumerator AsyncNewBuffer(int num, int numOfFrame = 1)
		{
			int index = num;
			while (index > 0)
			{
				for (int j = 0; j < numOfFrame; j++)
				{
					if (index > 0)
					{
						index--;
						NewBuffer();
					}
				}

				yield return null;
			}
		}

		public void NewBuffer(int num)
		{
			for (int i = 0; i < num; ++i)
				NewBuffer();
		}

		private void NewBuffer()
		{
			T obj;
			obj = Clone();
			obj.gameObject.SetActive(false);
			_pool.Enqueue(obj);
		}

		private T Clone()
		{
			T clone;

			if (_parent == null)
				clone = UnityEngine.Object.Instantiate(_prefab.gameObject, _prefab.transform.parent).GetComponent<T>();
			else
				clone = UnityEngine.Object.Instantiate(_prefab.gameObject, _parent).GetComponent<T>();

			clone.name = _prefab.name;

			return clone;
		}
	}
}