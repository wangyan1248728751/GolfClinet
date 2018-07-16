using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Golf;

public class lookat : MonoBehaviour {

	private void OnEnable()
	{
		transform.LookAt(GameEntry.Map._gameCameraManager._camera.transform);
		transform.rotation = Quaternion.Euler(new Vector3(-transform.eulerAngles.x, transform.eulerAngles.y + 180, transform.eulerAngles.z));
	}
}
