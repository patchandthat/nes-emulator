using System;
using NesEmulator.Processor;

namespace NesEmulator.Client;

using RColor = Raylib_cs.Color;

public static class MappingExtensions
{
    public static RColor ToRaylib(this Color c)
    {
        return new RColor(c.Red, c.Green, c.Blue, 255);
    }

    public static string ToBinaryString(this StatusFlags flags)
    {
        byte val =((byte)(flags));
        return Convert.ToString(val, 2).PadLeft(8, '0');
    }
    
    public static string ToHexString(this byte num)
    {
        return num.ToString("X").PadLeft(2, '0');
    }
    
    public static string ToHexString(this ushort num)
    {
        return num.ToString("X").PadLeft(4, '0');
    }
}