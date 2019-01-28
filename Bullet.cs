using Godot;
using System;

public class Bullet : RigidBody
{
    // Declare member variables here. Examples:

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
		GD.Print(this.GetName()," is colliding with ",who.GetName());
		try {
			((Tank)who).DecrementHealth();
			//GD.Print("Health decremented");
		} catch {
			//GD.Print("could not decrement health");
		}
		this.QueueFree();
	}
}
