using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NativeLib
{
	public class PropertyType
	{
		public string Name;
		public Type Type;

		public PropertyType(string name, Type type)
		{
			Name = name;
			Type = type;
		}

		public PropertyType(string name, object typeContext)
		{
			Name = name;
			Type = typeContext.GetType();
		}

		public object GetDefaultValue()
		{
			if (Type.IsValueType)
			{
				if (Type.GetMethod("CreateDefault") != null)
				{
					Type.GetMethod("CreateDefault").Invoke(null, new object[] { });
				}
				else
				{
					return Activator.CreateInstance(Type);
				}
			}

			throw new InvalidOperationException("Property Type is not value type. Cant create default.");
		}
	}

	public class PropertyTypeManager
	{
		private Dictionary<string, PropertyType> _propertyTypes = new Dictionary<string, PropertyType>();
		private Dictionary<Type, PropertyType> _typedPropertyTypes = new Dictionary<Type, PropertyType>();

		private List<string> _typeNames = new List<string>();

		public bool AddType<T>(string typeName, T typeObject)
		{
			if (_propertyTypes.ContainsKey(typeName)) return false;

			_propertyTypes.Add(typeName, new PropertyType(typeName, typeof(T)));
			_typedPropertyTypes.Add(typeof(T), _propertyTypes[typeName]);
			_typeNames.Add(typeName);
			return true;
		}

		public bool GetType(string typeName, out PropertyType type)
		{
			type = null;
			if (!_propertyTypes.ContainsKey(typeName)) return false;

			type = _propertyTypes[typeName];
			return true;
		}

		public bool DeleteType(string typeName)
		{
			if (!_propertyTypes.ContainsKey(typeName)) return false;

			_typedPropertyTypes.Remove(_propertyTypes[typeName].Type);
			_propertyTypes.Remove(typeName);
			_typeNames.Remove(typeName);
			return true;
		}

		public IEnumerable<string> GetTypeNames()
		{
			return _typeNames.AsEnumerable();
		}

		public bool TypeExists<T>()
		{
			return _typedPropertyTypes.ContainsKey(typeof(T));
		}

		public bool TypeExists<T>(T context)
		{
			return _typedPropertyTypes.ContainsKey(typeof(T));
		}

		public bool TypeExists(string name)
		{
			return _propertyTypes.ContainsKey(name);
		}
	}
}
