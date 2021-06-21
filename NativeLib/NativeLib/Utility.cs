using System;

namespace NativeLib
{
	public static class Utility
	{
		public static bool IsNull(object value)
		{
			return value == null;
		}

		public static void CreateIfNull<T>(T target, out T outTarget)
		{
			if (target == null)
			{
				outTarget = Activator.CreateInstance<T>();
			}
			else
			{
				outTarget = target;
			}
		}
	}
}
