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
	private Vector3 rotation = new Vector3(0, 0, 0);
	private int speed = 200;
	private float rotdeg = 0;
	private float rotrad = 0;

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
		GD.Print(Transform);
		Vector3 vel = new Vector3(0, 0, 0);
		//GD.Print("in physics process");
		direction = new Vector3(0, 0, 0);
		rotrad = 0;
		
		if(Input.IsActionPressed("ui_left"))
		{
			// todo: create rotspeed variable
			// todo: rotate around center of tank
			rotrad -= (float)0.3 * delta;
			//Rotate(Vector3.Left, Mathf.Pi);
			direction.x -= 1;
		}
		// repeat for other directions
		if(Input.IsActionPressed("ui_right"))
		{
			rotrad += (float)0.3 * delta;
			//Rotate(Vector3.Right, Mathf.Pi);
			direction.x += 1;
		}
		if(Input.IsActionPressed("ui_up"))
		{
			direction.z -= 1;
			//direction = new Vector3((float) Math.Cos(rotation.y), 0, (float) Math.Sin(rotation.y));
			//vel = new Vector3(0, 1, 0).Rotated(new Vector3(0,0,1), rotrad * Mathf.Pi) * speed * delta;
			vel = GetTransform().basis.z;
		}
		if(Input.IsActionPressed("ui_down"))
		{
			direction.z += 1;
		}
		GD.Print(rotation);
		GD.Print(direction);
		GD.Print("------");
		
		RotateY(rotrad);
		
		direction = direction.Normalized();
		direction = direction * speed * delta;
		//SetRotation(rotation);
		/*
		To move in a direction based on rotation -
		 - y (up/down) of the vector must be 0
		 - z is backward forward
		 - x is left right
		new Vector2((float)Math.Cos(radians), (float)Math.Sin(radians))
		^ inadequate
		https://docs.godotengine.org/en/3.0/tutorials/3d/using_transforms.html
		*/
		//LinearVelocity = Transform.basis.z * speed;
		vel = vel.Normalized();
		MoveAndSlide(vel, new Vector3(0, 1, 0));
	}
}
