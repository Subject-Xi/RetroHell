#region Script Synopsis
    //A monobehavior that is attached to any object that receives collisions from bullet/laser shots and instantiates explosions if set.
    //Examples: Any non-damaging solid object such as terrain, walls, platforms and so forth.
    //Learn more about the collision system at: https://neondagger.com/variabullet2d-system-guide/#collision-system
#endregion

using UnityEngine;
using System.Collections;

namespace ND_VariaBULLET
{
    public class ShotCollision : MonoBehaviour, IShotCollidable
    {
        public string[] CollisionList;
        public string LaserExplosion;
        public string BulletExplosion;
        public bool ParentExplosion = true;

        public IEnumerator OnLaserCollision(CollisionArgs sender)
        {
            if (CollisionFilter.collisionAccepted(sender.gameObject.layer, CollisionList))
            {
                CollisionFilter.setExplosion(LaserExplosion, ParentExplosion, this.transform, new Vector2(sender.point.x, sender.point.y), 0, this);
                yield return null;
            }
        }

        public IEnumerator OnCollisionEnter2D(Collision2D collision)
        {
            if (CollisionFilter.collisionAccepted(collision.gameObject.layer, CollisionList))
            {
                CollisionFilter.setExplosion(BulletExplosion, ParentExplosion, this.transform, collision.contacts[0].point, 0, this);
                yield return null;
            }
        }
    }
}