using UnityEngine;
using TMPro;

public class ConsumerInfoPanel : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI text;

	[SerializeField]
	private TextMeshProUGUI state;

	[SerializeField]
	private TextMeshProUGUI joules;

	[SerializeField]
	private TextMeshProUGUI enable;

	[SerializeField]
	private EnergyConsumer consumer;

	private void Update()
	{
		joules.text = $"Demand: {consumer.WattsUsed:0}W";

		if (consumer.IsConnected == false)
			state.text = "Disconnected";
		else if (consumer.IsPowered == false)
			state.text = "Unpowered";
		else
			state.text = "Powered";
	}

	public void Toggle()
	{
		consumer.Operational = !consumer.Operational;
		enable.text = consumer.Operational ? "Enabled" : "Disabled";
	}
}
