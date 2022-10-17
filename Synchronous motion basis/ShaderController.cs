using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderController : MonoBehaviour
{
    public Material[] materials;
    public UnityEngine.Vector4 mDirection;
    public MeshRenderer _mR;
    // Start is called before the first frame update
    void Start()
    {
        // Renderer renderer = gameObject.GetComponentInChildren<Renderer>(true);
        // if (renderer != null)
        //     materials = renderer.sharedMaterials;
        mDirection = new UnityEngine.Vector4(1f,0f,0f,0f);
        _mR = GetComponent<MeshRenderer>();
        // _mR.material.SetFloat("_Red", _ds);

    }

    // Update is called once per frame
    void Update()
    {
        _mR.material.SetVector("_Vector3",mDirection);
    }
}
