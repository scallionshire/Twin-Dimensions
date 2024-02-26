using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Sentence {
    [TextArea(3, 10)]
    public string text;
    public Twin twin;
}