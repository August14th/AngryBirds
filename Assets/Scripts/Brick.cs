using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Brick : MonoBehaviour
{

	public float Health = 100;

	private void OnCollisionEnter2D(Collision2D other)
	{
		var otherRigidBody = other.gameObject.GetComponent<Rigidbody2D>();
		if (otherRigidBody == null) return;
		float damage = otherRigidBody.velocity.magnitude * 10;
		if (damage >= 10)
		{
			// GetComponent<AudioSource>().Play();
		}

		Health -= damage;
		if (Health <= 0) Destroy(gameObject);
	}
}
