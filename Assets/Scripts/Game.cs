using UnityEngine;

public class Game : MonoBehaviour
{
	private const float ENERGY_SIM_UPDATE_HZ = 10;

	private static Game instance;

	public static Game Instance
	{
		get
		{
			if (instance == null)
			{
				instance = FindObjectOfType<Game>();
				instance?.Initalize();
			}
			return instance;
		}
	}

	public CircuitManager CircuitManager { get; private set; }

	public EnergySimulation EnergySimulation { get; private set; }

	public NetworkManager<ElectricalNetwork, Wire> WireSystem { get; private set; }

	private float lastUpdate;

	private void Initalize()
	{
		CircuitManager = new CircuitManager();
		EnergySimulation = new EnergySimulation();
		WireSystem = new NetworkManager<ElectricalNetwork, Wire>();
	}

	public void OnDisable()
	{
		instance = null;
	}

	public void Update()
	{
		var t = 1 / ENERGY_SIM_UPDATE_HZ;
		if (Time.time >= lastUpdate + t)
		{
			EnergyUpdate(Time.time - lastUpdate);
			lastUpdate = Time.time;
		}
	}

	public void EnergyUpdate(float dt)
	{
		CircuitManager.EnergyUpdateEarly(dt);
		EnergySimulation.EnergyUpdate(dt);
		CircuitManager.EnergyUpdateLate(dt);
	}
}
