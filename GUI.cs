using Godot;
using System;

public class GUI : MarginContainer
{
    private Tween _tween;
	private TextureProgress _bar;
	private Tank localPlayer;

    /*
	Quick overview of signalling in Godot:
	In signal dispatching class:
		AddUserSignal("dec_local_health");  - establishes signal
		EmitSignal("dec_local_health");		- emit the signal
	In signal receiving class:
		DispatchingClass.Connect("dec_local_health", this, nameof(MethodToCall));
	*/
	
    public override void _Ready()
    {
		GD.Print(GetPath());
		
        _bar = (TextureProgress) GetNode("HBoxContainer/Bars/Bar/Health/TextureProgress");
		
		// _ready() is called for children before parent
		localPlayer = (Tank) GetTree().GetRoot().FindNode(GetTree().GetNetworkUniqueId().ToString(), true, false);
		GD.Print(localPlayer);
    	_bar.MaxValue = localPlayer.getMaxHealth();
		_bar.Value = _bar.MaxValue;
		localPlayer.Connect("dec_local_health", this, nameof(DecrementBar));
		localPlayer.Connect("respawn", this, nameof(ResetBar));
    }

	public void DecrementBar()
	{
		_bar.Value--;
	}
	
	public void ResetBar()
	{
		_bar.Value = _bar.MaxValue;
	}
}
