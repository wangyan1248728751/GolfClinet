using GameFramework;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Golf
{
    public class FlightPathItem : MonoBehaviour
    {
        [SerializeField]
        public GameObject lineEnd = null;

		private void Start()
		{
			DontDestroyOnLoad(gameObject);
		}
	}
}
