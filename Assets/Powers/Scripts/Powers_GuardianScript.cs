using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powers_GuardianScript : MonoBehaviour
{
    public Animator animMachine;
    
    // Start is called before the first frame update
    void Start()
    {
        animMachine = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float speed = Input.GetAxis("Vertical");
        
        animMachine.SetFloat("charSpeed", speed);

        transform.position += transform.forward * speed * Time.deltaTime * 3;
    }
}
