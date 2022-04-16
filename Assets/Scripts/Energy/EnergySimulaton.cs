using System;
using System.Collections.Generic;

/// <summary>
/// Energy simulation tracks all energy items in the game indiscriminatly, it is used to quickly update all
/// objects simultaneously and ensure the state of the energy simulation is valid before the main circuit update.
/// </summary>
public class EnergySimulation
{
	private List<EnergyConsumer> consumers = new List<EnergyConsumer>();
	private List<Generator> generators = new List<Generator>();
	private List<Battery> batteries = new List<Battery>();

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

	public void AddBattery(Battery battery)
	{
		if (!batteries.Contains(battery)) batteries.Add(battery);
	}

	public void RemoveBattery(Battery battery)
	{
		if (batteries.Contains(battery)) batteries.Remove(battery);
	}

	/// <summary>
	/// Ensure all energy generators and consumers are connected, update info if circuit has changed.
	/// </summary>
	/// <param name="dt">delta time.</param>
	public void EnergyUpdate(float dt)
	{
		foreach (var generator in generators)
		{
			generator.EnergyUpdate(dt);
		}
		foreach (var battery in batteries)
		{
			battery.EnergyUpdate(dt);
		}
		foreach (var consumer in consumers)
		{
			consumer.EnergyUpdate(dt);
		}
	}
}
