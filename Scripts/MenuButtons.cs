using Godot;
using System;

public partial class MenuButtons : Node2D
{
	//нажали кнопку старт
	public void _on_start_pressed()
	{
		var vb1 = GetNode("Start") as Start;
		var vb2 = GetNode("Level") as Level;
		vb1.move(new Vector2(-576,300));
		vb2.move(new Vector2(0,300));
	}
	
	//вернулись обратно в меню
	public void _on_back_pressed()
	{
		var vb1 = GetNode("Start") as Start;
		var vb2 = GetNode("Level") as Level;
		vb1.move(new Vector2(0,300));
		vb2.move(new Vector2(576,300));
	}
	
	//выход из игры
	public void _on_quit_pressed()
	{
		GetTree().Quit();
	}
	
	//загрузка игры 4x4
	private void _on_first_pressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/play.tscn");
	}
	
	//загрузка 5x5
	private void _on_second_pressed()
	{
		
	}
}
