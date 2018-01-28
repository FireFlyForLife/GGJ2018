using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEntity : RaycastEntity
{
    public World2D World;
    public float AnimationTime = 5f;

    public int[] SpriteIds;
    public float FrameUpdateDelay = 0.3f;
    private float lastFrameUpdate = float.MinValue;
    private int frameIndex = 0;


	void Start () {
		World.Entities.Add(this);
        
        Destroy(this, AnimationTime);
	}
	
	void Update () {
		//Update the animation
	    if (lastFrameUpdate + FrameUpdateDelay < Time.time)
	    {
	        lastFrameUpdate = Time.time;
	        if (frameIndex == SpriteIds.Length - 1)
	            frameIndex = 0;
	        else
	            ++frameIndex;

	        TextureId = SpriteIds[frameIndex];
	    }

	}

    void OnDestroy()
    {
        World.Entities.Remove(this);
    }
}
