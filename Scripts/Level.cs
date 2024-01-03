using Godot;
using System;

public partial class Level : Node2D
{
	//сдвигаем на заданную позицию
	public void move(Vector2 target){
		Tween tween = CreateTween();
		tween.TweenProperty(GetNode("LevelBox"), "position", target, 1.0f);
	}
}
