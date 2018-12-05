using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.XR;

[System.Serializable]
public class Portals
{
    public GameObject PortalObject;
    public GameObject TargetPortalObject;
    public Door CloseThisDoor;
}

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]

public class PortalHandler : MonoBehaviour {

    public List<Portals> PortalList = new List<Portals>();
    public float AppearDistance = 0.05f;

    public GameObject MoveTarget;
    public bool DebugController = false;

    public float Speed = 1; 
    public float RotSpeed = 1; 

    private void Awake()
    {
        //GetComponent<Rigidbody>().isKinematic = true;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (DebugController)
        {
            CharacterController();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        RayCheck();
    }

    private void RayCheck()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position,transform.forward,out hit, 0.05f))
        {
            int index = -1;
            index = PortalList.FindIndex(p => p.PortalObject.name == hit.collider.gameObject.name);
           
            if (index >= 0)
            {
                Vector3 PosOffset = hit.collider.transform.position - hit.point;
               // PosOffset = transform.InverseTransformVector(PosOffset);
                //Vector3 dirVector = hit.collider.transform.position - transform.position;
                Teleport(index,PosOffset);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        int index = -1;
        index = PortalList.FindIndex(p => p.PortalObject.name == other.gameObject.name);

       if( index >= 0 ){
            //Teleport(index);
        }
        
    }

    private void Teleport(int value, Vector3 posoff)
    {
        Transform tr = PortalList[value].TargetPortalObject.transform;

        posoff.y = 0;
        posoff.z = 0;

        if (XRDevice.isPresent) {
            MoveTarget.transform.position = new Vector3(tr.position.x,  0 , tr.position.z);

            MoveTarget.transform.localEulerAngles -= PortalList[value].TargetPortalObject.transform.localEulerAngles - PortalList[value].PortalObject.transform.localEulerAngles;

            MoveTarget.transform.position += -transform.forward * AppearDistance;
            MoveTarget.transform.position += tr.TransformDirection(posoff);
        }
        else
        {
            transform.position = new Vector3(tr.position.x, transform.position.y, tr.position.z);

            transform.localEulerAngles -= PortalList[value].TargetPortalObject.transform.localEulerAngles - PortalList[value].PortalObject.transform.localEulerAngles;

            transform.position += -transform.forward * AppearDistance;
            transform.position += tr.TransformDirection(posoff);
        }

        
       
        PortalList[value].CloseThisDoor.ResetRot();

        
        int index = -1;
        index = PortalList.FindIndex(p => p.PortalObject.name == PortalList[value].TargetPortalObject.name);

        foreach (Renderer rend in PortalList[index].CloseThisDoor.GetComponentsInChildren<MeshRenderer>())
        {
            rend.enabled = false;
        }

        foreach (Collider col in PortalList[index].CloseThisDoor.GetComponentsInChildren<Collider>())
        {
            if (col.gameObject.GetInstanceID() != PortalList[index].CloseThisDoor.gameObject.GetInstanceID())
            {
                col.enabled = false;
            }
        }
    }

    private void CharacterController()
    {
        Vector3 velocity = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            velocity += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            velocity += Vector3.left;            
        }
        if (Input.GetKey(KeyCode.Q))
        {
            if (XRDevice.isPresent)
            {
                MoveTarget.transform.localEulerAngles += Vector3.down * Time.deltaTime * RotSpeed;
            }
            else
            {
                transform.localEulerAngles += Vector3.down * Time.deltaTime * RotSpeed;
            }
        }
        if (Input.GetKey(KeyCode.S))
        {
            velocity += Vector3.back;
        }
        if (Input.GetKey(KeyCode.D))
        {
            velocity += Vector3.right;            
        }
        if (Input.GetKey(KeyCode.E))
        {
            if (XRDevice.isPresent)
            {
                MoveTarget.transform.localEulerAngles += Vector3.up * Time.deltaTime * RotSpeed;
            }
            else
            {
                transform.localEulerAngles += Vector3.up * Time.deltaTime * RotSpeed;
            }
        }       

        Vector3 TargetPos = transform.TransformDirection(velocity) * Time.deltaTime * Speed; ;
        TargetPos.y = 0;

        MoveTarget.transform.position += TargetPos;
    }

}
