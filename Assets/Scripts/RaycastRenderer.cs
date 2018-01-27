//Used a tutorial written in c++ and converted everything in c# @ http://lodev.org/cgtutor/raycasting.html

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.UI;

//[Serializable]
//public class SpriteHolder
//{
//    public double x;
//    public double y;
//    public int texture;
//}

public class RaycastRenderer : MonoBehaviour
{
    public int Width = 512;
    public int Height = 384;

    public int spriteWidth = 64;
    public int spriteHeight = 64;

    public int texWidth = 512;
    public int texHeight = 512;

    //public List<SpriteHolder> Sprites;

    public RawImage ImageComponent;

    public World2D World;

    private Texture2D texture;
    private Color32[] blankScreen;

    private Color[] buffer; // y-coordinate first because it works per scanline
    private double[] ZBuffer;

    private int[] spriteOrder;
    private double[] spriteDistance;

    public float posX = 22, posY = 12;  //x and y start position
    public float dirX = -1, dirY = 0; //initial direction vector
    public float planeX = 0, planeY = 0.66f; //the 2d raycaster version of camera plane


    public RectTransform Rect { get { return (RectTransform)transform; } }

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame(); //we need to wait a frame so the layout can update our dimensions
        Width = (int)Rect.rect.width;
        Height = (int)Rect.rect.height;

        ZBuffer = new double[Width];
        buffer = new Color[Height * Width]; // y-coordinate first because it works per scanline

        spriteOrder = new int[100];
        spriteDistance = new double[100];

        blankScreen = new Color32[Width * Height];
        for (int i = 0; i < blankScreen.Length; i++)
        {
            if ((float)i / Width > Height * 0.5)
                blankScreen[i] = new Color32(0, 208, 255, 255);
            else
                blankScreen[i] = new Color32(220, 220, 220, 255);
        }

        texture = new Texture2D(Width, Height, TextureFormat.ARGB32, false);
        ImageComponent.texture = texture;
    }


 //   int counter = 0;

    void Update()
    {


        if (!texture || !gameObject.activeInHierarchy)
            return; //Start has not been executed    
      

        RenderImage();

        //-----------------------------UI----------------------------------------///

        GameObject.Find("Indicator1").GetComponent<ObjectIndicator>().GetPositions((int)posX, (int)posY, 0);
        GameObject.Find("Indicator2").GetComponent<ObjectIndicator>().GetPositions((int)posX, (int)posY, 1);
        GameObject.Find("Indicator3").GetComponent<ObjectIndicator>().GetPositions((int)posX, (int)posY, 2);
        GameObject.Find("Indicator4").GetComponent<ObjectIndicator>().GetPositions((int)posX, (int)posY, 3);

        ClearScreen();
        //HandleInput();
    }


    void RenderImage()
    {
      
        if (posX < 0 || posY < 0)
            Debug.Log(posX + " : " + posY);
        if (World.worldMap[(int)posY, (int)posX] > 0)
            throw new Exception("Starting in a wall!!!!");

        var Sprites = World.Entities;

        for (int x = 0; x < Width; x++)
        {
            //calculate ray position and direction
            float cameraX = 2 * x / (float)Width - 1; //x-coordinate in camera space
            float rayPosX = posX;
            float rayPosY = posY;
            float rayDirX = dirX + planeX * cameraX;
            float rayDirY = dirY + planeY * cameraX;
            //which box of the map we're in
            int mapX = (int)rayPosX;
            int mapY = (int)rayPosY;

            //length of ray from current position to next x or y-side
            float sideDistX;
            float sideDistY;

            //length of ray from one x or y-side to next x or y-side
            float deltaDistX = Mathf.Sqrt(1 + (rayDirY * rayDirY) / (rayDirX * rayDirX));
            float deltaDistY = Mathf.Sqrt(1 + (rayDirX * rayDirX) / (rayDirY * rayDirY));
            float perpWallDist;

            //what direction to step in x or y-direction (either +1 or -1)
            int stepX;
            int stepY;

            int hit = 0; //was there a wall hit?
            int side = -1; //was a NS or a EW wall hit?
            //calculate step and initial sideDist
            if (rayDirX < 0)
            {
                stepX = -1;
                sideDistX = (rayPosX - mapX) * deltaDistX;
            }
            else
            {
                stepX = 1;
                sideDistX = (mapX + 1.0f - rayPosX) * deltaDistX;
            }
            if (rayDirY < 0)
            {
                stepY = -1;
                sideDistY = (rayPosY - mapY) * deltaDistY;
            }
            else
            {
                stepY = 1;
                sideDistY = (mapY + 1.0f - rayPosY) * deltaDistY;
            }
            //perform DDA
            while (hit == 0)
            {
                //jump to next map square, OR in x-direction, OR in y-direction
                if (sideDistX < sideDistY)
                {
                    sideDistX += deltaDistX;
                    mapX += stepX;
                    side = 0;
                }
                else
                {
                    sideDistY += deltaDistY;
                    mapY += stepY;
                    side = 1;
                }
                //Check if ray has hit a wall
                if (World.worldMap[mapY, mapX] > 0) hit = 1;
            }
            //Calculate distance projected on camera direction (oblique distance will give fisheye effect!)
            if (side == 0) perpWallDist = (mapX - rayPosX + (1 - stepX) / 2) / rayDirX;
            else perpWallDist = (mapY - rayPosY + (1 - stepY) / 2) / rayDirY;

            //Calculate height of line to draw on screen
            int lineHeight = (int)(Height / perpWallDist);

            //calculate lowest and highest pixel to fill in current stripe
            int drawStart = -lineHeight / 2 + Height / 2;
            if (drawStart < 0) drawStart = 0;
            int drawEnd = lineHeight / 2 + Height / 2;
            if (drawEnd >= Height) drawEnd = Height - 1;

            int texNum = World.worldMap[mapY, mapX] - 1;

            //calculate value of wallX
            float wallX; //where exactly the wall was hit
            if (side == 0) wallX = posY + perpWallDist * rayDirY;
            else wallX = posX + perpWallDist * rayDirX;
            wallX -= Mathf.Floor(wallX);

            //x coordinate on the texture
            int texX_min = Mathf.RoundToInt(wallX * texWidth);
            int texX = Mathf.RoundToInt(wallX * texWidth * 8);
            if (side == 0 && rayDirX > 0) texX = texWidth - texX - 1;
            if (side == 1 && rayDirY < 0) texX = texWidth - texX - 1;

            ////choose wall color
            //Color color;
            //switch (World.worldMap[mapX, mapY])
            //{
            //    case 1: color = Color.red; break; //red
            //    case 2: color = Color.green; break; //green
            //    case 3: color = Color.blue; break; //blue
            //    case 4: color = Color.white; break; //white
            //    default: color = Color.yellow; break; //yellow
            //}

            for (int y = drawStart; y < drawEnd; y++)
            {
                int d_min = y * 256 - Height * 128 + lineHeight * 128;  //256 and 128 factors to avoid floats
                int texY_min = ((d_min * texHeight) / lineHeight) / 256;
                int d = y * 256 * 8 - Height * 128 * 8 + lineHeight * 128 * 8;  //256 and 128 factors to avoid floats
                int texY = ((d * texHeight) / lineHeight) / 256;

                Texture2D tex = RaycastResources.Instance.Textures[texNum];
                Color color;
                if (texX - texX_min == 0 || texY - texY_min == 0)
                {
                    Color total = new Color();
                    for (int i = texX_min; i < texX; i++)
                    {
                        for (int j = texY_min; j < texY; j++)
                        {
                            total += tex.GetPixel(i, j);
                        }
                    }

                    color = total / ((texX - texX_min) * (texY - texY_min));
                }
                else
                {
                    color = tex.GetPixel(texX, texY);
                }

                if (side == 1)
                {
                    color = color / 2f;
                    color.a = 1;
                }
                //buffer[y * texHeight + x] = color;
                texture.SetPixel(x, y, color);
            }

            //give x and y sides different brightness
            //if (side == 1) { color = color / 2; color.a = 1; }

            //draw the pixels of the stripe as a vertical line
            //texture.DrawLine(x, drawStart, x, drawEnd, color);
            //verLine(x, drawStart, drawEnd, color);

            //SET THE ZBUFFER FOR THE SPRITE CASTING
            ZBuffer[x] = perpWallDist;
        }

        //texture.SetPixels(buffer);
        //for (int x = 0; x < Width; x++) for (int y = 0; y < Height; y++) buffer[y][x] = 0; //clear the buffer instead of cls()
        //buffer = new Color[Width* Height];

        //SPRITE CASTING
        //sort sprites from far to close
        int activeSprites = 0;
        for (int i = 0; i < Sprites.Count; i++)
        {
            if (!Sprites[i].IsPlayer || ((FPSPlayer)Sprites[i]).IsAlive)
            {
                ++activeSprites;
                spriteOrder[i] = i;
                spriteDistance[i] = ((posX - Sprites[i].X) * (posX - Sprites[i].X) +
                                     (posY - Sprites[i].Y) * (posY - Sprites[i].Y)); //sqrt not taken, unneeded
            }
        }

        comboSort(spriteOrder, spriteDistance, activeSprites);

        //after sorting the sprites, do the projection and draw them
        for (int i = 0; i < activeSprites; i++)
        {
            //translate sprite position to relative to camera
            double spriteX = Sprites[spriteOrder[i]].X - posX;
            double spriteY = Sprites[spriteOrder[i]].Y - posY;

            if (Math.Abs(spriteX) < 0.0001 && Math.Abs(spriteY) < 0.0001) //We are standing right ontop of the sprite
                continue;

            //transform sprite with the inverse camera matrix
            // [ planeX   dirX ] -1                                       [ dirY      -dirX ]
            // [               ]       =  1/(planeX*dirY-dirX*planeY) *   [                 ]
            // [ planeY   dirY ]                                          [ -planeY  planeX ]

            //I think this is the determinant?
            double determinant = (planeX * dirY - dirX * planeY);

            double invDet = 1.0 / determinant; //required for correct matrix multiplication

            double transformX = invDet * (dirY * spriteX - dirX * spriteY);
            double transformY = invDet * (-planeY * spriteX + planeX * spriteY); //this is actually the depth inside the screen, that what Z is in 3D

            int spriteScreenX = (int)(((float)Width / 2) * (1 + transformX / transformY));

            //parameters for scaling and moving the sprites
            int uDiv = 1;
            int vDiv = 1;
            double vMove = 0.0;

            int vMoveScreen = (int)(vMove / transformY);

            //calculate height of the sprite on screen
            int spriteHeight = -1;
            try
            {
                spriteHeight =
                    Math.Abs((int)(Height / (transformY))) /
                    vDiv; //using "transformY" instead of the real distance prevents fisheye
            }
            catch (Exception ex)
            {
                Debug.Log("errror" + ex);
            }

            //calculate lowest and highest pixel to fill in current stripe
            int drawStartY = -spriteHeight / 2 + Height / 2 + vMoveScreen;
            if (drawStartY < 0) drawStartY = 0;
            int drawEndY = spriteHeight / 2 + Height / 2 + vMoveScreen;
            if (drawEndY >= Height) drawEndY = Height - 1;

            //calculate width of the sprite
            int spriteWidth = Math.Abs((int)(Height / (transformY))) / uDiv;
            int drawStartX = -spriteWidth / 2 + spriteScreenX;
            if (drawStartX < 0) drawStartX = 0;
            int drawEndX = spriteWidth / 2 + spriteScreenX;
            if (drawEndX >= Width) drawEndX = Width - 1;

            //loop through every vertical stripe of the sprite on screen
            for (int stripe = drawStartX; stripe < drawEndX; stripe++)
            {
                int texX = (int)(256 * (stripe - (-spriteWidth / 2 + spriteScreenX)) * this.texWidth / spriteWidth) / 256;
                //the conditions in the if are:
                //1) it's in front of camera plane so you don't see things behind you
                //2) it's on the screen (left)
                //3) it's on the screen (right)
                //4) ZBuffer, with perpendicular distance
                if (transformY > 0 && stripe > 0 && stripe < Width && transformY < ZBuffer[stripe])
                    for (int y = drawStartY; y < drawEndY; y++) //for every pixel of the current stripe
                    {
                        int d = (y - vMoveScreen) * 256 - Height * 128 + spriteHeight * 128; //256 and 128 factors to avoid floats
                        int texY = ((d * this.texHeight) / spriteHeight) / 256;
                        // spriteWidth * texY + texX]; //get current color from the texture

                        //TODO: Move out of expensive loop
                        Color color = RaycastResources.Instance.SpriteRegistery[Sprites[spriteOrder[i]].TextureId].texture.GetPixel(texX, texY);
                        var player = Sprites[spriteOrder[i]];
                        if (player.IsPlayer)
                        {
                            var teamcolor = ((FPSPlayer)player).GetColor();
                            color = color * teamcolor;
                        }
                        //if ((color & 0x00FFFFFF) != 0) buffer[y, stripe] = color; //paint pixel if it isn't black, black is the invisible color
                        if (Math.Abs(color.a - 1) < 0.1f)
                            texture.SetPixel(stripe, y, color);
                    }
            }
        }

        texture.Apply(false);

    }


    void ClearScreen()
    {
        texture.SetPixels32(blankScreen);
    }



    void comboSort(int[] order, double[] dist, int amount)
    {
        int gap = amount;
        bool swapped = false;
        while (gap > 1 || swapped)
        {
            //shrink factor 1.3
            gap = (gap * 10) / 13;
            if (gap == 9 || gap == 10) gap = 11;
            if (gap < 1) gap = 1;
            swapped = false;
            for (int i = 0; i < amount - gap; i++)
            {
                int j = i + gap;
                if (dist[i] < dist[j])
                {
                    Swap(ref dist[i], ref dist[j]);
                    Swap(ref order[i], ref order[j]);
                    swapped = true;
                }
            }
        }
    }

    //TODO: Place this method in a Utility class
    static void Swap<T>(ref T lhs, ref T rhs)
    {
        T temp;
        temp = lhs;
        lhs = rhs;
        rhs = temp;
    }

    #region OldCode

    /*void RenderImage()
{
    for (int x = 0; x < Width; x++)
    {
        //calculate ray position and direction
        float cameraX = 2 * x / Width - 1; //x-coordinate in camera space
        Vector2 rayPos = playerPosition;
        Vector2 rayDir = new Vector2(dir.x + cameraPlane.x * cameraX, dir.y + cameraPlane.x * cameraX);
        //which box of the map we're in
        int mapX = Mathf.RoundToInt(rayPos.x);
        int mapY = Mathf.RoundToInt(rayPos.y);

        //length of ray from current position to next x or y-side
        float sideDistX;
        float sideDistY;

        //length of ray from one x or y-side to next x or y-side
        float deltaDistX = Mathf.Sqrt(1 + (rayDir.y * rayDir.y) / (rayDir.x * rayDir.x));
        float deltaDistY = Mathf.Sqrt(1 + (rayDir.x * rayDir.x) / (rayDir.y * rayDir.y));
        float perpWallDist;

        //what direction to step in x or y-direction (either +1 or -1)
        int stepX;
        int stepY;

        int hit = 0; //was there a wall hit?
        int side = -1; //was a NS or a EW wall hit?
        //calculate step and initial sideDist
        if (rayDir.x < 0)
        {
            stepX = -1;
            sideDistX = (rayPos.x - mapX) * deltaDistX;
        }
        else
        {
            stepX = 1;
            sideDistX = (mapX + 1.0f - rayPos.x) * deltaDistX;
        }
        if (rayDir.y < 0)
        {
            stepY = -1;
            sideDistY = (rayPos.y - mapY) * deltaDistY;
        }
        else
        {
            stepY = 1;
            sideDistY = (mapY + 1.0f - rayPos.y) * deltaDistY;
        }
        //perform DDA
        while (hit == 0)
        {
            //jump to next map square, OR in x-direction, OR in y-direction
            if (sideDistX < sideDistY)
            {
                sideDistX += deltaDistX;
                mapX += stepX;
                side = 0;
            }
            else
            {
                sideDistY += deltaDistY;
                mapY += stepY;
                side = 1;
            }
            //Check if ray has hit a wall
            if (worldMap[mapX, mapY] > 0) hit = 1;
        }
        //Calculate distance projected on camera direction (oblique distance will give fisheye effect!)
        if (side == 0) perpWallDist = (mapX - rayPos.x + (1 - stepX) / 2f) / rayDir.x;
        else perpWallDist = (mapY - rayPos.y + (1 - stepY) / 2f) / rayDir.y;

        //Calculate height of line to draw on screen
        int lineHeight = (int)(Height / perpWallDist);

        //calculate lowest and highest pixel to fill in current stripe
        int drawStart = -lineHeight / 2 + Height / 2;
        if (drawStart < 0) drawStart = 0;
        int drawEnd = lineHeight / 2 + Height / 2;
        if (drawEnd >= Height) drawEnd = Height - 1;

        //choose wall color
        Color color;
        switch (worldMap[mapX, mapY])
        {
            case 1: color = Color.red; break; //red
            case 2: color = Color.green; break; //green
            case 3: color = Color.blue; break; //blue
            case 4: color = Color.white; break; //white
            default: color = Color.yellow; break; //yellow
        }

        //give x and y sides different brightness
        if (side == 1) { color = color / 2; color.a = 1; }

        //draw the pixels of the stripe as a vertical line
        Debug.Log("Drawing line at x:{" + x + "}, y1:{" + drawStart + "}, y2:{" + drawEnd + "}");
        texture.DrawLine(x, drawStart, x, drawEnd, color);
        //verLine(x, drawStart, drawEnd, color);
    }

    texture.Apply(false);
}*/

    /*void HandleInput()
{
    float moveSpeed = MovementSpeed * Time.deltaTime;

    //this could be done much more nicely
    if (Input.GetAxisRaw("Vertical") > 0) //up
    {
        if (worldMap[Mathf.RoundToInt(playerPosition.x + dir.x * moveSpeed), Mathf.RoundToInt(playerPosition.y)] == 0)
            playerPosition.x += dir.x * moveSpeed;
        if (worldMap[Mathf.RoundToInt(playerPosition.x), Mathf.RoundToInt(playerPosition.y + dir.y * moveSpeed)] == 0)
            playerPosition.y += dir.y * moveSpeed;
    }
    else if (Input.GetAxisRaw("Vertical") < 0) //down
    {
        if (worldMap[Mathf.RoundToInt(playerPosition.x - dir.x * moveSpeed), Mathf.RoundToInt(playerPosition.y)] == 0)
            playerPosition.x -= dir.x * moveSpeed;
        if (worldMap[Mathf.RoundToInt(playerPosition.x), Mathf.RoundToInt(playerPosition.y - dir.y * moveSpeed)] == 0)
            playerPosition.y -= dir.y * moveSpeed;
    }

    float turnAmount = TurnSpeed * Time.deltaTime;

    if (Input.GetAxisRaw("Horizontal") > 0) //right
    {
        float oldDirX = dir.x;
        dir.x = dir.x * Mathf.Cos(-turnAmount) - dir.y * Mathf.Sin(-turnAmount);
        dir.y = oldDirX * Mathf.Sin(-turnAmount) + dir.y * Mathf.Cos(-turnAmount);

        float oldPlaneX = cameraPlane.x;
        cameraPlane.x = cameraPlane.x * Mathf.Cos(-turnAmount) - cameraPlane.y * Mathf.Sin(-turnAmount);
        cameraPlane.y = oldPlaneX * Mathf.Sin(-turnAmount) + cameraPlane.y * Mathf.Cos(-turnAmount);
    }
    else if (Input.GetAxisRaw("Horizontal") < 0) //left
    {
        float oldDirX = dir.x;
        dir.x = dir.x * Mathf.Cos(turnAmount) - dir.y * Mathf.Sin(turnAmount);
        dir.y = oldDirX * Mathf.Sin(turnAmount) + dir.y * Mathf.Cos(turnAmount);

        float oldPlaneX = cameraPlane.x;
        cameraPlane.x = cameraPlane.x * Mathf.Cos(turnAmount) - cameraPlane.y * Mathf.Sin(turnAmount);
        cameraPlane.y = oldPlaneX * Mathf.Sin(turnAmount) + cameraPlane.y * Mathf.Cos(turnAmount);
    }
}*/

    #endregion
}
