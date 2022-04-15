using System;
using System.Collections.Generic;
using UnityEngine;

public class CircuitManager
{
	public bool IsDirty { get; private set; }

	private List<CircuitInfo> circuits = new List<CircuitInfo>();

	private List<Generator> loop_generators = new List<Generator>();

	private List<EnergyConsumer> consumers = new List<EnergyConsumer>();
	private List<Generator> generators = new List<Generator>();

	public void AddGenerator(Generator generator)
	{
		if (!generators.Contains(generator)) generators.Add(generator);
	}

	public void RemoveGenerator(Generator generator)
	{
		if (generators.Contains(generator)) generators.Remove(generator);
	}

	public void AddEnergyConsumer(EnergyConsumer consumer)
	{
		if (!consumers.Contains(consumer)) consumers.Add(consumer);
	}

	public void RemoveEnergyConsumer(EnergyConsumer consumer)
	{
		if (consumers.Contains(consumer)) consumers.Remove(consumer);
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

			loop_generators.Sort((a, b) => a.JoulesAvaliable.CompareTo(b.JoulesAvaliable));

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

						// Add transformers and batteries and stuff here if added.

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

		// Charge batteries & transformers here with whatever joules are remaining.
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
					circuit.consumers.Add(consumer);
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
}

public struct CircuitInfo
{
	public float wattsUsed;
	public float minBatteryPercent;

	public List<IEnergyConsumer> consumers;
	public List<Generator> generators;
}

public enum ConnectionStatus
{
	NotConnected,
	Unpowered,
	Powered
}