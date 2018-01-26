using System;
using System.Collections;
using System.Collections.Generic;
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

    public RaycastRenderer renderer;
    public World2D World
    {
        get { return renderer.World; }
        set { renderer.World = value; }
    }

    //private double posX = 22, posY = 12;  //x and y start position
    private float dirX = -1, dirY = 0; //initial direction vector
    private float planeX = 0, planeY = 0.66f; //the 2d raycaster version of camera plane

    private Collider2D collider2D;

    // Use this for initialization
    void Start ()
	{
	    collider2D = GetComponent<Collider2D>();
	    X = renderer.posX;
	    Y = renderer.posY;
	    dirX = renderer.dirX;
	    dirY = renderer.dirY;
	    planeX = renderer.planeX;
	    planeY = renderer.planeY;

	    TextureId = 0;
	    World.Entities.Add(this);
	}
	
	// Update is called once per frame
	void Update () {
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
            if (World.worldMap[(int)(X + dirX * moveSpeed), (int)(Y)] == 0) X += dirX * moveSpeed;
            if (World.worldMap[(int)(X), (int)(Y + dirY * moveSpeed)] == 0) Y += dirY * moveSpeed;
        }
        //move backwards if no wall behind you
        if (vert < 0) //down
        {
            if (World.worldMap[(int)(X - dirX * moveSpeed), (int)(Y)] == 0) X -= dirX * moveSpeed;
            if (World.worldMap[(int)(X), (int)(Y - dirY * moveSpeed)] == 0) Y -= dirY * moveSpeed;
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
            LineSegment line = new LineSegment(start, start + new Vector2(dirX, dirY)*ShotRange); //TODO: Get direction

            Vector2 dir = new Vector2(dirX, dirY)*ShotRange;
            ContactFilter2D filter = new ContactFilter2D();
            RaycastHit2D[] hits = new RaycastHit2D[20];
            int amount = Physics2D.Raycast(line.Start, dir, filter, hits);
            if (amount > 0)
            {
                for (var i = 0; i < amount; i++)
                {
                    var hit = hits[i];
                    if (hit.collider != collider2D)
                    {
                        Debug.Log("Shot someone!!!");
                    }
                }
            }
        }
    }

    void UpdateRendererPosition()
    {
        renderer.posX = X;
        renderer.posY = Y;
        renderer.dirX = dirX;
        renderer.dirY = dirY;
        renderer.planeX = planeX;
        renderer.planeY = planeY;
    }
}
