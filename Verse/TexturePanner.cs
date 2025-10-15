using UnityEngine;

namespace Verse;

public class TexturePanner
{
	protected Material material;

	protected int propertyID;

	protected Vector2 pan;

	public Vector2 direction;

	public float speed;

	public TexturePanner(Material material, Vector2 direction, float speed)
		: this(material, Shader.PropertyToID("_MainTex"), direction, speed)
	{
	}//IL_000c: Unknown result type (might be due to invalid IL or missing references)


	public TexturePanner(Material material, string property, Vector2 direction, float speed)
		: this(material, Shader.PropertyToID(property), direction, speed)
	{
	}//IL_0008: Unknown result type (might be due to invalid IL or missing references)


	public TexturePanner(Material material, int propertyID, Vector2 direction, float speed)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		this.material = material;
		this.propertyID = propertyID;
		this.direction = direction;
		((Vector2)(ref this.direction)).Normalize();
		this.speed = speed;
	}

	public virtual void Tick()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		pan -= direction * speed * material.GetTextureScale(propertyID).x * Find.TickManager.TickRateMultiplier;
		material.SetTextureOffset(propertyID, pan);
	}
}
