using System;
using System.Collections.Generic;
using System.Linq;

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
	}

	public readonly struct CustomProperty
	{
		public readonly PropertyType PropertyType;
		private readonly List<object> _values;

		public CustomProperty(PropertyType type, object value, int reserveSize)
		{
			PropertyType = type;
			_values = new List<object>(Enumerable.Repeat(value, reserveSize));
		}

		public void SetValue(int index, object value)
		{
			if (index >= _values.Capacity)
			{
				_values.AddRange(Enumerable.Repeat(_values[0], index + 1024 - _values.Capacity));
			}

			_values[index] = value;
		}

		public object GetValue(int index)
		{
			if (index >= _values.Capacity)
			{
				_values.AddRange(Enumerable.Repeat(_values[0], index + 1024 - _values.Capacity));
			}

			return _values[index];
		}
	}

	public class PropertyManager
	{
		private Dictionary<string, CustomProperty> _properties = new Dictionary<string, CustomProperty>();

		public bool AddProperty(string name, PropertyType type, object initValue, int reserveSize = 2048)
		{
			if (_properties.ContainsKey(name))
			{
				return false;
			}
			else
			{
				_properties.Add(name, new CustomProperty(type, initValue, reserveSize));
				return true;
			}
		}

		public bool DoesPropertyExist(string name)
		{
			return _properties.ContainsKey(name);
		}

		public bool SetPropertyAtIndex<T>(string name, T value, int index = 1)
		{
			if (_properties.ContainsKey(name) && _properties[name].PropertyType.Type == typeof(T))
			{
				_properties[name].SetValue(index, value);
				return true;
			}
			
			return false;
		}

		public bool GetPropertyAtIndex<T>(string name, out T value, int index = 1)
		{
			if (_properties.ContainsKey(name))
			{
				value = (T) _properties[name].GetValue(index);
				return true;
			}

			value = default;
			return false;
		}

		public bool DeleteProperty(string name)
		{
			return _properties.Remove(name);
		}

		public IEnumerable<CustomProperty> GetProperties()
		{
			return _properties.Values.AsEnumerable();
		}

		public IEnumerable<PropertyType> GetPropertyTypes()
		{
			return _properties.Values.Select(property => property.PropertyType);
		}
	}
}
