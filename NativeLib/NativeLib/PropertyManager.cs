using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

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
	}

	public class PropertyTypeManager
	{
		private Dictionary<string, PropertyType> _propertyTypes = new Dictionary<string, PropertyType>();
		private Dictionary<Type, PropertyType> _typedPropertyTypes = new Dictionary<Type, PropertyType>();

		public List<string> TypeNames = new List<string>();

		public bool AddType<T>(string typeName, T typeObject)
		{
			if (_propertyTypes.ContainsKey(typeName)) return false;

			_propertyTypes.Add(typeName, new PropertyType(typeName, typeof(T)));
			_typedPropertyTypes.Add(typeof(T), _propertyTypes[typeName]);
			TypeNames.Add(typeName);
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
			TypeNames.Remove(typeName);
			return true;
		}

		public bool TypeExists<T>()
		{
			return _typedPropertyTypes.ContainsKey(typeof(T));
		}
	}

	public class CustomProperty
	{
		public readonly PropertyType PropertyType;
		public object Value;

		public CustomProperty(PropertyType type, object value)
		{
			PropertyType = type;
			Value = value;
		}
	}

	public class PropertyManager
	{
		private Dictionary<string, CustomProperty> _properties = new Dictionary<string, CustomProperty>();

		public bool AddProperty(string name, PropertyType type, object initValue)
		{
			if (_properties.ContainsKey(name))
			{
				return false;
			}
			else
			{
				_properties.Add(name, new CustomProperty(type, initValue));
				return true;
			}
		}

		public bool DoesPropertyExist(string name)
		{
			return _properties.ContainsKey(name);
		}

		public bool GetProperty<T>(string name, out T value)
		{
			if (_properties.ContainsKey(name) && _properties[name].PropertyType.Type == typeof(T))
			{
				CustomProperty prop = _properties[name];
				if (prop.PropertyType.Type == typeof(T))
				{
					value = (T) prop.Value;
					return true;
				}
			}

			value = default;
			return false;
		}

		public bool DeleteProperty(string name)
		{
			return _properties.Remove(name);
		}

		public List<CustomProperty> GetProperties()
		{
			return _properties.Values.ToList();
		}
	}
}
