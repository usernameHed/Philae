using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class GardienZone : MonoBehaviour
{
    public Material[] material;
    public DissolveAnimationPlayer DissolveAnimationPlayer;
    [SerializeField]
    private float timeDurationWall;
    Renderer rend;

    // Start is called before the first frame update
    void Start()
    {
        
        rend = GetComponent<Renderer>();
        rend.enabled = true;
        rend.sharedMaterial = material[0];
    }

    void OnTriggerEnter(Collider col)
    {
        StopAllCoroutines();
        rend.sharedMaterial = material[1];
        StartCoroutine(ExecuteAfterTime(2));
       
    }
   
    [Button]
    public void LanchDisolve()
    {
        StopAllCoroutines();
        rend.sharedMaterial = material[1];
        StartCoroutine(ExecuteAfterTime(timeDurationWall));
    }


    private void ReturnToFirstState()
    {
        StopAllCoroutines();
        DissolveAnimationPlayer.Play();
        //rend.sharedMaterial = material[0];
        //sStartCoroutine(ExecuteAfterTime(3));

    }

    IEnumerator ExecuteAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        ReturnToFirstState();
        
    }

}
