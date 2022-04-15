using UnityEngine;


public class CameraDragger : MonoBehaviour
{
	[SerializeField, Header("Translation")]
	private float scalar = 50f;

	[SerializeField, Header("Zooming")]
	private float minDist;

	[SerializeField]
	private float maxDist;

	[SerializeField]
	private float zoomScalar;

	private new Camera camera;

	private void Awake()
	{
		camera = GetComponent<Camera>();
	}

	private void Update()
	{
		if (Input.GetMouseButton(0))
		{
			var delta = new Vector3(-Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y")) * scalar;
			var scale = Mathf.InverseLerp(minDist, maxDist, camera.orthographicSize) * 2 + 1;
			transform.position += delta * scale * Time.deltaTime;
		}

		if (Input.GetAxis("Mouse ScrollWheel") != 0)
		{
			camera.orthographicSize = Mathf.Clamp(camera.orthographicSize + (-Input.GetAxis("Mouse ScrollWheel") * zoomScalar), minDist, maxDist);
		}
	}
}
