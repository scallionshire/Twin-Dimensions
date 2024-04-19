using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RoomManager : MonoBehaviour
{
    [TextArea]
    [Tooltip("Doesn't do anything. Just comments shown in inspector")]
    public string Note = "In order for TopDownManager to work, the scene MUST HAVE parent gameobjects called 'Objects', 'Doors', 'USBPorts', 'Walls', and 'Extrudables'.";

    [Header("DEBUG ONLY")]
    public bool debugMode = false;
    public TopDownDataScriptable debugRoomData;

    [Header("DO NOT TOUCH")]
    public GameObject wallPrefab;
    public GameObject doorPrefab;
    public GameObject usbPortPrefab;
    public GameObject extrudablePrefab;
    public GameObject objectPrefab;
    public GameObject dialogueTriggerPrefab;

    private int currentRoom;

    void Start()
    {
        if (debugMode) {
            WipeScene();
            LoadCurrentScene();
        }
        currentRoom = 0;
    }

    public void LoadCurrentScene() {
        TopDownDataScriptable roomData;

        if (debugMode) {
            roomData = debugRoomData;
        } else {
            currentRoom = GameManager.instance.gameState.CurrentRoom;
            roomData = GameManager.instance.roomData[currentRoom];
        }

        // ------------- LOADING THE ROOM DATA -------------

        // Set the background sprite
        GameObject.Find("Background").GetComponent<SpriteRenderer>().sprite = roomData.backgroundSprite;

        // Set the background mask sprite
        GameObject.Find("Sprite Mask").GetComponent<SpriteMask>().sprite = roomData.backgroundMaskSprite;

        // Set the frame size
        GameObject.Find("Frame").GetComponent<SpriteRenderer>().size = roomData.frameSize;

        GameObject.FindGameObjectWithTag("Player").transform.position = roomData.playerInitPosition;

        // Set the wall data list
        GameObject wallParent = GameObject.Find("Walls");
        int index = 0;
        foreach (WallData wallData in roomData.walls) {
            GameObject wall = Instantiate(wallPrefab, wallData.transformData.position, wallData.transformData.rotation, wallParent.transform);
            wall.transform.localScale = wallData.transformData.scale;
            wall.GetComponent<BoxCollider2D>().size = wallData.colliderData.size;
            wall.GetComponent<BoxCollider2D>().offset = wallData.colliderData.offset;
            wall.name = "Wall" + index;
            index++;
        }

        // Set the object data list
        GameObject objectParent = GameObject.Find("Objects");
        foreach (ObjectData objectData in roomData.objects) {
            GameObject obj = Instantiate(objectPrefab, objectData.position, Quaternion.identity, objectParent.transform);
            obj.name = objectData.name;
            obj.GetComponent<SpriteRenderer>().sprite = objectData.sprite;
            obj.GetComponent<SpriteRenderer>().sortingOrder = objectData.sortingOrder;
            obj.GetComponent<SpriteRenderer>().color = objectData.color;

            if (objectData.colliderData.noCollider == false) {
                obj.GetComponent<BoxCollider2D>().size = objectData.colliderData.size;
                obj.GetComponent<BoxCollider2D>().offset = objectData.colliderData.offset;
                obj.GetComponent<BoxCollider2D>().isTrigger = objectData.colliderData.isTrigger;
            } else {
                Destroy(obj.GetComponent<BoxCollider2D>());
            }

            if (objectData.animatorController != null) {
                if (obj.GetComponent<Animator>() == null) {
                    obj.AddComponent<Animator>();
                }
                obj.GetComponent<Animator>().runtimeAnimatorController = objectData.animatorController;
            }

            obj.GetComponent<DialogueTrigger>().doNotRepeat = objectData.dialogueData.doNotRepeat;
            obj.GetComponent<DialogueTrigger>().isCutscene = objectData.dialogueData.isCutscene;
            obj.GetComponent<DialogueTrigger>().conditionToCheck = objectData.dialogueData.conditionToCheck;
            obj.GetComponent<DialogueTrigger>().noUSBDialogue = objectData.dialogueData.preConditionDialogue;
            obj.GetComponent<DialogueTrigger>().withUSBDialogue = objectData.dialogueData.postConditionDialogue;
        }

        if (debugMode) {
            // Set the USB port data list
            GameObject usbPortParent = GameObject.Find("USBPorts");
            foreach (USBPortData usbPortData in roomData.usbPorts) {
                GameObject usbPort = Instantiate(usbPortPrefab, usbPortData.position, Quaternion.identity, usbPortParent.transform);
                usbPort.name = usbPortData.name;
                usbPort.GetComponent<USBPorts>().id = usbPortData.portID;
                usbPort.transform.position = usbPortData.position;
                usbPort.GetComponent<SpriteRenderer>().sprite = usbPortData.sprite;
                usbPort.GetComponent<USBPorts>().level = usbPortData.level;

                if (usbPortData.colliderData.noCollider == false) {
                    usbPort.GetComponent<BoxCollider2D>().size = usbPortData.colliderData.size;
                    usbPort.GetComponent<BoxCollider2D>().offset = usbPortData.colliderData.offset;
                    usbPort.GetComponent<BoxCollider2D>().isTrigger = usbPortData.colliderData.isTrigger;
                } else {
                    Destroy(usbPort.GetComponent<BoxCollider2D>());
                }

                DialogueTrigger dialogueTrigger = usbPort.GetComponent<DialogueTrigger>();
                if (dialogueTrigger != null) {
                    usbPort.GetComponent<DialogueTrigger>().doNotRepeat = usbPortData.dialogueData.doNotRepeat;
                    usbPort.GetComponent<DialogueTrigger>().isCutscene = usbPortData.dialogueData.isCutscene;
                    usbPort.GetComponent<DialogueTrigger>().conditionToCheck = usbPortData.dialogueData.conditionToCheck;
                    usbPort.GetComponent<DialogueTrigger>().noUSBDialogue = usbPortData.dialogueData.preConditionDialogue;
                    usbPort.GetComponent<DialogueTrigger>().withUSBDialogue = usbPortData.dialogueData.postConditionDialogue;
                }
            }
        } else {
            // Instantiate usb panels if they've been activated
            switch (GameManager.instance.gameState.CurrentLevel) {
                case Level.tutorial:
                    for (int i = 0; i < GameManager.instance.gameState.TutorialLevelPorts.Count; i++) {
                        if (GameManager.instance.gameState.TutorialLevelPorts[i]) {
                            ActivatePanel(i, Level.tutorial);
                        }
                    }
                    break;
                case Level.computerlab:
                    for (int i = 0; i < GameManager.instance.gameState.ComputerLabLevelPorts.Count; i++) {
                        if (GameManager.instance.gameState.ComputerLabLevelPorts[i]) {
                            ActivatePanel(i, Level.computerlab);
                        }
                    }
                    break;
            }
        }
        
        // Set the extrudable data list
        GameObject extrudableParent = GameObject.Find("Extrudables");
        foreach (ObjectData extrudableData in roomData.extrudables) {
            GameObject extrudable = Instantiate(extrudablePrefab, extrudableData.position, Quaternion.identity, extrudableParent.transform);
            extrudable.name = extrudableData.name;
            extrudable.transform.position = extrudableData.position;

            // Get extrudable ID from name of extrudable and check if it has been extruded already
            var extId = Regex.Match(extrudable.name, @"\d+$");
            if (GameManager.instance.gameState.ExtrudablesAnimPlayed2D[int.Parse(extId.Value)]) {
                if (extrudableData.animatorController != null) {
                    Destroy(extrudable.GetComponent<Animator>());
                }
                extrudable.GetComponent<SpriteRenderer>().sprite = extrudableData.extrudedSprite;
            } else {
                extrudable.GetComponent<SpriteRenderer>().sprite = extrudableData.sprite;
                if (extrudableData.animatorController != null) {
                    extrudable.GetComponent<Animator>().runtimeAnimatorController = extrudableData.animatorController;
                }
            }

            extrudable.GetComponent<SpriteRenderer>().sortingOrder = extrudableData.sortingOrder;
            extrudable.GetComponent<SpriteRenderer>().color = extrudableData.color;

            if (extrudableData.colliderData.noCollider == false) {
                extrudable.GetComponent<BoxCollider2D>().size = extrudableData.colliderData.size;
                extrudable.GetComponent<BoxCollider2D>().offset = extrudableData.colliderData.offset;
                extrudable.GetComponent<BoxCollider2D>().isTrigger = extrudableData.colliderData.isTrigger;
            } else {
                Destroy(extrudable.GetComponent<BoxCollider2D>());
            }

            if (extrudable.GetComponent<DialogueTrigger>() != null) {
                extrudable.GetComponent<DialogueTrigger>().doNotRepeat = extrudableData.dialogueData.doNotRepeat;
                extrudable.GetComponent<DialogueTrigger>().isCutscene = extrudableData.dialogueData.isCutscene;
                extrudable.GetComponent<DialogueTrigger>().conditionToCheck = extrudableData.dialogueData.conditionToCheck;
                extrudable.GetComponent<DialogueTrigger>().noUSBDialogue = extrudableData.dialogueData.preConditionDialogue;
                extrudable.GetComponent<DialogueTrigger>().withUSBDialogue = extrudableData.dialogueData.postConditionDialogue;
            }
        }

        // Set the door data list
        GameObject doorParent = GameObject.Find("Doors");
        foreach (DoorData doorData in roomData.doors) {
            if ((doorData.name == "Door0" && GameManager.instance.gameState.Door0AnimPlayed2D) || (doorData.name == "Door1" && GameManager.instance.gameState.Door1AnimPlayed2D)) {
                continue;
            }

            GameObject door = Instantiate(doorPrefab, doorData.position, Quaternion.identity, doorParent.transform);
            door.name = doorData.name;
            door.transform.position = doorData.position;
            door.GetComponent<SpriteRenderer>().sprite = doorData.sprite;
        }

        // Set the dialogue trigger data list
        GameObject dialogueParent = GameObject.Find("DialogueTriggers");
        foreach (DialogueObject dialogueObject in roomData.dialogueTriggers) {
            GameObject dialogueTrigger = Instantiate(dialogueTriggerPrefab, dialogueObject.position, Quaternion.identity, dialogueParent.transform);
            dialogueTrigger.transform.localScale = dialogueObject.scale;
            DialogueTrigger trigger = dialogueTrigger.GetComponent<DialogueTrigger>();
            trigger.withUSBDialogue = dialogueObject.dialogue;
            trigger.name = dialogueObject.dialogue.dialogueName;
            trigger.conditionToCheck = dialogueObject.conditionToCheck;
        }
    }

    public void SaveCurrentScene() {
        TopDownDataScriptable newRoom = ScriptableObject.CreateInstance<TopDownDataScriptable>();

        // ------------- SAVING THE ROOM DATA -------------

        // Set the background sprite
        newRoom.backgroundSprite = GameObject.Find("Background").GetComponent<SpriteRenderer>().sprite;

        // Set the background mask sprite
        newRoom.backgroundMaskSprite = GameObject.Find("Sprite Mask").GetComponent<SpriteMask>().sprite;

        // Set the frame size
        newRoom.frameSize = GameObject.Find("Frame").GetComponent<SpriteRenderer>().size;

        newRoom.playerInitPosition = GameObject.FindGameObjectWithTag("Player").transform.position;

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
                sortingOrder = obj.GetComponent<SpriteRenderer>().sortingOrder,
                color = obj.GetComponent<SpriteRenderer>().color,
                animatorController = obj.GetComponent<Animator>() ? obj.GetComponent<Animator>().runtimeAnimatorController : null,
                colliderData = obj.GetComponent<BoxCollider2D>() ? new ColliderData
                {
                    size = obj.GetComponent<BoxCollider2D>().size,
                    offset = obj.GetComponent<BoxCollider2D>().offset,
                    isTrigger = obj.GetComponent<BoxCollider2D>().isTrigger
                } : new ColliderData{ noCollider = true },
                dialogueData = obj.GetComponent<DialogueTrigger>() ? new DialogueData
                {
                    doNotRepeat = obj.GetComponent<DialogueTrigger>().doNotRepeat,
                    isCutscene = obj.GetComponent<DialogueTrigger>().isCutscene,
                    conditionToCheck = obj.GetComponent<DialogueTrigger>().conditionToCheck,
                    preConditionDialogue = obj.GetComponent<DialogueTrigger>().noUSBDialogue,
                    postConditionDialogue = obj.GetComponent<DialogueTrigger>().withUSBDialogue
                } : new DialogueData(),
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
                level = usbPort.GetComponent<USBPorts>().level,
                colliderData = usbPort.GetComponent<BoxCollider2D>() ? new ColliderData
                {
                    size = usbPort.GetComponent<BoxCollider2D>().size,
                    offset = usbPort.GetComponent<BoxCollider2D>().offset,
                    isTrigger = usbPort.GetComponent<BoxCollider2D>().isTrigger
                } : new ColliderData{ noCollider = true },
                dialogueData = usbPort.GetComponent<DialogueTrigger>() ? new DialogueData
                {
                    doNotRepeat = usbPort.GetComponent<DialogueTrigger>().doNotRepeat,
                    isCutscene = usbPort.GetComponent<DialogueTrigger>().isCutscene,
                    conditionToCheck = usbPort.GetComponent<DialogueTrigger>().conditionToCheck,
                    preConditionDialogue = usbPort.GetComponent<DialogueTrigger>().noUSBDialogue,
                    postConditionDialogue = usbPort.GetComponent<DialogueTrigger>().withUSBDialogue
                } : new DialogueData()
            };
            newRoom.usbPorts.Add(usbPortData);
        }

        // Set the extrudable data list
        newRoom.extrudables = new List<ObjectData>();
        foreach (Transform extrudableTransform in GameObject.Find("Extrudables").transform) {
            GameObject extrudable = extrudableTransform.gameObject;
            ObjectData extrudableData = new ObjectData
            {
                name = extrudable.name,
                position = extrudable.transform.position,
                sprite = extrudable.GetComponent<SpriteRenderer>().sprite,
                sortingOrder = extrudable.GetComponent<SpriteRenderer>().sortingOrder,
                color = extrudable.GetComponent<SpriteRenderer>().color,
                animatorController = extrudable.GetComponent<Animator>() ? extrudable.GetComponent<Animator>().runtimeAnimatorController : null,
                colliderData = extrudable.GetComponent<BoxCollider2D>() ? new ColliderData
                {
                    size = extrudable.GetComponent<BoxCollider2D>().size,
                    offset = extrudable.GetComponent<BoxCollider2D>().offset,
                    isTrigger = extrudable.GetComponent<BoxCollider2D>().isTrigger
                } : new ColliderData{ noCollider = true },
                dialogueData = extrudable.GetComponent<DialogueTrigger>() ? new DialogueData
                {
                    doNotRepeat = extrudable.GetComponent<DialogueTrigger>().doNotRepeat,
                    isCutscene = extrudable.GetComponent<DialogueTrigger>().isCutscene,
                    conditionToCheck = extrudable.GetComponent<DialogueTrigger>().conditionToCheck,
                    preConditionDialogue = extrudable.GetComponent<DialogueTrigger>().noUSBDialogue,
                    postConditionDialogue = extrudable.GetComponent<DialogueTrigger>().withUSBDialogue
                } : new DialogueData()
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

        // Set the dialogue trigger data list
        newRoom.dialogueTriggers = new List<DialogueObject>();
        foreach (Transform dialogueTransform in GameObject.Find("DialogueTriggers").transform) {
            GameObject dialogueTrigger = dialogueTransform.gameObject;
            DialogueObject dialogueObject = new DialogueObject
            {
                conditionToCheck = dialogueTrigger.GetComponent<DialogueTrigger>().conditionToCheck,
                position = dialogueTrigger.transform.position,
                scale = dialogueTrigger.transform.localScale,
                dialogue = dialogueTrigger.GetComponent<DialogueTrigger>().withUSBDialogue
            };
            newRoom.dialogueTriggers.Add(dialogueObject);
        }

        // ------------- SAVING TO ASSET DATABASE -------------
        #if UNITY_EDITOR
            AssetDatabase.CreateAsset(newRoom, "Assets/GameData/RoomData/NewRoomData.asset");
        #endif
    }

    public void WipeScene() {
        Debug.Log("Wiping scene");
        foreach (Transform wallTransform in GameObject.Find("Walls").transform) {
            Destroy(wallTransform.gameObject);
        }

        foreach (Transform objTransform in GameObject.Find("Objects").transform) {
            Destroy(objTransform.gameObject);
        }

        foreach (Transform usbPortTransform in GameObject.Find("USBPorts").transform) {
            Destroy(usbPortTransform.gameObject);
        }

        foreach (Transform extrudableTransform in GameObject.Find("Extrudables").transform) {
            Destroy(extrudableTransform.gameObject);
        }

        foreach (Transform doorTransform in GameObject.Find("Doors").transform) {
            Destroy(doorTransform.gameObject);
        }

        foreach (Transform dialogueTransform in GameObject.Find("DialogueTriggers").transform) {
            Destroy(dialogueTransform.gameObject);
        }
    }

    public void ActivatePanel(int puzzleId, Level level) {
        TopDownDataScriptable roomData = GameManager.instance.roomData[currentRoom];

        // Load in requested puzzle panel
        GameObject usbPortParent = GameObject.Find("USBPorts");
        foreach (USBPortData usbPortData in roomData.usbPorts) {
            if (usbPortData.portID == puzzleId && usbPortData.level == level) {
                if (usbPortParent.transform.Find(usbPortData.name) != null) {
                    break;
                }

                GameObject usbPort = Instantiate(usbPortPrefab, usbPortData.position, Quaternion.identity, usbPortParent.transform);

                usbPort.name = usbPortData.name;
                usbPort.GetComponent<USBPorts>().id = usbPortData.portID;
                usbPort.transform.position = usbPortData.position;
                usbPort.GetComponent<SpriteRenderer>().sprite = usbPortData.sprite;
                usbPort.GetComponent<USBPorts>().level = usbPortData.level;

                if (usbPortData.colliderData.noCollider == false) {
                    usbPort.GetComponent<BoxCollider2D>().size = usbPortData.colliderData.size;
                    usbPort.GetComponent<BoxCollider2D>().offset = usbPortData.colliderData.offset;
                    usbPort.GetComponent<BoxCollider2D>().isTrigger = usbPortData.colliderData.isTrigger;
                } else {
                    Destroy(usbPort.GetComponent<BoxCollider2D>());
                }

                DialogueTrigger dialogueTrigger = usbPort.GetComponent<DialogueTrigger>();
                if (dialogueTrigger != null) {
                    usbPort.GetComponent<DialogueTrigger>().doNotRepeat = usbPortData.dialogueData.doNotRepeat;
                    usbPort.GetComponent<DialogueTrigger>().isCutscene = usbPortData.dialogueData.isCutscene;
                    usbPort.GetComponent<DialogueTrigger>().conditionToCheck = usbPortData.dialogueData.conditionToCheck;
                    usbPort.GetComponent<DialogueTrigger>().noUSBDialogue = usbPortData.dialogueData.preConditionDialogue;
                    usbPort.GetComponent<DialogueTrigger>().withUSBDialogue = usbPortData.dialogueData.postConditionDialogue;
                }
            }
        }
    }

    public void SwapExtrudableSprite(string name) {
        GameObject extrudable = GameObject.Find(name);
        if (extrudable != null) {
            foreach (ObjectData extrudableData in GameManager.instance.roomData[currentRoom].extrudables) {
                if (extrudableData.name == name) {
                    if (extrudableData.animatorController != null) {
                        Destroy(extrudable.GetComponent<Animator>());
                    }
                    extrudable.GetComponent<SpriteRenderer>().sprite = extrudableData.extrudedSprite;
                }
            }
        }
    }
}