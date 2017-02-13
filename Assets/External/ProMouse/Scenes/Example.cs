/*
  Created by Juan Sebastian Munoz Arango
  naruse@gmail.com
  www.pencilsquaregames.com
  all rights reserved
*/

using UnityEngine;
using System.Collections;

public class Example : MonoBehaviour {
    void Start() {
        // Sets the cursor position to (xPos, yPos) pixel coordinates relative to
        // Unity's window.
        //
        // The bottom-left of the window/screen(in full screen mode)
        // is at (0, 0). the top-right of the window/screen(in full screen mode)
        // is at (Screen.width, Screen.height)
        //
        //ProMouse.Instance.SetCursorPosition(/*int xPos*/10, /*int yPos*/10);

        // Sets the cursor position to (xPos, yPos) pixel coordinates relative to
        // the main display.
        //
        // The bottom-left of the main display is at (0, 0) and the top-right of the main display
        // is at (ProMouse.Instance.GetMainScreenSize().x, ProMouse.Instance.GetMainScreenSize().y)
        // which is your current main display resolution.
        //
        //ProMouse.Instance.SetGlobalCursorPosition(/*int xPos*/10, /*int yPos*/10);

        // Returns the value of the global mouse position relative to the main
        // display going from 0,0 on the bottom-left corner of your main display to 
        // (ProMouse.Instance.GetMainScreenSize().x, ProMouse.Instance.GetMainScreenSize().y)
        // on the top-right corner of your main display.
        //
        print("Global position: " + ProMouse.Instance.GetGlobalMousePosition());

        // Returns the value fo the local mouse position relative to Unity's
        // window, this is the same as Input.mousePosition.
        //
        print("Local  position: " + ProMouse.Instance.GetLocalMousePosition());

        // returns your main screen resolution, this is the same as calling
        // (Screen.currentResolution.width, Screen.currentResolution.height)
        //
        print("Main screen size: " + ProMouse.Instance.GetMainScreenSize());

        //set the mouse to the middle of our game screen
        print("Set mouse to center of the unity screen.");
        ProMouse.Instance.SetCursorPosition(Screen.width/2, Screen.height/2);
    }

    void OnGUI() {
        if(GUI.Button(new Rect(0, 0, 100, 20), "Go Down")) {
            ProMouse.Instance.SetCursorPosition(50, 10);
        }
        if(GUI.Button(new Rect(0, Screen.height - 20, 100, 20), "Go Right")) {
            ProMouse.Instance.SetCursorPosition(Screen.width - 50, 10);
        }
        if(GUI.Button(new Rect(Screen.width - 100, Screen.height - 20, 100, 20), "Go Up")) {
            ProMouse.Instance.SetCursorPosition(Screen.width - 50, Screen.height - 10);
        }
        if(GUI.Button(new Rect(Screen.width - 100, 0, 100, 20), "Go Left")) {
            ProMouse.Instance.SetCursorPosition(50, Screen.height - 10);
        }

        GUILayout.BeginArea(new Rect(Screen.width/2 - 125, Screen.height/2 - 75, 250 , 200));
        GUILayout.Label("<b>Window Position: " + GetWindowPosition() + "</b>");
            GUILayout.Space(10);
            GUILayout.Label("Mouse Coord  Unity: " + ProMouse.Instance.GetLocalMousePosition());
            GUILayout.Label("Mouse Coord Global: " + ProMouse.Instance.GetGlobalMousePosition());
            GUILayout.Label("Main Display  Size: " + 
                            ProMouse.Instance.GetMainScreenSize().x + 
                            "x" +
                            ProMouse.Instance.GetMainScreenSize().y);
            GUILayout.Label("Player Screen Size: " + Screen.width + "x" + Screen.height);
            GUILayout.Label("Global mouse coordinates:");
            GUILayout.BeginHorizontal();
                if(GUILayout.Button("To 0,0 global")) {
                    ProMouse.Instance.SetGlobalCursorPosition(0,0);
                }
                if(GUILayout.Button("To Scr.w,Scr.h")) {
                    ProMouse.Instance.SetGlobalCursorPosition((int)ProMouse.Instance.GetMainScreenSize().x,
                                                               (int)ProMouse.Instance.GetMainScreenSize().y);
                }
            GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    // Returns the top left position of Unity's window in pixel cordinates
    // relative to the main display coordinates that go from 0,0 on the
    // top-left corner of the main display to your main display resolution
    // on the bottom-right corner.
    //
    // Note: GetWindowPosition is relative to the main display and the location
    // for the 0,0 and the display resolution is different from the cursors 
    // position
    //
    private Vector2 GetWindowPosition() {
        Vector2 mouseOffset = ProMouse.Instance.GetGlobalMousePosition() -
            ProMouse.Instance.GetLocalMousePosition();
        return new Vector2((int) mouseOffset.x, (int)(mouseOffset.y + Screen.height));
    }
}
