using UnityEngine;
using System.Collections;

namespace ExtensionMethods
{

	public static class MyExtensions
	{
		public static Vector2 ToXZ(this Vector3 v)
		{
			return new Vector2(v.x, v.z);
		}
	}
}
