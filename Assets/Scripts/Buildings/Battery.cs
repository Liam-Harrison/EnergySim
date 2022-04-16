using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Battery : MonoBehaviour, IEnergyConsumer, IEnergyProducer, ICircuitConnected
{
	[SerializeField]
	private int powerOrder;

	[SerializeField]
	private float capacity;

	[SerializeField]
	private float decayPerSecond = 5;

	private float joulesUsedThisUpdate;

	private float dtThisUpdate;

	public bool Operational { get; set; }

	public float WattsUsed { get; private set; }

	public float WattsNeededWhenActive => 0;

	public int PowerOrder => powerOrder;

	public bool IsConnected => ConnectionStatus > ConnectionStatus.NotConnected;

	public bool IsPowered => ConnectionStatus == ConnectionStatus.Powered;

	public float ChargeCapacity { get; private set; }

	public float JoulesAvaliable { get; private set; }

	public bool IsEmpty { get => JoulesAvaliable <= 0f; }

	public virtual float Capacity { get => capacity; }

	public int CircuitID { get => Game.Instance.CircuitManager.GetCircuitID(this); }

	public int NodeID { get; private set; }

	public ConnectionStatus ConnectionStatus { get; private set; } = ConnectionStatus.Powered;

	public float PercentFull
	{
		get
		{
			if (Capacity == 0)
				return 0;
			return JoulesAvaliable / Capacity;
		}
	}

	public void OnEnable()
	{
		Game.Instance.EnergySimulation.AddBattery(this);
		Game.Instance.CircuitManager.AddEnergyConsumer(this);
	}

	public void OnDisable()
	{
		Game.Instance?.EnergySimulation.AddBattery(this);
		Game.Instance?.CircuitManager.RemoveEnergyConsumer(this);
	}

	public virtual void EnergyUpdate(float dt)
	{
		WattsUsed = 0;
		joulesUsedThisUpdate = 0;
		dtThisUpdate = dt;
		ConsumeEnergy(decayPerSecond * dt);
	}

	public void SetConnectionStatus(ConnectionStatus status)
	{
		ConnectionStatus = status;
		if (status == ConnectionStatus.NotConnected)
			Operational = false;
		else
			Operational = JoulesAvaliable > 0;
	}

	public void AddJoules(float joules)
	{
		JoulesAvaliable = Mathf.Min(Capacity, JoulesAvaliable + joules);
		joulesUsedThisUpdate += joules;
		ChargeCapacity -= joules;
		WattsUsed = joulesUsedThisUpdate / dtThisUpdate;
	}

	public void ConsumeEnergy(float joules)
	{
		JoulesAvaliable = Mathf.Max(0, JoulesAvaliable - joules);
	}
}
