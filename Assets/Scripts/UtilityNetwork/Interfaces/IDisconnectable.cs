using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDisconnectable
{
	bool Connect();
	bool Disconnect();
	bool IsDisconnected { get; }
}
