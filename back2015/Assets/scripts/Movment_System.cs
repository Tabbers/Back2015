using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;

public class Movment_System : MonoBehaviour {

	public 	int 		speed = 1;
	public 	float 		accuracy=0.5f;
	private int 		pointnumber = 0;
	private bool 		arrived=false;
	public  bool 		backtracing = false;
	public  Vector3 	mTarget;
	private float timeout = 0;
	private float timer   = 0;
	public  float Interval = 0.5f;
	private float timer2   = 0;
	public 	GameObject 	target;
	public 	Node 		root;
	public 	Node 		nTarget;
	public 	Node 		currentNode;
	public  List<Node>  Pathtaken;
	// Use this for initialization
	Stopwatch sw;
	void Start () {
		sw = new Stopwatch();
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
		MoveToNode();
	}
  	public void NextNode ()
	{
		Node previous = currentNode;
		List<Node> children;
		Node subtarget;
		if(Node.hasChildren(currentNode))
		{
			children = currentNode.Getchildren();
			subtarget = SearchTree(children);
			if(subtarget != nTarget)
			{
				Node reftarget = removeTarget(children);
				Node.removeNode(reftarget.Getparent(),reftarget);
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
				Node reftarget = removeTarget(children);
				Node.removeNode(reftarget.Getparent(),reftarget);
				Node.attachNode(subtarget,nTarget);
			}
			currentNode = subtarget;
			mTarget = currentNode.Getpoint1();
			timeout = (currentNode.GetDistance(0,transform.position)/speed)+0.5f;
			timer = timeout;
			pointnumber = 0;
		}
		UnityEngine.Debug.Log("Next Current: "+currentNode.toString());
		UnityEngine.Debug.Log("Next Tree: "+root.getTree());
	}
	public void RefreshNode()
	{
		Node previous = currentNode;
		List<Node> children = root.Getchildren();
		Node reftarget = removeTarget(children);
		Node.removeNode(reftarget.Getparent(),reftarget);
		Node subtarget = SearchTree(children);
		Node.attachNode(subtarget,nTarget);
		currentNode = subtarget;
		if(currentNode.reference != nTarget.reference) Pathtaken.Add(currentNode);
		mTarget = currentNode.Getpoint1();
		pointnumber = 0;
		timeout = (currentNode.GetDistance(pointnumber,transform.position)/speed)+1f;
		timer = timeout;
		UnityEngine.Debug.Log("Refresh Current: "+currentNode.toString());
		UnityEngine.Debug.Log("Refresh Tree after: "+root.getTree());
	}
	public void MoveToNode()
	{
		if(!backtracing)
		{
			if(pointnumber < currentNode.GetPoints().Count)
			{
				if(arrivedAtWaypoint()){
					pointnumber++;
					if(pointnumber != currentNode.GetPoints().Count){ 
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
				if(currentNode.reference != nTarget.reference )currentNode.explored =true;
				NextNode();
			}
			timer2 -= Time.deltaTime;
			if(timer2<=0) 
			{
				sw.Start();
				timer -= Time.deltaTime;
				if(timer <= 0)
				{
					UnityEngine.Debug.Log("Target unreachable in Time!");
					if(currentNode.reference != nTarget.reference ) currentNode.explored = true;
					mTarget = currentNode.GetPoint(currentNode.GetPoints().Count-1);
					pointnumber = currentNode.GetPoints().Count-1;
					backtracing = true;
					timer2 = Interval;
					
				}
				sw.Stop();
				long microseconds = sw.ElapsedTicks / (Stopwatch.Frequency / (1000L*1000L));
				Timestamp.SavetoFile("Raycast_Pathing.csv",microseconds);
				sw.Reset();
			}
		}
		else{
			if(pointnumber > 0)
			{
				if(arrivedAtWaypoint()){
					pointnumber--;
					mTarget = currentNode.GetPoint(pointnumber);
				}
				else{
					MoveToPoint();
				}
			}
			else
			{
				if(newBranchAvailable())
				{
					bool allexplored = true;
					foreach(Node child in currentNode.children)
					{
						if(child.explored == false)
						{
							allexplored = false;
							currentNode = child;
							mTarget = currentNode.Getpoint1();
							pointnumber = 0;
							timeout = (currentNode.GetDistance(pointnumber,transform.position)/speed)+1f;
							timer = timeout;
							break;
						}
					}
					if(allexplored)
					{
						List<Node> children = root.Getchildren();
						Node reftarget = removeTarget(children);
						Node.removeNode(reftarget.Getparent(),reftarget);
						currentNode.Addchildren(nTarget);
						NextNode();
					}
					backtracing = false;
				}
				else{
					if(currentNode.reference != nTarget.reference )currentNode.explored =true;
					currentNode = currentNode.Getparent();
					mTarget = currentNode.GetPoint(currentNode.GetPoints().Count-1);
					pointnumber = currentNode.GetPoints().Count-1; 
				}
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
	private Node removeTarget(List<Node> nodes)
	{
		Node subtarget = new Node();
		foreach(Node Child in nodes)
		{
			if (Node.hasChildren(Child))
			{
				subtarget = removeTarget(Child.Getchildren());
			}
			else
			{
				if(Child.reference == nTarget.reference)
				{
					subtarget = Child;
				}
			}
		}
		return subtarget;
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
				else subtarget = Child;
			}
			else{
				if(!Child.explored && Child.GetDistance(0,transform.position) < fdistance)
				{
					fdistance = Child.GetDistance(0,transform.position);
					subtarget = Child;
				}
			}
		}
		return subtarget;
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
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(currentNode.Getpoint1(), 1);
	}
}