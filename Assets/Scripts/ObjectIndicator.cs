using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
     //    this.GetComponent<Image>().enabled = true;

    }
// Update is called once per frame
void Update () {
        //Debug.Log(m_playerPosition);
        CheckDistance();
    }

    int counter = 0;
    private void CheckDistance()
    {
        // this.enabled = false;
        //Debug.Log("BUGGER");
        Debug.Log((m_objectPosition - m_playerVec).sqrMagnitude);

      

        if ((m_objectPosition - m_playerVec).sqrMagnitude < 4000)
            {
            //this.enabled = true;
            //GetComponent<Image>();
            Debug.Log("OEEEEEHh");

            Component test = GetComponent<Image>();// = false;

            counter = 0;
            //counter++;
            if (counter == 0)
            {
                GetComponent<Image>().enabled = false;
                counter++;
            }
            if(counter == 1)
            {
                GetComponent<Image>().enabled = true;
                counter = 0;
            }
        }
        //else
            //this.GetComponent<Image>().enabled = true;


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
