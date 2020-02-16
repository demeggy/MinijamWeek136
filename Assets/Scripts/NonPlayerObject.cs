using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NonPlayerObject : MonoBehaviour
{
    public enum Type
    {
        Enemy,
        Scenery
    }
    public bool Destructable;
    public Type type;
    private Rigidbody2D rb;
    public float Hp;
    public string Name;
    public float Speed;
    private float Rotate;
    public GameObject ExplosionFx;
    public GameObject Waypoint;
    private GameObject Target;
    private NavMeshAgent Agent;

    private float ScreenX;
    private float ScreenZ;

    private void Start()
    {
        Rotate = Random.Range(0.25f, 0.5f);

        if (type == Type.Enemy)
        {
            Agent = GetComponent<NavMeshAgent>();

            //instantiate first waypoint within screen
            NewWaypoint();
        }

        ScreenX = 18.5f;
        ScreenZ = 10.75f;
    }

    // Update is called once per frame
    void Update()
    {
        if (type == Type.Scenery)
        {
            //spin the object
            transform.Rotate(0, Rotate, 0);
        }

        //Enemy code
        else
        {
            MoveTowardsWaypoint();
        }

        //Destroy if hp less than zero
        if (Hp <= 0)
        {
            Instantiate(ExplosionFx, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

    //Move the enemy towards the waypoint
    private void MoveTowardsWaypoint()
    {
        //Agent.destination = Target.transform.position; //GameObject.FindGameObjectWithTag("Player").transform.position; //Target.transform.position;
    }

    private void NewWaypoint()
    {
        //instantiate first waypoint within screen
        float x = Random.Range(-ScreenX, ScreenX);
        float z = Random.Range(-ScreenZ, ScreenZ);

        Target = Instantiate(Waypoint, new Vector3(x, transform.position.y, z), transform.rotation);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Destructable)
        {
            if (other.gameObject.tag == "Projectile")
            {
                Debug.Log("ouch");
                Hp -= 1;
                Destroy(other.gameObject);
            }
            else if (other.gameObject.tag == "EnemyWaypoint" && type == Type.Enemy)
            {
                Destroy(other.gameObject);
                NewWaypoint();
            }
        }
    }

}
