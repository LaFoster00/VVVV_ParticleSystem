using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace NativeLib
{
	public enum ParticleNodeSlotType
	{
		Property = 0,
		Constant = 1,
	}

	public interface ParticleNode
	{
		string Name();

		//use to define the node sockets needed
		ParticleNodeSlotManager Define(PropertyTypeManager typeManager);
		ParticleNodeSlotManager GetNodeSlotManager();
		//used to execute specific behaviour
		void Execute(float DT);
		void PurgeDeadPropertyReference();
		bool SetNodeSlotPropertyReference(string name, CustomProperty property);
	}

	public class ParticleNodeSlotManager
	{
		private readonly Dictionary<string, ParticleNodeSlot> _nodeSlots = new Dictionary<string, ParticleNodeSlot>();

		public IEnumerable<ParticleNodeSlot> GetParticleNodeSlots()
		{
			return _nodeSlots.Values;
		}

		public bool AddNodeSlot(string name, string defaultProp, PropertyType type)
		{
			if (_nodeSlots.ContainsKey(name)) return false;
			_nodeSlots.Add(name, new ParticleNodeSlot()
			{
				Name = name,
				SlotType = ParticleNodeSlotType.Property,
				PropertyType = type,
				DefaultSocketProp = defaultProp,
			});
			return true;
		}

		public bool AddConstantNodeSlot(string name, PropertyType type, object defaultValue)
		{
			if (_nodeSlots.ContainsKey(name) || type.Type != defaultValue.GetType()) return false;
			_nodeSlots.Add(name, new ParticleNodeSlot()
			{
				Name = name,
				SlotType = ParticleNodeSlotType.Constant,
				PropertyType = type,
				ConstantSlotValue = defaultValue,
			});
			return true;
		}

		public bool GetConstantFromSlot<T>(string slotName, out T Value)
		{
			if (_nodeSlots.ContainsKey(slotName))
			{
				ParticleNodeSlot nodeSlot = _nodeSlots[slotName];
				if (nodeSlot.SlotType == ParticleNodeSlotType.Constant && typeof(T) == nodeSlot.PropertyType.Type)
				{
					Value = (T)nodeSlot.ConstantSlotValue;
					return true;
				}
			}

			Value = default;
			return false;
		}

		public bool GetPropertyFromSlot(string slotName, out CustomProperty property)
		{
			if (_nodeSlots.ContainsKey(slotName) && _nodeSlots[slotName].SlotType == ParticleNodeSlotType.Property)
			{
				property = _nodeSlots[slotName].Property;
				return true;
			}

			property = null;
			return false;
		}

		public void PurgeDeadPropertyReferences()
		{
			foreach (var nodeSlot in _nodeSlots.Where(
					nodeSlot => nodeSlot.Value.Property != null && !nodeSlot.Value.Property.Alive))
			{
				nodeSlot.Value.Property = null;
			}
		}

		public bool SetSlotConstant(string slotName, object value)
		{
			if (_nodeSlots.ContainsKey(slotName))
			{
				ParticleNodeSlot nodeSlot = _nodeSlots[slotName];
				if (nodeSlot.SlotType == ParticleNodeSlotType.Constant && value.GetType() == nodeSlot.PropertyType.Type)
				{
					nodeSlot.ConstantSlotValue = value;
					return true;
				}
			}

			return false;
		}

		public bool SetNodePropertyReference(string name, CustomProperty property)
		{
			if (_nodeSlots.ContainsKey(name))
			{
				_nodeSlots[name].Property = property;
				return true;
			}
			return false;
		}

		public void SetDefaultPropReferences(PropertyManager propertyManager)
		{
			foreach (var nodeSlot in _nodeSlots.Where(pair => pair.Value.SlotType == ParticleNodeSlotType.Property))
			{
				if (propertyManager.GetPropertyReference(nodeSlot.Value.DefaultSocketProp, out CustomProperty property))
				{
					if (property.PropertyType.Type == nodeSlot.Value.PropertyType.Type)
					{
						nodeSlot.Value.Property = property;
					}
				}
			}
		}
	}

	public class ParticleNodeSlot
	{
		public string Name = "New NodeSocket";
		public ParticleNodeSlotType SlotType;
		public PropertyType PropertyType;

		public object ConstantSlotValue;

		public string DefaultSocketProp;
		public CustomProperty Property;

		public bool SetConstantValue<T>(out T value)
		{
			if (typeof(T) == PropertyType.Type)
			{
				value = (T) ConstantSlotValue;
				return true;
			}

			value = default;
			return false;
		}

		public bool CompareType(ParticleNodeSlotType type)
		{
			return type == SlotType;
		}
	}
}
