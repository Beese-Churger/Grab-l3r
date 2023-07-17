using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeOnAwake : MonoBehaviour
{
	private Explodable _explodable;

	public void explode()
	{
		transform.position = GameObject.Find("Player").transform.position;
		_explodable = GetComponent<Explodable>();
		_explodable.explode();
		ExplosionForce ef = GameObject.FindObjectOfType<ExplosionForce>();
		ef.doExplosion(transform.position);
	}
}
