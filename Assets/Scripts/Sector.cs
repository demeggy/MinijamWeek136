using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sector : MonoBehaviour
{
    public string X;
    public string Y;
    public List<GameObject> EnemyList;
    public List<GameObject> ObjectList;

    private float ThreatLevel;
    private float ObjectCount;
    private float EnemyCount;

    public bool IsAwake;

    private void Start()
    {
        //Randomise Difficulty
        ThreatLevel = Random.Range(1, 3);

        //Randomise No Objects
        ObjectCount = Random.Range(4, 25);

        //Randomise No Enemy Spawns
        if (ThreatLevel == 1)
        {
            EnemyCount = Random.Range(1, 3);
        }
        else if (ThreatLevel == 2)
        {
            EnemyCount = Random.Range(3, 6);
        }
        else if (ThreatLevel == 3)
        {
            EnemyCount = Random.Range(6, 9);
        }

        //for each objectcount, instantiate with origin +/ -50 of the sectors centre
        //Debug.Log("Spawning " + ObjectCount + " objects in " + SectorName);

        for (int i = 0; i < ObjectCount; i++)
        {
            GameObject Object = ObjectList[Random.Range(0, ObjectList.Count)];
            Vector3 ObjectPos = new Vector3(transform.position.x + Random.Range(-50, 50), transform.position.y, transform.position.z + Random.Range(-50, 50));
            GameObject NewObject = Instantiate(Object, ObjectPos, Quaternion.identity);
            NewObject.transform.localScale *= Random.Range(1, 15);
        }

        //for each enemycount, instantiate with origin +/ -50 of the sectors centre
        //Debug.Log("Spawning " + EnemyCount + " enemies in " + SectorName + " (Threat Level " + ThreatLevel +")");

        for (int i = 0; i < EnemyCount; i++)
        {
            GameObject Enemy = EnemyList[Random.Range(0, EnemyList.Count)];
            Vector3 EnemyPos = new Vector3(transform.position.x + Random.Range(-50, 50), transform.position.y, transform.position.z + Random.Range(-50, 50));
            GameObject NewEnemy = Instantiate(Enemy, EnemyPos, Quaternion.identity);
            NewEnemy.transform.parent = transform;

            Transform EnemySprite = NewEnemy.transform.Find("EnemySprite");
            EnemySprite.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }



}
