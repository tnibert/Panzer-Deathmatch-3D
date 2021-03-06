using Godot;
using System;

/*
Code links:
http://gameprogrammingpatterns.com/observer.html

Math links:
https://docs.godotengine.org/en/3.0/tutorials/math/vector_math.html#doc-vector-math
https://docs.godotengine.org/en/3.0/tutorials/math/vectors_advanced.html#doc-vectors-advanced
https://docs.godotengine.org/en/3.0/tutorials/math/matrices_and_transforms.html
https://docs.godotengine.org/en/3.0/tutorials/3d/using_transforms.html
https://www.khanacademy.org/math/linear-algebra
https://www.mathsisfun.com/algebra/vectors-dot-product.html

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

todo: learn more about quaternions

Use IsNetworkServer() to control collision detection

*/

public class Tank : KinematicBody
{	
	// Keep references to our children, might be best to structure later
	private MeshInstance trackleft;
	private MeshInstance trackright;
	private MeshInstance body;
	private KinematicBody turret;
	private MeshInstance gun;
	private PackedScene bulletscene;
	private Camera thirdpersoncamera;
	private Camera firstpersoncamera;
	private Particles explosion;
	
	// Audio resources
	private Node soundgroup;
	private AudioStreamPlayer3D trackmovementsound;
	private AudioStreamPlayer3D firesound;
	private AudioStreamPlayer3D explodesound;
	
	private Vector3 direction = new Vector3();

	// for respawn delay
	private Timer deathtimer = new Timer();
	private int RESPAWN_SECONDS = 8;
	
	// these speeds are calculated in with other variables, they are not equivalent
	private int speed = 200;			// will be multiplied by delta
	private int bulletspeed = 10;
	private float rotspeed = 0.7f;
	private float turretrotspeed = 0.2f;
	private float rotrad = 0;
	private float pitch = 0;
	private float yaw = 0;
	
	private float originalY;
	
	private Transform spawnpoint;
	private int maxhealth = 6;
	protected int health;
	private bool firstperson = false;
	private bool localcontrolactive = true;
	
	// these mouse things aren't really necessary anymore...
	private Vector2 currentmousepos = new Vector2();
	
	/*
	const Input.MouseMode MOUSE_MODE_CONFINED = (Input.MouseMode) 3;
	const Input.MouseMode MOUSE_MODE_CAPTURED = (Input.MouseMode) 2;
	const Input.MouseMode MOUSE_MODE_HIDDEN = (Input.MouseMode) 1;
	const Input.MouseMode MOUSE_MODE_VISIBLE = (Input.MouseMode) 0;
	*/
	
	private Random seed = new Random();
	Color defaultTankColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
	
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
		// Get references to parts of tanks
        trackleft = (MeshInstance) GetNode("TrackLeft");
		trackright = (MeshInstance) GetNode("TrackRight");
		body = (MeshInstance) GetNode("Body");
		turret = (KinematicBody) GetNode("Turret");
		gun = (MeshInstance) GetNode("Turret/TurretMesh/Gun");
		
		// load sounds
		trackmovementsound = (AudioStreamPlayer3D) GetNode("SoundGroup/TrackMovementSound");
		firesound = (AudioStreamPlayer3D) GetNode("SoundGroup/FireSound");
		explodesound = (AudioStreamPlayer3D) GetNode("SoundGroup/ExplosionSound");
		soundgroup = GetNode("SoundGroup");
		
		// load cameras
		thirdpersoncamera = (Camera) GetNode("ThirdPersonCam");
		firstpersoncamera = (Camera) GetNode("Turret/TurretMesh/Gun/BulletSpawn/Camera");
		
		// reference to explosion for death
		explosion = (Particles) GetNode("Explosion/Particles");
		explosion.SetEmitting(false);
		
		// Load bullet scene
		bulletscene = ResourceLoader.Load("res://Bullet.tscn") as Godot.PackedScene;
		
		// Set a color for the tank
		setTankColor(defaultTankColor);
		
		health = maxhealth;
		
		// add signals
		AddUserSignal("dec_local_health");
		AddUserSignal("respawn");
		AddUserSignal("bullet_fired");
		
		// connect respawn timer
		AddChild(deathtimer);
		deathtimer.Connect("timeout", this, "Respawn");
		deathtimer.SetWaitTime(RESPAWN_SECONDS);
		
		SetProcess(true);
		
		// required to appear at spawn
		MoveAndSlide(new Vector3(0, 0, 0), new Vector3(0, 1, 0), true);
		
		// enforce no flying tanks issue #20
		originalY = Transform.origin.y;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    /*public override void _Process(float delta)
    {
      
    }*/

	public int getMaxHealth()
	{
		return maxhealth;
	}

	private void setTankColor(Color c)
	{
		foreach(MeshInstance mesh in new MeshInstance[] {trackleft, trackright, body, (MeshInstance) GetNode("Turret/TurretMesh"), gun})
		{
			// need to use Duplicate() or we change the color of both tanks, they share the SpatialMaterial reference
			SpatialMaterial mat = (SpatialMaterial) mesh.GetSurfaceMaterial(0).Duplicate(true);
			mat.AlbedoColor = c;
			mesh.SetSurfaceMaterial(0, mat);
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		// if the tank respawns in the air or underground, bring it back to earth (issue #20)
		if(Transform.origin.y != originalY)
		{
			MoveAndSlide(new Vector3(0, originalY-Transform.origin.y, 0), new Vector3(0, 1, 0), true);
		}
		
		direction = new Vector3(0, 0, 0);
		rotrad = 0;
		float TurretRot = 0;
		bool change = false;
		
		if(IsNetworkMaster() && localcontrolactive)
		{
			if(Input.IsActionPressed("ui_left"))
			{
				rotrad += rotspeed * delta;
				//Rotate(Vector3.Left, Mathf.Pi);
				change = true;
			}
			// repeat for other directions
			if(Input.IsActionPressed("ui_right"))
			{
				rotrad -= rotspeed * delta;
				//Rotate(Vector3.Right, Mathf.Pi);
				change = true;
			}
			if(Input.IsActionPressed("ui_up"))
			{
				//direction = new Vector3((float) Math.Cos(rotation.y), 0, (float) Math.Sin(rotation.y));
				//vel = new Vector3(0, 1, 0).Rotated(new Vector3(0,0,1), rotrad * Mathf.Pi) * speed * delta;
				direction = GetTransform().basis.z;
				change = true;
			}
			if(Input.IsActionPressed("ui_down"))
			{
				direction = -1 * GetTransform().basis.z;
				change = true;
			}
			if(Input.IsActionPressed("tur_left"))
			{
				TurretRot = rotspeed * delta;
				change = true;
			}
			if(Input.IsActionPressed("tur_right"))
			{
				TurretRot = -1 * rotspeed * delta;
				change = true;
			}

			// Only true on frame that key was pressed
			if(Input.IsActionJustPressed("ui_select"))
			{
				Rpc("NetFire");
				Fire();
			}
			if(Input.IsActionJustPressed("swapcam"))
			{
				SwapCamera();
			}
			
		}		
		
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
		
		if(change)
		{
			// play movement audio
			if(!trackmovementsound.IsPlaying())
			{
				trackmovementsound.Play();
			}
			
			//LocSetPosAndRot(direction, rotrad, deg2rad(yaw), deg2rad(pitch));
			LocSetPosAndRot(direction, rotrad, TurretRot);
			
			// announce movement
			RpcUnreliable("NetSetTransforms", this.Transform, turret.Transform);
		}
		else
		{
			trackmovementsound.Stop();
		}
		
		// we don't need this, cause out of scope after, but I like being explicit for sanity, blame biomed
		change = false;
	}
	
	/*public override void _Input(InputEvent @event)
	{
		// This is kind of a hack to get our mouse to direct our turret rotation
	    // Relative mouse position
	    if (@event is InputEventMouseMotion eventMouseMotion)
	        GD.Print("Mouse Motion at: ", eventMouseMotion.GetRelative());
	
	    // Print the size of the viewport
	    //GD.Print("Viewport Resolution is: ", GetViewPortRect().Size);
	}*/
	
	private float deg2rad(float val)
	{
		// yes, it's a shitty hack, but I'm on an airplane right now with no internet
		double ratio = 57.324840764331206;
		return (float) (val/ratio);
	}
	
	private void LocSetPosAndRot(Vector3 dir, float bodyrot, float turrot)
	{
		/*
		Do the transformations to the player
		*/
		RotateY(bodyrot);
		RotateTurret(turrot);
		MoveAndSlide(dir, new Vector3(0, 1, 0), true);
	}
	
	[Remote]
	private void NetSetTransforms(Transform bodytrans, Transform turrettrans)
	{
		/* syncs the transforms for the tank */
		//GD.Print(GetTree().GetNetworkUniqueId().ToString());
		
		this.Transform = bodytrans;
		turret.Transform = turrettrans;
	}
	
	private void RotateTurret(float rot)
	{
		/*
		To do turret rotation vertically, we will likely need to use quaternion here
		
		For some reason, when we rotate by two directions, the turret moves all over the place (around the tank)
		*/
		
		//GD.Print(yaw);
		turret.RotateY(rot);
		//turret.RotateX(pitch);
		//turret.SetRotation(new Vector3(pitch, yaw, 0));
	}
	
	private void Fire()
	{
		// todo: use ray casting for the bullet transformation and collision
		//GD.Print("Fire");
		
		// instantiate bullet
		RigidBody bullet = (RigidBody) bulletscene.Instance();
		Spatial bulletspawnpoint = (Spatial) GetNode("Turret/TurretMesh/Gun/BulletSpawn");
		Vector3 spawnpos = bulletspawnpoint.GetGlobalTransform().origin;		//.GetTranslation() for local space
		
		// set position and velocity
		bullet.SetTranslation(spawnpos);
		bullet.SetLinearVelocity(turret.GetGlobalTransform().basis.z * bulletspeed);
		
		// add to scene
		GetParent().AddChild(bullet);
		bullet.Show();
		firesound.Play();
		EmitSignal("bullet_fired", bullet);
	}
	
	[Remote]
	private void NetFire()
	{
		/*
		Physics engine does not produce same results across network
		*/
		Fire();
	}
	
	public int DecrementHealth()
	{
		// decrement health and change the color of the tank
		health--;

		// local health is master, sync across network for consistency
		// this must be done because RigidBody bullet is not consistent in moving between network clients
		if(IsNetworkMaster())
		{
			Rpc("NetDecHealth");
			
			// for health bar
			EmitSignal("dec_local_health");
		}

		setTankColor(new Color((float)seed.NextDouble(), (float)seed.NextDouble(), (float)seed.NextDouble()));

		// if the health has been depleted and we haven't previously entered this branch (timer hasn't been started)
		if(health <= 0 && deathtimer.IsStopped())
		{
			localcontrolactive = false;
			Explode();
			deathtimer.Start();
		}
		return health;
	}

	[Remote]
	private void NetDecHealth()
	{
		DecrementHealth();
	}
	
	public void Explode()
	{
		// note that we set emitting false in Respawn()
		explodesound.Play();
		explosion.SetEmitting(true);
	}
	
	public void SetRespawn()
	{
		// Set the respawn point to the current position
		spawnpoint = GetTransform();
	}
	
	private void Respawn()
	{	
		deathtimer.Stop();
		EmitSignal("respawn");
		health = maxhealth;
		setTankColor(defaultTankColor);
		
		explosion.SetEmitting(false);
		
		// move to spawn point
		this.Transform = spawnpoint;
		MoveAndSlide(new Vector3(0, 0, 0), new Vector3(0, 1, 0), true);
		
		// restore user controls
		localcontrolactive = true;
	}
	
	public Camera SetActiveCam()
	{
		thirdpersoncamera.MakeCurrent();
		firstperson = false;
		return thirdpersoncamera;
	}
	
	private void SwapCamera()
	{
		/*
		Switch between first person and third person cameras
		If we were to reparent the sound group, the sound would not adjust volume at camera switch
		However, I kind of like the effect of feeling inside vs outside the tank
		*/
		if(!firstperson)
		{
			firstpersoncamera.MakeCurrent();
			//Globals.reparent(soundgroup, this);
		}
		else
		{
			thirdpersoncamera.MakeCurrent();
			//Globals.reparent(soundgroup, thirdpersoncamera);
		}
		firstperson = !firstperson;
	}
}
