using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject Target;

    public Vector3 offset = new Vector3(0, 5, -10);





    void LateUpdate()
    {
     if(Target != null)
        {
            transform.position=Target.transform.position + offset;
        }    
    }
}
