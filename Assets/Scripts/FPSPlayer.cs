using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSPlayer : MonoBehaviour
{
    public float MovementSpeed = 5f;
    public float TurnSpeed = 3f;

    private RaycastRenderer renderer;
    public World2D World
    {
        get { return renderer.World; }
        set { renderer.World = value; }
    }

    public double posX = 22, posY = 12;  //x and y start position
    public double dirX = -1, dirY = 0; //initial direction vector
    public double planeX = 0, planeY = 0.66; //the 2d raycaster version of camera plane

    // Use this for initialization
    void Start ()
	{
	    renderer = GetComponent<RaycastRenderer>();
	    posX = renderer.posX;
	    posY = renderer.posY;
	    dirX = renderer.dirX;
	    dirY = renderer.dirY;
	    planeX = renderer.planeX;
	    planeY = renderer.planeY;
	}
	
	// Update is called once per frame
	void Update () {
		HandleInput();
	    UpdateRendererPosition();

	}

    void HandleInput()
    {
        double moveSpeed = MovementSpeed * Time.deltaTime;
        double rotSpeed = TurnSpeed * Time.deltaTime;

        if (Input.GetAxisRaw("Vertical") > 0) //up
        {
            if (World.worldMap[(int)(posX + dirX * moveSpeed), (int)(posY)] == 0) posX += dirX * moveSpeed;
            if (World.worldMap[(int)(posX), (int)(posY + dirY * moveSpeed)] == 0) posY += dirY * moveSpeed;
        }
        //move backwards if no wall behind you
        if (Input.GetAxisRaw("Vertical") < 0) //down
        {
            if (World.worldMap[(int)(posX - dirX * moveSpeed), (int)(posY)] == 0) posX -= dirX * moveSpeed;
            if (World.worldMap[(int)(posX), (int)(posY - dirY * moveSpeed)] == 0) posY -= dirY * moveSpeed;
        }
        //rotate to the right
        if (Input.GetAxisRaw("Horizontal") > 0) //right
        {
            //both camera direction and camera plane must be rotated
            double oldDirX = dirX;
            dirX = dirX * Math.Cos(-rotSpeed) - dirY * Math.Sin(-rotSpeed);
            dirY = oldDirX * Math.Sin(-rotSpeed) + dirY * Math.Cos(-rotSpeed);
            double oldPlaneX = planeX;
            planeX = planeX * Math.Cos(-rotSpeed) - planeY * Math.Sin(-rotSpeed);
            planeY = oldPlaneX * Math.Sin(-rotSpeed) + planeY * Math.Cos(-rotSpeed);
        }
        //rotate to the left
        if (Input.GetAxisRaw("Horizontal") < 0) //left
        {
            //both camera direction and camera plane must be rotated
            double oldDirX = dirX;
            dirX = dirX * Math.Cos(rotSpeed) - dirY * Math.Sin(rotSpeed);
            dirY = oldDirX * Math.Sin(rotSpeed) + dirY * Math.Cos(rotSpeed);
            double oldPlaneX = planeX;
            planeX = planeX * Math.Cos(rotSpeed) - planeY * Math.Sin(rotSpeed);
            planeY = oldPlaneX * Math.Sin(rotSpeed) + planeY * Math.Cos(rotSpeed);
        }
    }

    void UpdateRendererPosition()
    {
        renderer.posX = posX;
        renderer.posY = posY;
        renderer.dirX = dirX;
        renderer.dirY = dirY;
        renderer.planeX = planeX;
        renderer.planeY = planeY;
    }
}
