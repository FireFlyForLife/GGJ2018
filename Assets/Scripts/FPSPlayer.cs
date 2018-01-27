using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Shapes;
using UnityEngine;
using UnityEngineInternal;
using Collision2D = UnityEngine.Collision2D;

public class FPSPlayer : RaycastEntity
{
    public int PlayerNumber = 0;

    public float MovementSpeed = 5f;
    public float TurnSpeed = 3f;

    public float ShotRange = 10f;

    private int health = 100;
    public float HitShowTime = 0.3f;
    private float lastHitTime = float.MinValue;
    private List<Vector2> spawnPos = new List<Vector2>();
    private int spawnIndex = 0;
    public List<Vector2> SpawnPos { get { return spawnPos; } set { spawnPos = value; } }

    public int Health
    {
        get { return health; }
        set
        {
            health = value;
            lastHitTime = Time.time;

            if (Health <= 0)
            {
                // never spawn at the same position twice
                SetPosition(spawnPos[spawnIndex]);
                spawnIndex = ++spawnIndex % spawnPos.Count;
                health = 100;
            }
            //enabled = false;
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

        if (PlayerNumber < 2)
            return Color.blue;
        else
            return Color.green;
    }

    public RaycastRenderer Renderer;
    public World2D World
    {
        get { return Renderer.World; }
        set { Renderer.World = value; }
    }

    //private double posX = 22, posY = 12;  //x and y start position
    private float dirX = -1, dirY = 0; //initial direction vector
    private float planeX = 0, planeY = 0.66f; //the 2d raycaster version of camera plane

    private Collider2D collider2D;

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
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        UpdateRendererPosition();
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
            Vector2 start = new Vector2(X, Y);
            LineSegment line = new LineSegment(start, start + new Vector2(dirX, dirY) * ShotRange); //TODO: Get direction

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

                        Debug.Log("Shot someone!!!");
                        var player = hit.collider.GetComponent<FPSPlayer>();
                        if (player)
                        {
                            player.Health -= 100;
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
    }
}
