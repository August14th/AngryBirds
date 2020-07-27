using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackBird : Bird
{

	private readonly List<Enemy> _enemies = new List<Enemy>();

	protected override void CastSkill()
	{
		_enemies.ForEach(e =>
		{
			if (e) e.Dead();
		});
		Destroy(gameObject);
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Pig"))
		{
			_enemies.Add(other.gameObject.GetComponent<Pig>());
		}
		if (other.gameObject.CompareTag("Brick"))
		{
			_enemies.Add(other.gameObject.GetComponent<Brick>());
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Pig"))
		{
			_enemies.Remove(other.gameObject.GetComponent<Pig>());
		}
		if (other.gameObject.CompareTag("Brick"))
		{
			_enemies.Remove(other.gameObject.GetComponent<Brick>());
		}
	}
}
