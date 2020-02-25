#region Script Synopsis
    //A non-physics type bullet which moves in a linear trajectory but varying in speed along that path, similar to a wave.
    //Action and description of fields can be found at https://neondagger.com/variabullet2d-in-depth-shot-guide/#default-shot-prefabs
#endregion

using UnityEngine;

namespace ND_VariaBULLET
{
    public class ShotSpeedWave : ShotNonPhysics, IRePoolable
    {
        [Header("Wave Settings")]
        public bool ScaleToSpeed;   
        public AnimationCurve WaveForm;

        [Range(2,20)]
        public int WaveAccent = 5;

        [Range(1,100)]
        public int Cycles = 1;
        private int cycleCounter;

        [Range(1,10)]
        public int frequency;

        private float accumulator;
        private float prevPingPong;
        private bool cycleFlag;

        private float speedOriginal;

        public override void InitialSet()
        {
            base.InitialSet();
            speedOriginal = ShotSpeed;
            cycleCounter = -1;
            accumulator = 0;
        }

        public override void Update()
        {
            movement();
            base.Update();
        }

        private void movement()
        {
            if (cycleCounter >= Cycles)
                return;

            if (ScaleToSpeed)
                accumulator += Time.deltaTime / 25 * speedOriginal * scale * frequency;
            else
                accumulator += Time.deltaTime * scale * frequency;

            float minmaxBuffer = speedOriginal / WaveAccent;
            float ceiling = speedOriginal - minmaxBuffer;
            float floor = 0.001f + minmaxBuffer;

            float pingPong = Mathf.PingPong(accumulator, 1);
            ShotSpeed = Mathf.SmoothStep(ceiling, floor, WaveForm.Evaluate(pingPong));

            if (prevPingPong < pingPong && cycleFlag == false)
                { cycleFlag = true; cycleCounter++; }
            else if (prevPingPong > pingPong)
                cycleFlag = false;

            prevPingPong = pingPong;
        }

    }
}