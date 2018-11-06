using UnityEngine;

public class WFX_FPS : MonoBehaviour
{
  private readonly GUIStyle guiStyleHeader = new GUIStyle();
  float timeleft;

  private float fps;
  private int frames; // Frames drawn over the interval
 
  #region Non-public methods

  private void Awake()
  {
    guiStyleHeader.fontSize = 14;
    guiStyleHeader.normal.textColor = new Color(1, 1, 1);
  }

  private void OnGUI()
  {
     GUI.Label(new Rect(0, 0, 30, 30), "FPS: " + (int) fps, guiStyleHeader);
  }
	 
  private void Update()
  {
    timeleft -= Time.deltaTime;
    ++frames;

    if (timeleft <= 0.0) {
      fps = frames;
      timeleft = 1;
      frames = 0;
    }
  }
  #endregion
}