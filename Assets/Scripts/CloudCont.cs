using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudCont : MonoBehaviour
{   
    [SerializeField] GameObject Cloud;
    [SerializeField] int NoOfCloud;
    [SerializeField] float ReSpawnTime;
    [SerializeField] float DistanceBetween;
    GameObject[] Clouds;

    // Update is called once per frame
    private void Start() {
        Clouds = new GameObject[NoOfCloud];
        for(int i=0; i<Clouds.Length; i++){
            Clouds[i] = Instantiate(Cloud, transform.position + Vector3.left*DistanceBetween*i, Quaternion.identity);
            Clouds[i].name = i+"cloud";
            Clouds[i].transform.parent = transform;
        }       
    }
    
    float timecount = 0;
    int LastCloud = 0;
    void Update()
    {
        timecount += Time.deltaTime;
        if(timecount>= ReSpawnTime){ 
            Clouds[LastCloud].transform.position = transform.position;
            LastCloud++;
            if(LastCloud>=NoOfCloud) LastCloud = 0;
            
            timecount = 0;
        }
    }
}
