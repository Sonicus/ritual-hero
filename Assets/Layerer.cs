using UnityEngine;
using System.Collections;

public class Layerer : MonoBehaviour {

	// Use this for initialization
	void Start () {
	    foreach(ParticleSystem ps in this.GetComponentsInChildren<ParticleSystem>())
        {
            ps.GetComponent<Renderer>().sortingLayerName = "particles";
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
