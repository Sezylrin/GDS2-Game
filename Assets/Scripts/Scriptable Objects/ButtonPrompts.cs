using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public enum ControllerButtons
{
    DpadNorth,
    DpadEast,
    DpadSouth,
    DpadWest,
    FaceNorth,
    FaceEast,
    FaceSouth,
    FaceWest,
    RightTrigger,
    RightShoulder,
    LeftTrigger,
    LeftShoulder,
    RightStick,
    LeftStick,
    Start,
    Select
}
[CreateAssetMenu(fileName = "ControllerPrompt", menuName = "ScriptableObjects/Prompts/LoadedPrompts")]

public class ButtonPrompts : ScriptableObject
{
    [SerializedDictionary("Button", "Sprite")]
    public SerializedDictionary<ControllerButtons,SerializedDictionary<ControllerScheme,Sprite>> Prompts;

}
