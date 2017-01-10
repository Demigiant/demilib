using DG.DemiLib.Attributes;
using UnityEngine;

public class DeAttributesExample : MonoBehaviour
{
    [DeHeader("DeHeaders examples", "ffffff", "198519", FontStyle.Bold, 14)]
    public float aFloat = 2;
    [DeHeader("A default header")]
    public float aFloat0 = 2;
    [DeHeader("A colored header", "ffd860", "a154df")]
    public float aFloat1 = 2;
    [DeHeader("A colored header with custom font style and size", "ffffff", "ff5e31", FontStyle.Normal, 9)]
    public float aFloat2 = 2;
    [DeHeader("More custom font style and size", "380b0b", "ffc331", FontStyle.Normal, 16)]
    public float aFloat3 = 2;
    [DeHeader("A right aligned colored header", TextAnchor.MiddleRight, "ffffff", "327dcc")]
    public float aFloat4 = 2;
    [DeHeader("Another colored header then an array", "ffffff", "000000")]
    public int[] anArray = new []{ 1, 2, 3 };

    [DeHeader("DeColoredLabel examples", "ffffff", "198519", FontStyle.Bold, 14)]
    [DeColoredLabel("ffd860", "a154df")]
    public string aString = "Hellow";

    [DeHeader("DeConditional examples", "ffffff", "198519", FontStyle.Bold, 14)]
    [DeComment("You can choose to disable or completely hide a property based on given conditions")]
    [DeColoredLabel("ffffff", "222222")]
    public float conditioningFloat = 10;
    [DeConditional("conditioningFloat", 10f, FloatCondition.Is, ConditionalBehaviour.Hide)]
    public string conditionalS00 = "Visible if conditioningFloat == 10";
    [DeConditional("conditioningFloat", 10f, FloatCondition.LessThan)]
    public string conditionalS01 = "Enabled if conditioningFloat < 10";
    [DeConditional("conditioningFloat", 10f, FloatCondition.GreaterOrEqual)]
    public string conditionalS02 = "Enabled if conditioningFloat >= 10";
    [DeColoredLabel("ffffff", "222222")]
    public int conditioningInt = 69;
    [DeConditional("conditioningInt", 45)]
    public string conditionalS10 = "Enabled if conditioningInt == 45";
    [DeColoredLabel("ffffff", "222222")]
    public string conditioningString = "Gino";
    [DeConditional("conditioningString", "Gino")]
    public string conditionalS20 = "Enabled if conditioningString == \"Gino\"";
    [DeConditional("conditioningString", "", StringCondition.IsNot)]
    public string conditionalS21 = "Enabled if conditioningString != \"\"";
    [DeColoredLabel("ffffff", "222222")]
    public bool conditioningBool = true;
    [DeConditional("conditioningBool", true)]
    public string conditionalS30 = "Enabled if conditioningBool is TRUE";
    [DeConditional("conditioningBool", false)]
    public string conditionalS31 = "Enabled if conditioningBool is FALSE";
}