using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node
{
	private Vector3 point1;
	private Vector3 point2;
	private GameObject reference;

	private List<Node> children;
	private Node parent;

	public int Depth = 0;
	public bool explored = false;
	public bool isroot = false;
	public bool istarget = false;

	public Node()
	{
		children = new List<Node>();
	}
	public Node(GameObject reference, Vector3 point1)
	{
		this.reference = reference;
		this.point1 = point1;
		children = new List<Node>();
	}
	public Node(GameObject reference, Vector3 point1, Vector3 point2)
	{
		this.reference = reference;
		this.point1 = point1;
		this.point2 = point2;
		children = new List<Node>();
	}

	public void Setpoint1(Vector3 point1)
	{
		this.point1 = point1;
	}
	public void Setpoint2(Vector3 point2)
	{
		this.point2 = point2;
	}
	public void Setreference(GameObject reference)
	{
		this.reference = reference;
	}
	public void Addchildren(Node children)
	{
		if(!istarget)
		{
			this.children.Add(children);
		}
	}
	public void Removechildren(Node children)
	{
		if(!istarget)
		{
			this.children.Remove(children);
		}
	}
	public void Setparent(Node parent)
	{
		this.parent = parent;
	}
	public GameObject Getrefernce()
	{
		return reference;
	}
	public Vector3 Getpoint1()
	{
		return point1;
	}
	public Vector3 Getpoint2()
	{
		return point2;
	}
	public List<Node> Getchildren()
	{
		if(!istarget)
		{
			return children;
		}
		return new List<Node>();
	}
	public Node Getparent()
	{
		if(!isroot)
		{
			return parent;
		}
		return new Node();
	}
	public List<Vector3> GetPoints ()
	{
		List<Vector3> buffer = new List<Vector3> ();
		buffer.Add(point1);
		buffer.Add(point2);

		return buffer;
	}
	public float GetDistance(Vector3 position)
	{
		return Vector3.Distance(point1,position);
	}
	public string toString()
	{
		string buffer = "";
		buffer += "refernce: "+reference.name+"\n";
		buffer += "depth: "+Depth+"\n";
		buffer += "point 1: "+point1.ToString()+"\n";
		buffer += "point 2: "+point2.ToString()+"\n";
		if(!isroot)	buffer += "parent: "+parent.reference.name+"\n\n";
		if(istarget) buffer+="End of Tree \n\n";
		return buffer;
	}
	public string getTree ()
	{
		string buffer = toString();
		foreach (Node child in children)
		{
			buffer+= child.getTree();
		}
		return buffer;
	}
	public void Selfevaluate(List<GameObject> CollisionObjects, Vector3 position)
	{
		List<Vector3> testpoints = new List<Vector3>();
		testpoints.Add(point1);
		testpoints.Add(point2);
		foreach(Vector3 point in testpoints)
		{
			foreach(GameObject GO in CollisionObjects)
			{
				Bounds b = GO.collider.bounds;
				Vector3 RO = new Vector3(b.center.x + b.extents.x,position.y,b.center.z + b.extents.z);
				Vector3 LU = new Vector3(b.center.x - b.extents.x,position.y,b.center.z - b.extents.z);
				
				if(point.x < RO.x && 
				   point.x > LU.x &&
				   point.z < RO.z &&
				   point.z > LU.z)
				{
					removeNode(parent,this);
				}
			}
		}
		foreach (Node child in children)
		{
			child.Selfevaluate(CollisionObjects,position);
		}
	}
	public static void attachNode(Node Parent, Node Child)
	{
		Parent.Addchildren(Child);
		Child.Setparent(Parent);
	}
	public static void removeNode(Node Parent,Node Child)
	{
		Child.Setparent(null);
		Parent.Getchildren().Remove(Child);
	}
	public static void moveNode(Node ParentOld, Node ParentNew, Node Child)
	{
		removeNode(ParentOld,Child);
		attachNode(ParentNew,Child);
	}
	public static bool hasChildren(Node node)
	{
		if(node.children.Count ==0) return false;
		else return true;
	}

}
