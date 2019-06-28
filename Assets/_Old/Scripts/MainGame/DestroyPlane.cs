using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyPlane : MonoBehaviour
{
	public ObstacleControllSync ocs;
	private void OnCollisionEnter(Collision collision){
		if (collision.gameObject.CompareTag ("Obstacle") || collision.gameObject.CompareTag ("fallenObstacle")) {
			ocs.obstacle.Remove (collision.gameObject.GetComponent<ObsUpdate> ().id);
			Destroy (collision.gameObject);
		}
	}
}
