using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour {

	private void OnMouseDrag()
	{
		transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		// 调整z轴坐标=0
		transform.position -= new Vector3(0, 0, Camera.main.transform.position.z);
	}
}
