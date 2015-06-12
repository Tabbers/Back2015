using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class Node
{
	public List<Vector3> Pointlist;
	public GameObject reference;
	public List<Node> children;
	protected Node parent;

	public int  Depth = 0;
	public bool explored = false;
	public bool isroot = false;
	public bool istarget = false;

//=========================================================================================
// Constructor Methods ====================================================================
//=========================================================================================
	public Node()
	{
		children = new List<Node>();
	}
	public Node(GameObject reference)
	{
		children = new List<Node>();
		Pointlist = new List<Vector3>();
		this.reference = reference;
	}
	public Node(GameObject reference, Vector3 point1)
	{
		children = new List<Node>();
		Pointlist = new List<Vector3>();
		this.reference = reference;
		Pointlist.Add(point1);
	}
	public Node(GameObject reference, Vector3 point1, Vector3 point2)
	{
		children = new List<Node>();
		Pointlist = new List<Vector3>();
		this.reference = reference;
		Pointlist.Add(point1);
		Pointlist.Add(point2);
	}
	public Node(GameObject reference, Vector3 point1, Vector3 point2, Vector3 point3)
	{
		children = new List<Node>();
		Pointlist = new List<Vector3>();
		this.reference = reference;
		Pointlist.Add(point1);
		Pointlist.Add(point2);
		Pointlist.Add(point3);
	}

//=========================================================================================
// Set Methods ============================================================================
//=========================================================================================
	public void Setpoint1(Vector3 point1)
	{
		Pointlist.Insert(0,point1);
	}
	public void Setpoint2(Vector3 point2)
	{
		Pointlist.Insert(1,point2);
	}
	public void Setpoint3(Vector3 point3)
	{
		Pointlist.Insert(2,point3);
	}
	public void Setreference(GameObject reference)
	{
		this.reference = reference;
	}
	public void Setparent(Node parent)
	{
		this.parent = parent;
	}

//=========================================================================================
// Get Methods ============================================================================
//=========================================================================================
	public GameObject Getrefernce()
	{
		return reference;
	}
	public Vector3 Getpoint1()
	{
		return Pointlist[0];
	}
	public Vector3 Getpoint2()
	{
		return Pointlist[1];
	}
	public Vector3 Getpoint3()
	{
		return Pointlist[2];
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
		GameObject empty = new GameObject();
		empty.name = "Empty";
		return new Node(empty);
	}
	public List<Vector3> GetPoints ()
	{
		return Pointlist;
	}
	public Vector3 GetPoint (int index)
	{
		return Pointlist[index];
	}
	public float GetDistance(int index,Vector3 position)
	{
		return Vector3.Distance(Pointlist[index],position);
	}

//=========================================================================================
// Remove Methods =========================================================================
//=========================================================================================
	public void Removechildren(Node children)
	{
		if(!istarget)
		{
			this.children.Remove(children);
		}
	}
//=========================================================================================
// Add Methods =========================================================================
//=========================================================================================
	public void Addchildren(Node children)
	{
		if(!istarget)
		{
			this.children.Add(children);
		}
	}
	public void AddPoints(List<Vector3> points)
	{
		foreach(Vector3 point in points)
		{
			this.Pointlist.Add(point);
		}
	}

	public string toString()
	{
		string buffer = "";
		buffer += "refernce: "+reference.name+"\n";
		buffer += "depth: "+Depth+"\n";
		buffer += "points: "+Pointlist.Count.ToString()+"\n";
		buffer += "explored: "+explored.ToString()+"\n";
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
	public static void Selfevaluate(List<GameObject> CollisionObjects,Vector3 position,Node node)
	{
		foreach(Vector3 point in node.GetPoints())
		{
			foreach(GameObject GO in CollisionObjects)
			{
				Bounds b = GO.collider.bounds;
				Vector3 RO = new Vector3(b.center.x + b.extents.x+0.3f,position.y,b.center.z + b.extents.z+0.3f);
				Vector3 LU = new Vector3(b.center.x - b.extents.x-0.3f,position.y,b.center.z - b.extents.z-0.3f);
				
				if(point.x < RO.x && 
				   point.x > LU.x &&
				   point.z < RO.z &&
				   point.z > LU.z)
				{
					Node.removeNode(node.Getparent(),node);
					break;
				}
			}
			break;
		}
	}
	public static void attachNode(Node Parent, Node Child)
	{
		Parent.Addchildren(Child);
		Child.Setparent(Parent);
	}
	public static void removeNode(Node Parent,Node Child)
	{
		if(Node.hasChildren(Parent))
		{
			Parent.Getchildren().Remove(Child);
			Child.Setparent(null);
		}

	}
	public static void moveNode(Node ParentOld, Node ParentNew, Node Child)
	{
		removeNode(ParentOld,Child);
		attachNode(ParentNew,Child);
	}
	public static bool hasChildren(Node node)
	{
		if(node.children.Count == 0) return false;
		else return true;
	}
	public static Node getSibling(Node node)
	{
		Node buffer = new Node();
		List<Node> children = node.Getparent().Getchildren();
		foreach (Node child in children)
		{
			if(child!= node && child.reference == node.reference)
			{
				buffer = child;
				break;
			}
		}
		return buffer;
	}
	public string childrenString()
	{
		string buffer="";
		foreach (Node child in children)
		{
			buffer+=child.reference.name+"\n";
		}
		return buffer;
	}

}
