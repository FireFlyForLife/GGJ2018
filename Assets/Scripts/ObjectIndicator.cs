using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class ObjectIndicator : MonoBehaviour
{

    private int PlayerNumber;
    private Vector2 m_objectPosition = new Vector2();

    List<int> playerXPosList = new List<int>();
    List<int> playerYPosList = new List<int>();

    private Image m_ownImg;
    private FPSPlayer m_ownEntity;
    private RaycastEntity m_closest;
    private World2D m_world;
    private float timer;

    public void Initialize(FPSPlayer entity, Image image, World2D world)
    {
        m_ownImg = entity.DistIndicator;
        m_ownEntity = entity;
        m_world = world;
    }

    public float GetAngle(Vector3 closest)
    {
        /*;
        targetDir.Normalize();
        float dot = Vector3.Dot(dirVec,targetDir);
        return Mathf.Rad2Deg * Mathf.Acos(dot);*/
        //Vector3 dirVec = new Vector3(m_ownEntity.DirVec.x, m_ownEntity.DirVec.y, 0);
        //Vector3 ownPos = new Vector3(m_ownEntity.Position.x, m_ownEntity.Position.y, 0);
        //Vector3 targetDir = ownPos - closest;
        //targetDir.Normalize();
        //dirVec.Normalize();
        //float dot = Vector3.Dot(dirVec, targetDir);
        //return Mathf.Rad2Deg * Mathf.Acos(dot);
        Vector3 posDir = m_ownEntity.Position;// + m_ownEntity.DirVec;
        Vector2 relDir = closest - posDir;
        relDir.Normalize();
      //  float rad = Mathf.Atan2(relDir.y, relDir.x);
        float dot = Vector3.Dot(m_ownEntity.DirVec, new Vector3(relDir.x, relDir.y, 0));
        float det = m_ownEntity.DirVec.x * relDir.y - m_ownEntity.DirVec.y * relDir.x;
        float angle = Mathf.Atan2(det, dot);
        Debug.Log(Mathf.Rad2Deg * angle);
       
        return Mathf.Rad2Deg * angle;
    }

    public float GetClosestSqrDist()
    {
        float bestDist = float.MaxValue;
        for (int i = 0; i < m_world.Entities.Count; i++)
        {
            float dist = (m_world.Entities[i].Position - m_ownEntity.Position).sqrMagnitude;
            if (dist < bestDist)
            {
                if (dist == 0) continue;
                bestDist = dist;
                m_closest = m_world.Entities[i];
            }
        }

        m_ownImg.transform.SetPositionAndRotation(m_ownImg.transform.position, Quaternion.Euler(0, 0, GetAngle(m_closest.Position))); //= Quaternion.Euler(0, 0, GetAngle(m_closest.Position));
       /* var recttr = m_ownImg.rectTransform;
        recttr.localEulerAngles = new Vector3(0, 0, GetAngle(m_closest.Position));*/

        //Debug.Log(m_ownImg.transform.rotation.z);
        return bestDist;
    }

    public float GetTime()
    {
        float dist = GetClosestSqrDist();
        return dist / 100.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_ownEntity == null)
        {
            Debug.Log("OBJECT INDICATOR INITIALIZE NOT CALLED");
            return;
        }


        if (timer <= 0)
        {
            m_ownImg.enabled = !m_ownImg.isActiveAndEnabled;
            timer = GetTime();
        }
        else
        {
            timer -= Time.deltaTime;
            timer = Mathf.Min(timer, GetTime());
            if (timer > 5) m_ownImg.enabled = true;
        }

    }
}
