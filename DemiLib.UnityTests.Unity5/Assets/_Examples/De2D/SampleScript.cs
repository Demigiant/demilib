// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2018/06/08 12:12
// License Copyright (c) Daniele Giardini

using UnityEngine;

public class SampleScript : MonoBehaviour
{
    public void SampleIntCallback(int value)
    {
        Debug.Log("SampleIntCallback > " + value);
    }

    public void SampleStringCallback(string value)
    {
        Debug.Log("SampleIntCallback > " + value);
    }
}