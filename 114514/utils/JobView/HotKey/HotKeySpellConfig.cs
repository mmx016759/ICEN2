﻿
// ReSharper disable FieldCanBeMadeReadOnly.Global
namespace icen.utils.JobView.HotKey;

public struct HotKetSpell(string n, uint s, int t)
{
    public string Name = n;
    public uint spell = s;
    public int target = t;
}