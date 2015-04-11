using UnityEngine;
using System.Collections;

public class Projection_System : MonoBehaviour {

	public int iStepsize = 1;
	public int iLength = 10;
	public Vector2 vRaycast_Grid = new Vector2(4,4); 
	public GameObject[] goVisibleGO;
	private GameObject[,] goObjectsHit;

	public int iInterval=1;	
	private float iLastCalled=0;
	// Use this for initialization
	void Start () {
		goObjectsHit =  new GameObject[5,4];
		goVisibleGO = new GameObject[goObjectsHit.Length];
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if(Time.time < iLastCalled + iInterval)
		{
			//Debug.Log("Raycasting Started"+Time.time);
			SendRaycastArray();
			iLastCalled = Time.time;
		}
	}
	void SendRaycastArray()
	{
		for(int y=0; y < vRaycast_Grid.y*iStepsize; y+=iStepsize)
		{
			for(int x = -2; x < vRaycast_Grid.x*iStepsize-1*iStepsize; x+=iStepsize)
			{
				RaycastHit rhHit = new RaycastHit();
				Vector3 v3Offset = new Vector3(x,y,0);
				Vector3 vRaycast = transform.position+(transform.forward*iLength)+transform.TransformVector(v3Offset);
				Debug.DrawLine(transform.position,vRaycast, Color.red);
				if(Physics.Raycast(transform.position,vRaycast,out rhHit))
				{
					goObjectsHit[x+2,y] = rhHit.transform.gameObject;
				}
				else
				{
					goObjectsHit[x+2,y] = null;
				}

			}
		}
		goVisibleGO = convertArray(goObjectsHit, 5, 4);
	}
	GameObject[] convertArray(GameObject[,] GO, int X, int Y)
	{
		GameObject[] GOs = new GameObject[X*Y];

		for(int y =0; y < Y; y++ )
		{
			for(int x = 0; x < X; x++)
			{
				if(GO[x,y] != null)
				{
					GOs[x * Y + y] = GO[x,y];
				}
				else
				{
					GOs[x * Y + y] = null;
				}
			}
		}
		return GOs;
		
	}



}
