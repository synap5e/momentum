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

		public static float Clamp(this float tilt, float lowerLimit, float upperLimit)
		{
			float newTilt = tilt;
			newTilt = newTilt % 360;
			if (newTilt < 0)
			{
				newTilt += 360;
			}
			if (newTilt <= 180)
			{
				// looking down
				newTilt = Mathf.Min(lowerLimit, newTilt);
			}
			else if (newTilt != 0)
			{
				// looking up
				newTilt = Mathf.Max(upperLimit, newTilt);
			}
			return newTilt;
		}
	}
}
