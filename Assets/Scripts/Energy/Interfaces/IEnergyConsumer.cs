using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnergyConsumer
{
	float WattsUsed { get; }
	float WattsNeededWhenActive { get; }

	int PowerOrder { get; }

	bool IsConnected { get; }
	bool IsPowered { get; }

	void SetConnectionStatus(ConnectionStatus status);
}
