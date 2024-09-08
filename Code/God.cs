using Godot;
using System;
using System.Collections.Generic;

public partial class God : Node
{
	public static God yaco { get; set; }
	public EstadoGame estado = EstadoGame.Menu;

    public override void _Ready()
	{
        yaco = this;
    }
}
public enum EstadoGame
{
	Menu, Jugando1,Jugando2, Pausa,Victoria,Perdida
}