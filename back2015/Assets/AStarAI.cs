using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
//Note this line, if it is left out, the script won't know that the class 'Path' exists and it will throw compiler errors
//This line should always be present at the top of scripts which use pathfinding
using Pathfinding;

public class AStarAI : MonoBehaviour {
	//The point to move to
	public Transform target;
	
	private Seeker seeker;
	
	//The calculated path
	public Path path;
	
	//The AI's speed per second
	public float speed = 10;
	
	//The max distance from the AI to a waypoint for it to continue to the next waypoint
	public float nextWaypointDistance = 3;
	
	//The waypoint we are currently moving towards
	private int currentWaypoint = 0;

	Stopwatch sw;
	Stopwatch sw2;
	public Timestamp ts;
	public Timestamp ts2;
	public Timestamp ts3;
	public float iInterval = 0.5f;
	private float timer = 0.5f;
	
	public void Start () {
		sw  = new Stopwatch();
		sw2 = new Stopwatch();
		ts  = new Timestamp();
		ts2 = new Timestamp();
		ts3 = new Timestamp();
		sw2.Start();

		seeker = GetComponent<Seeker>();

		sw.Start();
			seeker.StartPath (transform.position,target.position, OnPathComplete);
		sw.Stop();

		if(sw.ElapsedTicks >0)
		{
			long microseconds1 = sw.ElapsedTicks / (Stopwatch.Frequency / (1000L*1000L));
			ts.saveData(microseconds1);
			sw.Reset();
		}//Start a new path to the targetPosition, return the result to the OnPathComplete function

	}
	
	public void OnPathComplete (Path p) {
		if (!p.error) {
			path = p;
			//Reset the waypoint counter
			currentWaypoint = 0;
		}
	}
	
	public void Update () {


		sw.Start();
			if (path == null) {
				//We have no path to move after yet
				return;
			}
			
			if (currentWaypoint >= path.vectorPath.Count) {
				Destroy(gameObject);
				return;
			}
			
			//Direction to the next waypoint
			transform.LookAt(path.vectorPath[currentWaypoint]);
			transform.position += transform.forward*speed*Time.deltaTime;
			
			//Check if we are close enough to the next waypoint
			//If we are, proceed to follow the next waypoint
			if (Vector3.Distance (transform.position,path.vectorPath[currentWaypoint]) < nextWaypointDistance) {
				currentWaypoint++;
				return;
			}
		sw.Stop();
		if(sw.ElapsedTicks >0)
		{
			long microseconds2 = sw.ElapsedTicks / (Stopwatch.Frequency / (1000L*1000L));
			ts2.saveData(microseconds2);
			sw.Reset();
		}
	}
	void OnDestroy() {
		sw2.Stop();
		
		ts.EmptyFile("AStar_pathing.csv");
		ts.SavetoFile("AStar_pathing.csv");
		
		ts2.EmptyFile("AStar_moving.csv");
		ts2.SavetoFile("AStar_moving.csv");
		
		ts3.EmptyFile("AStar_all.csv");
		long milliseconds = sw2.ElapsedTicks / (Stopwatch.Frequency / (1000L));
		ts3.saveData(milliseconds);
		ts3.SavetoFile("AStar_all.csv");

		GameObject.FindGameObjectWithTag("Start").GetComponent<AI_Spawn>().iSpawnd--;
		
	}
} 