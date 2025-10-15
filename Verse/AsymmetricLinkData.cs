using UnityEngine;

namespace Verse;

public class AsymmetricLinkData
{
	public class BorderData
	{
		public Color color = Color.black;

		public Vector2 size;

		public Vector3 offset;

		private Material colorMat;

		public Material Mat
		{
			get
			{
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				if ((Object)(object)colorMat == (Object)null)
				{
					colorMat = SolidColorMaterials.SimpleSolidColorMaterial(color);
				}
				return colorMat;
			}
		}
	}

	public LinkFlags linkFlags;

	public bool linkToDoors;

	public BorderData drawDoorBorderEast;

	public BorderData drawDoorBorderWest;
}
