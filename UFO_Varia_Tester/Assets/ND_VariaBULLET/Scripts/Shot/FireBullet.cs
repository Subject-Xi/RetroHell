#region Script Synopsis
    //Subclass of FireBase for instantiating, and maintaining a pool of, bullets and is attached to an emitter point gameobject.
    //Instantiation can occur at different intervals depending on methods chosen, rate, pause and pause length counters.
    //Learn more about firing scripts at: https://neondagger.com/variabullet2d-quick-start-guide/#firing-shots
#endregion

using UnityEngine;

namespace ND_VariaBULLET
{
    public class FireBullet : FireBase, IPooler
    {
        public Sprite SpriteOverride;
        public ParentType ParentToEmitter;

        [Range(100, 1)]
        public int ShotRate;
        private Timer shotRateCounter = new Timer(0);

        [Range(1, 100)]
        public float PauseRate;
        private Timer pauseRateCounter = new Timer(0);

        [Range(1, 100)]
        public int PauseLength;
        private Timer pauseLengthCounter = new Timer(0);

        public ObjectPool Pool = new ObjectPool();

        [SerializeField]
        private bool _poolingEnabled = false;
        public bool PoolingEnabled { get { return _poolingEnabled; } set { _poolingEnabled = value; } }
        public bool AutoPool;
        public int AutoPoolOverride;

        private bool triggered = false;
        private int increment;

        public override void Start()
        {
            if (Utilities.IsEditorMode())
            {
                if (!makeNodeOnly)
                    checkShotMismatch(typeof(ShotLaser), "Bullet");

                return;
            }

            base.Start();
            initPool();
        }

        private void initPool()
        {
            if (!PoolingEnabled)
                return;

            int poolSize = 0;

            if (AutoPoolOverride > 0)
                poolSize = AutoPoolOverride;
            else if (AutoPool)
                poolSize = calcObjectPool();

            for (int i = 0; i < poolSize; i++)
            {
                var pooledObject = Instantiate(Shot) as GameObject;
                AddToPool(pooledObject, this.transform);
            }
        }

        private int calcObjectPool()
        {
            float maxBulletSpeed = speedLimit;

            float calc1 = (maxBulletSpeed - ShotSpeed) / ShotRate * 3;
            float calc2 = (float)PauseLength / maxBulletSpeed / 2;
            float calc3 = calc1 - calc1 * calc2;
            return (int)(calc3 * PauseRate / maxBulletSpeed);
        }

        protected override bool ButtonPress()
        {
            if (Input.GetKeyDown(controller.CommandKey)) { firstShotCounterReset(); return false; }

            return Input.GetKey(controller.CommandKey);
        }

        protected override bool ButtonPressAutoHold()
        {
            return base.ButtonPressAutoHold();
        }

        protected override bool AutoFire()
        {
            if (controller.TriggerAutoFire && triggered)
                return true;
            else if (controller.TriggerAutoFire && !triggered)
            {
                triggered = true;
                firstShotCounterReset();
                return true;
            }          
            else
            {
                triggered = false;
                firstShotCounterReset();
                return false;
            }
        }

        protected override bool AutoFireAutoHold()
        {
            return base.AutoFireAutoHold();
        }

        protected override bool AutoHoldTemplate(bool commandType)
        { 
            if (commandType && !AutoHold)
            {
                firstShotCounterReset();
                AutoHold = true;
            }

            if (AutoHold && !AutoHoldCounter.Flag)
            {
                AutoHoldCounter.Run(controller.AutoHoldDuration);
                return true;
            }
            else
            {
                AutoHold = false;
                controller.TriggerAutoFire = false;
                AutoHoldCounter.Reset();
                return false;
            }
        }

        protected override bool ShootAtCurrentInterval()
        {
            shotRateCounter.Run(ShotRate);
            pauseRateCounter.Run(PauseRate + ShotRate);

            if (PauseRate == 100)
                PauseRate = Mathf.Infinity;

            if (!pauseLengthCounter.Flag)
            {
                if (!pauseRateCounter.Flag)
                {
                    if (shotRateCounter.Flag)
                        return true;
                }
                else
                {
                    pauseLengthCounter.Run(PauseLength);
                    pauseRateCounter.ForceFlag(PauseRate + ShotRate + 1);
                }
            }
            else
            {
                shotRateCounter.Reset();
                pauseRateCounter.Reset();
                pauseLengthCounter.Reset();
            }

            return false;
        }

        private void firstShotCounterReset() //resets on initial key command, allows for immediate spawn on first trigger/buttonpress
        {       
            shotRateCounter.ForceFlag(ShotRate + 1);
            pauseRateCounter.Reset();
            pauseLengthCounter.Reset();
        }

        public override void InstantiateShot()
        {
            GameObject firedShot;
            
            if (Pool.list.Count > 0)
                firedShot = RemoveFromPool(0);
            else
                firedShot = Instantiate(Shot) as GameObject;

            //Below two lines added to fix pooled bullets retaining parent localscale, resulting in incorrect 180 flipping when re-instantiated
            firedShot.transform.parent = null;
            firedShot.transform.localScale = Shot.transform.localScale;

            ShotBase shotScript = firedShot.GetComponent<ShotBase>();
            shotScript.Emitter = this.transform;
            shotScript.ShotSpeed = this.ShotSpeed;
            shotScript.Trajectory = this.angleToPercentage();
            shotScript.ExitPoint = controller.ExitPointOffset + LocalOffset;
            shotScript.FiringScript = this;

            if (ParentToEmitter == ParentType.never)
                shotScript.ParentToEmitter = ParentType.never;
            else if (ParentToEmitter == ParentType.always)
                shotScript.ParentToEmitter = ParentType.always;
            else
                shotScript.ParentToEmitter = ParentType.whileShotHeld;

            int physicsLayer = LayerMask.NameToLayer(rend.sortingLayerName);
            firedShot.layer = physicsLayer;

            shotScript.sortLayer = rend.sortingLayerName;
            shotScript.sortOrder = ++increment + rend.sortingOrder - 9999;      
            shotScript.InitialSet();

            if (audiosrc != null)
                audiosrc.Play();

            GlobalShotManager.Instance.ActiveBullets++;
        }

        private Vector2 angleToPercentage()
        {
            int globalDirection = (transform.lossyScale.x < 0) ? -1 : 1;
            float angle = Mathf.Abs(transform.rotation.eulerAngles.z); //absolute value fixes negative value at -360

            return CalcObject.RotationToShotVector(angle) * globalDirection;
        }

        public void AddToPool(GameObject pooledObject, Transform parent)
        {
            if (PoolingEnabled) { Pool.AddToPool(pooledObject, parent); }
        }

        public GameObject RemoveFromPool(int index)
        {
            return Pool.RemoveFromPool(index);
        }
    }

    public enum ParentType
    {
        never,
        always,
        whileShotHeld
    }
}