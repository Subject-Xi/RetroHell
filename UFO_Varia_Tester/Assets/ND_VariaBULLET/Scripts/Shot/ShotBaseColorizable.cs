#region Script Synopsis
    //A bullet subclass. Allows for adding color and opacity animations.
#endregion

using UnityEngine;

namespace ND_VariaBULLET
{
    public class ShotBaseColorizable : ShotBaseRotatable
    {
        private Timer DissipateTimer = new Timer(0);

        [Header("Colorize Settings")]
        [Range(0, 100)]
        public int DissipateDelay;

        [Range(0, 200)]
        public int DissipateSpeed;
        private float dissipateAccumulator;

        public Color[] ColorShift;
        private int shiftIndex;
        private int shiftDir = 1;

        [Range(0, 100)]
        public int colorShiftSpeed;
        public bool randomStartColor;
        private float shiftAccumulator;

        [Range(0.1f, 1f)]
        public float Opacity = 1;

        private bool staticColor;
        private bool staticOpacity;

        public override void InitialSet()
        {
            base.InitialSet();

            if (ColorShift.Length >= 2)
            {
                rend.color = ColorShift[0];
                shiftAccumulator = 0;
                shiftIndex = (randomStartColor) ? (int)Random.Range(0, ColorShift.Length - 1) : 0;
            }
            else
                staticColor = true;

            if (DissipateSpeed > 0)
            {
                DissipateTimer.Reset();
                dissipateAccumulator = 0;
            }
            else
                staticOpacity = true;

            rend.color = new Color(rend.color.r,rend.color.g, rend.color.b, Opacity); //to ensure opacity is set on 1st frame
        }

        public override void Update()
        {
            base.Update();

            Color newColor = colorLerp();
            newColor.a = opacityLerp();
            rend.color = newColor;
        }

        private Color colorLerp()
        {
            float r;
            float g;
            float b;

            if (!staticColor)
            {
                if (shiftIndex == 0)
                    shiftDir = 1;
                else if (shiftIndex == ColorShift.Length - 1)
                    shiftDir = -1;

                shiftAccumulator += Time.deltaTime / 8;
                r = Mathf.Lerp(ColorShift[shiftIndex].r, ColorShift[shiftIndex + shiftDir].r, shiftAccumulator * colorShiftSpeed);
                g = Mathf.Lerp(ColorShift[shiftIndex].g, ColorShift[shiftIndex + shiftDir].g, shiftAccumulator * colorShiftSpeed);
                b = Mathf.Lerp(ColorShift[shiftIndex].b, ColorShift[shiftIndex + shiftDir].b, shiftAccumulator * colorShiftSpeed);

                if (shiftAccumulator * colorShiftSpeed >= 1)
                {
                    shiftAccumulator = 0;
                    shiftIndex += shiftDir;
                }
            }
            else
            {
                r = rend.color.r;
                g = rend.color.g;
                b = rend.color.b;
            }

            return new Color(r, g, b);
        }

        private float opacityLerp()
        {
            if (staticOpacity)
                return Opacity;

            DissipateTimer.RunOnce(DissipateDelay);

            float a = Opacity;
            if (DissipateTimer.Flag)
            {
                dissipateAccumulator += Time.deltaTime / 8;
                a = Mathf.Lerp(Opacity, 0, dissipateAccumulator * DissipateSpeed);

                if (a <= 0.01f)
                    RePoolOrDestroy();
            }
            return a;
        }
    }
}