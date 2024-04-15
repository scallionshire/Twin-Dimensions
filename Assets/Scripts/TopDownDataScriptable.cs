using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/TopDownDataScriptable")]
public class TopDownDataScriptable : ScriptableObject
{
    public Sprite backgroundSprite; // map sprite to display
    public Sprite backgroundMaskSprite; // background mask sprite
    public Vector2 frameSize; // frame size
    public Vector3 glitchScale; // glitch overlay scale;
    
    public List<WallData> walls; // wall data list
    public List<ObjectData> objects; // object data list
    public List<USBPortData> usbPorts; // glitch data list
    public List<ObjectData> extrudables; // dialogue data list
    public List<DoorData> doors; // door data list
}