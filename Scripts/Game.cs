using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class Game : Node2D
{
	public Random rand; //счетчик сл. чисел
	public PackedScene tile; //игровая клетка
	public Node2D[,] grid; //сетка для отрисовки
	public int[,] values; //сетка для просчета
	public const int size = 4; //размер игры
	public int score; //счет игры
	public bool winGame; //выиграли игру
	
	//UI телефона
	public bool swiping = false;
	public float length = 50.0f;
	public Vector2 startPos;
	public Vector2 endPos;
	public double threshold = 10;
	
	//сброс игры
	public void reset(){
		score = 0;
		winGame = false;
		((Label)GetNode("Score")).Text = "Score: " + score;
		values = new int[size, size];
		Node2D node = (Node2D)GetNode("GameOver");
		node.Position = new Vector2(node.Position.X, 0);
		node = (Node2D)GetNode("WinGame");
		node.Position = new Vector2(node.Position.X, 0);
		genNumber();
		genNumber();
		updGrid();
	}
	
	//начальная инициализация
	public override void _Ready()
	{
		score = 0;
		winGame = false;
		((Label)GetNode("Score")).Text = "Score: " + score;
		rand = new Random();
		tile = ResourceLoader.Load("res://Scenes/tile.tscn") as PackedScene;
		grid = new Node2D[size, size];
		values = new int[size, size];
		for(int i = 0; i < size; i++){
			for(int j = 0; j < size; j++){
				var t = (Node2D)tile.Instantiate();
				t.Position = new Vector2(j * 142 + j + 1, i * 142 + i + 1 + 288);
				grid[i, j] = t;
				AddChild(t);
			}
		}
		genNumber();
		genNumber();
		updGrid();
	}
	
	//ввод с клавиатуры
	public override void _Input(InputEvent @event)
	{
		if(@event is InputEventKey keyEvent && keyEvent.Pressed && !winGame){
			GD.Print(keyEvent.Keycode);
			
			//копия
			var copy = new int[size, size];
			for(int i = 0; i < size; i++){
				for(int j = 0; j < size; j++){
					copy[i, j] = values[i, j];
				}
			}
			switch(keyEvent.Keycode){
				case Key.Left:{
					move(-1,0);
					break;
				}
				case Key.Right:{
					move(1,0);
					break;
				}
				case Key.Up:{
					move(0,-1);
					break;
				}
				case Key.Down:{
					move(0,1);
					break;
				}
			}
			
			//смотрим, что поменялось
			bool flag = true;
			for(int i = 0; i < size && flag; i++){
				for(int j = 0; j < size && flag; j++){
					if(copy[i, j] != values[i, j]){
						flag = false;
					}
				}
			}
			
			//если что-то поменялось
			if(!flag){
				genNumber();
				updGrid();
			}
		}
	}
	
	//вызывается каждый кадр игры
	public override void _Process(double delta){
		
		//выиграли игру
		if(winGame){
			return;
		}
		
		//в первый раз нажали
		if(Input.IsActionJustPressed("press")){
			if(!swiping){
				swiping = true;
				startPos = GetGlobalMousePosition();
			}
		}
		
		//на удержании
		if(Input.IsActionPressed("press")){
			if(swiping){
				Vector2 curPos = GetGlobalMousePosition();
				if(startPos.DistanceTo(curPos) >= length){
					
					//копия
					var copy = new int[size, size];
					for(int i = 0; i < size; i++){
						for(int j = 0; j < size; j++){
							copy[i, j] = values[i, j];
						}
					}
					
					//по горизонтали
					if(Math.Abs(startPos.Y - curPos.Y) <= threshold){
						if(curPos.X > startPos.X){
							move(1, 0);
							GD.Print("Horizontal Right");
						}
						else{
							move(-1, 0);
							GD.Print("Horizontal Left");
						}
						swiping = false;
					}
					
					//по вертикали
					else if(Math.Abs(startPos.X - curPos.X) <= threshold){
						if(curPos.Y > startPos.Y){
							move(0, 1);
							GD.Print("Vertical Down");
						}
						else{
							move(0, -1);
							GD.Print("Vertical Up");
						}
						swiping = false;
					}
					
					//смотрим, что поменялось
					bool flag = true;
					for(int i = 0; i < size && flag; i++){
						for(int j = 0; j < size && flag; j++){
							if(copy[i, j] != values[i, j]){
								flag = false;
							}
						}
					}
					
					//если что-то поменялось
					if(!flag){
						genNumber();
						updGrid();
					}
				}
			}
			else{
				swiping = false;
			}
		}
	}
	
	//сдвигаем ячейки в заданную сторону
	public void move(int dx, int dy){
		
		//вниз
		if(dy == 1){
			for(int i = size - 1; i >= 0; i--){
				
				//вправо
				if(dx == 1){
					for(int j = size - 1; j >= 0; j--){
						if(values[i, j] > 0){
							moveCell(i, j, dx, dy);
						}
					}
				}
				else{
					//влево
					for(int j = 0; j < size; j++){
						if(values[i, j] > 0){
							moveCell(i, j, dx, dy);
						}
					}
				}
			}
		}
		else{
			//вверх
			for(int i = 0; i < size; i++){
				
				//вправо
				if(dx == 1){
					for(int j = size - 1; j >= 0; j--){
						if(values[i, j] > 0){
							moveCell(i, j, dx, dy);
						}
					}
				}
				else{
					//влево
					for(int j = 0; j < size; j++){
						if(values[i, j] > 0){
							moveCell(i, j, dx, dy);
						}
					}
				}
			}
		}
	}
	
	//двигаем клетку
	public void moveCell(int i, int j, int dx, int dy){
		int px = j;
		int py = i;
		int nx = j + dx;
		int ny = i + dy;
		bool flag = false;
		while(nx >= 0 && nx < size && ny >= 0 && ny < size){
			
			//если числа совпадают
			if(values[ny, nx] > 0 && values[py, px] == values[ny, nx] && !flag){
				values[py, px] *= 2;
				values[ny, nx] = 0;
				flag = true; //только один раз
				
				//учитываем очки
				score += values[py, px];
				((Label)GetNode("Score")).Text = "Score: " + score;
				
				//если набрали 2048
				if(values[py, px] == 2048){
					winGame = true;
					Node2D node = (Node2D)GetNode("WinGame");
					node.Position = new Vector2(node.Position.X, 864);
				}
			}
			
			//числа разные
			else if(values[ny, nx] > 0 && values[py, px] != values[ny, nx]){
				break;
			}
			
			//обычный ход
			var t = values[py, px];
			values[py, px] = values[ny, nx];
			values[ny, nx] = t;
			px = nx;
			py = ny;
			nx += dx;
			ny += dy;
		}
	}
	
	//обновляем игровое поле
	public void updGrid(){
		for(int i = 0; i < size; i++){
			for(int j = 0; j < size; j++){
				setValue(grid[i, j], values[i, j]);
				if(values[i, j] > 0){
					setColor(grid[i, j], 0.8f, 0.8f, 0.8f);
				}
				else{
					setColor(grid[i, j], 0.58f, 0.58f, 0.58f);
				}
			}
		}
	}
	
	//случайная генерация блока
	public void genNumber(){
		
		//отбираем свободные клетки
		List<Vector2> freeCells = new List<Vector2>();
		for(int i = 0; i < size; i++){
			for(int j = 0; j < size; j++){
				if(values[i, j] == 0){
					freeCells.Add(new Vector2(j, i));
				}
			}
		}
		
		//случайным образом получаем позицию на игровом поле
		int idx = rand.Next(0, freeCells.Count);
		values[(int)freeCells[idx].Y, (int)freeCells[idx].X] = 2;
		
		//если одна клетка осталась
		if(checkGameOver()){
			Node2D node = (Node2D)GetNode("GameOver");
			node.Position = new Vector2(node.Position.X, 864);
		}
	}
	
	//проверяем проигрыш
	public bool checkGameOver(){
		for(int i = 0; i < size; i++){
			for(int j = 0; j < size; j++){
				int v = values[i, j];
				
				//если пустая клетка
				if(v == 0){
					return false;
				}
				
				//по 4-м направлениям
				List<Vector2> dirs = new List<Vector2>(){
					new Vector2(-1, 0),
					new Vector2(1, 0),
					new Vector2(0, 1),
					new Vector2(0, -1)
				};
				for(int k = 0; k < dirs.Count; k++){
					int x = j + (int)dirs[k].X;
					int y = i + (int)dirs[k].Y;
					
					//клетка валидная
					if(x >= 0 && x < size && y >= 0 && y < size){
						if(v == values[y, x]){
							return false;
						}
					}
				}
			}
		}
		return true;
	}
	
	//получить значение ячейки
	public int getValue(Node2D cell){
		var lb = (Label)(cell.GetChildren()[0].GetChildren()[0]);
		return int.Parse(lb.Text);
	}
	
	//заменить значение ячейки
	public void setValue(Node2D cell, int value){
		if(value > 0){
			((Label)(cell.GetChildren()[0].GetChildren()[0])).Text = value + "";
		}
		else{
			((Label)(cell.GetChildren()[0].GetChildren()[0])).Text = "";
		}
	}
	
	//заменить цвет ячейки
	public void setColor(Node2D cell, float r, float g, float b){
		((ColorRect)cell.GetChildren()[0]).Color = new Color(r,g,b,1.0f);
	}
	
	//сброс игры
	public void _on_area_2d_input_event_reset(Node viewport, InputEvent @event, long shape_idx)
	{
		if (@event is InputEventMouseButton btn && (int)btn.ButtonIndex == 1 && @event.IsPressed()) {
			reset();
		}
	}
}
