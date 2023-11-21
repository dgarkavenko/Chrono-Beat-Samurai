using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ETone
{
    E,
    A,
    D,
    G
}


[Serializable]

public struct Note
{
    public ETone Tone;
    public float Duration;
    public float Time;

}

[CreateAssetMenu(fileName = "Sheet", menuName = "Audio/Sheet", order = 1)]
public class Sheet : ScriptableObject
{
    public List<Note> Notes;

    public void AddNote(Note note)
    {
        Notes.Add(note);
    }

    public void Clear()
    {
        Notes.Clear();
    }
}