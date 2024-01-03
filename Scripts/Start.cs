using Godot;
using System;

public partial class Start : Node2D
{
	//сдвигаем на заданную позицию
	public void move(Vector2 target){
		Tween tween = CreateTween();
		tween.TweenProperty(GetNode("MenuBox"), "position", target, 1.0f);
	}
}
