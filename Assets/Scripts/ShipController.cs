using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipController : MonoBehaviour
{
    //Stats
    public int hp;
    public int shields;
    public enum Input_Type
    {
        Left,
        Right
    }
    public Input_Type inputType;

    //Weaps
    public GameObject Slot1;
    public GameObject Slot2;
    public float Slot1Cool;
    private float Slot1Timer;
    public float Slot2Cool;
    private float Slot2Timer;

    //Movement
    public float Turn;
    public float Thrust;

    //Misc
    private Rigidbody rb;
    public Camera SharedCam;
    public Camera ShipCam;
    public GameObject OtherPlayer;
    public GameObject Slipstream;
    private bool InFormation;
    private float FormationTimer;

    //UI
    private string CurSector;

    public Text Status;
    public Text Sector;
    public GameObject SharedUI;
    public GameObject SplitUI;
    private GameObject OtherPlayerMarker;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        OtherPlayerMarker = transform.Find("OtherPlayer").gameObject;
    }

    private void FixedUpdate()
    {
        //Check for splitscreen
        FollowCam();

        //Create slipstream
        CreateSlipstream();

        //Checks for Formation bonus
        FormationBonus();

        //Point Other Player Marker at other player if active
        if (OtherPlayerMarker.activeSelf)
        {
            //Rotate towards other player
            //Vector3 dirToTarget = OtherPlayer.transform.position - transform.position;
            //Vector3 forward = Vector3.up;
            //float rotation = Vector3.SignedAngle(dirToTarget, forward, Vector3.forward);
            //OtherPlayerMarker.transform.rotation = Quaternion.Euler(0, 0, rotation);
            //OtherPlayerMarker.transform.rotation = Quaternion.Euler(Vector3.RotateTowards(transform.position, OtherPlayer.transform.position,360,100);
        }


        //Player 1 Input
        if (inputType == Input_Type.Left)
        {

            //Movement
            float thrustInput;

            if (Input.GetKey(KeyCode.Joystick1Button6))
            {
                thrustInput = 1;
            }
            else
            {
                thrustInput = 0;
            }

            float y = thrustInput;
            float x = Input.GetAxisRaw("Horizontal") * -1;

            transform.Rotate(0, 0, x * 3);
            if (Input.GetKey(KeyCode.LeftShift))
            {
                y *= Thrust;
            }
            Vector3 force = transform.TransformDirection(0, Thrust * y, 0);
            rb.AddForce(force, ForceMode.Force);

            //Fire Weapon
            if (Input.GetKey(KeyCode.Joystick1Button4))
            {
                if (Slot1Timer <= Time.time)
                {
                    FireWeapon();
                }
            }

        }

        //Player 2 Input
        else
        {
            float thrustInput;

            if (Input.GetKey(KeyCode.Joystick1Button7)) { thrustInput = 1; } else { thrustInput = 0; }

            float y = thrustInput;
            float x = Input.GetAxisRaw("RightHorizontal") * -1;

            transform.Rotate(0, 0, x * 3);
            if (Input.GetKey(KeyCode.LeftShift))
            {
                y *= Thrust;
            }
            Vector3 force = transform.TransformDirection(0, Thrust * y, 0);
            rb.AddForce(force, ForceMode.Force);

            //Fire Weapon
            if (Input.GetKey(KeyCode.Joystick1Button5))
            {
                if (Slot1Timer <= Time.time)
                {
                    FireWeapon();
                }
            }
        }
    }

    //If players are flying next to each either for more than X seconds, they're in formation
    public void FormationBonus()
    {
        float dist = Vector3.Distance(transform.position, OtherPlayer.transform.position);
        if (dist < 3.5f)
        {
            FormationTimer += Time.deltaTime;
            Debug.Log(FormationTimer);
        }
        else
        {
            FormationTimer = 0;
            InFormation = false;
        }

        if (FormationTimer > 2.5f)
        {
            InFormation = true;
        }
    }

    //Creates a slipstream visual behind player
    public void CreateSlipstream()
    {
        Vector2 velocity = rb.velocity;

        if (velocity.magnitude > 0.25f)
        {
            GameObject NewSlipstream = Instantiate(Slipstream, transform.position, transform.rotation);

            if (InFormation == true)
            {
                NewSlipstream.GetComponent<SpriteRenderer>().color = Color.green;
            }
            else
            {
                NewSlipstream.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;
            }

            StartCoroutine(DestroySlipstream(NewSlipstream));
        }
        
    }

    //Coroutine for destroying slipstream effects
    IEnumerator DestroySlipstream(GameObject Slipstream)
    {
        yield return new WaitForSeconds(1);
        Destroy(Slipstream);
    }
    
    public void FireWeapon()
    {
        GameObject projectile = Instantiate(Slot1, transform.position, transform.rotation);
        projectile.GetComponent<Rigidbody>().velocity = transform.TransformDirection(Vector3.up * Slot1.GetComponent<Projectile>().Speed);
        Slot1Timer = Time.time + Slot1Cool;
    }

    public void FollowCam()
    {
        ShipCam.transform.position = new Vector3(transform.position.x, 21, transform.position.z);
        //Camera MainCamera = Camera.main;

        float dist = Vector3.Distance(transform.position, OtherPlayer.transform.position);
        if (dist < 25)
        {
            //Toggle the UI between Shared and Split
            SharedUI.SetActive(true);
            SplitUI.SetActive(false);

            //Toggle the Other Player UI Marker
            OtherPlayerMarker.SetActive(false);

            //Merge camera
            Vector3 CameraMidpoint = (transform.position + OtherPlayer.transform.position) * 0.5f;
            SharedCam.transform.position = new Vector3(CameraMidpoint.x,21,CameraMidpoint.z);
            SharedCam.enabled = true;
            ShipCam.enabled = false;
        }
        else
        {
            //Toggle the UI between Shared and Split
            SharedUI.SetActive(false);
            SplitUI.SetActive(true);

            //Toggle the Other Player UI Marker
            OtherPlayerMarker.SetActive(true);

            //Split camera
            ShipCam.enabled = true;
            SharedCam.enabled = false;            
            ShipCam.transform.position = new Vector3(transform.position.x, 21, transform.position.z);
        }
    }

    public void UpdateUI()
    {
        Sector.text = CurSector;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Sector")
        {
            CurSector = other.gameObject.GetComponent<Sector>().SectorName;
        }
    }
}
