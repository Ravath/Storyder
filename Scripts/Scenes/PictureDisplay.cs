using Godot;
using System;

public partial class PictureDisplay : TextureRect
{
	public void SetNewTexture(Texture2D texture)
	{
		Texture = texture;
		QueueRedraw();
	}
}
