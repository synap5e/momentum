using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TextureTilingController : MonoBehaviour {

    // Give us the texture so that we can scale proportianally the width according to the height variable below
    // We will grab it from the meshRenderer
    public Texture texture;
    public float textureToMeshZ = 2f; // Use this to contrain texture to a certain size

    private Vector3 prevScale = Vector3.one;
    private float prevTextureToMeshZ = -1f;

    // Use this for initialization
    void Start () {
        this.prevScale = gameObject.transform.lossyScale;
        this.prevTextureToMeshZ = this.textureToMeshZ;

        this.UpdateTiling();
    }

    // Update is called once per frame
    void Update () {
        // If something has changed
        //if(gameObject.transform.lossyScale != prevScale || !Mathf.Approximately(this.textureToMeshZ, prevTextureToMeshZ))
        //    this.UpdateTiling();
        this.UpdateTiling();
        // Maintain previous state variables
        this.prevScale = gameObject.transform.lossyScale;
        this.prevTextureToMeshZ = this.textureToMeshZ;
    }

    [ContextMenu("UpdateTiling")]
    void UpdateTiling()
    {
        // A Unity plane is 10 units x 10 units
        float planeSizeX = 10f;
        float planeSizeZ = 10f;

        // Figure out texture-to-mesh width based on user set texture-to-mesh height
        float textureToMeshX = ((float) this.texture.width / this.texture.height) * this.textureToMeshZ;
        float xScale = planeSizeX * gameObject.transform.lossyScale.x / textureToMeshX;
        float zScale = planeSizeZ * gameObject.transform.lossyScale.z  / textureToMeshZ;

        //Destroy(gameObject.GetComponent<Renderer>().material);
        gameObject.GetComponent<Renderer>().material = gameObject.GetComponent<Renderer>().sharedMaterial;

        //gameObject.GetComponent<Renderer>().material.SetTextureScale("_MainTex", new Vector2(xScale, zScale));
    }
}