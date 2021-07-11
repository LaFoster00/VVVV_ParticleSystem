using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace NativeLib
{
	public interface ParticleNode
	{
		//use to define the node sockets needed
		ParticleNodeSlotManager Define(PropertyTypeManager typeManager);
		ParticleNodeSlotManager GetNodeSlotManager();
		//used to execute specific behaviour
		void Execute();
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

		public bool AddNodeSlot(string name, PropertyType type)
		{
			if (_nodeSlots.ContainsKey(name)) return false;
			_nodeSlots.Add(name, new ParticleNodeSlot()
			{
				Name = name,
				PropertyType = type
			});
			return true;
		}


		public bool GetPropertyFromSlot(string slotName, out CustomProperty property)
		{
			if (_nodeSlots.ContainsKey(slotName))
			{
				property = _nodeSlots[slotName].Property;
				return true;
			}

			property = null;
			return false;
		}

		/*public bool AddNodeSlot(string name, CustomProperty property)
		{
			if (_nodeSlots.ContainsKey(name)) return false;
			_nodeSlots.Add(name, new ParticleNodeSlot()
			{
				Name = name,
				PropertyType = property.PropertyType,
				Property = property
			});
			return true;
		}*/

		public void PurgeDeadPropertyReferences()
		{
			foreach (var nodeSlot in _nodeSlots.Where(
					nodeSlot => nodeSlot.Value.Property != null && !nodeSlot.Value.Property.Alive))
			{
				nodeSlot.Value.Property = null;
			}
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
	}

	public class ParticleNodeSlot
	{
		public string Name = "New NodeSocket";
		public PropertyType PropertyType;
		public CustomProperty Property;
	}
}
