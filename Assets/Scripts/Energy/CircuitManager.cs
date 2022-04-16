using System;
using System.Collections.Generic;
using UnityEngine;

public class CircuitManager
{
	public bool IsDirty { get; private set; }

	private List<CircuitInfo> circuits = new List<CircuitInfo>();

	private List<Generator> loop_generators = new List<Generator>();

	private List<IEnergyConsumer> consumers = new List<IEnergyConsumer>();
	private List<Generator> generators = new List<Generator>();

	public void AddGenerator(Generator generator)
	{
		generators.Add(generator);
		IsDirty = true;
	}

	public void RemoveGenerator(Generator generator)
	{
		generators.Remove(generator);
		IsDirty = true;
	}

	public void AddEnergyConsumer(IEnergyConsumer consumer)
	{
		consumers.Add(consumer);
		IsDirty = true;
	}

	public void RemoveEnergyConsumer(IEnergyConsumer consumer)
	{
		consumers.Remove(consumer);
		IsDirty = true;
	}

	public int GetCircuitID(ICircuitConnected entity)
	{
		var network = Game.Instance.WireSystem.GetNetworkForDevice(entity);

		if (network == null) return int.MaxValue;
		return Game.Instance.WireSystem.Networks.IndexOf(network);
	}

	/// <summary>
	/// Refresh circuits and ensure the simulation is in the correct state before updating.
	/// </summary>
	/// <param name="dt">delta time.</param>
	public void EnergyUpdateEarly(float dt)
	{
		RefreshCircuits(dt);
	}

	/// <summary>
	/// Loop over each circuit and update it.
	/// </summary>
	/// <param name="dt">delta time.</param>
	public void EnergyUpdateLate(float dt)
	{
		for (int i = 0; i < circuits.Count; i++)
		{
			CircuitInfo circuit = circuits[i];

			circuit.wattsUsed = 0;

			var generators = circuit.generators;
			var consumers = circuit.consumers;
			var batteries = circuit.batteries;
			var powered = false;
			var hasGens = generators.Count > 0;

			loop_generators.Clear();

			foreach (var generator in generators)
			{
				if (generator.JoulesAvaliable > 0f)
				{
					powered = true;
					loop_generators.Add(generator);
				}
			}

			batteries.Sort((a, b) => a.JoulesAvaliable.CompareTo(b.JoulesAvaliable));
			loop_generators.Sort((a, b) => a.JoulesAvaliable.CompareTo(b.JoulesAvaliable));

			float minPercent = 1;
			foreach (var battery in batteries)
			{
				minPercent = Mathf.Min(minPercent, battery.PercentFull);
			}
			circuit.minBatteryPercent = minPercent;

			if (circuit.minBatteryPercent > 0)
				powered = true;

			if (powered)
			{
				foreach (var consumer in consumers)
				{
					if (consumer is BasicConsumer basic && !basic.Operational)
						continue;

					float wattsNeeded = consumer.WattsUsed * dt;

					if (wattsNeeded > 0)
					{
						var satisfied = false;
						foreach (var generator in loop_generators)
						{
							wattsNeeded = GetJoulesFromGenerator(wattsNeeded, generator, consumer);
							if (wattsNeeded <= 0f)
							{
								satisfied = true;
								break;
							}
						}

						if (!satisfied)
						{
							wattsNeeded = GetJoulesFromBatteries(wattsNeeded, batteries, consumer);
							satisfied = wattsNeeded <= 0f;
						}

						if (satisfied)
						{
							circuit.wattsUsed += consumer.WattsUsed;
						}
						else
						{
							circuit.wattsUsed += consumer.WattsUsed - wattsNeeded / dt;
						}
						consumer.SetConnectionStatus(satisfied ? ConnectionStatus.Powered : ConnectionStatus.Unpowered);
					}
					else
					{
						consumer.SetConnectionStatus(powered ? ConnectionStatus.Powered : ConnectionStatus.Unpowered);
					}
				}
			}
			else if (hasGens)
			{
				foreach (var consumer in consumers)
				{
					consumer.SetConnectionStatus(ConnectionStatus.Unpowered);
				}
			}
			else
			{
				foreach (var consumer in consumers)
				{
					consumer.SetConnectionStatus(ConnectionStatus.NotConnected);
				}
			}

			circuits[i] = circuit;
		}

		foreach (var circuit in circuits)
		{
			circuit.batteries.Sort((a, b) => (a.Capacity - a.JoulesAvaliable).CompareTo(b.Capacity - b.JoulesAvaliable));
			circuit.generators.Sort((a, b) => (a.Capacity - a.JoulesAvaliable).CompareTo(b.Capacity - b.JoulesAvaliable));

			float joules = 0;
			ChargeBatteries(circuit.batteries, circuit.generators, ref joules);

			float minPercent = 1;
			foreach (var battery in circuit.batteries)
			{
				minPercent = Mathf.Min(minPercent, battery.PercentFull);
			}
			circuit.minBatteryPercent = minPercent;
		}
	}

	private void RefreshCircuits(float dt)
	{
		var system = Game.Instance.WireSystem;

		if (system.IsDirty || IsDirty)
		{
			system.Update();
			var networks = system.Networks;
			circuits.Clear();
			foreach (var _ in networks)
			{
				var info = new CircuitInfo
				{
					generators = new List<Generator>(),
					consumers = new List<IEnergyConsumer>(),
					batteries = new List<Battery>(),
					minBatteryPercent = 1,
				};
				circuits.Add(info);
			}

			RebuildCircuitInfo();
		}
	}

	private void RebuildCircuitInfo()
	{
		foreach (var circuit in circuits)
		{
			foreach (var generator in generators)
			{
				var id = GetCircuitID(generator);
				if (id != int.MaxValue)
				{
					circuit.generators.Add(generator);
				}
			}
			foreach (var consumer in consumers)
			{
				var id = GetCircuitID(consumer);
				if (id != int.MaxValue)
				{
					if (consumer is Battery battery)
					{
						circuit.batteries.Add(battery);
						circuit.minBatteryPercent = Mathf.Min(circuit.minBatteryPercent, battery.PercentFull);
					}
					else
					{
						circuit.consumers.Add(consumer);
					}
				}
			}
		}
		IsDirty = false;
	}

	private float GetJoulesFromGenerator(float needed_joules, Generator generator, IEnergyConsumer consumer)
	{
		float joules = Mathf.Min(generator.JoulesAvaliable, needed_joules);
		needed_joules -= joules;
		generator.ApplyJouleDelta(-joules);
		return needed_joules;
	}

	private float GetJoulesFromBatteries(float needed_joules, List<Battery> batteries, IEnergyConsumer consumer)
	{
		foreach (var battery in batteries)
		{
			var joules = battery.JoulesAvaliable;
			if (joules > 0)
			{
				var prev = needed_joules;
				needed_joules = Mathf.Max(0, needed_joules - joules);
				var dt = needed_joules - prev;
				battery.ConsumeEnergy(-dt);

				if (needed_joules <= 0)
					return 0;
			}
		}
		return needed_joules;
	}

	private void ChargeBatteries(List<Battery> batteries, List<Generator> generators, ref float joules_used)
	{
		if (batteries.Count == 0)
			return;

		foreach (var generator in generators)
		{
			bool charging = true;
			while (charging && generator.JoulesAvaliable > 0)
			{
				charging = ChargeBatteriesFromGenerator(batteries, generator, ref joules_used);
			}
		}
	}

	private bool ChargeBatteriesFromGenerator(List<Battery> batteries, Generator generator, ref float joules_used)
	{
		float joules = generator.JoulesAvaliable;
		float consumed = 0;

		for (int i = 0; i < batteries.Count; i++)
		{
			var battery = batteries[i];
			float required = battery.Capacity - battery.JoulesAvaliable;
			if (required > 0)
			{
				float toAdd = Mathf.Min(required, joules / (batteries.Count - i));
				battery.AddJoules(toAdd);
				joules -= toAdd;
				consumed += toAdd;
			}
		}
		if (consumed > 0)
		{
			generator.ApplyJouleDelta(-consumed);
			joules_used += joules;
			return true;
		}
		return false;
	}
}

public class CircuitInfo
{
	public float wattsUsed;
	public float minBatteryPercent;

	public List<IEnergyConsumer> consumers;
	public List<Generator> generators;
	public List<Battery> batteries;
}

public enum ConnectionStatus
{
	NotConnected,
	Unpowered,
	Powered
}