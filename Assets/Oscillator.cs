using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{

	[SerializeField] private Vector3 movementVector = new Vector3(10f, 10f, 10f);
	[SerializeField] private float period = 2f;
	
	// todo remove from inspector later
	[Range(0,1)] [SerializeField] private float movementFactor; // 0 not moved, 1 for fully moved

	private Vector3 startingPos;
	
	// Use this for initialization
	void Start ()
	{
		startingPos = transform.position;
	}
	
	// Update is called once per frame
	void Update ()
	{
		// protect against period is zero
		if (period <= Mathf.Epsilon)
		{
			return;
		}

		float cycles = Time.time / period; // grows continually from 0

		const float tau = Mathf.PI * 2f;
		float rawSinWave = Mathf.Sin(cycles * tau); // goes from -1 to +1

		movementFactor = rawSinWave / 2f + 0.5f;
		Vector3 offset = movementVector * movementFactor;
		transform.position = startingPos + offset;
	}
}
