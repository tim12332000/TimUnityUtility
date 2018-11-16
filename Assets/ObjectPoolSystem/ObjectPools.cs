using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tim.Utility.ObjectPoolSystem
{
	public abstract class ObjectPools<TCompoent, TObjectPools> : MonoBehaviour
		where TCompoent : Component
		where TObjectPools : ObjectPools<TCompoent, TObjectPools>
	{
		private static ObjectPools<TCompoent, TObjectPools> _instance;

		protected static readonly Vector3 Farway = new Vector3(99999, 0, 99999);

		public static ObjectPools<TCompoent, TObjectPools> Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GameObject().AddComponent<TObjectPools>();
#if UNITY_EDITOR
					_instance.name = "[ObjectPools]" + typeof(TObjectPools).Name;
#endif
					_instance.transform.position = Farway;
				}

				return _instance;
			}
		}

		public Dictionary<int, ObjectPool<TCompoent>> pools = new Dictionary<int, ObjectPool<TCompoent>>();

		public IEnumerator AsyncPreload(TCompoent source, int num, int numOfFrame = 1)
		{
			yield return GetObjectPool(source).AsyncNewBuffer(num, numOfFrame);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="source"></param>
		/// <param name="num">Preload Num</param>
		public void Preload(TCompoent source, int num)
		{
			GetObjectPool(source).NewBuffer(num);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="source"></param>
		/// <param name="firstPreload">If ObjectPool is first create. reload Count of Obj. </param>
		/// <returns></returns>
		public ObjectPool<TCompoent> GetObjectPool(TCompoent source, int firstPreload = 0)
		{
			string sourceName = source.name;
			int hashName = sourceName.GetHashCode();
			if (!pools.ContainsKey(hashName))
			{
				GameObject poolParent = new GameObject();
#if UNITY_EDITOR
				poolParent.name = typeof(TObjectPools).Name + "Parent[" + sourceName + "]";
#endif
				TCompoent cloneSource = Instantiate(source);
				cloneSource.transform.SetParent(poolParent.transform);
				cloneSource.transform.localPosition = Vector3.zero;
				cloneSource.gameObject.SetActive(false);
				cloneSource.name = sourceName;
				pools[hashName] = new ObjectPool<TCompoent>(cloneSource, poolParent.transform, firstPreload);
			}

			return pools[hashName];
		}

		protected abstract void OnSpawn(TCompoent source);
		protected abstract void OnDespwn(TCompoent source);

		public TCompoent Spawn(TCompoent source)
		{
			TCompoent result = GetObjectPool(source).Spawn();
			OnSpawn(result);
			return result;
		}

		public void Despawn(TCompoent source)
		{
			OnDespwn(source);
			GetObjectPool(source).Despwan(source);
		}

		private void Awake()
		{
			_instance = this;
		}
	}
}