using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasScaler : MonoBehaviour
{
	[SerializeField]
	private Vector2 worldSize;

	[SerializeField]
	private Vector2 canvasSize;

	private void OnValidate()
	{
		var scale = worldSize / canvasSize;

		if (scale.x == 0 || scale.y == 0)
			return;

		GetComponent<RectTransform>().sizeDelta = canvasSize;
		GetComponent<RectTransform>().localScale = scale;
	}
}
