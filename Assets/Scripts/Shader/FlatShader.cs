using UnityEngine;

public class FlatShader : MonoBehaviour
{
    void Start()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null || mf.sharedMesh == null) return;

        Mesh oldMesh = mf.sharedMesh;
        Mesh newMesh = new Mesh();
        
        Vector3[] oldVerts = oldMesh.vertices;
        int[] triangles = oldMesh.triangles;
        Vector3[] newVerts = new Vector3[triangles.Length];
        
        // 부드럽게 이어진 면들을 억지로 다 끊어버리는 작업
        for (int i = 0; i < triangles.Length; i++)
        {
            newVerts[i] = oldVerts[triangles[i]];
            triangles[i] = i;
        }
        
        newMesh.vertices = newVerts;
        newMesh.triangles = triangles;
        newMesh.RecalculateNormals(); // 여기서 면의 각이 확 살아남
        
        mf.mesh = newMesh;
    }
}