using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;

public class Movment_System : MonoBehaviour {
	
	public 	int 		speed       = 1;
	public 	float 		accuracy    =0.5f;
	[HideInInspector]
	public  int 		pointnumber = 0;
	[HideInInspector]
	public  int			cornercount = 0; 
	private bool 		arrived=false;
	[HideInInspector]
	public  bool 		backtracing = false;
	[HideInInspector]
	public  Vector3 	mTarget;
	[HideInInspector]
	public  float timeout  = 0;
	[HideInInspector]
	public  float timer    = 0;
	public  float Interval = 0.5f;
	private float timer2   = 0;
	public 	GameObject 	target;
	[HideInInspector]
	public 	Node 		root;
	[HideInInspector]
	public 	Node 		nTarget;
	public 	Node 		currentNode;
	[HideInInspector]
	public  List<Node>  Pathtaken;
	Projection_System   psScript;
	// Use this for initialization
	Stopwatch sw;
	Stopwatch sw2;
	public Timestamp ts;
	public Timestamp ts2;
	void Start () {
		sw  = new Stopwatch();
		sw2 = new Stopwatch();
		sw2.Start();
		ts  = new Timestamp();
		ts2  = new Timestamp();

		psScript = GetComponent<Projection_System>();
		Pathtaken = new List<Node>();
		root = new Node(GameObject.FindGameObjectWithTag("Start"),transform.position);
		root.isroot = true;
		root.Depth  = 0;
		try
		{
			target = GameObject.FindGameObjectWithTag("Dest");
			nTarget = new Node(target,target.transform.position);
			nTarget.istarget = true;
			nTarget.Depth = 1;
			root.Addchildren(nTarget);
			nTarget.Setparent(root);
		}
		catch(System.Exception e)
		{
			UnityEngine.Debug.Log("No Target selected or invalid target Type");
		}
		currentNode = root;
		NextNode();
	}
	// Update is called once per frame
	void FixedUpdate () 
	{
		sw.Start();
			MoveToNode();
		sw.Stop();
		long microseconds = sw.ElapsedTicks / (Stopwatch.Frequency / (1000L*1000L));
		ts.saveData(microseconds);
		sw.Reset();
	}
	public void NextNode ()
	{
		List<Node> children;
		Node subtarget;
		if(Node.hasChildren(currentNode))
		{
			children = currentNode.Getchildren();
			subtarget = SearchTree(children);
			if(subtarget != nTarget)
			{
				if(nTarget.Getparent() != null) Node.removeNode(nTarget.Getparent(),nTarget);
				Node.attachNode(subtarget,nTarget);	
			}
			currentNode = subtarget;
			mTarget = currentNode.Getpoint1();
			timeout = (currentNode.GetDistance(0,transform.position)/speed)+0.5f;
			timer = timeout;
			pointnumber = 0;
		}
		else
		{
			children = root.Getchildren();
			subtarget = SearchTree(children);
			if(subtarget != nTarget)
			{
				if(nTarget.Getparent() != null) Node.removeNode(nTarget.Getparent(),nTarget);
				Node.attachNode(subtarget,nTarget);
			}
			currentNode = subtarget;
			mTarget = currentNode.Getpoint1();
			timeout = (currentNode.GetDistance(0,transform.position)/speed)+0.5f;
			timer = timeout;
			pointnumber = 0;
		}
	}
	public void RefreshNode()
	{
		cornercount ++;
		List<Vector3> Corners = new List<Vector3>();
		for(int i = 0; i< currentNode.GetPoints().Count; i++)
		{
			Corners.Add(currentNode.GetPoint(i));
		}
		if(nTarget.Getparent() != null) Node.removeNode(nTarget.Getparent(),nTarget);
		if(currentNode != nTarget) Node.Selfevaluate(psScript.CollisionObjects, transform.position,currentNode);
		Node subtarget = SearchTree(root.Getchildren());
		
		RaycastHit rhHit = new RaycastHit ();
		if(Physics.Raycast (transform.position, subtarget.Getpoint1() - transform.position, out rhHit,	Vector3.Distance(subtarget.Getpoint1(),transform.position)))
		{
			bool reset = true;
			Node sibling = new Node();
			foreach(GameObject GO in psScript.CollisionObjects)
			{
				if(GO == rhHit.transform.gameObject) 
				{
					sibling  = Node.getSibling(subtarget);
					Node.removeNode(subtarget.Getparent(),subtarget);
					subtarget = sibling;
					reset = false;
				}
			}
			if(!reset) sibling.AddPoints(Corners);
			else cornercount = 0;
		}
		
		if(currentNode != nTarget && currentNode != null)
		{
			Node subtarget2 = SearchTree(currentNode.Getchildren());
			if(subtarget2 == root) Node.attachNode(subtarget,nTarget);
			else Node.attachNode(subtarget2,nTarget);
		}
		else{
			Node.attachNode(subtarget,nTarget);
		}
		currentNode = subtarget;
		if(currentNode.reference != nTarget.reference) Pathtaken.Add(currentNode);
		mTarget = currentNode.Getpoint1();
		pointnumber = 0;
		timeout = (currentNode.GetDistance(pointnumber,transform.position)/speed)+1f;
		timer = timeout;
	}
	public void MoveToNode()
	{
		if(!backtracing)
		{
			if(pointnumber < currentNode.GetPoints().Count)
			{
				if(arrivedAtWaypoint()){
					pointnumber++;
					if(pointnumber < currentNode.GetPoints().Count){ 
						mTarget = currentNode.GetPoint(pointnumber);
						timeout = (currentNode.GetDistance(pointnumber,transform.position)/speed)+1f;
						timer = timeout;
					}
				}
				else{
					MoveToPoint();
				}
			}
			else
			{
				if(currentNode == nTarget) GameObject.Destroy(gameObject);
				if(currentNode.reference != nTarget.reference )currentNode.explored =true;
				NextNode();
			}
			timer2 -= Time.deltaTime;
			if(timer2<=0) 
			{
				sw.Start();
				timer -= Time.deltaTime;
				if(timer <= 0 && currentNode.reference != nTarget.reference )
				{
					Node sibling = Node.getSibling(currentNode);
					if( sibling != new Node() && !sibling.explored)
					{
						UnityEngine.Debug.Log("Target unreachable in Time!");
						currentNode.explored = true;
						currentNode = currentNode.Getparent();
						mTarget = currentNode.GetPoint(currentNode.GetPoints().Count-1);
						pointnumber = currentNode.GetPoints().Count-1;
						timeout = (currentNode.GetDistance(pointnumber,transform.position)/speed)+1f;
						timer = timeout;
						backtracing = true;
					}
					else
					{
						currentNode.explored = true;
						foreach(Node child in currentNode.Getchildren())
						{
							Node.removeNode(currentNode,child);
							Node.attachNode(sibling,child);
						}
						NextNode();
					}
				}
			}
		}
		else{
			if(pointnumber > 0)
			{
				if(arrivedAtWaypoint()){
					pointnumber--;
					mTarget = currentNode.GetPoint(pointnumber);
					timeout = (currentNode.GetDistance(pointnumber,transform.position)/speed)+1f;
					timer = timeout;
				}
				else{
					MoveToPoint();
				}
			}
			else
			{
				if(currentNode != root) currentNode = currentNode.Getparent();
				pointnumber = currentNode.GetPoints().Count-1;
				mTarget     = currentNode.GetPoint(currentNode.GetPoints().Count-1);
				timeout = (currentNode.GetDistance(pointnumber,transform.position)/speed)+1f;
				timer = timeout;
			}
		}
	}
	public void MoveToPoint()
	{
		transform.LookAt(mTarget);
		transform.position += transform.forward*speed*Time.deltaTime;
	}
	public bool arrivedAtWaypoint()
	{
		
		if(transform.position.x < mTarget.x+accuracy && 
		   transform.position.x > mTarget.x-accuracy &&
		   transform.position.z < mTarget.z+accuracy &&
		   transform.position.z > mTarget.z-accuracy) 
			return true;
		else 
			return false;
	}
	private Node SearchTree(List<Node> nodes)
	{
		float fdistance = Mathf.Infinity;
		Node subtarget = root;
		foreach(Node Child in nodes)
		{
			if (Node.hasChildren(Child))
			{	
				if(Child.explored)subtarget = SearchTree(Child.Getchildren());
				else
				{
					if(Child.GetDistance(0,transform.position) < subtarget.GetDistance(0,transform.position))
					{
						subtarget = Child;
					}
				}
			}
			else{
				if(!Child.explored && Child.GetDistance(0,transform.position) < fdistance && subtarget == root )
				{
					fdistance = Child.GetDistance(0,transform.position);
					subtarget = Child;
				}
			}
		}
		return subtarget;
	}
	public List<Node>SearchTreeForNode(List<Node> nodes,GameObject reference)
	{
		List<Node> subtargets = new List<Node>();
		foreach(Node Child in nodes)
		{
			if (Node.hasChildren(Child))
			{	
				foreach(Node node in SearchTreeForNode(Child.Getchildren(),reference))
				{
					subtargets.Add(node);
				}
			}
			if(Child.reference.name == reference.name)
			{
				subtargets.Add(Child);
			}
		}
		return subtargets;
	}
	private bool newBranchAvailable()
	{
		if(Node.hasChildren(currentNode))
		{
			return true;
		}
		return false;
	}
	public void OnDrawGizmos() {
		if(currentNode != null)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(mTarget, 1);
		}
	}
	void OnDestroy() {
		int i = gameObject.GetComponent<FPS>().iNumber;
		ts.EmptyFile("Raycasting_Moving"+i.ToString()+".csv");
		ts.SavetoFile("Raycasting_Moving"+i.ToString()+".csv");

		ts2.EmptyFile("Raycasting_all"+i.ToString()+".csv");
		long milliseconds = sw2.ElapsedTicks / (Stopwatch.Frequency / (1000L));
		ts2.saveData(milliseconds);
		ts2.SavetoFile("Raycasting_all"+i.ToString()+".csv");
	}

}