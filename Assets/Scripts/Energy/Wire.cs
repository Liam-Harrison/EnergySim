using UnityEngine;
using UnityEngine.UI;

public enum WattageRating
{
	Max1000,
	Max2000,
}

public class Wire : MonoBehaviour, IDisconnectable
{
	LineRenderer lineRenderer;

	public static float GetWattageAsFloat(WattageRating rating)
	{
		switch (rating)
		{
			case WattageRating.Max1000:
				return 1000;
			case WattageRating.Max2000:
				return 2000;
			default:
				return 0;
		}
	}

	public WattageRating WattageRating { get; set; }

	public bool IsDisconnected { get; set; }

	public float OverloadedTime { get; set; }

	public void OnEnable()
	{
		lineRenderer = GetComponent<LineRenderer>();
	}

	public void OnDisable()
	{
		
	}

	public object deviceA;
	public object deviceB;
	public Button a;
	public Button b;

	public void PlacementCompleted(object deviceA, Button a, object deviceB, Button b)
	{
		this.deviceA = deviceA;
		this.deviceB = deviceB;
		this.a = a;
		this.b = b;

		Game.Instance.WireSystem.AddToNetwork(this);
	}

	public void Update()
	{
		if (a == null || b == null)
			return;

		lineRenderer.SetPosition(0, AdjustedLinePosition(a.transform.position));
		lineRenderer.SetPosition(1, AdjustedLinePosition(b.transform.position));
	}

	private Vector3 AdjustedLinePosition(Vector3 pos)
	{
		pos = new Vector3(pos.x, pos.y, -0.75f);
		return pos;
	}

	public bool Connect()
	{
		throw new System.NotImplementedException();
	}

	public bool Disconnect()
	{
		throw new System.NotImplementedException();
	}
}
