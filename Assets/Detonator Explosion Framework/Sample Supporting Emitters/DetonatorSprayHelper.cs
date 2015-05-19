using UnityEngine;
using System.Collections;

public class DetonatorSprayHelper : MonoBehaviour {
public float startTimeMin = 0;
public float startTimeMax = 0;
public float stopTimeMin = 10;
public float stopTimeMax = 10;

public Material firstMaterial;
public Material secondMaterial;

private float startTime;
private float stopTime;

//the time at which this came into existence
private bool  isReallyOn;

void Start (){
	isReallyOn = GetComponent<ParticleEmitter>().emit;
	
	//this kind of emitter should always start off
	GetComponent<ParticleEmitter>().emit = false;
	
	//get a random number between startTimeMin and Max
	startTime = (Random.value * (startTimeMax - startTimeMin)) + startTimeMin + Time.time;
	stopTime = (Random.value * (stopTimeMax - stopTimeMin)) + stopTimeMin + Time.time;
	
	//assign a random material
	GetComponent<Renderer>().material = Random.value > 0.5f ? firstMaterial : secondMaterial;
}

void FixedUpdate (){
	//is the start time passed? turn emit on
	if (Time.time > startTime)
	{
		GetComponent<ParticleEmitter>().emit = isReallyOn;
	}
	
	if (Time.time > stopTime)
	{
		GetComponent<ParticleEmitter>().emit = false;
	}
}
}
