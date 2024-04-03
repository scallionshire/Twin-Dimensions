using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

public class TopDownManager : MonoBehaviour
{
    public void SaveCurrentScene() {
        TopDownDataScriptable newRoom = ScriptableObject.CreateInstance<TopDownDataScriptable>();

        // ------------- SAVING THE ROOM DATA -------------

        // Set the background sprite
        newRoom.backgroundSprite = GameObject.Find("Background").GetComponent<SpriteRenderer>().sprite;

        // Set the frame size
        newRoom.frameSize = GameObject.Find("Frame").GetComponent<SpriteRenderer>().size;

        // Set the glitch overlay scale
        newRoom.glitchScale = GameObject.Find("Glitch").transform.localScale;

        // Set the wall data list
        newRoom.walls = new List<WallData>();
        foreach (Transform wallTransform in GameObject.Find("Walls").transform) {
            GameObject wall = wallTransform.gameObject;
            WallData wallData = new WallData
            {
                transformData = new TransformData
                {
                    position = wall.transform.position,
                    rotation = wall.transform.rotation,
                    scale = wall.transform.localScale
                },
                colliderData = new ColliderData
                {
                    size = wall.GetComponent<BoxCollider2D>().size,
                    offset = wall.GetComponent<BoxCollider2D>().offset
                },
                position = wall.transform.position,
            };
            newRoom.walls.Add(wallData);
        }

        // Set the object data list
        newRoom.objects = new List<ObjectData>();
        foreach (Transform objTransform in GameObject.Find("Objects").transform) {
            GameObject obj = objTransform.gameObject;
            ObjectData objectData = new ObjectData
            {
                name = obj.name,
                position = obj.transform.position,
                sprite = obj.GetComponent<SpriteRenderer>().sprite,
                colliderData = obj.GetComponent<BoxCollider2D>() ? new ColliderData
                {
                    size = obj.GetComponent<BoxCollider2D>().size,
                    offset = obj.GetComponent<BoxCollider2D>().offset
                } : null,
                dialogueData = obj.GetComponent<DialogueTrigger>() ? new DialogueData
                {
                    doNotRepeat = obj.GetComponent<DialogueTrigger>().doNotRepeat,
                    isCutscene = obj.GetComponent<DialogueTrigger>().isCutscene,
                    conditionToCheck = obj.GetComponent<DialogueTrigger>().conditionToCheck,
                    preConditionDialogue = obj.GetComponent<DialogueTrigger>().noUSBDialogue,
                    postConditionDialogue = obj.GetComponent<DialogueTrigger>().withUSBDialogue
                } : null,
            };
            
            newRoom.objects.Add(objectData);
        }

        // Set the USB port data list
        newRoom.usbPorts = new List<USBPortData>();
        foreach (Transform usbPortTransform in GameObject.Find("USBPorts").transform) {
            GameObject usbPort = usbPortTransform.gameObject;
            USBPortData usbPortData = new USBPortData
            {
                name = usbPort.name,
                portID = usbPort.GetComponent<USBPorts>().id,
                position = usbPort.transform.position,
                sprite = usbPort.GetComponent<SpriteRenderer>().sprite,
                colliderData = usbPort.GetComponent<BoxCollider2D>() ? new ColliderData
                {
                    size = usbPort.GetComponent<BoxCollider2D>().size,
                    offset = usbPort.GetComponent<BoxCollider2D>().offset
                } : null,
                dialogueData = usbPort.GetComponent<DialogueTrigger>() ? new DialogueData
                {
                    doNotRepeat = usbPort.GetComponent<DialogueTrigger>().doNotRepeat,
                    isCutscene = usbPort.GetComponent<DialogueTrigger>().isCutscene,
                    conditionToCheck = usbPort.GetComponent<DialogueTrigger>().conditionToCheck,
                    preConditionDialogue = usbPort.GetComponent<DialogueTrigger>().noUSBDialogue,
                    postConditionDialogue = usbPort.GetComponent<DialogueTrigger>().withUSBDialogue
                } : null
            };
            newRoom.usbPorts.Add(usbPortData);
        }

        // Set the extrudable data list
        newRoom.extrudables = new List<ExtrudableData>();
        foreach (Transform extrudableTransform in GameObject.Find("Extrudables").transform) {
            GameObject extrudable = extrudableTransform.gameObject;
            ExtrudableData extrudableData = new ExtrudableData
            {
                id = extrudable.GetComponent<Extrudable>().extrudableId,
                transformData = new TransformData
                {
                    position = extrudable.transform.position,
                    rotation = extrudable.transform.rotation,
                    scale = extrudable.transform.localScale
                },
                direction = extrudable.GetComponent<Extrudable>().extrudeDirection,
                amount = extrudable.GetComponent<Extrudable>().extrudeAmount,
                shouldExtrude = extrudable.GetComponent<Extrudable>().isExtruding,
                shouldLoop = extrudable.GetComponent<Extrudable>().shouldLoop,
                alreadyExtruded = extrudable.GetComponent<Extrudable>().finishedExtruding
            };
            newRoom.extrudables.Add(extrudableData);
        }

        // Set the door data list
        newRoom.doors = new List<DoorData>();
        foreach (Transform doorTransform in GameObject.Find("Doors").transform) {
            GameObject door = doorTransform.gameObject;
            DoorData doorData = new DoorData
            {
                name = door.name,
                position = door.transform.position,
                sprite = door.GetComponent<SpriteRenderer>().sprite
            };
            newRoom.doors.Add(doorData);
        }

        // ------------- SAVING TO ASSET DATABASE -------------
        AssetDatabase.CreateAsset(newRoom, "Assets/GameData/NewRoomData.asset");
    }
}