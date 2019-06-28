using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
	static SocketObject so;
	static DataWorker dw;

	PlayerScript mine;

    [SerializeField] LayerMask layerMask;

    // Start is called before the first frame update
    void Start()
    {
		so = SocketObject.Instance;
		dw = DataWorker.Instance;
		mine = GetComponent<PlayerScript> ();
		StartCoroutine (AttackByTime());
    }

	IEnumerator AttackByTime(){
		while(true){
			RaycastHit hit;
			if (Input.GetMouseButton(0) && Physics.Raycast (mine.cam.transform.position, Camera.main.transform.forward, out hit, mine.pd.attackRange,layerMask)) {
				Debug.Log (hit.collider.gameObject.tag);
				if (hit.collider.gameObject.CompareTag ("OthersAvater")) {
                    GameObject paret = hit.transform.parent.gameObject;
                    var mf = hit.transform.GetComponent<SkinnedMeshRenderer>();
                    //検出したオブジェクトのローカル座標に変換
                    Vector3 start = mf.transform.worldToLocalMatrix.MultiplyPoint(Camera.main.transform.position);
                    Vector3 end = mf.transform.worldToLocalMatrix.MultiplyPoint(hit.point);
                    var data = new Dictionary<string,string> ();
					data ["TYPE"] = "Hit";
					data ["trg"] = paret.GetComponent<PlayerScript> ().pd.id;
                    data["startX"] = start.x.ToString();
                    data["startY"] = start.y.ToString();
                    data["startZ"] = start.z.ToString();
                    data["endX"] = end.x.ToString();
                    data["endY"] = end.y.ToString();
                    data["endZ"] = end.z.ToString();
                    so.EmitMessage ("ToOwnRoom", data);
				} else if (hit.collider.gameObject.CompareTag ("fallenObstacle") || hit.collider.gameObject.CompareTag ("Obstacle")) {
					var data = new Dictionary<string,string> ();
					data ["TYPE"] = "DestroyObs";
					data ["n"] = hit.collider.gameObject.GetComponent<ObsUpdate>().id.ToString ();
					data ["attacker"] = so.id.ToString ();
					so.EmitMessage ("ToOwnRoom", data);
					Debug.Log ("Send:" + hit.collider.gameObject.GetComponent<ObsUpdate>().id.ToString () + "破壊");
					//hit.collider.gameObject.GetComponent<ObsUpdate>().Destroy ();
				}else if(hit.collider.gameObject.CompareTag ("Switch")){
					var data = new Dictionary<string,string> ();
					data ["TYPE"] = "PushSwitch";
					data ["trg"] = hit.collider.gameObject.transform.parent.gameObject.GetComponent<PlayerScript> ().pd.id;
					so.EmitMessage ("ToOwnRoom", data);
				}
				yield return new WaitForSeconds (mine.pd.getAttackSpeed());

			}
			yield return null;
		}
	}

	Vector3 getAngleVec(Vector3 from,Vector3 to){
		Vector3 fromVec = new Vector3 (from.x, 0, from.z);
		Vector3 toVec = new Vector3 (to.x, 0, to.z);
		return Vector3.Normalize (toVec - fromVec);
	}

}
