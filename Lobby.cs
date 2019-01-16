using Godot;
using System;

public class Lobby : Node2D
{
    // Declare member variables here. Examples:
    LineEdit hostip;
	Button hostbutton;
	Button joinbutton;
	Globals globals = new Globals();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        hostip = (LineEdit) GetNode("IPaddressInput");
		hostbutton = (Button) GetNode("HostButton");
		joinbutton = (Button) GetNode("JoinButton");
		
		// connect the button "pressed" signal to StartGame() method
		hostbutton.Connect("pressed", this, "HostGame");
		joinbutton.Connect("pressed", this, "JoinGame");
		GetTree().Connect("network_peer_connected", this, "PlayerConnected");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    /*public override void _Process(float delta)
    {

    }*/
	
	private void JoinGame()
	{
		GD.Print("Joining network");
		NetworkedMultiplayerENet host = new NetworkedMultiplayerENet();
		host.CreateClient(hostip.Text, 4242);
		GetTree().SetNetworkPeer(host);
		joinbutton.Hide();
	}
	
	private void HostGame()
	{
		// initialize network game
		NetworkedMultiplayerENet host = new NetworkedMultiplayerENet();
		var resp = host.CreateServer(4242,2);
		if(resp != 0)
		{
			GD.Print("ruh roh");
			return;
		}
		hostbutton.Hide();
		//startbutton.disabled = true;
		GetTree().SetNetworkPeer(host);
	}
	
	private void PlayerConnected(int id)
	{
		GD.Print("Connection established");
		globals.otherPlayerId = id;
		PackedScene map = ResourceLoader.Load("res://map.tscn") as Godot.PackedScene;
    	Spatial game = (Spatial)map.Instance();
		GetTree().GetRoot().AddChild(game);
		Hide();
	}
}
