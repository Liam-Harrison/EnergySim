using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Conduit
{
	None,
}

public class UtilityNetwork
{
	public virtual void AddItem(object item) { }

	public virtual void RemoveItem(object item) { }

	public virtual void ConnectItem(object item) { }

	public virtual bool HasItem(object item) { return false; }

	public virtual void Reset() { }

	public void DisconnectItem(object item) { }

	public int id;

	public Conduit conduit;
}
