using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum State { STOPPED, MOVING, JUMPINGHIGH, JUMPINGLOW }
public enum StopState { STANDING, LYINGDOWN, EATING }

public class AnimalControllerScript : MonoBehaviour {
	Animation anim;
	NavMeshAgent agent;
	Vector3 center_point;
	Vector3 bed1_point;
	Vector3 bed2_point;

	State state;
	StopState stopState;

	void Start () {
		//rb = GetComponent<Rigidbody> ();
		center_point = GameObject.Find ("center_point").transform.position;
		bed1_point = GameObject.Find ("bed1_point_dest").transform.position;
		bed2_point = GameObject.Find ("bed2_point_dest").transform.position;
		anim = GetComponent<Animation> ();
		agent = GetComponent<NavMeshAgent> ();
		agent.autoTraverseOffMeshLink = false;
		agent.destination = agent.transform.position;
		anim["Walk"].wrapMode = WrapMode.Loop;
		handleNewState (State.STOPPED);
		goToCenter ();
		stopState = StopState.EATING;
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
	}

	private void logState (State newState) {
		Debug.Log ("State : " + this.state + " -->" + newState);
	}

	private void handleNewState (State newState) {
		if (state == newState) {
			return;
		}
		logState (newState);
		switch (newState) {
			case State.STOPPED:
				{
					switch (stopState) {
						case StopState.STANDING:
							{
								this.anim.CrossFade ("Idled");
								break;
							}
						case StopState.LYINGDOWN:
							{
								this.anim.CrossFade ("Lie Down");
								this.anim.CrossFadeQueued ("Rest");
								break;
							}
						case StopState.EATING:
							{
								this.anim.CrossFade ("start & End Eating");
								break;
							}
						default:
							{ break; }
					}
					break;
				}
			case State.MOVING:
				{
					this.anim.CrossFade ("Walk");
					break;

				}
			case State.JUMPINGHIGH:
				{
					if (state == State.MOVING || state == State.STOPPED) {
						this.anim.CrossFade ("Jump High");
					}
					break;

				}
			case State.JUMPINGLOW:
				{
					break;
				}
			default:
				{ break; }
		}
		this.state = newState;
	}

	void FixedUpdate () {
		if (Vector3.Distance (agent.transform.position, agent.destination) <= 0.5f) {
			handleNewState (State.STOPPED);
		} else {
			if (agent.isOnOffMeshLink) {
				if (!isUp ()) {
					if (state != State.JUMPINGLOW) {
						handleNewState (State.JUMPINGHIGH);
						stopState = StopState.STANDING;
					}
				} else {
					if (state != State.JUMPINGHIGH)
						handleNewState (State.JUMPINGLOW);
					stopState = StopState.LYINGDOWN;
				}

				OffMeshLinkData data = agent.currentOffMeshLinkData;
				Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
				agent.transform.position = Vector3.MoveTowards (agent.transform.position, endPos, agent.speed * Time.deltaTime);
				// Agent reached jump/drop target point
				if (agent.transform.position == endPos) {
					agent.CompleteOffMeshLink ();
				}
			} else {
				handleNewState (State.MOVING);
			}
		}
	}

	// Movements
	public void moveTo (Vector3 destination) {
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
		stopState = StopState.STANDING;
		agent.destination = agent.transform.position;
	}

	private bool isUp () {
		return agent.transform.position.y > 1.00f;
	}

}