using Godot;
using System;
using System.Collections.Generic;

public partial class Minefield : TileMapLayer
{
	RandomNumberGenerator rng = new RandomNumberGenerator();
	private const int TargetLayer = 0;
	private const int TargetSourceId = 0;
	private static readonly Dictionary<string, Vector2I> Cells = new Dictionary<string, Vector2I>
	{
		{ "closed", new Vector2I(1, 2) },
		{ "open",   new Vector2I(2, 2) },
		{ "bomb",   new Vector2I(3, 2) },
		{ "flag",   new Vector2I(4, 2) },
		{ "one",    new Vector2I(1, 1) },
		{ "two",    new Vector2I(1, 2) },
		{ "three",  new Vector2I(1, 3) },
		{ "four",   new Vector2I(1, 4) },
		{ "five",   new Vector2I(1, 5) },
		{ "six",    new Vector2I(1, 6) },
		{ "seven",  new Vector2I(1, 7) },
		{ "eight",  new Vector2I(1, 8) }
	};
	private Dictionary<Vector2I, string> Field = new Dictionary<Vector2I, string>();
	
	int mapWidth = 10;
	int mapHeight = 20;
	int bombNum = 10;
	int offsetX = -5;
	int offsetY = -10;
	
	private const int TargetAlternativeTile  = 0;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		rng.Randomize();
		
		
		FillMap(mapWidth, mapHeight, offsetX, offsetY);
		SetBombs(mapWidth, mapHeight, bombNum, offsetX, offsetY);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	private void FillMap(int width, int height, int offsetX, int offsetY)
	{
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				SetCell(new Vector2I(x + offsetX, y + offsetY), TargetSourceId, Cells["closed"]);
			}
		}
		
	}
	
	private void SetBombs(int width, int height, int bombNum, int offsetX, int offsetY)
	{	
		for(int i=0;i < bombNum;i++){
			try{
				Vector2I CellPos = new Vector2I(rng.RandiRange(0, width-1) + offsetX, rng.RandiRange(0,height-1) + offsetY);
				Field.Add(CellPos, "bomb");
			}
			catch{
				Vector2I CellPos = new Vector2I(rng.RandiRange(0, width-1) + offsetX, rng.RandiRange(0,height-1) + offsetY);
				Field.Add(CellPos, "bomb");
			}
		}
		foreach (var bomb in Field){
			SetCell(bomb.Key, TargetSourceId, Cells["bomb"]);
		}
	}
	
	
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseClick && mouseClick.Pressed)
		{	
			Vector2 mousePositionFloat = ToLocal(GetGlobalMousePosition());
			Vector2I mousePos = new Vector2I(Convert.ToInt32(mousePositionFloat.X), Convert.ToInt32(mousePositionFloat.Y));
			
			Vector2I TILE = ConvertToTile(mousePos);
		}
	}
	public Vector2I ConvertToTile(Vector2I mousePosition){
		int newTileX = (mousePosition.X - offsetX*16)/16;
		int newTileY = (mousePosition.Y - offsetY*16)/16;
		Vector2I Tile = new Vector2I(newTileX, newTileY);
		GD.Print(newTileX + "  -  " + newTileY);
		return Tile;
	}
}
