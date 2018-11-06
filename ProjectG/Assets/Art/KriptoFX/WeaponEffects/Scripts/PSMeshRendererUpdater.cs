using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[ExecuteInEditMode]
public class PSMeshRendererUpdater : MonoBehaviour
{
    public GameObject MeshObject;

    const string materialName = "MeshEffect";
    List<Material[]> rendererMaterials = new List<Material[]>();
    List<Material[]> skinnedMaterials = new List<Material[]>();
    public bool IsActive = true;

    bool currentActiveStatus = true;

    void Update()
    {
        if (currentActiveStatus != IsActive)
        {
            currentActiveStatus = IsActive;
            Activation(currentActiveStatus);
        }
    }

    public void Activation(bool activeStatus)
    {
        if (MeshObject == null) return;
        var particles = MeshObject.GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in particles)
        {
            if (activeStatus) ps.Play();
            else ps.Stop();
        }
        var light = MeshObject.GetComponentInChildren<Light>();
        if (light != null) light.enabled = IsActive;
        var meshRend = MeshObject.GetComponentInChildren<MeshRenderer>();
        if (meshRend != null)
        {
            var mat = meshRend.sharedMaterials[meshRend.sharedMaterials.Length - 1];
            Color color = Color.black;
            if (mat.HasProperty("_TintColor"))
                color = mat.GetColor("_TintColor");
            color.a = !activeStatus ? 0 : 1;

            meshRend.sharedMaterials[meshRend.sharedMaterials.Length - 1].SetColor("_TintColor", color);
        }
        var skinMeshRend = MeshObject.GetComponentInChildren<SkinnedMeshRenderer>();
        if (skinMeshRend != null)
        {
            var mat = skinMeshRend.sharedMaterials[skinMeshRend.sharedMaterials.Length - 1];
            Color color = Color.black;
            if (mat.HasProperty("_TintColor"))
                color = mat.GetColor("_TintColor");

            color.a = !activeStatus ? 0 : 1;
            skinMeshRend.sharedMaterials[skinMeshRend.sharedMaterials.Length - 1].SetColor("_TintColor", color);
        }
    }

    public void UpdateMeshEffect()
    {
        rendererMaterials.Clear();
        skinnedMaterials.Clear();
        if (MeshObject == null) return;
        UpdatePSMesh(MeshObject);
        AddMaterialToMesh(MeshObject);
    }

    public void UpdateMeshEffect(GameObject go)
    {
        rendererMaterials.Clear();
        skinnedMaterials.Clear();
        if (go == null)
        {
            Debug.Log("You need set a gameObject");
            return;
        }
        MeshObject = go;
        UpdatePSMesh(MeshObject);
        AddMaterialToMesh(MeshObject);
    }

    private void UpdatePSMesh(GameObject go)
    {
        var ps = GetComponentsInChildren<ParticleSystem>();
        var meshRend = go.GetComponentInChildren<MeshRenderer>();
        var skinMeshRend = go.GetComponentInChildren<SkinnedMeshRenderer>();
        foreach (var particleSys in ps)
        {
            particleSys.transform.gameObject.SetActive(false);
            var sh = particleSys.shape;
            if (sh.enabled)
            {
                if (meshRend != null)
                {
                    sh.shapeType = ParticleSystemShapeType.MeshRenderer;
                    sh.meshRenderer = meshRend;
                }
                if (skinMeshRend != null)
                {
                    sh.shapeType = ParticleSystemShapeType.SkinnedMeshRenderer;
                    sh.skinnedMeshRenderer = skinMeshRend;
                }
            }
            particleSys.transform.gameObject.SetActive(true);
        }
    }

    private void AddMaterialToMesh(GameObject go)
    {
        var meshMatEffect = GetComponentInChildren<WFX_MeshMaterialEffect>();
        if (meshMatEffect == null) return;

        var meshRenderers = go.GetComponentsInChildren<MeshRenderer>();
        var skinMeshRenderers = go.GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (var meshRenderer in meshRenderers)
        {
            rendererMaterials.Add(meshRenderer.sharedMaterials);
            meshRenderer.sharedMaterials = AddToSharedMaterial(meshRenderer.sharedMaterials, meshMatEffect);
        }

        foreach (var skinMeshRenderer in skinMeshRenderers)
        {
            skinnedMaterials.Add(skinMeshRenderer.sharedMaterials);
            skinMeshRenderer.sharedMaterials = AddToSharedMaterial(skinMeshRenderer.sharedMaterials, meshMatEffect);
        }
    }

    Material[] AddToSharedMaterial(Material[] sharedMaterials, WFX_MeshMaterialEffect meshMatEffect)
    {
        if (meshMatEffect.IsFirstMaterial) return new[] { meshMatEffect.Material };
        var materials = sharedMaterials.ToList();
        for (int i = 0; i < materials.Count; i++)
        {
            if (materials[i].name.Contains(materialName)) materials.RemoveAt(i);
        }
        //meshMatEffect.Material.name = meshMatEffect.Material.name + materialName;
        materials.Add(meshMatEffect.Material);
        return materials.ToArray();
    }

    void OnDestroy()
    {
        Activation(true);
        if (MeshObject == null) return;
        var meshRenderers = MeshObject.GetComponentsInChildren<MeshRenderer>();
        var skinMeshRenderers = MeshObject.GetComponentsInChildren<SkinnedMeshRenderer>();

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            if (rendererMaterials.Count == meshRenderers.Length)
                meshRenderers[i].sharedMaterials = rendererMaterials[i];

            var materials = meshRenderers[i].sharedMaterials.ToList();
            for (int j = 0; j < materials.Count; j++)
            {
                if (materials[j].name.Contains(materialName)) materials.RemoveAt(j);
            }
            meshRenderers[i].sharedMaterials = materials.ToArray();

        }

        for (int i = 0; i < skinMeshRenderers.Length; i++)
        {
            if (skinnedMaterials.Count == skinMeshRenderers.Length)
                skinMeshRenderers[i].sharedMaterials = skinnedMaterials[i];

            var materials = skinMeshRenderers[i].sharedMaterials.ToList();
            for (int j = 0; j < materials.Count; j++)
            {
                if (materials[j].name.Contains(materialName)) materials.RemoveAt(j);
            }
            skinMeshRenderers[i].sharedMaterials = materials.ToArray();

        }
        rendererMaterials.Clear();
        skinnedMaterials.Clear();
    }
}