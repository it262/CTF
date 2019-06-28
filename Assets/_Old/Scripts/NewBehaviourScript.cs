using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;
    SkinnedMeshRenderer mf;
    //MeshFilter mf;
    MeshCollider mc;
    Vector3[] test;

    [SerializeField]
    Mesh originMesh;

    Mesh copyMesh;

    // Start is called before the first frame update
    void Start()
    {
        copyMesh = new Mesh();
        copyMesh = GameObject.Instantiate(originMesh);
        /*
        copyMesh.vertices = originMesh.vertices;
        copyMesh.uv = originMesh.uv;
        copyMesh.triangles = originMesh.triangles;
        */
        mf = GetComponent<SkinnedMeshRenderer>();
        mc = GetComponent<MeshCollider>();
        mf.sharedMesh = copyMesh;
        Debug.Log(mf.sharedMesh.vertices.Length);
        mf.sharedMesh.RecalculateBounds();
        mf.sharedMesh.RecalculateNormals();
        mc.sharedMesh = mf.sharedMesh;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Input.GetMouseButton(0) && Physics.Raycast(ray, out hit, Mathf.Infinity,layerMask))
        {
            Debug.Log(hit.transform.gameObject);
            if (hit.transform.gameObject.CompareTag("OthersAvater"))
            {
                var mf = hit.transform.GetComponent<SkinnedMeshRenderer>();
                //var mc = hit.transform.GetComponent<MeshCollider>();

                //検出したオブジェクトのローカル座標に変換
                Vector3 start = mf.transform.worldToLocalMatrix.MultiplyPoint(Camera.main.transform.position);
                Vector3 end = mf.transform.worldToLocalMatrix.MultiplyPoint(hit.point);

                transformMesh(start, end);
            }
        }
    }

    private void transformMesh(Vector3 start, Vector3 end)
    {
        
        //ローカル座標を受け取る
        test = copyMesh.vertices;
        for (int i = 0; i < test.Length; i++)
        {
            Vector3 transPoint = test[i];
            float distance = Vector3.Distance(end, transPoint);
            if (distance < 5)
            {
                test[i] += (transPoint - start).normalized * (1 / Mathf.Sqrt(distance * distance)) * Time.deltaTime;
            }
        }
        mf.sharedMesh.vertices = test;
        mf.sharedMesh.RecalculateBounds();    //メッシュコンポーネントのプロパティboundsを再計算する
        mc.sharedMesh = mf.sharedMesh;
    }
}
