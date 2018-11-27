using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Tim.Utility
{
	[ExecuteInEditMode]
	public class PreviewInEditor : MonoBehaviour
	{
		public bool SelectedShow;
		public Color Color = Color.yellow;
		public Vector3 Size = new Vector3(1, 0.5f, 1);

		private void OnDrawGizmos()
		{
			if (SelectedShow)
				return;

			Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
			Gizmos.color = Color;
			Gizmos.DrawCube(Vector3.zero, Size);
		}

		void OnDrawGizmosSelected()
		{
			if (!SelectedShow)
				return;

			Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
			Gizmos.color = Color;
			Gizmos.DrawCube(Vector3.zero, Size);
		}
	}
}