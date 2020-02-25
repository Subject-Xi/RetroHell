#region Script Synopsis
    //A "Singleton pattern" MonoBehavior which is lazily instantiated whenever a shot is fired.
    //Alternatively, instantiation is forced when directly called from ForceGlobalShotManager when attached to a persistent scene gameobject.
    //Main role is in managing global settings such as target FPS, explosions and global shot speed and engine throttling.
    //Learn more about the GlobalShotManager at: https://neondagger.com/variabullet2d-system-guide/#globalshotmanager
#endregion

using UnityEngine;
using System.Collections.Generic;
using System;

namespace ND_VariaBULLET
{
    public class GlobalShotManager : MonoBehaviour, IPooler
    {
        public static bool DemoMode;
        private static GlobalShotManager _instance;
        public static GlobalShotManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    string folder = (DemoMode) ? "GlobalManager/DemoVersion" : "GlobalManager";
                    _instance = Instantiate(Resources.Load<GameObject>("ND_VariaBullet/" + folder + "/GlobalShotManager")).GetComponent<GlobalShotManager>();
                }
                
                return _instance;
            }
        }

        [Range(0, 60)] public int TestFrameRate = 0;
        public bool VSync;

        [Range(0.1f, 5)] public float SpeedScale = 1;

        public GameObject[] ExplosionPrefabs;

        public bool PoolingEnabled
        {
            get { return true; }
            set { PoolingEnabled = value; } //necessary to satisfy IPooler. Pooling can only be true in this case
        }

        [Range(1, 100)] public int PoolSize;

        public int ActiveBullets { get; set; }

        public bool EmulateCPUThrottle;
        public float ThrottlePerBullet;

        [Range(0.1f, 0.9f)] public float MaxThrottle;
        public int MaxBulletUntilThrottle;

        [Range(1, 1000)] public float OutBoundsRange = 260;

        private Dictionary<string, ObjectPool> explosionPool = new Dictionary<string, ObjectPool>();
        private Dictionary<string, AudioSource> sfxPool = new Dictionary<string, AudioSource>();

        void Awake()
        {
            foreach (GameObject explosion in ExplosionPrefabs)
            {
                explosionPool.Add(explosion.name, new ObjectPool());

                Explosion eScript = explosion.GetComponent<Explosion>();
                if (eScript.SoundFX != null)
                {
                    AudioSource soundFX = gameObject.AddComponent<AudioSource>();
                    soundFX.clip = eScript.SoundFX;
                    soundFX.playOnAwake = false;
                    sfxPool.Add(explosion.name, soundFX);
                }
            }
        }

        public void Update()
        {
            if (TestFrameRate > 0)
                setFrameRate();

            if (EmulateCPUThrottle)
                setThrottle();
        }


        private void setFrameRate()
        {
            Application.targetFrameRate = TestFrameRate;
            QualitySettings.vSyncCount = (VSync) ? 1 : 0;
        }

        private void setThrottle()
        {
            int difference = Math.Max(0, ActiveBullets - MaxBulletUntilThrottle);
            float throttle = (float) difference * ThrottlePerBullet;
            Time.timeScale = (throttle > MaxThrottle) ? Time.timeScale : 1 - throttle;
        }

        public GameObject ExplosionRequest(string name, object sender)
        {
            ObjectPool pooledExplosion;

            try
            {
                pooledExplosion = explosionPool[name];
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException("ERROR: " + sender + " requested Explosion named \"" + name +
                                               "\" was not found. Make sure the Explosion prefab name or request string is correct.");
            }

            if (pooledExplosion.Size == 0)
            {
                foreach (GameObject explosion in ExplosionPrefabs)
                {
                    if (explosion.name == name)
                    {
                        for (int i = 0; i < PoolSize; i++)
                        {
                            GameObject copy = Instantiate(explosion);
                            copy.name = name;
                            AddToPool(copy, this.transform);
                        }
                        break;
                    }
                }
            }

            if (sfxPool.ContainsKey(name))
                sfxPool[name].Play();

            return explosionPool[name].RemoveFromPool(0);
        }

        public void AddToPool(GameObject poolObject, Transform parent)
        {
            explosionPool[poolObject.name].AddToPool(poolObject, parent);
        }

        public GameObject RemoveFromPool(int index)
        {
            throw new NotImplementedException();
        }
    }
}