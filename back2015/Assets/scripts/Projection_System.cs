using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Projection_System : MonoBehaviour
{

	public int iStepsize = 1;
	public int iLength = 10;
	public Vector2 vRaycast_Grid = new Vector2 (4, 4); 
	public GameObject[] goVisibleGO;
	private GameObject[,] goObjectsHit;
	public List<GameObject> CollisionObjects;
	private float fSpacing;
	public float fGutter;

	public int iInterval = 1;	
	private float iLastCalled = 0;
	Movment_System msScript;
	// Use this for initialization
	void Start ()
	{
		goObjectsHit = new GameObject[5, 4];
		goVisibleGO = new GameObject[goObjectsHit.Length];
		msScript = gameObject.GetComponentInParent<Movment_System>();
		fSpacing = gameObject.GetComponentInParent<BoxCollider>().bounds.extents.x;

	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{

		if (Time.time < iLastCalled + iInterval) {
			//Debug.Log("Raycasting Started"+Time.time);
			SendRaycastArray ();
			getGameObjects ();
			iLastCalled = Time.time;
		}
	}
	void SendRaycastArray ()
	{
		for (int y=0; y < vRaycast_Grid.y*iStepsize; y+=iStepsize)
		{
			for (int x = -2; x < vRaycast_Grid.x*iStepsize-1*iStepsize; x+=iStepsize) 
			{
				RaycastHit rhHit = new RaycastHit ();
				Vector3 v3Offset = new Vector3 (x, y, 0);
				Vector3 vRaycast = transform.position + (transform.forward * iLength) + transform.TransformVector (v3Offset);
				Debug.DrawLine (transform.position, vRaycast, Color.red);
				if (Physics.Raycast (transform.position, vRaycast, out rhHit)) 
				{
					goObjectsHit [x + 2, y] = rhHit.transform.gameObject;
				} else 
				{
					goObjectsHit [x + 2, y] = null;
				}

			}
		}
		goVisibleGO = convertArray (goObjectsHit, 5, 4);
	}
	GameObject[] convertArray (GameObject[,] GO, int X, int Y)
	{
		GameObject[] GOs = new GameObject[X * Y];

		for (int y =0; y < Y; y++) {
			for (int x = 0; x < X; x++) 
			{
				if (GO [x, y] != null) 
				{
					GOs [x * Y + y] = GO [x, y];
				}
				else 
				{
					GOs [x * Y + y] = null;
				}
			}
		}
		return GOs;

	}
    List<Vector3> getCorner (GameObject gObject)
	{
		List<Vector3> Corners;
		Corners= new List<Vector3> ();
		Bounds b = gObject.collider.bounds;
		Vector3 RO = new Vector3(b.center.x + b.extents.x,b.center.y,b.center.z + b.extents.z);
		Vector3 RU = new Vector3(b.center.x + b.extents.x,b.center.y,b.center.z - b.extents.z);
		Vector3 LO = new Vector3(b.center.x - b.extents.x,b.center.y,b.center.z + b.extents.z);
		Vector3 LU = new Vector3(b.center.x - b.extents.x,b.center.y,b.center.z - b.extents.z);

		Debug.Log(RO);
		Debug.Log(RU);
		Debug.Log(LO);
		Debug.Log(LU);

		RO =(RO+new Vector3(fSpacing,0,fSpacing));
		RU =(RU+new Vector3(fSpacing,0,-(fSpacing)));
		LO =(LO+new Vector3(-(fSpacing),0,fSpacing));
		LU =(LU+new Vector3(-(fSpacing),0,-(fSpacing)));

		Corners.Add(RO);
		Corners.Add(LO);
		Corners.Add(LU);
		Corners.Add(RU);

		List<Vector3> CornerList = new List<Vector3>();
		Vector3 buffer= new Vector3(0,0,0);
		float distance = Mathf.Infinity;
		foreach(Vector3 CornerInner in Corners)
		{
			float currentdistance = Vector3.Distance(transform.position,CornerInner);
			if (currentdistance < distance)
			{
				buffer = CornerInner;
				distance = currentdistance;
			}
		}
		Corners.Remove(buffer);
		CornerList.Add (buffer);
		buffer= new Vector3(0,0,0);
		distance = Mathf.Infinity;
		foreach(Vector3 CornerInner in Corners)
		{
			float currentdistance = Vector3.Distance(CornerList[0],CornerInner);
			if (currentdistance < distance)
			{
				buffer = CornerInner;
				distance = currentdistance;
			} 
		}
		CornerList.Add (buffer);
		return CornerList;
	}
	void getGameObjects ()
	{ 
		bool known = false;
		List<GameObject> Buffer;
		Buffer = new List<GameObject> ();
		foreach (GameObject GO in goVisibleGO) 
		{
			if (GO != null) 
			{
				if (CollisionObjects.Count > 0) 
				{
					foreach (GameObject GO2 in CollisionObjects) 
					{
						if (GO.GetInstanceID () == GO2.GetInstanceID ()) 
						{
							known = true;
							break;
						}
					}
					if (!known) 
					{
						Buffer.Add (GO);
					}	
				} 
				else 
				{
					Buffer.Add (GO);
				}
			}
		}
		bool kown = false;
		foreach (GameObject Go in Buffer) 
		{
			foreach(GameObject Go2 in CollisionObjects)
			{
				if(Go.GetInstanceID() == Go2.GetInstanceID())
				{
					known = true;
					break;
				}
			}
			if(!known)
			{
				List<Vector3> buffer = getCorner(Go);
				foreach (Vector3 vect in buffer)
				{
					msScript.insertWaypoint(vect);
				}
				CollisionObjects.Add(Go);
			}
		}
	}



}
