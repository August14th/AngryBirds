
using UnityEngine;

public class GreenBird : Bird
{
    protected override void CastSkill()
    {
        var speed = RigidBody.velocity;
        var newSpeed = new Vector2(-speed.x, speed.y);
        
        SetSpeed(newSpeed);
    }
}