using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataStructures
{
    public enum ParserFailType
    {
        NONE = 0,
        INPUT_TOO_LONG = 1
    }

    public enum TargetCheckType
    {
        NONE = 0,
        ITEM = 1,
        DOOR = 2
    }

    public class ParserResultArgs : EventArgs
    {
        public bool success;
        public string verb;
        public string target;
        public ParserFailType errorType;

        public ParserResultArgs(bool success, string verb, string target, ParserFailType failType)
        {
            this.success = success;
            this.verb = verb;
            this.target = target;
            this.errorType = failType;
        }
    }

    public class VerbCheckResult
    {
        public bool success;
        public string verb;

        public VerbCheckResult(bool success, string verb)
        {
            this.success = success;
            this.verb = verb;
        }
    }

    public class TargetCheckResult
    {
        public bool success;
        public TargetCheckType targetType;
        public ObjectBase target;
        public Item targetItem;
        public Door targetDoor;

        public TargetCheckResult(bool success, TargetCheckType targetType, ObjectBase target)
        {
            this.success = success;
            this.targetType = targetType;
            this.target = target;
        }
    }
}
