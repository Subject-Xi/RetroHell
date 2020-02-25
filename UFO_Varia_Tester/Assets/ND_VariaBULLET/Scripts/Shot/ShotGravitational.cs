#region Script Synopsis
    //A physics based shot with a rigidbody that responds to gravity, rotating along a center-of-mass pivot point.
    //Action and description of fields can be found at https://neondagger.com/variabullet2d-in-depth-shot-guide/#default-shot-prefabs
#endregion

using UnityEngine;

namespace ND_VariaBULLET
{
    public class ShotGravitational : ShotPhysics, IRePoolable
    {
        [Header("Gravity Settings")]
        [Range(1, 30)]
        public float GravityScale = 1f;
        public Vector2 CenterMassLocation;
        public bool CenterMassPivot;
        public float PivotRotationSpeed;

        protected override void movement()
        {
            body = GetComponent<Rigidbody2D>();
            body.isKinematic = false;
            body.gravityScale = this.GravityScale;
            body.centerOfMass = GetComponent<SpriteRenderer>().sprite.pivot;

            body.AddForce(
                new Vector2(ShotSpeed * Trajectory.x, ShotSpeed * Trajectory.y),
                ForceMode2D.Impulse
            );
        }

        void FixedUpdate()
        {
            if (!CenterMassPivot) return;

            if (rotationSpeed + rotationSpeedRange > 0)
                Utilities.Warn("WARNING: Rotation Greater Than 0 May Conflict with CenterMassPivot", this.gameObject.name);

            transform.rotation = CalcObject.VectorToRotationSlerp(transform.rotation, body.velocity, PivotRotationSpeed);
        }
    }
}