using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class DropFix : MonoBehaviour
{
  [Tooltip("This amount will be added to the right of the calculated width of the DropDown panel.")]
  public float Padding = 0;
  [Tooltip("Whether this should keep its DropDown panel constrained within the width of the game view (Unity automatically handles the vertical). Standard Windows operation is to run off the side of the window or screen.")]
  public bool ConstrainToCanvas = true;
  [Tooltip("If true, this will constrain its DropDown panel to the rect of the parent canvas, rather than the root. May be useful for custom windows.")]
  public bool UseParentCanvas = false;

  private ScrollRect Template;
  void Start()
  {
    Template = GetComponent<ScrollRect>();
    if (Template == null)
    {
      Debug.LogError("DropFix script needs to be on the 'Template' object, dummy.");
      return;
    }

    //Delay one frame so we can be sure the dropdown has finished its processing, in 
    // particular enabling/disabling the scrollbar.
    StartCoroutine(StartDelay());
  }

  IEnumerator StartDelay()
  {
    //Set all the dropdown text object to overflow their text.  This makes the Text set
    // its internal preferredWidth property to the needed width to show all the text.
    Text[] Items = GetComponentsInChildren<Text>();
    foreach (Text child in Items)
      child.horizontalOverflow = HorizontalWrapMode.Overflow;

    //Let the item text objects update, and also ensure the dropdown finishes its thing.
    yield return null;

    //Setting the anchor to bottom left, that is, the bottom left of the parent.
    // This is the upper left of the dropdown area.
    // Also preserving the width of the original panel.
    RectTransform RT = transform as RectTransform;
    float width = RT.rect.width;
    RT.anchorMax = new Vector2(0, 0);
    RT.anchorMin = new Vector2(0, 0);

    //Start with the calculated dropdown width and see if we need to make it bigger
    // based on the requested width and position of the text control.
    foreach (Text child in Items)
    {
      //the reported width of the text + the "Left" offset amount on the RectTransform
      float max = child.preferredWidth + ((RectTransform)child.transform).offsetMin.x;
      //Adds just a little more, so the boundary isn't right up against the last character.
      // This defaults to 5 with a fresh dropdown.
      max += Mathf.Abs(child.transform.localPosition.x);
      if (max > width)
        width = max;
    }

    //If the scroll bar is visible, we need more padding.
    Scrollbar vert = Template.verticalScrollbar;
    float scrollWidth = 0;
    if (vert != null && vert.gameObject.activeSelf)
      scrollWidth = ((RectTransform)vert.transform).rect.width;

    //combine the new minimum width, the width of the scrollbar, and any user-defined padding.
    width += scrollWidth + Padding;

    //offset is half the newly-gained width over default, so we'll need to slap that on the left.
    float offset = (width - RT.rect.width) / 2;
    RT.localPosition += new Vector3(offset, 0);

    //set new width.
    RT.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, width);

    if (!ConstrainToCanvas)
      yield break;

    // Get right edge of dropdown and right edge of root canvas,
    //  if the dropdown goes off the canvas on the right, make it go to the left instead
    Canvas rootCanvas = GetComponentInParent<Canvas>();
    if (rootCanvas != null)
    {
      //While most people will want the root canvas, some might perhaps want nested canvases to
      // instead be the limit, such as with custom windows.
      if(!UseParentCanvas)
        while (!rootCanvas.isRootCanvas)
          rootCanvas = rootCanvas.transform.parent.GetComponentInParent<Canvas>();

      Vector3[] corners = new Vector3[4];

      RT.GetWorldCorners(corners);
      float rectTransformRightEdge = corners[2].x;

      ((RectTransform)rootCanvas.transform).GetWorldCorners(corners);
      float rootCanvasRightEdge = corners[2].x;

      if (rectTransformRightEdge > rootCanvasRightEdge)
        RT.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, width);
    }
  }
}
