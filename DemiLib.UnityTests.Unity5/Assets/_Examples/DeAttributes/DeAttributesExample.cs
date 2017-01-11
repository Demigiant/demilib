using DG.DemiLib.Attributes;
using UnityEngine;

public class DeAttributesExample : MonoBehaviour
{
    const string _MainHeaderTextColor = "ffeb42";
    const string _MainHeaderBgColor = "198519";
    const FontStyle _MainHeaderFontStyle = FontStyle.Bold;
    const int _MainHeaderFontSize = 18;

    // DeHeaders examples
    [DeDivider(2, _MainHeaderBgColor, 3, -3)]
    [DeHeader("DeHeaders examples", _MainHeaderTextColor, _MainHeaderBgColor, _MainHeaderFontStyle, _MainHeaderFontSize)]
    [DeComment("Header decorator with various options for colors etc.", _MainHeaderTextColor, _MainHeaderBgColor)]
    //
    [DeHeader("A default header")]
    public float aRandomVariable = -1;
    [DeHeader("A colored header", "ffd860", "a154df")]
    [DeHeader("A colored header with custom font style and size", "ffffff", "ff5e31", FontStyle.Normal, 9)]
    [DeHeader("More custom font style and size", "380b0b", "ffc331", FontStyle.Italic, 16)]
    [DeHeader("A right aligned colored header", TextAnchor.MiddleRight, "ffffff", "327dcc")]
    [DeHeader("Another colored header then an array", "ffffff", "000000")]
    public int[] anArray = new []{ 1, 2, 3 };

    // DeColoredLabel examples
    [DeDivider(2, _MainHeaderBgColor, 3, -3)]
    [DeHeader("DeColoredLabel examples", _MainHeaderTextColor, _MainHeaderBgColor, _MainHeaderFontStyle, _MainHeaderFontSize)]
    [DeComment("Colors the label of a property in the Inspector", _MainHeaderTextColor, _MainHeaderBgColor)]
    //
    [DeColoredLabel("ffd860", "a154df")]
    public string aString0 = "Hellow";
    [DeColoredLabel("ffffff", "ff5e31")]
    public string aString1 = "Hellow again";
    [DeColoredLabel("222222", "ffc331")]
    public string aString2 = "Hellow enough";

    // DeConditional examples
    [DeDivider(2, _MainHeaderBgColor, 3, -3)]
    [DeHeader("DeConditional examples", _MainHeaderTextColor, _MainHeaderBgColor, _MainHeaderFontStyle, _MainHeaderFontSize)]
    [DeComment("You can choose to disable or completely hide a property based on given conditions.", _MainHeaderTextColor, _MainHeaderBgColor)]
    //
    [DeColoredLabel("ffffff", "222222")]
    public float conditioningFloat = 10;
    [DeConditional("conditioningFloat", 10f, Condition.Is, ConditionalBehaviour.Hide)]
    public string conditionalS00 = "Visible if conditioningFloat == 10";
    [DeConditional("conditioningFloat", 10f, Condition.LessThan)]
    public string conditionalS01 = "Enabled if conditioningFloat < 10";
    [DeConditional("conditioningFloat", 10f, Condition.GreaterOrEqual)]
    public string conditionalS02 = "Enabled if conditioningFloat >= 10";
    [DeColoredLabel("ffffff", "222222")]
    public int conditioningInt = 69;
    [DeConditional("conditioningInt", 45)]
    public string conditionalS10 = "Enabled if conditioningInt == 45";
    [DeColoredLabel("ffffff", "222222")]
    public string conditioningString = "Gino";
    [DeConditional("conditioningString", "Gino")]
    public string conditionalS20 = "Enabled if conditioningString == \"Gino\"";
    [DeConditional("conditioningString", "", Condition.IsNot)]
    public string conditionalS21 = "Enabled if conditioningString != \"\"";
    [DeColoredLabel("ffffff", "222222")]
    public bool conditioningBool = true;
    [DeConditional("conditioningBool", true)]
    public string conditionalS30 = "Enabled if conditioningBool is TRUE";
    [DeConditional("conditioningBool", false)]
    public string conditionalS31 = "Enabled if conditioningBool is FALSE";

    // DeComment examples
    [DeDivider(2, _MainHeaderBgColor, 3, -3)]
    [DeHeader("DeComment examples", _MainHeaderTextColor, _MainHeaderBgColor, _MainHeaderFontStyle, _MainHeaderFontSize)]
    [DeComment("Shows a box with the desired text, with options for colors etc.", _MainHeaderTextColor, _MainHeaderBgColor)]
    //
    [DeComment("This is how a default DeComment looks like when optional parameters are not used.")]
    [DeComment("This one is colored, gosh!\nIt almost looks like a colored DeHeader, if not for the wordWrap.", "ffd860", "a154df")]

    // DeDivider examples
    [DeDivider(2, _MainHeaderBgColor, 3, -3)]
    [DeHeader("DeDivider examples", _MainHeaderTextColor, _MainHeaderBgColor, _MainHeaderFontStyle, _MainHeaderFontSize)]
    [DeComment("Dividers, with options for color, size and margins", _MainHeaderTextColor, _MainHeaderBgColor)]
    //
    [DeDivider]
    [DeDivider(4, "ff5e31")]
    [DeDivider(1, "a154df")]

    // DeButton examples
    [DeDivider(2, _MainHeaderBgColor, 3, -3)]
    [DeHeader("DeButton examples", _MainHeaderTextColor, _MainHeaderBgColor, _MainHeaderFontStyle, _MainHeaderFontSize)]
    [DeComment("You can place as many DeButton as you want (as long as they're above a field), and use them to call any method of any class.", _MainHeaderTextColor, _MainHeaderBgColor)]
    //
    [DeButton("Method Test 0", typeof(DeAttributesExample), "SampleMethodTest0")]
    [DeButton("Method Test 1 with params", "ffcf40", typeof(DeAttributesExample), "SampleMethodTest1", 45, "hellow")]
    [DeButton("Method Test 2 (static)", "ffcf40", "ff0000", typeof(DeAttributesExample), "SampleMethodTest2")]
    [DeButton("H3 Test 3", DePosition.HThirdLeft, "444444", typeof(DeAttributesExample), "SampleMethodTest3")]
    [DeButton("H3 Test 4", DePosition.HThirdMiddle, "00ff00", typeof(DeAttributesExample), "SampleMethodTest4")]
    [DeButton("H3 Test 5", DePosition.HThirdRight, "0000ff", typeof(DeAttributesExample), "SampleMethodTest5")]
    [DeButton("H2 Test 3", DePosition.HHalfLeft, typeof(DeAttributesExample), "SampleMethodTest3")]
    [DeButton("H2 Test 4", DePosition.HHalfRight, typeof(DeAttributesExample), "SampleMethodTest4")]
    [DeButton("Non-extended Test 5", DePosition.HDefault, typeof(DeAttributesExample), "SampleMethodTest5")]
    public string run0 = "Just some buttons above me";


    public void SampleMethodTest0()
    {
        Debug.Log("Public SampleMethodTest 0 was called, wohooo!");
    }
    void SampleMethodTest1(float f, string s)
    {
        Debug.Log(string.Format("Private SampleMethodTest 1 was called with params: {0}, \"{1}\"", f, s));
    }
    static void SampleMethodTest2()
    {
        Debug.Log("Private Static SampleMethodTest2 was called, ta-dah!");
    }
    void SampleMethodTest3()
    {
        Debug.Log("Private SampleMethodTest3 was called, sha-zam!");
    }
    void SampleMethodTest4()
    {
        Debug.Log("Private SampleMethodTest4 was called, ka-pow!");
    }
    void SampleMethodTest5()
    {
        Debug.Log("Private SampleMethodTest5 was called, wa-blammo!");
    }
}