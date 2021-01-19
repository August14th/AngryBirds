
public class YellowBird : Bird
{
	protected override void CastSkill()
	{
		Speed = RigidBody.velocity * 2;
	}
}