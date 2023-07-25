using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeOnAwake : MonoBehaviour
{
	private Explodable _explodable;

	public void explode(string target)
	{
		transform.position = GameObject.Find(target).transform.position;
		_explodable = GetComponent<Explodable>();
		_explodable.explode();
		ExplosionForce ef = FindObjectOfType<ExplosionForce>();
		ef.doExplosion(transform.position);
	}
}
