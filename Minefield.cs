using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

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
		{ "1",    new Vector2I(1, 1) },
		{ "2",    new Vector2I(2, 1) },
		{ "3",  new Vector2I(3, 1) },
		{ "4",   new Vector2I(4, 1) },
		{ "5",   new Vector2I(5, 1) },
		{ "6",    new Vector2I(6, 1) },
		{ "7",  new Vector2I(7, 1) },
		{ "8",  new Vector2I(8, 1) }
	};
	private Dictionary<Vector2I, string> Field = new Dictionary<Vector2I, string>();
	private Dictionary<Vector2I, bool> FlaggedOnes = new Dictionary<Vector2I, bool>();
	private Dictionary<Vector2I, bool> OpenedOnes = new Dictionary<Vector2I, bool>();
	
	static int mapWidth = 10;
	static int mapHeight = 20;
	int bombNum = 20;
	int offsetX = -1*(mapWidth/2);
	int offsetY = -1*(mapHeight/2);
	bool hasOpened = false;
	
	private const int TargetAlternativeTile  = 0;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		rng.Randomize();
		
		for(int x = 0; x < mapWidth; x++){
				for(int y = 0; y < mapHeight;y++){
					Field.Add(new Vector2I(x,y), "closed");
					FlaggedOnes.Add(new Vector2I(x,y), false);
					OpenedOnes.Add(new Vector2I(x,y), false);
				}
			}
		
		
		FillMap(mapWidth, mapHeight, offsetX, offsetY);
		SetBombs(mapWidth, mapHeight, bombNum, offsetX, offsetY);
		SetNums(mapWidth, mapHeight, offsetX, offsetY);
		foreach(KeyValuePair<Vector2I, string> Tile in Field){
			if(Tile.Value == "closed"){
				SetCell(new Vector2I(Tile.Key.X + offsetX, Tile.Key.Y + offsetY), TargetSourceId, Cells["flag"]);
				break;
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
		if (OpenedOnes.Count(v => v.Value == false) == bombNum)
		{
			GD.Print("You win!");
			SetProcess(false); 
		}
	}
	private void FillMap(int width, int height, int offsetX, int offsetY)
	{
		foreach(KeyValuePair<Vector2I, string> Tile in Field)
		{
			Vector2I Position = new Vector2I(Tile.Key.X + offsetX, Tile.Key.Y + offsetY);
			SetCell(Position, TargetSourceId, Cells["closed"]);
		}
		
	}
	
	private void SetBombs(int width, int height, int bombNum, int offsetX, int offsetY)
	{
		int placed = 0;
		while (placed < bombNum)
		{
			Vector2I cellPos = new Vector2I( //global positioning NEEDS OFFSET
				rng.RandiRange(0, width - 1) , 
				rng.RandiRange(0, height - 1)
			);
			
			if(Field[cellPos] != "bomb")
			{
				Field[cellPos] = "bomb" ;
				placed++;
			}
			else{continue;}
		}
	}
	private void SetNums(int width, int height, int offsetX, int offsetY)
	{
			//width,x : 0-9
			//height,y : 0-19
			
		foreach(KeyValuePair<Vector2I, string> Tile in Field)
		{
			Vector2I cellPos = new Vector2I(Tile.Key.X,Tile.Key.Y);
			Vector2I tilePosForSetting = new Vector2I(Tile.Key.X + offsetX, Tile.Key.Y + offsetY);
			(int bombsAround,bool BombOnCell) = CheckNeighbours(cellPos);
			
			
			if(!BombOnCell && bombsAround > 0){
				Field[cellPos] = $"{bombsAround}";
			}
		}
		
			//check for neigbors bobm
			
			
			//if there is bobm, increase local bombCounter
			//if no bobms, skip the cell and increase placed counter
			
	}

	public (int, bool) CheckNeighbours(Vector2I cellPos){
		bool BombOnCell = false;
		if(Field[cellPos] == "bomb"){
			return (0, BombOnCell = true);
		}
		
		int bombCount = 0;
		for(int x=-1 ; x <= 1; x++){ //starts from top_left
			for(int y=-1 ; y <= 1; y++){
				if(Field.ContainsKey(new Vector2I(cellPos.X + x, cellPos.Y + y)) && Field[new Vector2I(cellPos.X + x, cellPos.Y + y)] == "bomb"){
					bombCount++;	
				};
			}
		}
		
		return (bombCount, BombOnCell);
	}
	
	
	public void OpenArea(Vector2I startTileSetting)
	{
		Queue<Vector2I> toProcess = new Queue<Vector2I>();
		toProcess.Enqueue(startTileSetting);

		HashSet<Vector2I> visited = new HashSet<Vector2I>();

		while (toProcess.Count > 0)
		{
			Vector2I currentSetting = toProcess.Dequeue();
			if (visited.Contains(currentSetting)) continue;
			visited.Add(currentSetting);

			Vector2I dictPos = new Vector2I(currentSetting.X - offsetX, currentSetting.Y - offsetY);
			if (!Field.ContainsKey(dictPos)) continue;

			string content = Field[dictPos];

			// Якщо це БОМБА — зупиняємося (не відкриваємо її)
			if (content == "bomb") continue;

			// Якщо це ЦИФРА — показуємо її і зупиняємо розширення в цей бік
			if (new[] {"1","2","3","4","5","6","7","8"}.Contains(content))
			{
				SetCell(currentSetting, TargetSourceId, Cells[content]);
				OpenedOnes[dictPos] = true;
				continue; 
			}

			// Якщо це ПОРОЖНЯ клітина (була "closed" і стала "open")
			if (content == "closed")
			{
				SetCell(currentSetting, TargetSourceId, Cells["open"]);
				Field[dictPos] = "open"; // Позначаємо як відкриту в словнику
				OpenedOnes[dictPos] = true;
				
				// Додаємо всіх 8 сусідів у чергу для перевірки
				for (int x = -1; x <= 1; x++)
				{
					for (int y = -1; y <= 1; y++)
					{
						if (x == 0 && y == 0) continue;
						toProcess.Enqueue(new Vector2I(currentSetting.X + x, currentSetting.Y + y));
					}
				}
			}
		}
	}	
		
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseClick && mouseClick.Pressed){
			// 1. Отримуємо локальну позицію миші відносно вузла
				Vector2 localPos = ToLocal(GetGlobalMousePosition());
				
				// 2. Використовуємо вбудований метод для конвертації в координати сітки
				Vector2I tilePosForSetting = LocalToMap(localPos);
				Vector2I tilePosForUser = new Vector2I(tilePosForSetting.X - offsetX, tilePosForSetting.Y - offsetY);
				
				string content = Field[tilePosForUser];
				
				
			if(mouseClick.ButtonIndex == MouseButton.Left){
					
				
				if (!Field.ContainsKey(tilePosForUser)) return;
				
				
				//DEBUG_DEBUG_DEBUG_DEBUG_DEBUG_DEBUG_DEBUG_DEBUG
				//GD.Print($"click onto tile: {tilePosForUser}");
				//GD.Print($"on this tile is: {Field[tilePosForUser]}");
				
				
				if(Field[tilePosForUser] == "closed"){
					OpenArea(tilePosForSetting);
				}
				
				
				
				if (content == "bomb")
				{
					SetCell(tilePosForSetting, TargetSourceId, Cells["bomb"]);
					GD.Print("Kaboom!");
					OpenedOnes[tilePosForUser] = true;
				}
				else 
				{
					if (new[] {"1","2","3","4","5","6","7","8"}.Contains(content))
					{
						SetCell(tilePosForSetting, TargetSourceId, Cells[content]);
						OpenedOnes[tilePosForUser] = true;
					}
					else
					{
						SetCell(tilePosForSetting, TargetSourceId, Cells["open"]);
						OpenedOnes[tilePosForUser] = true;
					}
				}
			}
			
			else if(mouseClick.ButtonIndex == MouseButton.Right){
				if(!FlaggedOnes[tilePosForUser] && !OpenedOnes[tilePosForUser]){
					SetCell(tilePosForSetting, TargetSourceId, Cells["flag"]);
					FlaggedOnes[tilePosForUser] = true;
				}
				else if(FlaggedOnes[tilePosForUser] && !OpenedOnes[tilePosForUser]){
					SetCell(tilePosForSetting, TargetSourceId, Cells["closed"]);
					FlaggedOnes[tilePosForUser] = false;
				}
			}
			
		}
			
			
		}
	
	}
