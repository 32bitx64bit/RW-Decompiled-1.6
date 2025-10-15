using UnityEngine;

namespace Verse;

public struct TextureAndColor
{
	private Texture2D texture;

	private Color color;

	public bool HasValue => (Object)(object)texture != (Object)null;

	public Texture2D Texture => texture;

	public Color Color => color;

	public static TextureAndColor None => new TextureAndColor(null, Color.white);

	public TextureAndColor(Texture2D texture, Color color)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		this.texture = texture;
		this.color = color;
	}

	public static implicit operator TextureAndColor(Texture2D texture)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return new TextureAndColor(texture, Color.white);
	}
}
