using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public class DebugAttribute : System.Attribute
{
    static DebugAttribute()
    {
    }
}

[System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field)]
public class DebugPropertyAttribute : System.Attribute
{
    static DebugPropertyAttribute()
    {
    }
}

