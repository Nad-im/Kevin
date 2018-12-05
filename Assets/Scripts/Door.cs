using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

    private bool Opened = false;

    private bool PlayerDetected = false;

    public float DoorSpeed = 1;
    public Transform DoorPivot;
    public float TargetRotZ = 85.8f;

    private float CurrrentRot = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(PlayerDetected && Input.GetKeyDown(KeyCode.Space)){
            Open();
        }

        Mechanic();

    }

    private void Open()
    {
        Opened = !Opened;       
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            PlayerDetected = true;
            
            foreach (Renderer rend in GetComponentsInChildren<MeshRenderer>(true))
            {
                rend.enabled = true;
            }
            foreach (Collider col in GetComponentsInChildren<Collider>(true))
            {
                col.enabled = true;
            }
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerDetected = false;
            if (Opened)
            {               
                Open();
            }
        }
    }

    private void Mechanic()
    {      
        CurrrentRot = Mathf.MoveTowards(CurrrentRot, Opened ? TargetRotZ : 0, DoorSpeed);

        DoorPivot.transform.localEulerAngles = new Vector3(DoorPivot.transform.localEulerAngles.x, DoorPivot.transform.localEulerAngles.y, CurrrentRot);

    }

    public void ResetRot()
    {
        CurrrentRot = 0;
    }
}
