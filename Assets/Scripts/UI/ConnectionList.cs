using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionList : MonoBehaviour
{
	[SerializeField]
	private GameObject button;

	[SerializeField]
	private MonoBehaviour device;

	private List<LinePlacer> placers = new List<LinePlacer>();

	private void Start()
	{
		ConnectionRefreshed();
	}

	public void ConnectionRefreshed()
	{
		var conns = ConnectionManager.Instance.GetConnections(device).Length;

		int goal = conns + 1;
		int delta = goal - placers.Count;

		if (delta > 0)
		{
			for (int i = 0; i < delta; i++)
			{
				CreateButton();
			}
		}
	}

	private void CreateButton()
	{
		var placer = Instantiate(button, transform).GetComponent<LinePlacer>();
		placer.SetDevice(device);
		placers.Add(placer);
	}
}
