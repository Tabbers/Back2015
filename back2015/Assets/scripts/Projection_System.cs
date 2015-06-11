using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;

public class Projection_System : MonoBehaviour
{

	public int iStepsize = 1;
	public int iLength = 10;
	public int worldSizexhalf = 50;
	public int worldSizezhalf = 50;
	public Vector2 vRaycast_Grid = new Vector2 (4, 4); 
	public GameObject[] goVisibleGO;
	private GameObject[,] goObjectsHit;
	public List<GameObject> CollisionObjects;
	private float fSpacing = 1;
	public LayerMask layers;

	public float iInterval = 0.5f;
	private float timer = 1f;
	private float iLastCalled = 0;
	Movment_System msScript;
	Stopwatch sw;
	// Use this for initialization
	void Start ()
	{
		goObjectsHit = new GameObject[5,4];
		goVisibleGO = new GameObject[goObjectsHit.Length];
		msScript = gameObject.GetComponent<Movment_System>();
		sw = new Stopwatch();	
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		timer -= Time.deltaTime;
		if (timer <= 0) {
			sw.Start();
				SendRaycastArray ();
				getGameObjects ();
				timer = iInterval;
			sw.Stop();
			long microseconds = sw.ElapsedTicks / (Stopwatch.Frequency / (1000L*1000L));
			Timestamp.SavetoFile("Raycast_Pathing.csv",microseconds);
			sw.Reset();
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
				Vector3 vRaycast = transform.forward*10 + transform.TransformVector (v3Offset);
				UnityEngine.Debug.DrawRay(transform.position,vRaycast,Color.red,0.3f);
				if (Physics.Raycast(transform.position, vRaycast, out rhHit,iLength,layers)) 
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
    void getNodes (GameObject gObject)
	{
		List<Vector3> Corners;
		Corners= new List<Vector3> ();
		Bounds b = gObject.collider.bounds;
		Vector3 RO = new Vector3(b.center.x + b.extents.x,gameObject.transform.position.y,b.center.z + b.extents.z);
		Vector3 RU = new Vector3(b.center.x + b.extents.x,gameObject.transform.position.y,b.center.z - b.extents.z);
		Vector3 LO = new Vector3(b.center.x - b.extents.x,gameObject.transform.position.y,b.center.z + b.extents.z);
		Vector3 LU = new Vector3(b.center.x - b.extents.x,gameObject.transform.position.y,b.center.z - b.extents.z);

		RO =(RO+new Vector3(fSpacing,0,fSpacing));
		LO =(LO+new Vector3(-fSpacing,0,fSpacing));

		RU =(RU+new Vector3(fSpacing,0,-fSpacing));
		LU =(LU+new Vector3(-fSpacing,0,-fSpacing));

		Corners.Add(RO);
		Corners.Add(LO);
		Corners.Add(LU);
		Corners.Add(RU);

		List<Vector3> CornerList = new List<Vector3>();
		Vector3 buffer = new Vector3(0,0,0);

		buffer = GetClosetsCorner(transform.position,Corners);
		Corners.Remove(buffer);
		CornerList.Add (buffer);

		buffer = GetClosetsCorner(CornerList[0],Corners);
		Corners.Remove(buffer);
		CornerList.Insert(0,buffer);

		Node NewNode = new Node(gObject, CornerList[1], CornerList[0]);

		CornerList = new List<Vector3>();
		buffer = GetClosetsCorner(transform.position,Corners);
		Corners.Remove(buffer);
		CornerList.Add (buffer);
		
		buffer = GetClosetsCorner(CornerList[0],Corners);
		Corners.Remove(buffer);
		CornerList.Insert(0,buffer);
		Node NewNode2;
		if(Vector3.Distance(CornerList[0],msScript.nTarget.Getpoint1()) < Vector3.Distance(CornerList[1],msScript.nTarget.Getpoint1()) )
		{
			NewNode2 = new Node(gObject, CornerList[1], CornerList[0]);
		}
		else
		{
			NewNode2 = new Node(gObject, CornerList[1], CornerList[0],RO);
		}

		if(checkNode(NewNode2) && checkNode(NewNode))
		{
			NewNode.Setreference(gObject);
			NewNode.Depth = msScript.currentNode.Depth;
			NewNode2.Setreference(gObject);
			NewNode2.Depth = msScript.currentNode.Depth;
			msScript.currentNode.Depth ++;
				
			if(msScript.currentNode.Getrefernce() == msScript.nTarget.Getrefernce())
			{
				if(Vector3.Distance(NewNode.Getpoint1(),msScript.nTarget.Getpoint1()) > Vector3.Distance(NewNode2.Getpoint1(),msScript.nTarget.Getpoint1()))
				{
					Node.attachNode(msScript.currentNode.Getparent(),NewNode);
				}
				else Node.attachNode(msScript.currentNode.Getparent(),NewNode2);
			}
			else
			{
				if(Vector3.Distance(NewNode.Getpoint1(),msScript.nTarget.Getpoint1()) > Vector3.Distance(NewNode2.Getpoint1(),msScript.nTarget.Getpoint1()))
				{
					Node.attachNode(msScript.currentNode.Getparent(),NewNode);
					Node tmpNode = msScript.currentNode;
					Node.removeNode(msScript.currentNode.Getparent(),msScript.currentNode);
					Node.attachNode(NewNode,tmpNode);
				}
				else
				{
					Node.attachNode(msScript.currentNode.Getparent(),NewNode2);
					Node tmpNode = msScript.currentNode;
					Node.removeNode(msScript.currentNode.Getparent(),msScript.currentNode);
					Node.attachNode(NewNode2,tmpNode);
				}
			}

		}
		else
		{
			if(checkNode(NewNode))
			{
				NewNode.Setreference(gObject);
				NewNode.Depth = msScript.currentNode.Depth;
				if(msScript.currentNode.Getrefernce() == msScript.nTarget.Getrefernce())
				{
					Node.attachNode(msScript.currentNode.Getparent(),NewNode);
				}
				else
				{
					Node.attachNode(msScript.currentNode.Getparent(),NewNode);
					Node tmpNode = msScript.currentNode;
					Node.removeNode(msScript.currentNode.Getparent(),msScript.currentNode);
					Node.attachNode(NewNode,tmpNode);
				}
			}
			if(checkNode(NewNode2))
			{
				NewNode2.Setreference(gObject);
				NewNode2.Depth = msScript.currentNode.Depth;
				if(msScript.currentNode.Getrefernce() == msScript.nTarget.Getrefernce())
				{
					Node.attachNode(msScript.currentNode.Getparent(),NewNode2);
				}
				else
				{
					Node.attachNode(msScript.currentNode.Getparent(),NewNode2);
					Node tmpNode = msScript.currentNode;
					Node.removeNode(msScript.currentNode.Getparent(),msScript.currentNode);
					Node.attachNode(NewNode2,tmpNode);
				}
			}

		}
		//msScript.root.Selfevaluate(CollisionObjects,gameObject.transform.position);
		msScript.RefreshNode();
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
		RaycastHit rhHit2 = new RaycastHit ();
		UnityEngine.Debug.DrawRay (transform.position, msScript.mTarget - transform.position, Color.red,0.3f);
		Physics.Raycast (transform.position, msScript.mTarget - transform.position, out rhHit2,	Vector3.Distance(msScript.mTarget,transform.position));
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
			if(rhHit2.transform !=null)
			{
				if(!known && Go == rhHit2.transform.gameObject && Go.name != transform.name)
				{
					CollisionObjects.Add(Go);
					getNodes(Go);
				}
			}
		}
	}
	private Vector3 GetClosetsCorner (Vector3 Origin, List<Vector3> Corners)
	{
		Vector3 buffer   = new Vector3(0,0,0);
		float distance = Mathf.Infinity;
		foreach(Vector3 CornerInner in Corners)
		{
			float currentdistance = Vector3.Distance(Origin,CornerInner);
			if (currentdistance < distance)
			{
				buffer = CornerInner;
				distance = currentdistance;
			} 
		}
		return buffer;
	}
	private bool checkNode(Node testnode)
	{
		List<Vector3> testpoints = new List<Vector3>();
		testpoints.Add(testnode.Getpoint1());
		testpoints.Add(testnode.Getpoint2());
		foreach(Vector3 point in testpoints)
		{
			if(point.x < worldSizexhalf && 
			   point.x > -worldSizexhalf &&
			   point.z < worldSizezhalf &&
			   point.z > -worldSizezhalf)
			{
				foreach(GameObject GO in CollisionObjects)
				{
					Bounds b = GO.collider.bounds;
					Vector3 RO = new Vector3(b.center.x + b.extents.x,gameObject.transform.position.y,b.center.z + b.extents.z);
					Vector3 LU = new Vector3(b.center.x - b.extents.x,gameObject.transform.position.y,b.center.z - b.extents.z);
					
					if(point.x < RO.x && 
					   point.x > LU.x &&
					   point.z < RO.z &&
					   point.z > LU.z)
					{
						return false;	
					}
					else{
						return true;
					}
				}
			}
			else
			{
				return false;
			}
		}
		return false;
	}


}
