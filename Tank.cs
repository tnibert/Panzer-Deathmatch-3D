using Godot;
using System;

public class Tank : KinematicBody
{
	// Keep references to our children, might be best to structure later
	private MeshInstance trackleft;
	private MeshInstance trackright;
	private MeshInstance body;
	private MeshInstance turret;
	private MeshInstance gun;
	
	private Vector3 direction = new Vector3();
	private int speed = 200;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        trackleft = (MeshInstance) GetNode("TrackLeft");
		trackright = (MeshInstance) GetNode("TrackRight");
		body = (MeshInstance) GetNode("Body");
		turret = (MeshInstance) GetNode("Turret");
		gun = (MeshInstance) GetNode("Turret/Gun");
		
		Color tankcolor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
		
		foreach(MeshInstance mesh in new MeshInstance[] {trackleft, trackright, body, turret, gun})
		{
			SpatialMaterial mat = (SpatialMaterial) mesh.GetSurfaceMaterial(0);
			mat.AlbedoColor = tankcolor;
			mesh.SetSurfaceMaterial(0, mat);
		}
		SetProcess(true);
		GD.Print("end ready");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
	public override void _PhysicsProcess(float delta)
	{
		//GD.Print("in physics process");
		direction = new Vector3(0, 0, 0);
		
		if(Input.IsActionPressed("ui_left"))
		{
			direction.x -= 1;
		}
		// repeat for other directions
		if(Input.IsActionPressed("ui_right"))
		{
			direction.x += 1;
		}
		if(Input.IsActionPressed("ui_up"))
		{
			direction.z -= 1;
		}
		if(Input.IsActionPressed("ui_down"))
		{
			direction.z += 1;
		}
		direction = direction.Normalized();
		direction = direction * speed * delta;
		MoveAndSlide(direction, new Vector3(0, 1, 0));
	}
}
