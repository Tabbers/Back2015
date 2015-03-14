using UnityEngine;
using System.Collections;

public class Projection_System : MonoBehaviour {

	public int iStepsize = 1;
	public Vector2 vRaycast_Grid = new Vector2(4,4); 
	public GameObject[] goVisibleGO;
	private GameObject[,] goObjectsHit;

	public int iInterval=1;	
	private float iLastCalled=0;
	// Use this for initialization
	void Start () {
		goObjectsHit =  new GameObject[5,4];
	}
	
	// Update is called once per frame
	void Update () 
{
		if(Time.time < iLastCalled + iInterval)
		{
			SendRaycastArray();
			iLastCalled = Time.time;
		}
	}
	void SendRaycastArray()
	{

		for(int y =0; y < vRaycast_Grid.y*iStepsize; y+=iStepsize )
		{
			for(int x = -2; x < vRaycast_Grid.x*iStepsize-1*iStepsize; x+=iStepsize)
			{
				RaycastHit rhHit = new RaycastHit();
				Vector3 v3RelDir = new Vector3(10,y,x) ;
				Vector3 vRaycast = transform.localRotation * v3RelDir;
				Debug.DrawLine(transform.position,vRaycast, Color.red);
				if(Physics.Raycast(transform.position,vRaycast,out rhHit))
				{
					goObjectsHit[x,y] = rhHit.transform.gameObject;
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
				GOs[x * Y + y] = GO[x,y];
			}
		}
		return GOs;
		
	}
}
