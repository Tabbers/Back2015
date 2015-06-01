using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Movment_System : MonoBehaviour {

	
	public List<Vector3> Waypoints;
	public int speed = 1;
	public int Waypoint_pointer=0;
	public float accuracy=0.5f;
	private bool arrived=false;
	// Use this for initialization
	void Start () {
		Waypoints = new List<Vector3>();
		addWaypoint(GameObject.FindGameObjectWithTag("Dest").transform.position);
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if(!arrived && Waypoints.Count != Waypoint_pointer)
		{
			if(!arrivedAtWaypoint())
			{
				 MoveToNextPoint();
			}
			else{
				if(Waypoints.Count == Waypoint_pointer) arrived = true;
				else Waypoint_pointer ++;
			}
		}
		else 
		{
			Debug.Log(gameObject.name+" arrived");
			arrived = true;
		}
	}
  	public void addWaypoint (Vector3 SingleWaypoint)
	{
		
		Waypoints.Add(SingleWaypoint);
	}
	public void insertWaypoint (Vector3 SingleWaypoint)
	{
		Waypoints.Insert(Waypoint_pointer,SingleWaypoint);
	}
	public void MoveToNextPoint()
	{
		Vector3 Targetpoint = Waypoints[Waypoint_pointer];
		transform.LookAt(Targetpoint);
		transform.position += transform.forward*speed*Time.deltaTime;
	}
	public Vector3 GetGoal()
	{
		return Waypoints[Waypoints.Count-1];
	}
	public bool arrivedAtWaypoint()
	{

		if(transform.position.x < Waypoints[Waypoint_pointer].x+accuracy && 
		   transform.position.x > Waypoints[Waypoint_pointer].x-accuracy &&
		   transform.position.z < Waypoints[Waypoint_pointer].z+accuracy &&
		   transform.position.z > Waypoints[Waypoint_pointer].z-accuracy) 
		return true;
		else 
		return false;
	}
}