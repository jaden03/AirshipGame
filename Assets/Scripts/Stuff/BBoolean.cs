using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BBoolean
{
    public class BetterBoolean
    {
        public bool[] booleans;
        public string[] names;
    }

    public static BetterBoolean changeBoolean(BetterBoolean bboolean, string name, bool value)
    {
        for (int i = 0; i < bboolean.booleans.Length; i++)
        {
            if (bboolean.names[i] == name)
                bboolean.booleans[i] = value;
        }
        return bboolean;
    }

    public static BetterBoolean swapBoolean(BetterBoolean bboolean, string name)
    {
        for (int i = 0; i < bboolean.booleans.Length; i++)
        {
            if (bboolean.names[i] == name)
                bboolean.booleans[i] = !bboolean.booleans[i];
        }
        return bboolean;
    }

    public static bool checkBoolean(BetterBoolean bboolean, string name)
    {
        bool value = false;
        for (int i = 0; i < bboolean.booleans.Length; i++)
        {
            if (bboolean.names[i] == name)
            {
                if (bboolean.booleans[i] == true)
                    value = true;
                else
                    value = false;
            }
        }
        return value;
    }

    public static bool OR(BetterBoolean bboolean)
    {
        foreach (bool b in bboolean.booleans)
        {
            if (b)
                return true;
        }
        return false;
    }

    public static bool NOR(BetterBoolean bboolean)
    {
        foreach (bool b in bboolean.booleans)
        {
            if (!b)
                return true;
        }
        return false;
    }

    public static bool AND(BetterBoolean bboolean)
    {
        foreach (bool b in bboolean.booleans)
        {
            if (!b)
                return false;
        }

        return true;
    }

    public static bool NAND(BetterBoolean bboolean)
    {
        foreach (bool b in bboolean.booleans)
        {
            if (b)
                return false;
        }
        return true;
    }
}
