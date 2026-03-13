using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PopulateFromFolderList : IEnumerable<GameObject>
{
    [SerializeField] private List<GameObject> _items = new();

    public List<GameObject> Items => _items;
    public int Count => _items.Count;
    public GameObject this[int i] => _items[i];

    public IEnumerator<GameObject> GetEnumerator() => _items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
}
