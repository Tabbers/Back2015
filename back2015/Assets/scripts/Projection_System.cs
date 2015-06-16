using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;

public class Projection_System : MonoBehaviour
{
	public int iFramecount=0;
	public int iStepsize = 1;
	public int iLength = 10;
	public int worldSizexhalf = 50;
	public int worldSizezhalf = 50;
	[HideInInspector]
	public Vector2 vRaycast_Grid = new Vector2 (4,4); 
	[HideInInspector]
	public GameObject[] goVisibleGO;
	private GameObject[,] goObjectsHit;
	[HideInInspector]
	public List<GameObject> CollisionObjects;
	[HideInInspector]
	public List<GameObject> Dynamic;
	[HideInInspector]
	public List<GameObject> Eval;
	[HideInInspector]
	public List<Vector3> 	LastPosition;
	private float fSpacing = 1;
	public LayerMask layers;

	public float iInterval = 0.5f;
	private float timer = 1f;
	private float iLastCalled = 0;
	Movment_System msScript;
	Stopwatch sw;
	public Timestamp ts;
	// Use this for initialization
	void Start ()
	{
		sw = new Stopwatch();
		ts = new Timestamp();

		CollisionObjects = new List<GameObject>();
		Dynamic = new List<GameObject>();
		Eval = new List<GameObject>();
		LastPosition = new List<Vector3>();
		goObjectsHit = new GameObject[5,4];
		goVisibleGO = new GameObject[goObjectsHit.Length];
		msScript = gameObject.GetComponent<Movment_System>();

	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if(iFramecount > 5)
		{
			sw.Start();
				getDynamicGameObjects();
				recalculateNodePositions();
				iFramecount = 0;
			sw.Stop();
		}
		timer -= Time.deltaTime;
		if (timer <= 0) {	
			sw.Start();
				SendRaycastArray ();
				getGameObjects ();
				timer = iInterval;
			sw.Stop();
		}
		if(sw.ElapsedTicks >0)
		{
			long microseconds = sw.ElapsedTicks / (Stopwatch.Frequency / (1000L*1000L));
			ts.saveData(microseconds);
			sw.Reset();
		}
		iFramecount++;
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
		Node NewNode2 = new Node(gObject, CornerList[1], CornerList[0]);
		/*if(Vector3.Distance(CornerList[0],msScript.nTarget.Getpoint1()) < Vector3.Distance(CornerList[1],msScript.nTarget.Getpoint1()) )
		{
			NewNode2 = new Node(gObject, CornerList[1], CornerList[0]);
		}
		else NewNode2 = new Node(gObject, CornerList[1], CornerList[0],LO);*/

		if(checkNode(NewNode2) && checkNode(NewNode))
		{
			NewNode.Setreference(gObject);
			NewNode.Depth = msScript.currentNode.Depth;
			NewNode2.Setreference(gObject);
			NewNode2.Depth = msScript.currentNode.Depth;
			msScript.currentNode.Depth ++;
				
			if(msScript.currentNode.Getrefernce() == msScript.nTarget.Getrefernce())
			{
				/*if(Vector3.Distance(NewNode.Getpoint1(),msScript.nTarget.Getpoint1()) > Vector3.Distance(NewNode2.Getpoint1(),msScript.nTarget.Getpoint1()))
				{
					Node.attachNode(msScript.currentNode.Getparent(),NewNode);
				}
				else Node.attachNode(msScript.currentNode.Getparent(),NewNode2);*/
				Node.attachNode(msScript.currentNode.Getparent(),NewNode);
				Node.attachNode(msScript.currentNode.Getparent(),NewNode2);
			}
			else
			{
				Node.attachNode(msScript.currentNode.Getparent(),NewNode);
				Node.attachNode(msScript.currentNode.Getparent(),NewNode2);
				Node.removeNode(msScript.currentNode.Getparent(),msScript.currentNode);
				if(Vector3.Distance(NewNode.Getpoint1(),msScript.nTarget.Getpoint1()) > Vector3.Distance(NewNode2.Getpoint1(),msScript.nTarget.Getpoint1()))
				{
					Node.attachNode(NewNode,msScript.currentNode);
				}
				else Node.attachNode(NewNode2,msScript.currentNode);
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
					Node.removeNode(msScript.currentNode.Getparent(),msScript.currentNode);
					Node.attachNode(NewNode,msScript.currentNode);
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
					Node.removeNode(msScript.currentNode.Getparent(),msScript.currentNode);
					Node.attachNode(NewNode2,msScript.currentNode);
				}
			}

		}
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
					Eval.Add(Go);
					LastPosition.Add(Go.transform.position);
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
	private void getDynamicGameObjects()
	{
		List<GameObject> toRemove = new List<GameObject>();
		int i=0;
		if(Eval.Count > 0)
		{
			foreach(GameObject GOE in Eval)
			{
				if(GOE.transform.position != LastPosition[i]){
					Dynamic.Add(GOE);
				}
				else{
					LastPosition.RemoveAt(i);
				}
				toRemove.Add(GOE);
				i++;
			}
		}
		i=0;
		foreach (GameObject GOE in toRemove)
		{
			Eval.Remove(GOE);
			i++;
		}
	}
	private void recalculateNodePositions()
	{
		List<Node> NodesToAdapt;
		int i=0;
		bool recalculate=false;
		foreach(GameObject GOE in Dynamic)
		{
			List<Node> children = msScript.root.Getchildren();
			NodesToAdapt = msScript.SearchTreeForNode(children,GOE);
			foreach (Node node in NodesToAdapt)
			{
				int j=0;
				foreach(Vector3 point in node.GetPoints())
				{
					Vector3 changes = GOE.transform.position - LastPosition[i];
					Vector3 buffer  = point + changes;
					node.Setpoint(buffer,j);
					j++;
				}
			}
			LastPosition[i] = GOE.transform.position;
			i++;
			if(GOE == msScript.currentNode.reference) recalculate = true;
		}
		if(recalculate)
		{
			if(msScript.pointnumber < msScript.currentNode.GetPoints().Count-1)
			{
				msScript.mTarget = msScript.currentNode.GetPoint(msScript.pointnumber);
				msScript.timeout = (msScript.currentNode.GetDistance(msScript.pointnumber,transform.position)/msScript.speed)+0.5f;
				msScript.timer = msScript.timeout;
			}
		}
	}
	void OnDestroy() {
		int i = gameObject.GetComponent<FPS>().iNumber;
		ts.EmptyFile("Raycast_pathing"+i.ToString()+".csv");
		ts.SavetoFile("Raycast_pathing"+i.ToString()+".csv");
	}

}
