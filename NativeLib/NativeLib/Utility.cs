using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

		public static T UnsafeCast<T>(object value)
		{
			return (T)value;
		}

		public static T CreateFromType<T>(Type type)
		{
			return (T) type.GetMethod("CreateDefault", new Type[]{})?.Invoke(null, new object[]{});
		}

		public static IEnumerable<Type> GetParticleNodesFromContext(ParticleNode node, out List<string> types)
		{
			types = new List<string>();
			foreach (var type in node.GetType().Assembly.GetTypes())
			{
				types.Add(type.FullName);
			}

			return node.GetType().Assembly.GetTypes().Where(t => typeof(ParticleNode).IsAssignableFrom(t));
		}

		public static void GetParticleNodeTypesDictionary(ParticleNode node, out Dictionary<string, Type> types, out IEnumerable<string> names)
		{
			Type nodeType = node.GetType();
			Assembly assembly = nodeType.Assembly;
				types = new Dictionary<string, Type>();
			foreach (var type in assembly.GetTypes().Where(t => t != nodeType && typeof(ParticleNode).IsAssignableFrom(t)))
			{
				types.Add(type.Name, type);
			}

			names = types.Keys;
		}

		public static string GetTypeName(object target)
		{
			return $"Target Typename: {target.GetType().Name}; Target Namespace: {target.GetType().Namespace}";
		}

		public static bool UnsetPinsExisting(IEnumerable<ParticleNode> nodes)
		{
			return nodes.Any(node => node.GetNodeSlotManager().GetParticleNodeSlots()
				.Any(slot => slot.Property == null || !slot.Property.Alive));
		}
	}
}
