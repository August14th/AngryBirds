using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxScrolling : MonoBehaviour
{

	private Camera _camera;

	private Vector3 _previousPos;

	public float ParallaxFactor;

	private void Start()
	{
		_camera = Camera.main;
		_previousPos = _camera.transform.position;
	}

	private void Update()
	{
		var pos = _camera.transform.position;
		Vector3 delta = pos - _previousPos;
		delta.y = delta.z = 0;
		transform.position += delta / ParallaxFactor;
		_previousPos = pos;
	}
}
