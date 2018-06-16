using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum MOVEMENT_STATE { STOPPED, MOVING, JUMPINGHIGH, JUMPINGLOW }
public enum STOPPED_STATE { STANDING, LYINGDOWN, EATING, PLAYING }

public class AnimalControllerScript : MonoBehaviour {
	Animation anim;
	NavMeshAgent agent;
	Vector3 center_point;
	Vector3 bed1_point;
	Vector3 bed2_point;
	Vector3 bowl_position;

	MOVEMENT_STATE movState;
	STOPPED_STATE stopState;

	void Start () {
		center_point = GameObject.Find ("center_point").transform.position;
		bed1_point = GameObject.Find ("bed1_point_dest").transform.position;
		bed2_point = GameObject.Find ("bed2_point_dest").transform.position;
		bowl_position = GameObject.Find ("bowl").transform.position;
		anim = GetComponent<Animation> ();
		agent = GetComponent<NavMeshAgent> ();
		agent.autoTraverseOffMeshLink = false;
		agent.destination = agent.transform.position;
		anim["Walk"].wrapMode = WrapMode.Loop;
		setMovementState (MOVEMENT_STATE.STOPPED);
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.F1))
			goToCenter ();
		if (Input.GetKeyDown (KeyCode.F2))
			goToBed1 ();
		if (Input.GetKeyDown (KeyCode.F3))
			goToBed2 ();
		if (Input.GetKeyDown (KeyCode.F5))
			goToBowlAndEat ();
		if (Input.GetKeyDown (KeyCode.F6))
			goToCenterAndPlay ();
		if (Input.GetKeyDown (KeyCode.S))
			stopMovement ();
	}

	// Movements	

	public void goToBed1 () {
		setStoppedState (STOPPED_STATE.LYINGDOWN);
		moveTo (bed1_point);
		agent.stoppingDistance = 0f;
	}
	public void goToBed2 () {
		setStoppedState (STOPPED_STATE.LYINGDOWN);
		moveTo (bed2_point);
		agent.stoppingDistance = 0f;
	}
	public void goToCenter () {
		setStoppedState (STOPPED_STATE.STANDING);
		moveTo (center_point);
		agent.stoppingDistance = 0f;
	}

	public void stopMovement () {
		setStoppedState (STOPPED_STATE.STANDING);
		agent.destination = agent.transform.position;
	}

	private void moveTo (Vector3 destination) {
		agent.destination = destination;
	}

	// Scenarios

	public void goToBowlAndEat () {
		setStoppedState (STOPPED_STATE.EATING);
		moveTo (bowl_position);
		agent.stoppingDistance = 1.7f;
	}

	public void goToCenterAndPlay () {
		setStoppedState (STOPPED_STATE.PLAYING);
		moveTo (center_point);
		agent.stoppingDistance = 0f;
	}

	/////////////////////*************************************************/////////////////////

	void FixedUpdate () {
		if (Vector3.Distance (agent.transform.position, agent.destination) <= stopRadius ()) {
			setMovementState (MOVEMENT_STATE.STOPPED);
		} else {
			if (agent.isOnOffMeshLink) {
				if (!isUp ()) {
					if (movState != MOVEMENT_STATE.JUMPINGLOW) {
						setMovementState (MOVEMENT_STATE.JUMPINGHIGH);
					}
				} else {
					if (movState != MOVEMENT_STATE.JUMPINGHIGH)
						setMovementState (MOVEMENT_STATE.JUMPINGLOW);
				}

				OffMeshLinkData data = agent.currentOffMeshLinkData;
				Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
				agent.transform.position = Vector3.MoveTowards (agent.transform.position, endPos, agent.speed * Time.deltaTime);
				if (agent.transform.position == endPos) {
					agent.CompleteOffMeshLink ();
				}
			} else {
				setMovementState (MOVEMENT_STATE.MOVING);
			}
		}
	}

	private bool isUp () {
		return agent.transform.position.y > 1.00f;
	}

	private float stopRadius () {
		if (stopState == STOPPED_STATE.EATING) {
			return 1.8f;
		} else {
			return 0.5f;
		}
	}

	// State machine

	private void handleStoppedState () {
		Debug.Log ("HANDLING STOPPED_STATE : " + this.stopState);
		switch (this.stopState) {
			case STOPPED_STATE.STANDING:
				{
					this.anim.CrossFade ("Idled");
					break;
				}
			case STOPPED_STATE.LYINGDOWN:
				{
					this.anim.CrossFade ("Lie Down");
					this.anim.CrossFadeQueued ("Rest");
					break;
				}
			case STOPPED_STATE.EATING:
				{
					this.anim.CrossFade ("start & End Eating");
					break;
				}
			case STOPPED_STATE.PLAYING:
				{
					this.anim.CrossFade ("Jump High");
					this.anim.CrossFadeQueued ("Hit Right");
					this.anim.CrossFadeQueued ("Barking");
					this.anim.CrossFadeQueued ("Hit Front");
					this.anim.CrossFadeQueued ("Idled");
					break;
				}
			default:
				{ break; }
		}
	}

	private void setStoppedState (STOPPED_STATE newState) {
		if (stopState == newState) {
			return;
		}
		logStopState (newState);
		this.stopState = newState;
	}

	private void setMovementState (MOVEMENT_STATE newState) {
		if (movState == newState) {
			return;
		}

		logMoveState (newState);

		switch (newState) {
			case MOVEMENT_STATE.STOPPED:
				{
					handleStoppedState ();
					break;
				}

			case MOVEMENT_STATE.MOVING:
				{
					this.anim.CrossFade ("Walk");
					break;
				}
			case MOVEMENT_STATE.JUMPINGHIGH:
				{
					if (movState == MOVEMENT_STATE.MOVING || movState == MOVEMENT_STATE.STOPPED) {
						this.anim.CrossFade ("Jump High");
					}
					break;

				}
			case MOVEMENT_STATE.JUMPINGLOW:
				{
					break;
				}
			default:
				{ break; }
		}
		this.movState = newState;
	}

	private void logMoveState (MOVEMENT_STATE newState) {
		Debug.Log ("MOVEMENT_STATE : " + this.movState + "-->" + newState);
	}

	private void logStopState (STOPPED_STATE newState) {
		Debug.Log ("STOPPED_STATE : " + this.stopState + "-->" + newState);
	}

}