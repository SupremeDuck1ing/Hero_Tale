using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : MonoBehaviour
{

    public float fadeSpeed = 0.3f;
    Material[] materials;
    SkinnedMeshRenderer[] meshRenderers;

    private float spawnTime ;
    // Start is called before the first frame update
    void Start()
    {

        meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        int totalMaterials = 0;
        // Get total materials
        foreach(SkinnedMeshRenderer renderer in meshRenderers)
        {
            totalMaterials += renderer.materials.Length;
        }

        materials = new Material[totalMaterials];

        int currIndex = 0;
        int OtherIndex = 0;
        foreach(SkinnedMeshRenderer renderer in meshRenderers)
        {
            for (OtherIndex = 0; OtherIndex < renderer.materials.Length; OtherIndex++)
            {
                materials[currIndex] = renderer.materials[OtherIndex];
                currIndex++;
            }
        }

        spawnTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        
        SetAlpha((Time.time - spawnTime) * fadeSpeed);
    }

    void SetAlpha(float alpha)
    {
        for (int i = 0; i < materials.Length; i++)
        {
            Color color = materials[i].color;
            color.a = Mathf.Clamp(alpha, 0, 1);
            materials[i].color = color;
        }
    }

    void OnEnable()
    {

    }


}
