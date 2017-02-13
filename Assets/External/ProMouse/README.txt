DESCRIPTION:

How many times were you needing to change your mouse cursor position and you
where not able to do it because Input.mousePosition is a read only value?

With ProMouse scripting utility its now really easy to take control over the
mouse position within Unity and change it acordingly to your needs, this
package works seamlessly across Windows, OSX and linux operating systems.

ProMouse package includes all the scripts and dlls required to control your mouse position
with a simple and small API to move your mouse locally or globally in the Screen.

To find how to use this utility from code, refer to "Example.cs"

NOTE:
- All the global coordinates for this package are relative to the main screen.
- THIS PACKAGE REQUIRES UNITY PRO in order to work

For more detailed info on how to use this package, you can also check:
http://www.youtube.com/watch?v=Pe_6oKoUOnI


USAGE:

The only class you should access when using this package is the ProMouse
class.

The ProMouse class is a singleton that you can call from anywhere in your
code and exposes the following functions:


    // Sets the cursor position to (xPos, yPos) pixel coordinates relative to
    // Unity's window.
    //
    // The bottom-left of the window/screen(in full screen mode)
    // is at (0, 0). the top-right of the window/screen(in full screen mode)
    // is at (Screen.width, Screen.height)
    //
    public void SetCursorPosition(int xPos, int yPos);

    // Sets the cursor position to (xPos, yPos) pixel coordinates relative to
    // the main display
    //
    // The bottom-left of the main display is at (0, 0) and the top-right of the main display
    // is at (ProMouse.Instance.GetMainScreenSize().x, ProMouse.Instance.GetMainScreenSize().y)
    // which is your current main display resolution.
    //
    public void SetGlobalCursorPosition(int xPos, int yPos);

    // Returns the value of the global mouse position relative to the main
    // display going from 0,0 on the bottom-left corner of your main display to
    // (ProMouse.Instance.GetMainScreenSize().x, ProMouse.Instance.GetMainScreenSize().y)
    // on the top-right corner of your main display.
    //
    public Vector2 GetGlobalMousePosition();

    // Returns the value fo the local mouse position relative to Unity's
    // window, this is the same as Input.mousePosition
    //
    public Vector2 GetLocalMousePosition();

    // returns your main screen resolution, this is the same as calling
    // (Screen.currentResolution.width, Screen.currentResolution.height)
    //
    public Vector2 GetMainScreenSize();


*** Example usage. ***
In order to access any function of the ProMouse class you just need to type
"ProMouse.Instance." + the name of the function you want to call.
In example for setting the mouse position at 0,0:
    void Start() {
        ProMouse.Instance.SetCursorPosition(0,0);
    }

For checking all the functions of the ProMouse class working just refer to
the Example.cs class that comes with the project.


***** For Advanced users: *****

ProCursor.cs is a singleton class that creates a hidden GameObject to act as a way
to call a coroutine for moving our mouse as we need to wait one frame in
order to get the updated mouse position.

This coroutine is needed in order to update the mouse position and get the
correct values but you dont need to worry about that, you just need to call:

ProMouse.Instance.SetCursorPosition(pixels on x, pixels on y) or
ProMouse.Instance.SetGlobalCursorPosition(Pixels on x, pixels on y)

Depending on your needs.

In case you need to do an operation between when the mouse gets updated to
the new position after moving the mouse (which only takes one frame), there
is a flag that lets you know if there is an operation being performed on the
mouse and can be accesed like this:
(bool) ProMouse.Instance.MouseBusy; (read-only)

*** *** *** *** *** *** *** ***

KNOWN ISSUES:
The ProMouse package will work straight on any platform natively, but if you are
testing your projects in a virtual machine the mouse position gets internally
updated but you will not see your cursor moved; This is caused because the
virtual machine installs mouse drivers in order to seamlessly go from the
Host OS to the virtualized OS.

So basically what you need to do is to disable the mouse integration with the
virtual machine.

Here are some links you can check regarding vmware but they are not
assured to work: (VMware specific)
        * http://stackoverflow.com/questions/14553155/xwarppointer-does-not-work-on-ubuntu-12-04-in-wmware-player
        * http://stackoverflow.com/questions/969031/programaticly-move-mouse-in-vmware-c-sharp

Maybe this will work on other virtual machines maybe not, if it
happens to work out of the box in other virtualizing software just share it
in the comments so people can know where it works.

/***** Linux 64 *****/
When Building for Linux_64, just on the build folder, copy libMouseLib64.so
to be copied to _Data/Plugins/x86_64 and change the name to libMouseLib.so (remove the "64")


If you are still having problems working with the provided dlls, I've
included the source code for each OS dll. Of course if you have problems you
can still mail me and I'll reply as soon as possible.

Check my other projects!
- Pro Pivot Modifier: Modify your meshes pivot quickly and easy.
-- https://www.assetstore.unity3d.com/en/#!/content/8913

- Pro Draw Call optimizer: Reduce drastically your drawcalls with a few clicks.
-- https://www.assetstore.unity3d.com/#/content/16538
