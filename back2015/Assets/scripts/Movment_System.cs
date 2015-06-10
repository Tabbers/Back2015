using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Movment_System : MonoBehaviour {

	public 	int 		speed = 1;
	public 	float 		accuracy=0.5f;
	private int 		pointnumber = 0;
	private bool 		arrived=false;
	public Vector3 	mTarget;
	private float timeout = 0;
	private float timer   = 0;
	public 	GameObject 	target;
	public 	Node 		root;
	public 	Node 		nTarget;
	public 	Node 		currentNode;
	// Use this for initialization
	void Start () {
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
			Debug.Log("No Target selected or invalid target Type");
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
		List<Node> children = currentNode.Getchildren();
		float fdistance = Mathf.Infinity;
		Node subtarget = currentNode.Getparent();
		foreach(Node Child in children)
		{
			if(!Child.explored && Child.GetDistance(transform.position) < fdistance)
			{
				fdistance = Child.GetDistance(transform.position);
				subtarget = Child;
			}
		}
		currentNode = subtarget;
		mTarget = subtarget.Getpoint1();
		timeout = (currentNode.GetDistance(transform.position)/speed)+0.5f;
		timer = timeout;
		pointnumber = 0;
		Debug.Log("Current: "+currentNode.toString());
		Debug.Log("Tree: "+root.getTree());
	}
	public void RefreshNode()
	{
		List<Node> children = currentNode.Getparent().Getchildren();
		Node subtarget = SearchTree(children);
		Node.removeNode(currentNode.Getparent(),nTarget);
		Node.removeNode(currentNode,nTarget);
		Node.attachNode(subtarget,nTarget);
		currentNode = subtarget;
		mTarget = subtarget.Getpoint1();
		timeout = (currentNode.GetDistance(transform.position)/speed)+1f;
		timer = timeout;
		pointnumber = 0;
		Debug.Log("Current: "+currentNode.toString());
		Debug.Log("Tree: "+root.getTree());
	}
	public void MoveToNode()
	{
		switch(pointnumber)
		{
			case 0:
				MoveToPoint();
				if(arrivedAtWaypoint()){
					pointnumber++;
					mTarget = currentNode.Getpoint2();
				}
			break;
			case 1:
				MoveToPoint();
				if(arrivedAtWaypoint()) pointnumber++;
			break;
			case 2: 
				NextNode();
				currentNode.explored = true;
				pointnumber = 0;
			break;
		}
		timer -= Time.deltaTime;
		if(timer <= 0)
		{
			currentNode.explored = true;
			currentNode = currentNode.Getparent();
			NextNode();
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
		Node subtarget = currentNode.Getparent();
		foreach(Node Child in nodes)
		{
			if (Node.hasChildren(Child))
			{
				subtarget = SearchTree(Child.Getchildren());
			}
			else{
				if(!Child.explored && Child.GetDistance(transform.position) < fdistance)
				{
					fdistance = Child.GetDistance(transform.position);
					subtarget = Child;
				}
			}
		}
		return subtarget;
	}
}