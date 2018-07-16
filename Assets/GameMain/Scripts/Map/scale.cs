using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scale : MonoBehaviour {
    [SerializeField]
    Button Sbtn;
    [SerializeField]
    Image map;
    [SerializeField]
    Button Cbtn;
	// Use this for initialization
	void Start () {
        Sbtn.onClick.AddListener(() =>
        {
            map.transform.localScale = new Vector3(1f, 1f, 1f);
            Cbtn.gameObject.SetActive(true);
            Sbtn.GetComponent<Button>().enabled = false;
        });
        Cbtn.onClick.AddListener(() =>
        {
            map.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            Cbtn.gameObject.SetActive(false);
            Sbtn.GetComponent<Button>().enabled = true;
        });
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
