using UnityEngine;
using UnityEngine.UI;

public class LinePlacer : MonoBehaviour
{
	[SerializeField]
	private MonoBehaviour device;

	public MonoBehaviour Device { get => device; }

	public void SetDevice(MonoBehaviour device)
	{
		this.device = device;
	}

	public void ButtonPressed(Button button)
	{
		ConnectionManager.Instance.ButtonPressed(device, button);
	}
}
