using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionController : MonoBehaviour
{

    private float MissionType;
    private float MissionReward;
    private float TargetCount;
    private string X;
    private string Y;
    public List<GameObject> EnemyList;
    public List<GameObject> TargetList;
    public List<GameObject> BossList;
    public List<GameObject> SectorList;
    public Text StatusBox;

    public List<string> EnemyName;
    public List<string> EnemyTitle;

    // Start is called before the first frame update
    void Start()
    {
        NewMission();
    }

    void NewMission()
    {
        // //Randomise MissionType (Clear, Bounty, ?)
        MissionType = Random.Range(1, 2);

        //Clear enemies
        if(MissionType == 1)
        {
            //Randomise specific mission parameters
            TargetCount = Random.Range(3, 9);
            MissionReward = 10 * TargetCount;
            GameObject Sector = SectorList[Random.Range(0, SectorList.Count)];

            //Update Status UI
            StatusBox.text = "Clear out " + TargetCount + " enemy ships at Sector " + Sector.GetComponent<Sector>().X + " " + Sector.GetComponent<Sector>().Y;

            //Instantiate target enemies at target sector
            for (int i = 0; i < TargetCount; i++)
            {
                GameObject Enemy = EnemyList[Random.Range(0, EnemyList.Count)];
                Vector3 EnemyPos = new Vector3(Sector.transform.position.x + Random.Range(-50, 50), Sector.transform.position.y, Sector.transform.position.z + Random.Range(-50, 50));
                GameObject NewEnemy = Instantiate(Enemy, EnemyPos, Quaternion.identity);
                NewEnemy.transform.parent = Sector.transform;
                NewEnemy.transform.name = "Target Enemy";
                
                //Signify targets with a yellow rendering
                Transform EnemySprite = NewEnemy.transform.Find("EnemySprite");
                EnemySprite.gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;

                //Add enemy to target list
                TargetList.Add(NewEnemy);
            }
        }

        //Destroy specific enemy
        else if (MissionType == 2)
        {
            //Randomise specific mission parameters

            MissionReward = Random.Range(75, 150);
            GameObject Sector = SectorList[Random.Range(0, SectorList.Count)];

            //Update Status UI
            StatusBox.text = "Bounty: " + EnemyName[Random.Range(0, EnemyName.Count)] + " the " + EnemyTitle[Random.Range(0, EnemyTitle.Count)] + " at Sector " + Sector.GetComponent<Sector>().X + " " + Sector.GetComponent<Sector>().Y;

            //Instantiate target enemies at target sector
            GameObject Enemy = EnemyList[Random.Range(0, EnemyList.Count)];
            Vector3 EnemyPos = new Vector3(Sector.transform.position.x + Random.Range(-50, 50), Sector.transform.position.y, Sector.transform.position.z + Random.Range(-50, 50));
            GameObject NewEnemy = Instantiate(Enemy, EnemyPos, Quaternion.identity);
            NewEnemy.transform.parent = Sector.transform;
            NewEnemy.transform.name = "Target Enemy";
            NewEnemy.GetComponent<NonPlayerObject>().Speed = Random.Range(5, 7.5f);
            NewEnemy.GetComponent<NonPlayerObject>().Hp = Random.Range(5, 10);

            //Signify targets with a yellow rendering
            Transform EnemySprite = NewEnemy.transform.Find("EnemySprite");
            EnemySprite.gameObject.GetComponent<SpriteRenderer>().color = new Color(196,0,255,255);

            //Add enemy to target list
            TargetList.Add(NewEnemy);
        }
        else if (MissionType == 3)
        {

        }

        // //Randomise MissionReward
        // //Randomise Target from list
        // //Randomise Sector from list
        // //Randomise TargetCount
        // //Update Status UI with Mission text
    }

    // Update is called once per frame
    void Update()
    {
        if(MissionType == 1 || MissionType == 2)
        {
            if (TargetList.Count == 0)
            {
                MissionType = 0;
                CompleteMission();
            }
        }        
    }

    public void CompleteMission()
    {
        StatusBox.text = "Mission Complete - all targets destroyed! Prepare for next mission...";
        //award money
        Invoke("NewMission", 3f);
    }
}
