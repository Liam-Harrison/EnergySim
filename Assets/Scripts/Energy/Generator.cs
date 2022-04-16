using UnityEngine;

public abstract class Generator : MonoBehaviour, IEnergyProducer, ICircuitConnected
{
	// Interface Properties

	public virtual float JoulesAvaliable { get; protected set; }

	// Properties

	private bool operational = true;

	public bool Operational
	{
		get => operational;
		set
		{
			var changed = operational != value;
			operational = value;
			if (changed) OperationalChanged();
		}
	}

	public virtual float Capacity { get => capacity; }

	public bool IsEmpty { get => JoulesAvaliable <= 0f; }

	public float WattageRating { get => wattageRating; }

	public int CircuitID { get => Game.Instance.CircuitManager.GetCircuitID(this); }

	public float PercentFull
	{
		get
		{
			if (Capacity == 0)
				return 0;
			return JoulesAvaliable / Capacity;
		}
	}

	// Inspector

	[SerializeField]
	private float wattageRating;

	[SerializeField]
	private float capacity;

	protected virtual void OperationalChanged()
	{

	}

	public void OnEnable()
	{
		Game.Instance.EnergySimulation.AddGenerator(this);
		Game.Instance.CircuitManager.AddGenerator(this);
	}

	public void OnDisable()
	{
		Game.Instance?.EnergySimulation.RemoveGenerator(this);
		Game.Instance?.CircuitManager.RemoveGenerator(this);
	}

	public virtual void EnergyUpdate(float dt)
	{
		CheckConnection();
	}

	public virtual void ApplyJouleDelta(float delta)
	{
		JoulesAvaliable = Mathf.Clamp(JoulesAvaliable + delta, 0, Capacity);
	}

	public void ConsumeEnergy(float joules)
	{
		JoulesAvaliable = Mathf.Max(0, JoulesAvaliable - joules);
	}

	private void CheckConnection()
	{

	}
}
