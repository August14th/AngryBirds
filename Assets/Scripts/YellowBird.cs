
public class YellowBird : Bird
{
	protected override void CastSkill()
	{
		SetSpeed(RigidBody.velocity * 2);
	}
}