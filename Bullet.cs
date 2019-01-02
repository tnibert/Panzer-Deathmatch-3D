using Godot;
using System;

public class Bullet : RigidBody
{
    // Declare member variables here. Examples:

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var expireTimer = (Timer) GetNode("ExpireTimer");
        expireTimer.Connect("timeout", this, "OnExpireTimerTimeout");
		expireTimer.Start();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

    public void OnExpireTimerTimeout()
    {
        this.QueueFree();
    }
}
