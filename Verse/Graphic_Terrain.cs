namespace Verse;

public class Graphic_Terrain : Graphic_Single
{
	public override string ToString()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		return $"Terrain(path={path}, shader={base.Shader}, color={color})";
	}
}
