#region Script Synopsis
    //Linear automator, which automates controller parameters in a fluid progression.
    //Becomes attached to a controller via the attached spreadpattern (controller) script.
    //Learn more about automators at: https://neondagger.com/variabullet2d-in-depth-controller-guide/#automators
#endregion

using UnityEngine;

namespace ND_VariaBULLET
{
    public class AutomateLinear : AutomateBase
    {
        public float From;
        public float To;
        public float Speed;

        void Update()
        {
            delay.RunOnce(InitialDelay);
            if (!delay.Flag) return;

            accumulator += Time.deltaTime;
            controlLink[Destination]((method(From, To, Speed)));
        }

        protected override float SinglePass(float from, float to, float speed)
        {
            float relativeSpeed = speed / Mathf.Abs(from - to);
            return Mathf.Lerp(from, to, accumulator * relativeSpeed);
        }

        protected override float Continuous(float from, float to, float speed)
        {
            float direction = (from <= to) ? 1 : -1;
            return accumulator * speed * direction;
        }

        protected override float PingPong(float from, float to, float speed)
        {
            float relativeSpeed = speed / Mathf.Abs(from - to);
            return Mathf.Lerp(from, to, Mathf.PingPong(accumulator * relativeSpeed, 1));
        }

        protected override float Randomized(float from, float to, float speed)
        {
            return Random.Range(from, to);
        }
    }
}