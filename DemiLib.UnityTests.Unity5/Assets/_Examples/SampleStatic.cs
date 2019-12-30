using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SampleStatic
{
    public const string PublicConstStringField = "-->PublicConstStringField";
    public static string PublicStaticStringProperty { get { return "-->PublicStaticStringProperty"; } }
}