using Godot;
using System;

public class Globals : Godot.Node
{
    // Declare member variables here. Examples:
    public int otherPlayerId = -1;
	
	/**
	* Reparent node o to become a child of node n
	*/
	public static void reparent(Node o, Node n)
	{
		Node oldparent = o.GetParent();
		Node target = n;
		Node source = o;
		
		oldparent.RemoveChild(source);
		target.AddChild(source);
		source.SetOwner(target);
	} 
}
