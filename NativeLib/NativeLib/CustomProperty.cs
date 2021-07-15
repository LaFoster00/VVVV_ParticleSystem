using System;
using System.Collections.Generic;
using System.Linq;

namespace NativeLib
{
	public class CustomProperty
	{
		public bool Alive = true;
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

		public bool GetValueTyped<T>(int index, out T value)
		{
			if (typeof(T) != PropertyType.Type)
			{
				value = default;
				return false;
			}

			if (index >= _values.Capacity)
			{
				_values.AddRange(Enumerable.Repeat(_values[0], index + 1024 - _values.Capacity));
			}
			
			value = (T)_values[index];
			return true;
		}

		public IEnumerable<T> GetValues<T>()
		{
			return _values.Cast<T>();
		}
	}

	public class PropertyManager
	{
		private Dictionary<string, CustomProperty> _properties = new Dictionary<string, CustomProperty>();

		private Dictionary<PropertyType, List<CustomProperty>> _typedProperties =
			new Dictionary<PropertyType, List<CustomProperty>>();

		public PropertyManager Create()
		{
			return new PropertyManager();
		}

		public bool AddProperty(string name, PropertyType type, object initValue, int reserveSize = 2048)
		{
			if (_properties.ContainsKey(name))
			{
				return false;
			}
			else
			{
				_properties.Add(name, new CustomProperty(type, initValue, reserveSize));
				if (!_typedProperties.ContainsKey(type))
				{
					_typedProperties.Add(type, new List<CustomProperty>());
				}
				_typedProperties[type].Add(_properties.Last().Value);
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

		public bool GetProperty<T>(string name, IEnumerable<T> values)
		{
			if (_properties.ContainsKey(name))
			{
				values = _properties[name].GetValues<T>();
				return true;
			}

			values = default;
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
			if (!_properties.ContainsKey(name)) return false;
			_typedProperties.Remove(_properties[name].PropertyType);
			_properties[name].Alive = false;
			_properties.Remove(name);
			return true;
		}

		public bool GetPropertyReference(string name, out CustomProperty property)
		{
			if (!_properties.ContainsKey(name))
			{
				property = null;
				return false;
			}

			property = _properties[name];
			return true;
		}

		public IEnumerable<CustomProperty> GetProperties()
		{
			return _properties.Values.AsEnumerable();
		}

		public IEnumerable<PropertyType> GetPropertyTypes()
		{
			return _properties.Values.Select(property => property.PropertyType);
		}

		public void GetPropertiesOfType(PropertyType type, out bool typeFound, out IEnumerable<CustomProperty> properties)
		{
			if (_typedProperties.ContainsKey(type))
			{
				typeFound = true;
				properties = _typedProperties[type];
			}
			else
			{
				typeFound = false;
				properties = null;
			}
		}
	}
}
