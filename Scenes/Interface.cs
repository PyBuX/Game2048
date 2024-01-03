using Godot;
using System;

public partial class Interface : Node2D
{
	//переход обратно в меню
	public void _on_area_2d_input_event_menu(Node viewport, InputEvent @event, long shape_idx)
	{
		if (@event is InputEventMouseButton btn && (int)btn.ButtonIndex == 1 && @event.IsPressed()) {
			GetTree().ChangeSceneToFile("res://Scenes/game.tscn");
		}
	}
}
