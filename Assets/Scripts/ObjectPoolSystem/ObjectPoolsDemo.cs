using UnityEngine;
using UnityEditor;
namespace Tim.Utility.ObjectPoolSystem
{
	// You Can Extend ObjectPools For Any Kind Compoent
	public class BulletObjectPools : ObjectPools<Bullet, BulletObjectPools>
	{
		protected override void OnDespwn(Bullet source)
		{
			Debug.LogFormat("[BulletObjectPools][OnDespwn] name {0}", source.name);
		}

		protected override void OnSpawn(Bullet source)
		{
			Debug.LogFormat("[BulletObjectPools][OnSpawn] name {0}", source.name);
		}
	}

	public class ObjectPoolsDemo : MonoBehaviour
	{
		[SerializeField]
		private Bullet _sourceA;
		[SerializeField]
		private Bullet _sourceB;

		[SerializeField]
		private KeyCode _spwanKeyA = KeyCode.A;
		[SerializeField]
		private KeyCode _spwanKeyB = KeyCode.B;

		private BulletObjectPools _pools = new BulletObjectPools();

		public void Update()
		{
			if (Input.GetKeyDown(_spwanKeyA))
			{
				Bullet b = _pools.Spawn(_sourceA);
				b.transform.position = new Vector3(1, 0, 0);
				b.OnTrigger = ()=> _pools.Despawn(b);
			}

			if (Input.GetKeyDown(_spwanKeyB))
			{
				Bullet b = _pools.Spawn(_sourceB);
				b.transform.position = new Vector3(-1, 0, 0);
				b.OnTrigger = () => _pools.Despawn(b);
			}
		}
	}
}