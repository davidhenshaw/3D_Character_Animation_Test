using System.Collections;
using System.Collections.Generic;
using UniBT;
using UnityEngine;

[CreateAssetMenu(menuName = "Behavior Tree/Root")]
public class RootSO : ScriptableObject
{
    public string displayName;

    [HideInInspector]
    [SerializeReference]
    Root _root;

    /// <summary>
    /// Returns a clone of the root stored in this ScriptableObject
    /// </summary>
    public Root GetRootClone()
    {
        if(_root == null)
        {
            _root = new Root();
        }

        return _root.CloneObject() as Root;
    }

    public Root GetRoot()
    {
        if (_root == null)
        {
            _root = new Root();
        }
        return _root;
    }

    public void SetRoot(Root root)
    {
        _root = root;
    }

    public bool HasRootReference()
    {
        return _root != null;
    }
}
