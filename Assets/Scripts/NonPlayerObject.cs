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
    public GameObject Slot1;
    private NavMeshAgent Agent;
    private GameObject Parent;
    public float AttackRange;
    private float Slot1Timer;
    public float Slot1Cool;

    private MissionController MissionController;

    private void Start()
    {
        MissionController = GameObject.FindGameObjectWithTag("MissionController").GetComponent<MissionController>();

        Rotate = Random.Range(0.25f, 0.5f);

        if (type == Type.Enemy)
        {
            //Get Navmesh Agent
            Agent = GetComponent<NavMeshAgent>();

            //Create the Enemy's waypoint
            Target = Instantiate(Waypoint, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation);
            Target.transform.parent = transform;
            //Move the Enemy's waypoint
            MoveWaypoint();
        }
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
            MoveTowards();
        }

        //Destroy if hp less than zero
        if (Hp <= 0)
        {

            for (int i = 0; i < GameObject.FindGameObjectWithTag("MissionController").GetComponent<MissionController>().TargetList.Count; i++)
            {
                //Remove the target from the list
                if (GameObject.FindGameObjectWithTag("MissionController").GetComponent<MissionController>().TargetList[i] == gameObject)
                {
                    GameObject.FindGameObjectWithTag("MissionController").GetComponent<MissionController>().TargetList.Remove(gameObject);
                }
            }

            Instantiate(ExplosionFx, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

    //Move the enemy towards the waypoint
    private void MoveTowards()
    {

        //Work out if the closest player is within the attack range, if so, switch to attack mode
        float DistanceToClosestPlayer = Vector3.Distance(transform.position, FindClosestPlayer().transform.position);

        if(DistanceToClosestPlayer > AttackRange)
        {
            MoveWaypoint();
        }
        else
        {
            Agent.destination = FindClosestPlayer().transform.position;

            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 10f))
            {
                if (hit.transform.tag == "Player")
                {
                    if (Slot1Timer <= Time.time)
                    {
                        FireWeapon();
                    }
                }
            }
        }
    }


    //Returns the closest player when attacking
    public GameObject FindClosestPlayer()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Player");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }

    //Fires weapon in slot 1
    public void FireWeapon()
    {
        GameObject projectile = Instantiate(Slot1, transform.position,transform.rotation);
        projectile.GetComponent<Rigidbody>().velocity = transform.TransformDirection(Vector3.forward * Slot1.GetComponent<Projectile>().Speed);
        Slot1Timer = Time.time + Slot1Cool;
    }

    //Move the waypoint onwards
    private void MoveWaypoint()
    {
        Parent = transform.parent.gameObject;

        float x = Parent.transform.position.x + Random.Range(-50f,50f);
        float z = Parent.transform.position.z + Random.Range(-50f, 50f);

        Target.transform.position = new Vector3(x,transform.position.y,z);

        Agent.destination = Target.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Destructable)
        {
            if (other.gameObject.tag == "Projectile")
            {
                Hp -= 1;
                Destroy(other.gameObject);
            }
        }
    }

}
