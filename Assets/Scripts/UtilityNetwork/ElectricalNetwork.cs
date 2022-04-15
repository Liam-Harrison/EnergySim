using System.Collections.Generic;

public class ElectricalNetwork : UtilityNetwork
{
	public override void AddItem(object item)
	{
		if (item is Wire wire)
		{
			wires.Add(wire);

			if (wattages.ContainsKey(wire.WattageRating) == false)
				wattages[wire.WattageRating] = new List<Wire>();

			wattages[wire.WattageRating].Add(wire);
		}
	}

	public override void RemoveItem(object item)
	{
		if (item is Wire wire)
		{
			wires.Remove(wire);
			wattages[wire.WattageRating].Remove(wire);
		}
	}

	public override void Reset()
	{
		wires.Clear();
		wattages.Clear();
	}

	public override bool HasItem(object item)
	{
		if (item is Wire wire)
		{
			return wires.Contains(wire);
		}
		else
		{
			foreach (var device in ConnectionManager.Instance.devices)
			{
				if (item == device)
					return true;
			}
		}
		return false;
	}

	public List<Wire> wires = new List<Wire>();

	public Dictionary<WattageRating, List<Wire>> wattages = new Dictionary<WattageRating, List<Wire>>();
}
