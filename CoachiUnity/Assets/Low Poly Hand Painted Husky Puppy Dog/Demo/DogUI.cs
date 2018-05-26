using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogUI : MonoBehaviour
{
	[SerializeField] Animator anim;

	public void SetSpeed(float speed)
	{
		anim.SetFloat("Speed", speed);
	}

	public void SetIdleState(int state)
	{
		anim.SetFloat("Idle State", state);
	}
}
