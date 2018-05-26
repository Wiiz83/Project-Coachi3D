using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class AnimalControllerScript : MonoBehaviour {
	[SerializeField] Animator anim;
	private Rigidbody rb;
	// navigation
	NavMeshAgent agent;
	// positions
	Vector3 center_point;
	Vector3 bed1_point;
	Vector3 bed2_point;
	Vector3 destination;
	bool isStopped = true;

	float speed = 0.5f;

	// Use this for initialization
	void Start () {
		loadPoints ();
		rb = GetComponent<Rigidbody> ();
		agent = GetComponent<NavMeshAgent> ();
		anim_lieDown ();
	}

	// Update is called once per frame
	void Update () {

		// move
		if (Input.GetKeyDown (KeyCode.F1))
			goToCenter ();
		if (Input.GetKeyDown (KeyCode.F2))
			goToBed1 ();
		if (Input.GetKeyDown (KeyCode.F3))
			goToBed2 ();
		if (Input.GetKeyDown (KeyCode.S))
			stopMovement ();

		// state
		if (Input.GetKeyDown (KeyCode.F5))
			anim_sit ();
		if (Input.GetKeyDown (KeyCode.F6))
			anim_stand ();
		if (Input.GetKeyDown (KeyCode.F7))
			anim_lieDown ();
		if (Input.GetKeyDown (KeyCode.F8))
			anim_twoFeet ();

		//Actions
		if (Input.GetKeyDown (KeyCode.F9))
			anim_bark ();
		if (Input.GetKeyDown (KeyCode.F10))
			anim_jump ();
	}

	void FixedUpdate () {
		// detect stopped
		if (Vector3.Distance (transform.position, agent.destination) <= 1f) {
			if (!isStopped) {
				anim_stop ();
				isStopped = true;
			}
		}
	}

	// Movements
	public void moveTo (Vector3 destination) {
		isStopped = false;
		anim_walk ();
		this.destination = destination;
		agent.destination = destination;
	}

	public void goToBed1 () {
		moveTo (bed1_point);
	}
	public void goToBed2 () {
		moveTo (bed2_point);
	}
	public void goToCenter () {
		moveTo (center_point);
	}

	public void stopMovement () {
		if (isStopped)
			return;
		moveTo (transform.position);
	}

	// Animation 

	public void anim_stop () {
		anim_speed (0);
		anim_stand ();
	}
	public void anim_stand () {
		anim_idleState (0);
	}
	public void anim_sit () {
		anim_idleState (1);
	}
	public void anim_lieDown () {
		anim_idleState (2);
	}
	public void anim_twoFeet () {
		anim_idleState (3);
	}
	public void anim_bark () {
		anim_trigger ("Bark");
	}

	public void anim_jump () {
		anim_trigger ("Jump");
	}

	private void anim_walk () {
		anim_speed (speed);
	}

	private void anim_speed (float speed) {
		anim.SetFloat ("Speed", Math.Abs (speed));
	}

	private void anim_idleState (int state) {
		anim.SetFloat ("Idle State", state);
	}
	private void anim_trigger (String trigger) {
		anim.SetTrigger (trigger);
	}

	// positions
	private void loadPoints () {
		center_point = GameObject.Find ("center_point").transform.position;
		bed1_point = GameObject.Find ("bed1_point").transform.position;
		bed2_point = GameObject.Find ("bed2_point").transform.position;
	}

}