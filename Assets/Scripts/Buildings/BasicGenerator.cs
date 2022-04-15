using System;
using UnityEngine;

public class BasicGenerator: Generator
{
	private const float JOULE_DECAY = -100;

	[SerializeField]
	private Material On;

	[SerializeField]
	private Material Off;

	public override void EnergyUpdate(float dt)
	{
		base.EnergyUpdate(dt);

		if (Operational && WattageRating > 0)
		{
			ApplyJouleDelta(WattageRating * dt);
		}
		else
		{
			ApplyJouleDelta(JOULE_DECAY * dt);
		}
	}

	protected override void OperationalChanged()
	{
		GetComponent<MeshRenderer>().material = Operational ? On : Off;
	}
}
