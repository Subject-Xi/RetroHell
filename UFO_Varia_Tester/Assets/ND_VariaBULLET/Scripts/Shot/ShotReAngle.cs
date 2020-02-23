#region Script Synopsis
    //A bullet type shot which changes it's trajectory once, resulting in an "elbow" re-angled path.
    //Action and description of fields can be found at https://neondagger.com/variabullet2d-in-depth-shot-guide/#default-shot-prefabs
#endregion

using UnityEngine;

namespace ND_VariaBULLET
{
    public class ShotReAngle : ShotNonPhysics, IRePoolable
    {
        [Header("ReAngle Settings")]
        public bool VerticalOrientation;
        private int vertMod;
        public int Embellish;

        public bool AutoEmbellish;
        public Vector2 AutoEmbellishRange;

        [Range(1,10)]
        public int AutoEmbellishSpeed;

        public int ReAngleTime;
        private Timer reAngle = new Timer(0);
        private bool reAngleTriggered;

        public override void InitialSet()
        {
            base.InitialSet();

            reAngle.Reset();
            reAngleTriggered = false;

            vertMod = (VerticalOrientation) ? 90 : 0;
        }

        public override void Update()
        {
            base.Update();
            
            if (Emitter == null) //if weapon is switched out, emitter becomes null so can no longer can process re-angle
                return;

            if (AutoEmbellish)
                spray();

            movement();
        }

        private void movement()
        {
            reAngle.RunOnce(ReAngleTime);

            if (reAngle.Flag)
            {
                if (reAngleTriggered)
                    return;

                float angle = Emitter.transform.rotation.eulerAngles.z - vertMod;
                angle += (angle < 0) ? 360 : 0;

                float embellish = Embellish;
                embellish = (angle < 180) ? embellish * -1 : embellish;

                if (Emitter.transform.lossyScale.x < 0)
                    embellish -= 180;

                Trajectory = CalcObject.RotationToShotVector(embellish + vertMod);
                transform.rotation = Quaternion.AngleAxis(embellish + vertMod, Vector3.forward);

                reAngleTriggered = true;
            }
        }

        private void spray()
        {
            if (!AutoEmbellish)
                return;

            Embellish = (int)Mathf.Lerp(AutoEmbellishRange.x, AutoEmbellishRange.y,
                Mathf.PingPong(Time.time * AutoEmbellishSpeed * Timer.deltaCounter, 1)
            );
        }
    }
}