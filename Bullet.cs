using Godot;
using System;

public class Bullet : RigidBody
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
		SetMaxContactsReported(1);
        var expireTimer = (Timer) GetNode("ExpireTimer");
        expireTimer.Connect("timeout", this, "OnExpireTimerTimeout");
		expireTimer.Start();
		this.Connect("body_entered", this, "colliding");
		
		SetProcess(true);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    /*public override void _Process(float delta)
    {
        Godot.Collections.Array coll = GetCollidingBodies();
		if(coll.Length() > 0)
		{
        	GD.Print(coll);
		}
    }*/

    public void OnExpireTimerTimeout()
    {
        this.QueueFree();
    }
	
	public void colliding(Node who)
	{
		//GD.Print(this.GetName()," is colliding with ",who.GetName());
		// todo: maybe remove this try catch (what happens if bullet collides with wall?)
		try {
			Tank localPlayer = (Tank) GetTree().GetRoot().FindNode(GetTree().GetNetworkUniqueId().ToString(), true, false);
			
			// only decrement the health of the local player
			// we will update the remotes via RPC
			if(who == localPlayer)
			{
				((Tank)who).DecrementHealth();
			}
			//GD.Print("Health decremented");
		} catch {
			//GD.Print("could not decrement health");
		}
		this.QueueFree();
	}
}
