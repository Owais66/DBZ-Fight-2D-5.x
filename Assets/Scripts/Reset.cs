using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reset : MonoBehaviour {
	[SerializeField] GameObject[] Player;
	Vector3 Playerpos;
	Vector3 Enemypos;

	private void Start() {
		Playerpos = Player[0].transform.position;
		Enemypos = Player[1].transform.position;
	}
	private void Update() {
	// 	foreach(GameObject player in Player)
	// 	if(Input.GetKeyDown(KeyCode.Space)) player.transform.position = Vector2.zero;
	// }
		if(Input.GetKeyDown(KeyCode.Space)){
			Player[0].transform.position = Playerpos;
			Player[1].transform.position = Enemypos;
		}
	}
}
