using System;
using UnityEngine;

[Serializable]
public abstract class Event : ScriptableObject
{
    public abstract void Execute(GameManager a_sessionManager);
}
