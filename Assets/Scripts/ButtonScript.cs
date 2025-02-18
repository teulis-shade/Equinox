using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public bool buttonDown = false;
    [SerializeField] private Button button;
    private GameObject buttonBase;

    Vector3 buttonPosition;
    Quaternion buttonRotation;
    float damp;
    float min;
    float max;
    float force;
    float breakForce;

    [SerializeField] bool constraintX;
    [SerializeField] bool constraintY;
    [SerializeField] bool constraintZ;

    [SerializeField] float cooldown;
    bool justLeft = false;

    protected void Start()
    {
        buttonPosition = button.transform.position;
        buttonRotation = button.transform.rotation;
        SpringJoint spring = button.GetComponent<SpringJoint>();
        buttonBase = spring.connectedBody.gameObject;
        damp = spring.damper;
        min = spring.minDistance;
        max = spring.maxDistance;
        force = spring.spring;
        breakForce = spring.breakForce;

        button.buttonScript = this;
    }
    private void OnTriggerEnter(Collider other)
    {
        //if {
            if (button != null && other.gameObject.Equals(button.gameObject))
            {
                buttonDown = true;
            }
            else if (!justLeft && (other.GetComponent<SpringJoint>() == null && transform.parent.GetComponent<SpringJoint>() == null) && (other.gameObject.GetComponent<Button>() != null || (other.gameObject.transform.parent != null && other.gameObject.transform.parent.GetComponent<Button>() != null) && button == null))
            {
                if (other.gameObject.transform.parent != null && other.gameObject.transform.parent.GetComponent<Button>() != null)
                {
                    button = other.gameObject.transform.parent.GetComponent<Button>();
                }
                else
                {
                    button = other.gameObject.GetComponent<Button>();
                }
                button.transform.position = buttonPosition;
                button.transform.rotation = buttonRotation;
                RigidbodyConstraints rbConstraints = RigidbodyConstraints.FreezeRotation;
                if (constraintX){rbConstraints |= RigidbodyConstraints.FreezePositionX;}
                if (constraintY){rbConstraints |= RigidbodyConstraints.FreezePositionY;}
                if (constraintZ){rbConstraints |= RigidbodyConstraints.FreezePositionZ;}
                button.ReAttach(this, rbConstraints);
                SpringJoint joint = button.gameObject.AddComponent<SpringJoint>();
                joint.damper = damp;
                joint.minDistance = min;
                joint.maxDistance = max;
                joint.spring = force;
                joint.breakForce = breakForce;
                joint.connectedBody = buttonBase.GetComponent<Rigidbody>();
            }
        //}
        //Debug.Break();
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.Equals(button.gameObject))
        {
            //print("tristan was right");
            buttonDown = false;
        }
    }

    protected void Update()
    {
        if (buttonDown)
        {
            ButtonOn();
        }
        else
        {
            ButtonOff();
        }
    }

    IEnumerator ButtonCooldown()
    {
        justLeft = true;
        yield return new WaitForSeconds(cooldown);
        justLeft = false;
    }
    public virtual void ButtonOn()
    {

    }

    public virtual void ButtonOff()
    {
        
    }

    public void Break()
    {
        button.buttonScript = null;
        button = null;
        buttonDown = false;
        StartCoroutine(ButtonCooldown());
    }
}
