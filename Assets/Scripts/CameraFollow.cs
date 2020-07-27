using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraFollow : MonoBehaviour
{
	private Vector3 _startPos;

	private GameObject _birdToFollow;

	private const float MinX = 0;
	
	private const float MaxX = 10;

	// Use this for initialization
	void Start ()
	{
		_startPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (_birdToFollow)
		{
			var birdPos = _birdToFollow.transform.position;
			float x = Mathf.Clamp(birdPos.x, transform.position.x, MaxX);
			transform.position = new Vector3(x, _startPos.y, _startPos.z);
		}
	}

	public void StartFollow(GameObject bird)
	{
		_birdToFollow = bird;
	}

	public void MoveToStartPos(UnityAction onCompleted)
	{
		float duration = Vector2.Distance(transform.position, _startPos) / 10f;
		if (duration < 0.1f) duration = 0.1f;
		transform.positionTo(duration, _startPos).setOnCompleteHandler(x => {onCompleted.Invoke();});
	}
}
