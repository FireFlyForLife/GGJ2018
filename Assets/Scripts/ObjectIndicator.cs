using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectIndicator : MonoBehaviour {

    private int PlayerNumber;
    private Vector2 m_objectPosition = new Vector2();

    List<int> playerXPosList = new List<int>();
    List<int> playerYPosList = new List<int>();

    private Vector2 m_playerVec = new Vector2();

    // Use this for initialization
    void Start()
    {
        //ccd  m_playerPosition = m_gameManager.RoomGenerator.SpawnPointGenerator.SpawnPointList[PlayerNumber];
        m_objectPosition = new Vector2(0, 0); 
    }
	
	// Update is called once per frame
	void Update () {
        //Debug.Log(m_playerPosition);
        CheckDistance();
    }

    private void CheckDistance()
    {
        this.enabled = false;
        //Debug.Log("BUGGER");

        if ((m_objectPosition - m_playerVec).sqrMagnitude < 50)
            {
            this.enabled = true;
           // Debug.Log("OEEEEEHh");
        }


  
    }

    public Vector2 GetPositions(int posX, int posY, int Index)
    {

        PlayerNumber = Index;
        //   Debug.Log(a_playerList[4].x);
        if (playerXPosList.Count < 4 && playerYPosList.Count < 4)
        {
            playerXPosList.Add(posX);
            playerYPosList.Add(posY);

           
        }
        else
        {

            m_playerVec = new Vector2();
            //int temp = PlayerNumber;
            m_playerVec = new Vector2(playerXPosList[PlayerNumber], playerYPosList[PlayerNumber]);

            //Debug.Log(tempVec);


            playerXPosList.RemoveRange(0, 4);
            playerYPosList.RemoveRange(0, 4);
            playerXPosList.Add(posX);
            playerYPosList.Add(posY);
        }


        
      

        return new Vector2();
    }

    public void ShowPosition()
    {
        Debug.Log(playerXPosList[0]);
    }
}
