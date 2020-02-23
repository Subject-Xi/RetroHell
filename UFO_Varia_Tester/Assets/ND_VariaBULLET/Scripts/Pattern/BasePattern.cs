#region Script Synopsis
//Base class for a SpreadPattern Controller script, mainly involved in creating emitters and triggering fire.
//Attached to the Controller gameobject to form the core behavior for the controller/emitters.
//Learn more about controllers/emitters at: http://neondagger.com/variabullet2d-quick-start-guide/#setting-emitters-controls
#endregion

using UnityEngine;
using System;
using System.Collections.Generic;

namespace ND_VariaBULLET
{
    public abstract class BasePattern : MonoBehaviour
    {
        [SerializeField]
        private AutoSetOrigin origin;

        public CommandType FireCommand;
        public KeyCode CommandKey = KeyCode.Space;

        [Range(2, 999)]
        public float AutoHoldDuration = 45;
        public bool TriggerAutoFire = false;

        public bool FreezeEdits = false;

        [Range(0, 40)]
        public int EmitterAmount = 0;
        public PrefabType DefaultEmitter;
        protected List<GameObject> Emitters;
        protected List<GameObject> EmittersCached;

        public SortLayerName sortLayer;
        private SortLayerName prevLayer;
        private static int sortOrder = 9999;

        [SerializeField]
        [Range(0.1f, 1f)]
        protected float PointScale = 0.2f;

        public PointDisplayType pointDisplay = PointDisplayType.Always;

        [Range(-80, 80)]
        public float ExitPointOffset = 1f;

        [Range(-360, 360)]
        public float ParentRotation;

        [Range(-180, 180)]
        public float Pitch;

        [Range(-360, 360)]
        public float SpreadDegrees;

        [Range(-20, 20)]
        public float SpreadRadius;
        
        public bool AutoCompRadius;

        [SerializeField]
        protected bool AutoRadiusToSprite;

        [Range(-360, 360)]
        public float CenterRotation;

        public bool autoCenter = true;

        void delayOnAwake()
        {
            TriggerAutoFire = true;
        }

        public void Awake()
        {
            setIndicatorDisplay(true);

            if (!Utilities.IsEditorMode())
            {
                if (TriggerAutoFire)
                {
                    TriggerAutoFire = false;
                    Invoke("delayOnAwake", 0.12f);
                }
            }
        }

        public virtual void LateUpdate()
        {
            linkEmittersAtLaunch();
            setEmitters();          
            setOriginPoint();
            setIndicatorDisplay(false);
            checkSortLayerChanged();
        }

        public void setEmitters()
        {
            if (Emitters != null && Emitters.Count == EmitterAmount)
                return;

            Action addNewEmitter = () => {
                GameObject p = Resources.Load<GameObject>("ND_VariaBullet/EmitterPrefabs/" + DefaultEmitter.ToString() + "/Emitter");
                GameObject newEmitter = Instantiate(p);
                newEmitter.transform.parent = this.transform;
                newEmitter.transform.localPosition = new Vector2(0, 0);
                newEmitter.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingLayerName = this.sortLayer.ToString();
                newEmitter.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = sortOrder += 10;
                Emitters.Add(newEmitter);
            };
            Action<string, bool, GameObject, List<GameObject>, List<GameObject>> manageCache = (name, isActive, emit, emitList1, emitList2) => {
                emit.name = name;
                emit.SetActive(isActive);
                emitList1.Add(emit);
                emitList2.Remove(emit);
            };

            if (Emitters == null)
            {
                Emitters = new List<GameObject>();
                EmittersCached = new List<GameObject>();

                for (int i = 0; i < EmitterAmount; i++)
                    addNewEmitter();

                return;
            }

            int difference;
            int totalEmitters = Emitters.Count + EmittersCached.Count;

            if (EmitterAmount < Emitters.Count)
            {
                difference = Emitters.Count - EmitterAmount;

                for (int i = 0; i < difference; i++)
                {
                    GameObject lastEmitter = Emitters[Emitters.Count - 1];
                    manageCache("Emitter(Cached)", false, lastEmitter, EmittersCached, Emitters);
                }
            }
            else if (EmitterAmount > Emitters.Count)
            {
                difference = EmitterAmount - totalEmitters;

                if (EmitterAmount > totalEmitters)
                {                                        
                    foreach (GameObject emitterStored in EmittersCached.ToArray()) //ToArray fix to ensure collection items aren't removed as it's being iterated over.
                        manageCache("Emitter(Clone)", true, emitterStored, Emitters, EmittersCached);

                    for (int i = 0; i < difference; i++)
                        addNewEmitter();
                }
                else
                {
                    difference = EmitterAmount - Emitters.Count;

                    for (int i = 0; i < difference; i++)
                    {
                        GameObject lastEmitter = EmittersCached[EmittersCached.Count - 1];
                        manageCache("Emitter(Clone)", true, lastEmitter, Emitters, EmittersCached);
                    }
                }
            }
        }

        private void linkEmittersAtLaunch() //for re-establishing emitter list on project startup
        {
            if (Emitters != null)
                return;

            if (transform.childCount > 0)
            {
                Emitters = new List<GameObject>();
                EmittersCached = new List<GameObject>();

                foreach (Transform child in this.transform)
                {
                    if (child.parent == this.transform)
                    {
                        if (child.gameObject.activeSelf)
                            Emitters.Add(child.gameObject);
                        else
                            EmittersCached.Add(child.gameObject);
                    }
                }
            }
        }

        private void setOriginPoint()
        {
            if (origin == AutoSetOrigin.Manual)
                return;

            Transform originPoint = transform.parent;
            Transform mainSource = originPoint.parent;

            if (mainSource == null || mainSource.GetComponent<SpriteRenderer>().sprite == null)
                return;

            float x = 0;

            if (origin == AutoSetOrigin.Tip)
                x = mainSource.GetComponent<SpriteRenderer>().sprite.bounds.center.x + mainSource.GetComponent<SpriteRenderer>().sprite.bounds.size.x / 2;
            else
                x = mainSource.GetComponent<SpriteRenderer>().sprite.bounds.center.x;

            originPoint.localPosition = new Vector2(x, 0);
        }

        private void setIndicatorDisplay(bool forceOnStart)
        {
            if (Utilities.IsEditorMode() || forceOnStart)
            {
                bool display = false;

                switch (pointDisplay)
                {
                    case PointDisplayType.Always: display = true;
                        break;
                    case PointDisplayType.Never: display = false;
                        break;
                    case PointDisplayType.EditorOnly: display = (forceOnStart) ? false : true;
                        break;
                }

                foreach (Transform child in transform)
                {
                    Transform point = child.GetChild(0);
                    if (point.name == "Point")
                        point.GetComponent<SpriteRenderer>().enabled = display;
                }
            }
        }

        private void checkSortLayerChanged()
        {
            if (sortLayer != prevLayer)
            {
                foreach (GameObject emitter in Emitters)
                    emitter.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingLayerName = this.sortLayer.ToString();
            }

            prevLayer = sortLayer;
        }

        public void clearEmitterCache()
        {
            if (EmittersCached.Count == 0) { Utilities.Warn("No cached Emitters found.", this, transform.parent.parent); return; }

            foreach (GameObject emitter in EmittersCached)
                DestroyImmediate(emitter.gameObject);

            EmittersCached = new List<GameObject>();
            sortOrder = 9999;
        }

        public void cloneFirstEmitter()
        {
            if (EmitterAmount <= 1) { Utilities.Warn("Not enough Emitters to clone to (2 or more required).", this, transform.parent.parent); return; }

            Action<FireBase> copyProcess = (FireBase src) =>
            {
                for (int i = 1; i < Emitters.Count; i++)
                {
                    FireBase targ = Emitters[i].transform.GetChild(0).gameObject.GetComponent<FireBase>();

                    targ.customIndicator = src.customIndicator;
                    targ.Shot = src.Shot;
                    targ.ShotSpeed = src.ShotSpeed;
                    targ.LocalOffset = src.LocalOffset;
                    targ.makeNodeOnly = src.makeNodeOnly;
                    targ.SpriteColor = src.SpriteColor;

                    
                    if (src.GetType() == typeof(FireBullet)) //extended copying in case of FireBullet type emitter
                    {
                        FireBullet srcXT = src as FireBullet;
                        FireBullet targXT = Emitters[i].transform.GetChild(0).gameObject.GetComponent<FireBullet>();

                        targXT.SpriteOverride = srcXT.SpriteOverride;
                        targXT.ParentToEmitter = srcXT.ParentToEmitter;
                        targXT.ShotRate = srcXT.ShotRate;
                        targXT.PauseRate = srcXT.PauseRate;
                        targXT.PauseLength = srcXT.PauseLength;
                        targXT.PoolingEnabled = srcXT.PoolingEnabled;
                        targXT.AutoPool = srcXT.AutoPool;
                        targXT.AutoPoolOverride = srcXT.AutoPoolOverride;
                    }
                }
            };

            var scriptToClone = Emitters[0].transform.GetChild(0).gameObject.GetComponent<FireBase>();

            if (DefaultEmitter == PrefabType.Bullet)
                copyProcess(scriptToClone as FireBullet);    
            else
                copyProcess(scriptToClone as FireExpanding);
        }

        public void ResetEmitters()
        {
            foreach (GameObject emitter in Emitters)
                Destroy(emitter);

            foreach (GameObject emitterCached in EmittersCached)
                Destroy(emitterCached);

            Emitters = new List<GameObject>();
            EmittersCached = new List<GameObject>();

            EmitterAmount = 0;
        }

        public void addAutomator(string type)
        {
            if (type == "Linear")
                gameObject.AddComponent<AutomateLinear>();
            else
                gameObject.AddComponent<AutomateStepped>();
        }

        public enum AutoSetOrigin
        {
            Manual,
            Tip,
            Center
        }

        public enum PatternSelect
        {
            Stack,
            Radial
        }

        public enum CommandType
        {
            ButtonPress = 0,
            ButtonPressAutoHold,
            Automatic,
            AutomaticAutoHold
        }

        public enum SortLayerName
        {
            ND_PlayerBullet,
            ND_EnemyBullet,
            ND_SelfColliding,
            ND_UserBullet1,
            ND_UserBullet2,
            ND_UserBullet3,
            ND_UserBullet4,
            ND_UserBullet5,
            ND_UserBullet6,
            ND_UserBullet7,
            ND_UserBullet8,
            Player,
            Enemy
        }

        public enum PrefabType
        {
            Bullet,
            Expanding
        }

        public enum PointDisplayType
        {
            Always,
            Never,
            EditorOnly
        }

        delegate void Action<T1, T2, T3, T4, T5>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);
    }
}