using UnityEngine;
using TMPro;

public class GenInfoPanel : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI text;

	[SerializeField]
	private TextMeshProUGUI wattage;

	[SerializeField]
	private TextMeshProUGUI joules;

	[SerializeField]
	private TextMeshProUGUI enable;

	[SerializeField]
	private Generator generator;

	private void Update()
	{
		wattage.text = $"Rating: {generator.WattageRating:0}W";
		joules.text = $"Joules: {generator.JoulesAvaliable:0}J ({generator.PercentFull * 100:0}%)";
	}

	public void Toggle()
	{
		generator.Operational = !generator.Operational;
		enable.text = generator.Operational ? "Enabled" : "Disabled";
	}
}
