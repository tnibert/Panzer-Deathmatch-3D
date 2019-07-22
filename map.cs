using Godot;
using System;

public class map : Spatial
{
	/*
	This is poorly named, but this is the root object for our gameplay
	*/
	
    private PackedScene tankscene;
	private KinematicBody localplayer;
	private KinematicBody remoteplayer;
	private Globals globals;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
		globals = (Globals)GetNode("/root/Globals");
		
        // instantiate the players
		tankscene = ResourceLoader.Load("res://Tank.tscn") as Godot.PackedScene;
		
		// signal for HUD to listen for
		AddUserSignal("tanks_created");
		
		Tank localplayer = (Tank) tankscene.Instance();
		localplayer.SetName(GetTree().GetNetworkUniqueId().ToString());
		localplayer.SetNetworkMaster(GetTree().GetNetworkUniqueId());
		
		Tank remoteplayer = (Tank) tankscene.Instance();
		remoteplayer.SetName(globals.otherPlayerId.ToString());
		remoteplayer.SetNetworkMaster(globals.otherPlayerId);
		
		Vector3 serverspawn = new Vector3(12, 0, 12);
		Vector3 clientspawn = new Vector3(-12, 0, -12);
		
		// set spawn positions
		if(GetTree().IsNetworkServer()) {
			localplayer.Translate(serverspawn);
			remoteplayer.Translate(clientspawn);
		} else {
			localplayer.Translate(clientspawn);
			remoteplayer.Translate(serverspawn);
		}
		localplayer.SetRespawn();
		remoteplayer.SetRespawn();
		
		AddChild(localplayer);
		AddChild(remoteplayer);
		
		localplayer.SetActiveCam();
		
		// Notify the HUD that the tanks are available
		EmitSignal("tanks_created");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
