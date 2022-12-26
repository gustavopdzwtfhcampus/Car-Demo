using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectorVal : MonoBehaviour 
{
	public float EffectorDistance = 3.0f;
	public float EffectorRecoverySpeed = 10.0f;
	public bool Inverted = false;
	public AnimationCurve FallOffCurve;

	void Start()
	{
		if (FallOffCurve.length == 0) {
			FallOffCurve = new AnimationCurve (new Keyframe (0, 1), new Keyframe (1, 1));
		}
	}

}
