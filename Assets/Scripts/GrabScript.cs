using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GrabScript : MonoBehaviour
{
    public GameObject player;
    public Transform posHold;
    private GameObject pickUpText;
    private GameObject lookObject;
    private GameObject grabbedObject;
    private GameObject parentGrabbedObject;
    private Rigidbody grabRigid;
    private bool currLook = false;
    private Vector3 turnVector = new Vector3(1, 1, 1);
    private float chuckSpeed = 300f;

    private void Start()
    {
        pickUpText = GameObject.Find("PickUp"); //this will later be in the canvas' code instead of in grab script
        pickUpText.SetActive(false);
    }

    void Update() {
        if (!(PauseScript.isPaused))
        {
            // Checks to see if object is in grabbable range.
            if (grabbedObject == null)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 5f))
                {
                    if (hit.transform.gameObject.tag == "Grabbable")
                    {
                        lookObject = hit.transform.gameObject;
                        currLook = true;
                        pickUpText.SetActive(true);
                        pickUpText.GetComponent<TMP_Text>().SetText("Press E to Grab");
                    }
                }
                else
                {
                    lookObject = null;
                    currLook = false;
                    pickUpText.SetActive(false);
                    
                }
            }


            // If looking at object and key pressed is E then pick up the object.
            if (currLook && grabbedObject == null && Input.GetKeyDown(KeyCode.E))
            {
                grabObject(lookObject);
            }
            else if (grabbedObject != null)
            {
                moveObject();
                if (Input.GetKeyDown(KeyCode.E))
                {
                    dropObject(false);
                }
                else if (Input.GetKeyDown(KeyCode.Q))
                {
                    dropObject(true);
                }
            }
        }

    }

    public void grabObject(GameObject grabObj) {
        //Save old parent
        
        if (grabObj.transform.parent.gameObject.layer == 7) { //"GrappleHead"
            //If part of grapplehead diasychaing and get its parentvariable
            //parentGrabbedObject = ((GrappleHead) grabObj.transform.parent.gameObject).parentGrabbedObj;
            //Debug.Break();
            parentGrabbedObject = FindObjectOfType<GrappleHead>().parentGrabbedObj;
            
            //parentGrabbedObject = grabObj.transform.parent.gameObject.parentGrabbedObj;

            //NEED TO UNSET THIS SOMEWHERE
        }
        else
        {
            parentGrabbedObject = grabObj.transform.parent.gameObject;
        }

        grabbedObject = grabObj;
        grabRigid = grabbedObject.GetComponent<Rigidbody>();
        grabRigid.isKinematic = true;
        grabRigid.transform.parent = posHold.transform;
        grabbedObject.transform.rotation = posHold.rotation;
        //grabbedObject.transform.localScale = Vector3.one;
        turnOffCollider(grabbedObject);
        
        //pickUpText.SetActive(false);

        //new
        pickUpText.SetActive(true);
        pickUpText.GetComponent<TMP_Text>().SetText("Press Q to Throw");
    }

    public void turnOffCollider(GameObject gm)
    {
        if (gm.GetComponent<Collider>() != null)
        {
            gm.GetComponent<Collider>().enabled = false;
        }
        for (int i = 0; i < gm.transform.childCount; ++i)
        {
            turnOffCollider(gm.transform.GetChild(i).gameObject);
        }
    }

    public void turnOnCollider(GameObject gm)
    {
        if (gm.GetComponent<Collider>() != null)
        {
            gm.GetComponent<Collider>().enabled = true;
        }
        for (int i = 0; i < gm.transform.childCount; ++i)
        {
            turnOnCollider(gm.transform.GetChild(i).gameObject);
        }
    }

    // Drops Object
    // I was trying to get it to drop directly in front of the camera, but I was busy.
    // Can finish later, unless someone else wants to take care of it.
    void dropObject(bool chuck) {
        print("dropping");
        print(parentGrabbedObject);
        //its assigning grappledhead as child
        StartCoroutine(WaitToReturn(.5f, grabbedObject));
        grabRigid.isKinematic = false;
        grabbedObject.transform.parent = null;
        if (chuck) {
            grabbedObject.transform.position += transform.forward;
            grabRigid.AddForce(transform.forward * chuckSpeed);
            grabRigid.AddTorque(turnVector * 1f);
        }
        //reasign to old parent
        //dumb vitto code breaking stuff here
        grabbedObject.transform.parent = parentGrabbedObject.transform;
        //grabbedObject.transform.localScale = Vector3.one;
        parentGrabbedObject = null;

        grabbedObject = null;
        pickUpText.SetActive(false);
    }

    IEnumerator WaitToReturn(float time, GameObject grabbedObject)
    {
        yield return new WaitForSeconds(time);
        turnOnCollider(grabbedObject);
    }

    void moveObject() {
        grabbedObject.transform.position = posHold.transform.position;
    }

    public bool getObjectGrabbed()
    {
        return (grabbedObject != null);
    }

}
