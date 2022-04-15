using System;
using UnityEngine;

public class EnergyConsumer : MonoBehaviour, IEnergyConsumer, ICircuitConnected
{
	// Interface Properties 

	public float WattsUsed { get => IsActive ? WattageRating : 0; }

	public float WattsNeededWhenActive => WattageRating;

	public int PowerOrder => powerOrder;

	public bool IsConnected => CircuitID != ushort.MaxValue;

	public bool IsPowered { get; private set; }

	// Properties

	private bool operational = true;

	public bool Operational
	{
		get => operational;
		set
		{
			var changed = operational != value;
			operational = value;
			if (changed) OperationalChanged();
		}
	}

	public float OverloadTimeRemaining { get; private set; }

	public int CircuitID { get; private set; } = int.MaxValue;

	public float WattageRating { get => watts; }

	public bool IsActive { get; private set; } = true;

	public int NodeID { get; private set; }

	// Inspector

	private MeshRenderer mesh;

	[SerializeField]
	private int powerOrder;

	[SerializeField]
	private float watts;

	public void OnEnable()
	{
		mesh = GetComponent<MeshRenderer>();
		Game.Instance.EnergySimulation.AddEnergyConsumer(this);
		Game.Instance.CircuitManager.AddEnergyConsumer(this);
	}

	public void OnDisable()
	{
		Game.Instance?.EnergySimulation.RemoveEnergyConsumer(this);
		Game.Instance?.CircuitManager.RemoveEnergyConsumer(this);
	}

	public virtual void SetConnectionStatus(ConnectionStatus status)
	{
		switch (status)
		{
			case ConnectionStatus.NotConnected:
				break;
			case ConnectionStatus.Unpowered:
				if (IsPowered)
				{
					IsPowered = false;
					OverloadTimeRemaining = 6;
				}
				break;
			case ConnectionStatus.Powered:
				if (!IsPowered && OverloadTimeRemaining <= 0f)
				{
					IsPowered = true;
				}
				break;
			default:
				return;
		}
	}

	public virtual void EnergyUpdate(float dt)
	{
		CircuitID = Game.Instance.CircuitManager.GetCircuitID(this);

		if (!IsConnected)
		{
			IsPowered = false;
			SetConnectionStatus(ConnectionStatus.NotConnected);
		}
		else if (!IsPowered)
		{
			SetConnectionStatus(ConnectionStatus.Unpowered);
		}
		else
		{
			SetConnectionStatus(ConnectionStatus.Powered);
		}

		OverloadTimeRemaining = Mathf.Max(OverloadTimeRemaining - dt, 0);
	}

	protected virtual void OperationalChanged()
	{

	}
}
