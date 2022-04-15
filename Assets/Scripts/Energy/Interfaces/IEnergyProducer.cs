using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnergyProducer
{
	float JoulesAvaliable { get; }

	void ConsumeEnergy(float joules);
}
