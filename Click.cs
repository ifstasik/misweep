using Godot;
using System;

public partial class Click : Sprite2D
{
	
	private Texture2D _normalTexture = GD.Load<Texture2D>("res://img/DozerSprite.jpg");
	private Texture2D _awakeTexture = GD.Load<Texture2D>("res://img/DozerAwakeStatic.jpg");
	RandomNumberGenerator rng = new RandomNumberGenerator();
		
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		rng.Randomize();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
		
	}
	

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseClick && mouseClick.Pressed)
		{
			
			// HasPointed checks if Vector2 LOCALLY exists in Rect of sprite. ToLocal converts global mousepos to local
			
			Vector2 mousePos = ToLocal(GetGlobalMousePosition());
			GD.Print(mousePos);
			if (GetRect().HasPoint(mousePos))
			{
				ChangeSpriteTemporary();
			}
		}
	}

	private async void ChangeSpriteTemporary()
	{
		GD.Print("Dozer found!");
		
		Texture = _awakeTexture;

		await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);

		Texture = _normalTexture;
		float scaleF = rng.RandfRange(0f, 0.5f);
		Vector2 newScale = new Vector2(scaleF, scaleF);
		Scale = newScale;
		Vector2 newPos = new Vector2(rng.RandfRange(-535f+(400f*Scale.X), 535f-(400f*Scale.X)), rng.RandfRange(-283f+(400f*Scale.Y), 283f-(283f*Scale.Y)));
		Position = newPos;
		GD.Print("newPos" + Position);
		GD.Print("newScale" + Scale);
	}
}
