using System;
using System.Collections.Generic;

[Serializable]
public class ObjectBase
{
    public string id;
    public string name;
    public string shortDescription;
    public string extendedDescription;
    public List<string> extendedDescriptionBlock;
    public Verb validVerbs;
}
