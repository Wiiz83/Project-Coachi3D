using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class AnimalControllerScript : MonoBehaviour {
	[SerializeField] Animator anim;
	//private Rigidbody rb;
	// navigation
	float speed = 0.5f;
	bool isStopped = true;
	private bool jumped = false;
	private bool isOnBed = false;
	NavMeshAgent agent;
	// positions
	Vector3 center_point;
	Vector3 bed1_point;
	Vector3 bed2_point;

	void Start () {
		//rb = GetComponent<Rigidbody> ();
		center_point = GameObject.Find ("center_point").transform.position;
		bed1_point = GameObject.Find ("bed1_point").transform.position;
		bed2_point = GameObject.Find ("bed2_point").transform.position;
		agent = GetComponent<NavMeshAgent> ();
		anim_lieDown ();
		agent.autoTraverseOffMeshLink = false;
		isOnBed = isUp ();
	}

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

		// action
		if (Input.GetKeyDown (KeyCode.F9))
			anim_bark ();
		if (Input.GetKeyDown (KeyCode.F10))
			anim_jump ();
	}

	void FixedUpdate () {
		// Agent reached destination
		if (Vector3.Distance (transform.position, agent.destination) <= 0.5f) {
			if (!isStopped) {
				stopAnimal ();
			} else if (isOnBed) {
				anim_lieDown ();
			}
		}

		// Agent reached jump/drop point
		if (agent.isOnOffMeshLink) {
			if (!jumped && !isOnBed) { // Don't jump if already jumped or dropping from bed
				anim_jump ();
				jumped = true;
			}
			// do jump / drop
			OffMeshLinkData data = agent.currentOffMeshLinkData;
			Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
			agent.transform.position = Vector3.MoveTowards (agent.transform.position, endPos, agent.speed * Time.deltaTime);
			// Agent reached jump/drop target point
			if (agent.transform.position == endPos) {
				agent.CompleteOffMeshLink ();
				jumped = false;
				isOnBed = isUp ();
			}
		}
	}

	// Movements
	public void moveTo (Vector3 destination) {
		isStopped = false;
		anim_walk ();
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

	public void stopAnimal () {
		stopMovement ();
		anim_speed (0);
		anim_stand ();
	}

	public void stopMovement () {
		isStopped = true;
		agent.destination = agent.transform.position;
	}

	private bool isUp () {
		return agent.transform.position.y > 1.1f;
	}

	// Animation 

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

}