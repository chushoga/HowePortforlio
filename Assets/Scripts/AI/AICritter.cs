﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// requires a sphere collider for the collision detection.
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class AICritter : MonoBehaviour {

	// test object to spawn and see the location of newDir
	public GameObject spawnTo;

	[SerializeField] float wanderRange = 10f;
	[SerializeField] float wanderSpeed = 2.5f;
	[SerializeField] float turnSpeed = 50f;

	// collision bounds(make larger than the character)
	// this is a required component
	SphereCollider collisionBarrier; 

	// required component.
	Rigidbody rb;

	// target position
	Vector3 targetPos;

	bool isMoving = true;

	// Use this for initialization
	void Start () {
		// get Rigidbody
		rb = GetComponent<Rigidbody>();

		//lock rotaiton
		rb.constraints = RigidbodyConstraints.FreezeRotation;


		collisionBarrier = GetComponent<SphereCollider>(); // grab the sphere collider

		targetPos = RandomDirection();

		LookTowards(); // look at new direction
	}
	
	// Update is called once per frame
	void Update () {

		// do a random check if should choose new direction or stand still for a random abount of time before moving.
		if (isMoving == true) {
			
			// update movement
			transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * wanderSpeed);

		} else {
			// stand still
		}


		if (Mathf.Round(transform.position.x) == Mathf.Round(targetPos.x) && Mathf.Round(transform.position.z) == Mathf.Round(targetPos.z)){
			Debug.Log("reached pos");
			targetPos = RandomDirection();

		}
		LookTowards(); // look at new direction

		// TODO: have the gameobject move forward towards they way they are facing.
		if(Input.GetKey(KeyCode.W)) {
			transform.position += transform.forward * Time.deltaTime * wanderSpeed;
		}
		else if(Input.GetKey(KeyCode.S)) {
			rb.position -= transform.forward * Time.deltaTime * wanderSpeed;
		}

		if(Input.GetKey(KeyCode.D)) {
			transform.Rotate(0, Time.deltaTime * turnSpeed, 0);
		}
		else if(Input.GetKey(KeyCode.A)) {
			transform.Rotate(0, Time.deltaTime * -turnSpeed, 0);
		}
	}

	// pick a random direction and go
	Vector3 RandomDirection(){
		
		Vector3 position = new Vector3(Random.Range(-wanderRange, wanderRange), 0, Random.Range(-wanderRange, wanderRange));

		//Instantiate(spawnTo, position, Quaternion.identity);
		ChooseMoveType(); // choose a movement type. Standing or moving.

		return position;

	}

	void LookTowards (){
		Vector3 targetDir = new Vector3(targetPos.x - transform.position.x, 0, targetPos.z - transform.position.z);
		float step = turnSpeed * Time.deltaTime;
		Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, 5.0f * Time.deltaTime, 20.0F);
		//Debug.DrawRay(transform.position, newDir, Color.red);
		transform.rotation = Quaternion.LookRotation(newDir);

		Debug.DrawRay(transform.position, newDir, Color.red);

	}

	// If Object collides with the proximity trigger then 
	// choose a new direction and face it.
	void OnTriggerEnter(Collider collision)
	{
		
		if(collision.gameObject.tag != "Ground"){
			Debug.Log("reached pos");
			targetPos = RandomDirection();
			LookTowards(); // look at new direction
			//Debug.Log(collision.gameObject.tag);
		}

	}

	// If object touches directly with body collier then
	// choose a new direction and face it.
	void OnCollisionEnter(Collision collision)
	{

		if(collision.gameObject.tag != "Ground"){
			Debug.Log("reached pos");
			targetPos = RandomDirection();
			LookTowards(); // look at new direction

		}

	}

	// choose if moving or stopped.
	// 0 = stopped
	// 1 = moving
	void ChooseMoveType(){
		float rnd = Random.Range(0.0f,100.0f);

		if(rnd >= 50){
			isMoving = true;
		} else {
			isMoving = false;
			StartCoroutine(MovementPause());
		}

		Debug.Log("move type:" + rnd);
	}

	IEnumerator MovementPause(){
		float time = Random.Range(0.0f,10.0f);

		yield return new WaitForSeconds(time);

		// chose new random directio to face
		targetPos = RandomDirection();

		// start moving
		isMoving = true;
	}
}
