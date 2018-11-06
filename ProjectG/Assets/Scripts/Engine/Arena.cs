using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena
{
    public string Name { get; private set; }
    public string Path { get; private set; }

    public Arena(string path, string name)
    {
        Path = path;
        Name = name;
    }
}
