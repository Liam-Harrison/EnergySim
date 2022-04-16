using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BatteryInfoPanel : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI state;

	[SerializeField]
	private TextMeshProUGUI watts;

	[SerializeField]
	private TextMeshProUGUI joules;

	[SerializeField]
	private TextMeshProUGUI enable;

	[SerializeField]
	private Battery battery;

	private void Update()
	{
		watts.text = $"Demand: {battery.WattsUsed:0}W";
		joules.text = $"Stored: {battery.JoulesAvaliable:0}J ({battery.PercentFull * 100:0}%)";

		if (battery.IsConnected == false)
			state.text = "Disconnected";
		else if (battery.IsPowered == false)
			state.text = "Unpowered";
		else
			state.text = "Powered";
	}

	public void Toggle()
	{
		battery.Operational = !battery.Operational;
		enable.text = battery.Operational ? "Enabled" : "Disabled";
	}
}
