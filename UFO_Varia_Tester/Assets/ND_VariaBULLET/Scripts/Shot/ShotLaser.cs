#region Script Synopsis
    //A standard laser type shot which instantiates beginning, middle and end segments.
    //Learn more about laser type shots at: https://neondagger.com/variabullet2d-in-depth-shot-guide/#laser-shots
#endregion

using UnityEngine;

namespace ND_VariaBULLET
{
    public class ShotLaser : ShotBase
    {
        [Range(1,30)]
        public int HitsPerSecond;

        public LaserExpression expressionType = LaserExpression.continuous;

        [Header("Animation Settings")]
        public Sprite[] OriginImg;
        public Sprite[] MainImg; 
        public Sprite[] TipImg;
        public Sprite[] BlastImg;

        private BasicAnimation originAnim;
        private BasicAnimation mainAnim;
        private BasicAnimation tipAnim;
        private BasicAnimation blastAnim;

        private GameObject originGo;
        private GameObject mainGo;
        private GameObject tipGo;

        [Range(0, 10)]
        public int FrameSkip;

        [Header("Collision Settings")]
        [Range(.1f, .9f)]
        public float CollisionThickness = 0.5f;

        [Range(0.1f, 1)]
        public float CollisionTipZone = 0.5f;

        private float ppu;
        private int maxDistance;

        private int globalDirection;
        private int releaseDirection;

        private int collidesWith;
        private bool collided;

        private float _collTiming;
        private float collTiming {
            get { _collTiming = (_collTiming > 60 / HitsPerSecond) ? 0 : _collTiming; return _collTiming; }
            set { _collTiming = value; }
        }

        public override void InitialSet()
        {
            transform.parent = Emitter;
            transform.localPosition = new Vector2(ExitPoint, 0);
            transform.rotation = Emitter.rotation;

            if (expressionType == LaserExpression.continuous)
                FiringScript.OnStoppedFiring.AddListener(destroy);
            else
                FiringScript.OnStoppedFiring.AddListener(UnParent);
        }

        private void destroy()
        {
            FiringScript.OnStoppedFiring.RemoveListener(destroy);
            Destroy(gameObject);
        }

        protected override void UnParent()
        {
            FiringScript.OnStoppedFiring.RemoveListener(UnParent);
            transform.parent = null;

            ShotLaserPacket packetScript = gameObject.AddComponent<ShotLaserPacket>();
            packetScript.ShotSpeed = this.ShotSpeed;
            packetScript.ShotDirection = this.globalDirection;
            packetScript.ReleaseDirection = this.releaseDirection;

            packetScript.FrameSkip = this.FrameSkip;
            packetScript.originAnim = this.originAnim;
            packetScript.mainAnim = this.mainAnim;
            packetScript.tipAnim = this.tipAnim;

            Destroy(this);
        }

        public override void Start()
        {
            collidesWith = Physics2D.GetLayerCollisionMask(gameObject.layer);
            globalDirection = (transform.parent.lossyScale.x < 0) ? -1 : 1; //needed to get the parent transform lossyScale to determine firing side

            if (Screen.width > Screen.height)
                maxDistance = (int)(Screen.width + Screen.width / 4f) * globalDirection;
            else
                maxDistance = (int)(Screen.height + Screen.height / 4f) * globalDirection;

            originGo = initLaserParts(new GameObject("Origin"), new Vector2(0, 0), OriginImg);
            mainGo = initLaserParts(new GameObject("Main"), new Vector2(originGo.GetComponent<SpriteRenderer>().sprite.bounds.size.x * globalDirection, 0), MainImg);
            tipGo = initLaserParts(new GameObject("Tip"), new Vector2(0, 0), TipImg);

            originAnim = initAnimations(originGo, OriginImg);
            mainAnim = initAnimations(mainGo, MainImg);          
            blastAnim = initAnimations(tipGo, BlastImg);
            tipAnim = initAnimations(tipGo, TipImg);

            ppu = mainGo.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
        }

        public override void Update()
        {
            base.Update();
            movement();
        }

        
        public void LateUpdate() //required to check hit detection before next Update runs
        {
            laserCast();
        }

        private void movement()
        {
            collided = false;

            originGo.SetActive(true);
            mainGo.SetActive(true);
            tipGo.SetActive(true);

            originAnim.Animate(FrameSkip);
            mainAnim.SyncAnimate(originAnim.FrameNum);

            releaseDirection = (transform.parent.lossyScale.x < 0) ? -1 : 1;              
        }

        //Gets the rotation angle of the gameobject, factors in lossyscale if parent is flipped and uses the returned vector to cast the ray in the appropriate direction
        private Vector2 getDirectionVector2D(float angle)
        {
            Vector2 castAngle = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
            float flip = (transform.lossyScale.x >= 0) ? 1 : -1; //factors in the global scale in case a parent has been flipped.
            return castAngle * flip;
        }

        private GameObject initLaserParts(GameObject part, Vector3 initPosition, Sprite[] sprite)
        {
            part.transform.parent = this.transform;
            part.transform.localPosition = initPosition;
            part.transform.localScale = new Vector3(part.transform.localScale.x * globalDirection, part.transform.localScale.y);
            part.transform.rotation = new Quaternion();

            SpriteRenderer partSprite = part.AddComponent<SpriteRenderer>();
            partSprite.sprite = sprite[0];
            partSprite.sortingLayerName = sortLayer;
            partSprite.sortingOrder = sortOrder;
            partSprite.color = FiringScript.SpriteColor;

            return part;
        }

        private BasicAnimation initAnimations(GameObject part, Sprite[] frames)
        {
            SpriteRenderer rend = part.GetComponent<SpriteRenderer>();
            return new BasicAnimation(ref rend, ref frames);
        }

        private void laserCast()
        {
            float speed = scaledSpeed * ppu;

            Vector2 directionToCast = getDirectionVector2D(transform.rotation.eulerAngles.z);
            RaycastHit2D ray = Physics2D.BoxCast(mainGo.transform.position, new Vector2(MainImg[0].bounds.size.x, MainImg[0].bounds.size.y * CollisionThickness), 0, directionToCast, maxDistance, collidesWith);

            if (ray.collider != null)
            {
                laserCollision(ray, speed);
            }
            else
            {
                float laserExtension = Mathf.MoveTowards(mainGo.transform.localScale.x, maxDistance, Time.deltaTime * speed);
                mainGo.transform.localScale = new Vector3(laserExtension, mainGo.transform.localScale.y, 1);

                tipAnim.SyncAnimate(originAnim.FrameNum);
            }

            tipGo.transform.localPosition = new Vector2(mainGo.transform.localScale.x / ppu + originGo.GetComponent<SpriteRenderer>().sprite.bounds.size.x * globalDirection, 0);

            if (collided)
            {
                if (expressionType == LaserExpression.packet)
                {
                    ray.collider.SendMessage("OnLaserCollision", new CollisionArgs(gameObject, ray.point, DamagePerHit), SendMessageOptions.DontRequireReceiver);
                    Destroy(gameObject);
                }
                else
                {
                    if (collTiming == 0)
                        ray.collider.SendMessage("OnLaserCollision", new CollisionArgs(gameObject, ray.point, DamagePerHit), SendMessageOptions.DontRequireReceiver);

                    collTiming += Timer.deltaCounter;
                }
            }
        }

        private void laserCollision(RaycastHit2D ray, float speed)
        {
            float distance = ray.distance;
            float blastArea = BlastImg[0].bounds.size.x * ppu * CollisionTipZone;
            
            if (mainGo.transform.localScale.x * globalDirection >= distance * ppu - blastArea)
            {                                              //Fix for left facing
                mainGo.transform.localScale = new Vector3((distance * globalDirection * ppu) - (blastArea * globalDirection), mainGo.transform.localScale.y, 1);
                blastAnim.SyncAnimate(originAnim.FrameNum);
                collided = true;
            }
            else
            {                                                                           //Fix for left facing
                float laserExtension = Mathf.MoveTowards(mainGo.transform.localScale.x, (distance * globalDirection * ppu) - (blastArea * globalDirection), Time.deltaTime * speed);
                mainGo.transform.localScale = new Vector3(laserExtension, mainGo.transform.localScale.y, 1);

                float bufferZone = 0.002f;

                if (laserExtension * globalDirection >= (distance * ppu) - blastArea - bufferZone)
                    { blastAnim.SyncAnimate(originAnim.FrameNum); collided = true; }
                else
                    tipAnim.SyncAnimate(originAnim.FrameNum);
            }
        }

        private void debugDraw(Vector2 start, Vector2 end)
        {
            Debug.DrawLine(start, end, Color.green, 2);
        }

        protected override void OnOutBounds()
        {
            //No Implementation. maxDistance determines distance while OnStoppedFiring destroys object
            return;
        }

        public enum LaserExpression
        {
            continuous,
            packet
        }
    }
}