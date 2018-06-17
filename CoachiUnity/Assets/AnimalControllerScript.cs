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
	Vector3 play_point;

	MOVEMENT_STATE previousMovState;
	STOPPED_STATE futureStopState;
	STOPPED_STATE previousStopState;

	void Start () {
		center_point = GameObject.Find ("center_point").transform.position;
		bed1_point = GameObject.Find ("bed1_point_dest").transform.position;
		bed2_point = GameObject.Find ("bed2_point_dest").transform.position;
		bowl_position = GameObject.Find ("bowl").transform.position;
		play_point = GameObject.Find ("play_point").transform.position;
		anim = GetComponent<Animation> ();
		agent = GetComponent<NavMeshAgent> ();
		agent.autoTraverseOffMeshLink = false;
		agent.destination = agent.transform.position;
		anim["Walk"].wrapMode = WrapMode.Loop;
		anim["Simple Idle"].wrapMode = WrapMode.Loop;
		anim["Rest"].wrapMode = WrapMode.Loop;
		setMovementState (MOVEMENT_STATE.STOPPED);
		setStoppedState (STOPPED_STATE.STANDING);
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
		moveTo (bed1_point);
		agent.stoppingDistance = 0f;
		setStoppedState (STOPPED_STATE.LYINGDOWN);

	}
	public void goToBed2 () {
		moveTo (bed2_point);
		agent.stoppingDistance = 0f;
		setStoppedState (STOPPED_STATE.LYINGDOWN);
	}
	public void goToCenter () {
		moveTo (center_point);
		agent.stoppingDistance = 0f;
		setStoppedState (STOPPED_STATE.STANDING);
	}

	public void stopMovement () {
		agent.destination = agent.transform.position;
		setStoppedState (STOPPED_STATE.STANDING);
	}

	private void moveTo (Vector3 destination) {
		agent.destination = destination;
	}

	// Scenarios

	public void goToBowlAndEat () {
		moveTo (bowl_position);
		agent.stoppingDistance = 1.7f;
		setStoppedState (STOPPED_STATE.EATING);
	}

	public void goToCenterAndPlay () {
		moveTo (play_point);
		agent.stoppingDistance = 0f;
		setStoppedState (STOPPED_STATE.PLAYING);
	}

	/////////////////////*************************************************/////////////////////

	void FixedUpdate () {
		if (this.anim.IsPlaying ("Stund Up")) {
			agent.isStopped = true;
		} else {
			agent.isStopped = false;;
		}
		if (Vector3.Distance (agent.transform.position, agent.destination) <= stopRadius ()) {
			setMovementState (MOVEMENT_STATE.STOPPED);
			if (this.futureStopState == STOPPED_STATE.PLAYING)
				transform.LookAt (GameObject.Find ("Camera").transform.position);
		} else {
			if (agent.isOnOffMeshLink) {
				if (!isUp ()) {
					if (previousMovState != MOVEMENT_STATE.JUMPINGLOW) {
						setMovementState (MOVEMENT_STATE.JUMPINGHIGH);
					}
				} else {
					if (previousMovState != MOVEMENT_STATE.JUMPINGHIGH)
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
		if (futureStopState == STOPPED_STATE.EATING) {
			return 1.8f;
		} else {
			return 0.5f;
		}
	}

	// Animations 

	private void animateStopping () {
		Debug.Log ("HANDLING STOPPED_STATE : " + this.futureStopState);
		switch (this.futureStopState) {
			case STOPPED_STATE.STANDING:
				{
					this.anim.CrossFade ("Simple Idle");
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
					GetComponent<AudioSource> ().PlayDelayed (1);
					this.anim.CrossFade ("Barking");
					this.anim.CrossFadeQueued ("Attack");
					this.anim.CrossFadeQueued ("Attack");
					this.anim.CrossFadeQueued ("Lie Down");
					this.anim.CrossFadeQueued ("Rest");
					break;
				}
			default:
				{ break; }
		}
	}

	private void animateStartingToMove () {
		Debug.Log ("HANDLING STOPPED -> MOVED : " + this.futureStopState);
		switch (this.previousStopState) {
			case STOPPED_STATE.STANDING:
				{
					this.anim.CrossFade ("Walk");
					break;
				}
			case STOPPED_STATE.LYINGDOWN:
				{
					this.anim.CrossFade ("Stund Up");
					this.anim.CrossFadeQueued ("Walk");
					break;
				}
			case STOPPED_STATE.EATING:
				{
					this.anim.CrossFade ("Walk");
					break;
				}
			case STOPPED_STATE.PLAYING:
				{
					this.anim.CrossFade ("Stund Up");
					this.anim.CrossFadeQueued ("Walk");
					break;
				}
			default:
				{ break; }
		}
	}

	private void setStoppedState (STOPPED_STATE newState) {
		if (futureStopState == newState) {
			return;
		}
		logStopState (newState);
		this.previousStopState = this.futureStopState;
		this.futureStopState = newState;
	}

	private void setMovementState (MOVEMENT_STATE newState) {
		if (previousMovState == newState) {
			return;
		}

		logMoveState (newState);

		switch (newState) {
			case MOVEMENT_STATE.STOPPED:
				{
					animateStopping ();
					break;
				}

			case MOVEMENT_STATE.MOVING:
				{
					if (previousMovState == MOVEMENT_STATE.STOPPED)
						animateStartingToMove ();
					else
						this.anim.CrossFade ("Walk");
					break;
				}
			case MOVEMENT_STATE.JUMPINGHIGH:
				{
					if (previousMovState == MOVEMENT_STATE.MOVING || previousMovState == MOVEMENT_STATE.STOPPED) {
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
		this.previousMovState = newState;
	}

	private void logMoveState (MOVEMENT_STATE newState) {
		Debug.Log ("MOVEMENT_STATE : " + this.previousMovState + "-->" + newState);
	}

	private void logStopState (STOPPED_STATE newState) {
		Debug.Log ("STOPPED_STATE : " + this.futureStopState + "-->" + newState);
	}
}