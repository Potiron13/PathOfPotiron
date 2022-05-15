using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MockHelper
{
    public static string generateID()
    {
        return Guid.NewGuid().ToString("N");
    }
}
