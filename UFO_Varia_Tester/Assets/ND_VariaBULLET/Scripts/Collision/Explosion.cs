#region Script Synopsis
    //Monobehavior attached to explosion prefabs in order to animate and then destroy/repool once the animation completes.
    //Learn more about explosions at: https://neondagger.com/variabullet2d-system-guide/#explosions
#endregion

using UnityEngine;

namespace ND_VariaBULLET
{
    public class Explosion : MonoBehaviour, IRePoolable
    {
        public Sprite[] Frames;
        public int FrameSkip;       
        public bool FlickerSkipFrames;

        public AudioClip SoundFX;
        public bool PreventRePool;

        private BasicAnimation anim;
        private Vector2 scale;

        void Awake()
        {
            var rend = GetComponent<SpriteRenderer>();
            anim = new BasicAnimation(ref rend, ref Frames);
            anim.flickerSkipFrames = FlickerSkipFrames;

            scale = transform.localScale;
        }

        void OnEnable()
        {
            if (anim != null)
                anim.Reset(0);         
        }

        void Update()
        {
            if (!anim.AnimateOnce(FrameSkip)) return;
            if (PreventRePool) { Destroy(this.gameObject); return; }

            RePool(GlobalShotManager.Instance);
        }

        public void RePool(IPooler poolingScript)
        {
            transform.localScale = scale;
            poolingScript.AddToPool(this.gameObject, GlobalShotManager.Instance.transform);          
        }
    }
}