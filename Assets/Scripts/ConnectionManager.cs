using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Connection
{
	public object da, db;
	public Button a, b;
	public LineRenderer line;
	public Wire wire;
}

public class ConnectionManager : MonoBehaviour
{
	private static ConnectionManager instance;

	public static ConnectionManager Instance
	{
		get => instance ?? (instance = FindObjectOfType<ConnectionManager>());
	}

	[SerializeField]
	private GameObject linePrefab;

	public List<Connection> connections = new List<Connection>();
	public List<object> devices = new List<object>();

	private Connection placing;

	public void Update()
	{
		if (placing != null)
		{
			placing.line.SetPosition(1, AdjustedLinePosition(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
		}

		if (Input.GetMouseButton(0) && placing != null && EventSystem.current.IsPointerOverGameObject() == false)
		{
			Destroy(placing.line.gameObject);
			placing = null;
		}
	}

	public void ButtonPressed(object device, Button button)
	{
		if (TryGetConnection(button, out var conn))
		{
			Button start;
			if (button == conn.a)
				start = conn.b;
			else
				start = conn.a;

			RemoveConnection(button);
			StartLine(device, start);
		}
		else if (placing != null)
		{
			EndLine(device, button);
		}
		else
		{
			StartLine(device, button);
		}
	}

	public void EndLine(object device, Button button)
	{
		if (device == placing.da)
			return;

		placing.b = button;
		placing.db = device;
		placing.line.SetPosition(1, button.transform.position);
		placing.wire = placing.line.gameObject.GetComponent<Wire>();

		connections.Add(placing);

		if (!devices.Contains(placing.da))
		devices.Add(placing.da);

		if (!devices.Contains(placing.db))
			devices.Add(placing.db);

		placing.wire.PlacementCompleted(placing.da, placing.a, placing.db, placing.b);

		placing.a.GetComponentInParent<ConnectionList>().ConnectionRefreshed();
		placing.b.GetComponentInParent<ConnectionList>().ConnectionRefreshed();

		placing = null;
	}

	public void StartLine(object device, Button button)
	{
		var line = Instantiate(linePrefab).GetComponent<LineRenderer>();
		line.SetPosition(0, AdjustedLinePosition(button.transform.position));
		line.SetPosition(1, AdjustedLinePosition(Camera.main.ScreenToWorldPoint(Input.mousePosition)));

		var conn = new Connection();
		conn.a = button;
		conn.line = line;
		conn.da = device;
		placing = conn;
	}

	private Vector3 AdjustedLinePosition(Vector3 pos)
	{
		pos = new Vector3(pos.x, pos.y, -0.75f);
		return pos;
	}

	private void RemoveConnection(Button button)
	{
		if (TryGetConnection(button, out var conn))
		{
			Destroy(conn.line.gameObject);
			connections.Remove(conn);

			for (int i = devices.Count - 1; i >= 0; i--)
			{
				var device = devices[i];
				bool found = false;

				foreach (var c in connections)
				{
					if (c.da == device || c.db == device)
					{
						found = true;
						break;
					}
				}

				if (!found)
					devices.RemoveAt(i);
			}
		}
	}

	public bool TryGetConnection(Button button, out Connection connection)
	{
		foreach (var c in connections)
		{
			if (c.a == button || c.b == button)
			{
				connection = c;
				return true;
			}
		}

		connection = default;
		return false;
	}

	public Connection[] GetConnections(object device)
	{
		List<Connection> devices = new List<Connection>();
		foreach (var conn in connections)
		{
			if (conn.da == device)
				devices.Add(conn);
			else if (conn.db == device)
				devices.Add(conn);
		}
		return devices.ToArray();
	}
}
