using Godot;
using System;

/*
Math links:
https://docs.godotengine.org/en/3.0/tutorials/math/vector_math.html#doc-vector-math
https://docs.godotengine.org/en/3.0/tutorials/math/vectors_advanced.html#doc-vectors-advanced
https://docs.godotengine.org/en/3.0/tutorials/math/matrices_and_transforms.html

unit vectors == direction vectors == normals

dot product tells you if a vector is more, less, or equal to 90 degrees
	- can be used to determine if one entity is facing another
todo: research cross product more - perpendicular vector between two vectors

Unit normal vectors (often abbreviated normals):
"Unit vectors that are perpendicular to a surface (so, they describe the orientation of the surface)"

The dot product between a unit vector and any point in space returns the distance from the point to the plane.
If it is below the plane, value will be negative, can determine side of plane point is on.

Planes have polarity (switch polarity by * -1, operator '-' implemented in godot).  Godot has a Plane type.
N.Dot(point) - D; 
plane.DistanceTo(point);
^ the same, distance from plane to point.  D is distance from origin to plane, N is plane.


*/

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
	private float rotspeed = 0.7f;
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

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    /*public override void _Process(float delta)
    {
      
    }*/

	public override void _PhysicsProcess(float delta)
	{
		//GD.Print(Transform);
		//GD.Print("in physics process");
		direction = new Vector3(0, 0, 0);
		rotrad = 0;
		
		if(Input.IsActionPressed("ui_left"))
		{
			// todo: create rotspeed variable
			// todo: rotate around center of tank
			rotrad -= rotspeed * delta;
			//Rotate(Vector3.Left, Mathf.Pi);
		}
		// repeat for other directions
		if(Input.IsActionPressed("ui_right"))
		{
			rotrad += rotspeed * delta;
			//Rotate(Vector3.Right, Mathf.Pi);
		}
		if(Input.IsActionPressed("ui_up"))
		{
			//direction = new Vector3((float) Math.Cos(rotation.y), 0, (float) Math.Sin(rotation.y));
			//vel = new Vector3(0, 1, 0).Rotated(new Vector3(0,0,1), rotrad * Mathf.Pi) * speed * delta;
			direction = GetTransform().basis.z;
		}
		if(Input.IsActionPressed("ui_down"))
		{
			direction = -1 * GetTransform().basis.z;
		}
		if(Input.IsActionPressed("tur_left"))
		{
			RotateTurret(-1 * rotspeed * delta);
		}
		if(Input.IsActionPressed("tur_right"))
		{
			RotateTurret(rotspeed * delta);
		}
		if(Input.IsActionPressed("ui_select"))
		{
			// looks like we will have to lock this somehow, input is very sensitive
			Fire();
		}
		
		//GD.Print(direction);
		//GD.Print("------");
		
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
		
		/*
		Velocity measures the change in position per unit of time.
		The new position is found by adding velocity to the previous position.
		*/
		
		MoveAndSlide(direction, new Vector3(0, 1, 0));
	}
	
	private void RotateTurret(float rot)
	{
		GD.Print("Rotating Turret");
	}
	
	private void Fire()
	{
		GD.Print("Fire");
		// instantiate bullet
	}
}
