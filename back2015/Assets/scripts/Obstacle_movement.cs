using UnityEngine;
using System.Collections;

public class Obstacle_movement : MonoBehaviour {

	public Vector3 v3Start;
	private Vector3 v3End;
	public Vector3 v3Shift;
	public float fCount = 0.0f;
	public float fSpeed = 1.0f;
	private bool bDirection;
	private int  iDirection = 1;
	// Use this for initialization
	void Start () 
	{
		v3Start = gameObject.transform.position;
		v3End = gameObject.transform.position + v3Shift;
	}
	
	// Update is called once per frame
	void Update () 
	{
		move();
	}
	void move ()
	{
		if(fCount >= 1 && bDirection) 
		{
			bDirection= false;
			iDirection = -1;
		}
		if(fCount <= 0 && !bDirection)
		{
			bDirection= true;
			iDirection = 1;
		}
		fCount += Time.deltaTime * fSpeed * iDirection;
		gameObject.transform.position = Vector3.Lerp(v3Start, v3End, fCount);
	}
}
