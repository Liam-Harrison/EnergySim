using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager<NetworkType, BuildingType> where NetworkType : UtilityNetwork, new() where BuildingType : MonoBehaviour
{
	public bool IsDirty { get; private set; }

	public NetworkManager()
	{

	}

	public void Update()
	{
		if (IsDirty)
		{
			IsDirty = false;

			foreach (var network in Networks)
			{
				network.Reset();
			}
			Networks.Clear();
			RebuildNetworks();
		}
	}

	public UtilityNetwork GetNetworkForDevice(object device)
	{
		foreach (var network in Networks)
		{
			if (network.HasItem(device))
				return network;
		}
		return null;
	}

	private void RebuildNetworks()
	{
		var visted = new HashSet<object>();
		var queue = new Queue<object>();
		var devices = ConnectionManager.Instance.devices;

		var wires = new List<Wire>();
		foreach (var device in devices)
		{
			foreach (var conn in ConnectionManager.Instance.GetConnections(device))
			{
				var wire = conn.line.GetComponent<Wire>();
				wires.Add(wire);
			}
		}

		foreach (var wire in wires)
		{
			if (visted.Contains(wire) == false)
			{
				queue.Enqueue(wire);
				visted.Add(wire);
				var network = Activator.CreateInstance<NetworkType>();
				Networks.Add(network);

				while (queue.Count > 0)
				{
					var currrent = (Wire) queue.Dequeue();

					if (currrent is IDisconnectable diss && diss.IsDisconnected)
						continue;
					else if (currrent != null)
					{
						network.AddItem(currrent);
						network.ConnectItem(currrent);
					}

					var conns = ConnectionManager.Instance.GetConnections(currrent.deviceA);
					foreach (var conn in conns)
					{
						if (!visted.Contains(conn.wire))
						{
							visted.Add(conn.wire);
							queue.Enqueue(conn.wire);
						}
					}

					conns = ConnectionManager.Instance.GetConnections(currrent.deviceB);
					foreach (var conn in conns)
					{
						if (!visted.Contains(conn.wire))
						{
							visted.Add(conn.wire);
							queue.Enqueue(conn.wire);
						}
					}
				}
			}
		}
	}

	public void AddToNetwork(object item)
	{
		if (item != null)
		{
			items.Add(item);
		}
		IsDirty = true;
	}

	private List<object> items = new List<object>();

	public List<UtilityNetwork> Networks { get; private set; } = new List<UtilityNetwork>();
}
