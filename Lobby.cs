using Godot;
using System;

public class Lobby : Node
{
    // Declare member variables here. Examples:
    LineEdit host;
	Button startbutton;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        host = (LineEdit) GetNode("IPaddressInput");
		startbutton = (Button) GetNode("StartButton");
		
		// connect the button "pressed" signal to StartGame() method
		startbutton.Connect("pressed", this, "StartGame");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    /*public override void _Process(float delta)
    {

    }*/
	
	private void StartGame()
	{
		GD.Print(host.Text);
		// initialize network game
	}
}
