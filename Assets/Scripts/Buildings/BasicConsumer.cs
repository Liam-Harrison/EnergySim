using UnityEngine;

public class BasicConsumer : EnergyConsumer
{
	[SerializeField]
	private Material NotConnected;

	[SerializeField]
	private Material Powered;

	[SerializeField]
	private Material Unpowered;

	public override void EnergyUpdate(float dt)
	{
		base.EnergyUpdate(dt);
	}

	protected override void OperationalChanged()
	{

	}

	public override void SetConnectionStatus(ConnectionStatus status)
	{
		base.SetConnectionStatus(status);

		switch (status)
		{
			case ConnectionStatus.NotConnected:
				GetComponent<MeshRenderer>().material = NotConnected;
				break;
			case ConnectionStatus.Unpowered:
				GetComponent<MeshRenderer>().material = Unpowered;
				break;
			case ConnectionStatus.Powered:
				GetComponent<MeshRenderer>().material = Powered;
				break;
			default:
				break;
		}
	}
}
