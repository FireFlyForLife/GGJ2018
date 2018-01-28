using System;
using System.Collections.Generic;
using Shapes;
using UnityEngine;
using UnityEngine.UI;

public class FPSPlayer : RaycastEntity
{
    [SerializeField]
    GameObject winScreen;
    private const int maxScore = 30;
    private int m_score;

    public int Score
    {
        get { return m_score; }
        set
        {
            m_score = value;
            scoreText.text = "Score: " + m_score;
            if (m_score >= maxScore)
            {
                GameObject.FindObjectOfType<FPSGameMode>().IsOpened = false;
                winScreen.SetActive(true);
            }

        }
    }
    public Image SplashEffect;
    public int PlayerNumber = 0;
    [SerializeField]
    public Image DistIndicator;
    [SerializeField]
    private Text scoreText;
    public float MovementSpeed = 5f;
    public float TurnSpeed = 3f;

    public float ShotRange = 10f;
    private float lastFireTime = float.MinValue;
    public float SplashDuration = 0.2f;
    public int DamagePerShot = 50;
    public AudioClip[] ShotSounds;

    private int health = 100;
    public float HitShowTime = 0.3f;
    private float lastHitTime = float.MinValue;
    private List<Vector2> spawnPos = new List<Vector2>();
    private int spawnIndex = 0;
    public List<Vector2> SpawnPos { get { return spawnPos; } set { spawnPos = value; } }

    public Image KeyIcon;

    //Animation
    public int[] SpriteIds;
    private float lastUpdate = 0;
    public float FrameUpdateDelay = 0.25f;
    [SerializeField]
    private int textureIndex = 0;

    private AudioSource audioSource;

    public GameObject SplashHitObject;
    public RaycastEntity pickup;

    public void Hit(int dmg, FPSPlayer player)
    {
        int v = Health - dmg;
        if (v <= 0)
            player.Score += 1;
        Health = v;
    }

    public int Health
    {
        get { return health; }
        set
        {
            health = value;
            lastHitTime = Time.time;

            if (Health <= 0)
            {
                if (pickup != null)
                {
                    pickup.Position = Position;
                    pickup.enabled = true;
                    this.pickup = null;
                }
                var prefab = Resources.Load<GameObject>("ExplosionPrefab");
                GameObject explosionObject = GameObject.Instantiate(prefab, transform.position, Quaternion.identity);
                var explosion = explosionObject.GetComponent<ParticleEntity>();
                explosion.World = World;

                // never spawn at the same position twice
                spawnIndex = ++spawnIndex % spawnPos.Count;
                SetPosition(spawnPos[spawnIndex]);
                health = 100;
            }
        }
    }

    public bool IsAlive
    {
        get { return health > 0; }
    }

    public Color GetColor()
    {
        if (lastHitTime + HitShowTime > Time.time)
            return Color.red;

        return Color.white;

        //if (PlayerNumber < 2)
        //    return Color.blue;
        //else
        //    return Color.green;

    }

    public RaycastRenderer Renderer;
    public World2D World
    {
        get { return Renderer.World; }
        set { Renderer.World = value; }
    }

    public Vector2 DirVec { get { return new Vector2(dirX, dirY); } }
    //private double posX = 22, posY = 12;  //x and y start position
    private float dirX = -1, dirY = 0; //initial direction vector
    private float planeX = 0, planeY = 0.66f; //the 2d raycaster version of camera plane

    private new Collider2D collider2D;

    public void SetPosition(Vector2 v)
    {
        Renderer.posX = v.x;
        Renderer.posY = v.y;
        X = v.x;
        Y = v.y;
    }

    // Use this for initialization
    void Start()
    {
        Score = 0;
        DistIndicator.GetComponent<ObjectIndicator>().Initialize(this, DistIndicator, World);
        collider2D = GetComponent<Collider2D>();
        //X = Renderer.posX;
        //Y = Renderer.posY;
        dirX = Renderer.dirX;
        dirY = Renderer.dirY;
        planeX = Renderer.planeX;
        planeY = Renderer.planeY;

        TextureId = 0;
        World.Entities.Add(this);

        IsPlayer = true;

        audioSource = gameObject.AddComponent<AudioSource>();

        TextureId = PlayerNumber;
    }


    private PointSystem m_pointsystem = new PointSystem();
    public PointSystem Pointsystem { get { return m_pointsystem; } }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        UpdateRendererPosition();
        UpdateEffects();
    }

    private void UpdateEffects()
    {
        bool b = lastFireTime + SplashDuration > Time.time;
        SplashEffect.enabled = b;

        if (SplashHitObject)
        {
            foreach (Transform childTransform in SplashHitObject.transform)
            {
                var img = childTransform.GetComponent<Image>();
                if (img)
                {
                    const float hitDeteriation = 1.5f;
                    var c = img.color;
                    c.a = Math.Max(0, lastHitTime + hitDeteriation - Time.time);
                    img.color = c;
                }
            }
        }

        if (SpriteIds.Length > 0)
        {
            if (lastUpdate + FrameUpdateDelay < Time.time)
            {
                lastUpdate = Time.time;
                if (textureIndex == SpriteIds.Length - 1)
                    textureIndex = 0;
                else
                    ++textureIndex;

                TextureId = SpriteIds[textureIndex];
            }
        }

        KeyIcon.enabled = pickup != null;
    }


    void HandleInput()
    {
        float moveSpeed = MovementSpeed * Time.deltaTime;
        float rotSpeed = TurnSpeed * Time.deltaTime;

        float vert = Input.GetAxisRaw("Player" + PlayerNumber + "_Vertical");
        float hori = Input.GetAxisRaw("Player" + PlayerNumber + "_Horizontal");

        if (vert > 0) //up
        {
            if (World.worldMap[(int)(Y), (int)(X + dirX * moveSpeed)] <= 0) X += dirX * moveSpeed;
            if (World.worldMap[(int)(Y + dirY * moveSpeed), (int)(X)] <= 0) Y += dirY * moveSpeed;
        }
        //move backwards if no wall behind you
        if (vert < 0) //down
        {
            if (World.worldMap[(int)(Y), (int)(X - dirX * moveSpeed)] <= 0) X -= dirX * moveSpeed;
            if (World.worldMap[(int)(Y - dirY * moveSpeed), (int)(X)] <= 0) Y -= dirY * moveSpeed;
        }
        //rotate to the right
        if (hori > 0) //right
        {
            //both camera direction and camera plane must be rotated
            float oldDirX = dirX;
            dirX = dirX * Mathf.Cos(-rotSpeed) - dirY * Mathf.Sin(-rotSpeed);
            dirY = oldDirX * Mathf.Sin(-rotSpeed) + dirY * Mathf.Cos(-rotSpeed);
            float oldPlaneX = planeX;
            planeX = planeX * Mathf.Cos(-rotSpeed) - planeY * Mathf.Sin(-rotSpeed);
            planeY = oldPlaneX * Mathf.Sin(-rotSpeed) + planeY * Mathf.Cos(-rotSpeed);
        }
        //rotate to the left
        if (hori < 0) //left
        {
            //both camera direction and camera plane must be rotated
            float oldDirX = dirX;
            dirX = dirX * Mathf.Cos(rotSpeed) - dirY * Mathf.Sin(rotSpeed);
            dirY = oldDirX * Mathf.Sin(rotSpeed) + dirY * Mathf.Cos(rotSpeed);
            float oldPlaneX = planeX;
            planeX = planeX * Mathf.Cos(rotSpeed) - planeY * Mathf.Sin(rotSpeed);
            planeY = oldPlaneX * Mathf.Sin(rotSpeed) + planeY * Mathf.Cos(rotSpeed);
        }


        if (Input.GetButtonDown("Player" + PlayerNumber + "_Fire"))
        {
            lastFireTime = Time.time;
            int clipnum = UnityEngine.Random.Range(0, ShotSounds.Length - 1);
            audioSource.PlayOneShot(ShotSounds[clipnum]);

            Vector2 start = new Vector2(X, Y);
            LineSegment line = new LineSegment(start, start + new Vector2(dirX, dirY) * ShotRange);

            Vector2 dir = new Vector2(dirX, dirY) * ShotRange;
            ContactFilter2D filter = new ContactFilter2D();
            RaycastHit2D[] hits = new RaycastHit2D[20];


            
            Vector2 hitVector = new Vector2();
            if (GridSystem.IntersectsElement(start, dir, World, Renderer.Rect.rect.width, out hitVector) == true)
            {

                int amount = Physics2D.Raycast(line.Start, dir, filter, hits, (hitVector - line.Start).magnitude);
                if (amount > 0)
                {


                    for (var i = 0; i < amount; i++)
                    {
                        var hit = hits[i];
                        if (hit.collider == collider2D)
                            continue;

                        m_pointsystem.ShotPlayer();
                        //-----------------------------------------Add Points shooting------------------------------//
                        Debug.Log("Shot someone!!!");
                        var player = hit.collider.GetComponent<FPSPlayer>();
                        if (player)
                        {
                            //player.Health -= DamagePerShot;
                            player.Hit(DamagePerShot, this);
                            player.Pointsystem.GotShot();
                        }
                    }
                }
                else
                    return;
            }
        }
    }

    void UpdateRendererPosition()
    {
        Renderer.posX = X;
        Renderer.posY = Y;
        Renderer.dirX = dirX;
        Renderer.dirY = dirY;
        Renderer.planeX = planeX;
        Renderer.planeY = planeY;
        if (pickup != null)
        {
            pickup.Position = new Vector2(X, Y);
        }
    }
}
