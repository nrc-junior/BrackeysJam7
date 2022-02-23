using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GuardaBehaviors))]
public class PathPatrolEditor : Editor {
    private GuardaBehaviors guarda;

    private bool repaint;

    private float handleSize = .25f;
    
    private SelectionInfo selectionInfo;

    
    
    void OnEnable() {
        guarda = target as GuardaBehaviors;
        selectionInfo = new SelectionInfo();
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        
        if (GUILayout.Button("Add Patrol Point")) {
            guarda.patrolPoints.Add(guarda.transform.position + Vector3.right);
            repaint = true;
        } 
    }
    
    private void OnSceneGUI() {
        Event guiEvent = Event.current;
 
        if (guiEvent.type == EventType.Repaint) {
            Draw();
        } else if (guiEvent.type == EventType.Layout && selectionInfo.mouseIsOverPoint) {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        } else {
            GetInput(guiEvent);
            if (repaint) {
                HandleUtility.Repaint();
                repaint = false;
            }
        }
    }
    void Draw() {
        for (int i = 0; i < guarda.patrolPoints.Count; i++) {
            Vector2 point = guarda.patrolPoints[i];
            
            if (i == selectionInfo.pointIdx)
                Handles.color = selectionInfo.pointIsSelected ? Color.cyan : Color.red;
            else Handles.color = Color.white;
            
            Handles.Label(point + Vector2.up, i.ToString());
            Handles.DrawSolidDisc(point, Vector3.forward, handleSize);
            if (i + 1 < guarda.patrolPoints.Count) {
                Handles.DrawDottedLine(point, guarda.patrolPoints[i+1], 1);
            }

        }
    }
    
    void GetInput(Event guiEvent) {
        Ray mouseRay = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
        float worldPlaneHeight = 0;
        float dstToDrawPlane = (worldPlaneHeight - mouseRay.origin.z) / mouseRay.direction.z;
        bool is2d = SceneView.currentDrawingSceneView.orthographic; 

        Vector3 mousePos = is2d ? mouseRay.origin : mouseRay.GetPoint(dstToDrawPlane);
        
        
        Handles.DrawWireCube(mousePos, Vector3.one);
        repaint = true;
 
        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None) 
            HandleLeftMouseDown(mousePos);

        if (guiEvent.type == EventType.MouseUp && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None)
            HandleLeftMouseUp();

        if (guiEvent.type == EventType.MouseDrag && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None)
            HandleLeftMouseDrag(mousePos);
        
        UpdateMouseOverSelection(mousePos);
    }
    // Inputs 
    
    void HandleLeftMouseDown(Vector2 mousePos) {
        if (selectionInfo.mouseIsOverPoint) {
            selectionInfo.pointIsSelected = true;
            repaint = true;
        }
    }
        
    void HandleLeftMouseUp() {
        if (selectionInfo.pointIsSelected) {
            selectionInfo.pointIsSelected = false;
            selectionInfo.pointIdx = -1;
            repaint = true;
        }
    }
    
    void HandleLeftMouseDrag(Vector2 mousePos) {
        if (selectionInfo.pointIsSelected) {    
            guarda.patrolPoints[selectionInfo.pointIdx] = mousePos;
            repaint = true;
        }
    }

    // End Inputs 
    void UpdateMouseOverSelection(Vector2 pos) {
        int mouseOverIdx = -1;
        for (int i = 0; i < guarda.patrolPoints.Count; i++) {
            if (Vector2.Distance(guarda.patrolPoints[i], pos) < handleSize) {
                mouseOverIdx = i;
                break;
            }
        }

        if (mouseOverIdx != selectionInfo.pointIdx) {
            selectionInfo.pointIdx = mouseOverIdx;
            selectionInfo.mouseIsOverPoint = mouseOverIdx != -1;
            repaint = true;
        }
        
    }
  
}

public class SelectionInfo {
    public int pointIdx = -1;
    public bool mouseIsOverPoint;
    public bool pointIsSelected;

}