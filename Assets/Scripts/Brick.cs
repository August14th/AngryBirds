using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Brick : Enemy
{

	public float Health = 100;

	private void OnCollisionEnter2D(Collision2D other)
	{
		var otherRigidBody = other.gameObject.GetComponent<Rigidbody2D>();
		if (otherRigidBody == null) return;
		float damage = otherRigidBody.velocity.magnitude * 10;
		Health -= damage;
		if (Health <= 0) Dead();
	}


	public override void Dead()
	{
		Destroy(gameObject);
	}
}
