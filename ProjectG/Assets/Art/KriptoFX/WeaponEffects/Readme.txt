version 1.1

NOTE: 
For correct work as in demo scene you need enable "HDR" on main camera and add bloom posteffect. 
https://www.assetstore.unity3d.com/en/#!/content/51515 link on free unity physically correct bloom.
Use follow settings:
Threshold 2
Radius 7
Intencity 1
High quality true
Anti flicker true

In forward mode, HDR does not work with antialiasing. So you need disable antialiasing (edit->project settings->quality)
or use deffered rendering mode.

Mesh effects works on mobile / PC / consoles with vertexlit / forward / deferred renderer and dx9, dx11, openGL. 
All effects optimized for mobile and pc. For mobile use optimized prefabs with optimized shaders.
NOTE: Mobile distortions work correctly only with script "WFX_BloomAndDistortion.cs". Add script to camera. Also, it allow you to use mobile bloom.

 

Effect using:
1) Just drag&drop prefab on scene.
2) Set the "Mesh Object" of script "PSMeshRendererUpdater".
3) Click "Update Mesh Renderer".


For creating effect in runtime, just use follow code: 

var currentInstance = Instantiate(Effect, position, new Quaternion()) as GameObject; 
var psUpdater = currentInstance.GetComponent<PSMeshRendererUpdater>();
psUpdater.UpdateMeshEffect(MeshObject);


You can change scale of effect using tranform scale of gameObject. 



If you have some questions, you can write me to email "kripto289@gmail.com" 