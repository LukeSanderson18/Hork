using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetGame : MonoBehaviour
{

    public StartTargetGame stg;
    public List<Transform> childies;
    public GameObject Target;

    public GameObject curTarget;
    // Use this for initialization
    void Start()
    {
        foreach (Transform child in transform)
        {
            childies.Add(child.transform);
        }
    }


    public void CreateTarget()
    {
        curTarget = Instantiate(Target, childies[Random.Range(0, childies.Count)].transform.position, Quaternion.identity) as GameObject;
    }

    private void Update()
    {
        if (curTarget == null && stg.running && stg.numberOfTargets < 5)
        {
            stg.numberOfTargets++;
            if (stg.numberOfTargets < 5)
            {
                CreateTarget();
            }
        }

        if (stg.running && stg.numberOfTargets == 5)
        {
            stg.running = false;
            stg.numberOfTargets = 0;
            print(stg.text.text);
        }
    }

}
