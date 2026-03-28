using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public enum BallType
{
    Dodecagon,
    Tetragon,
    Pentagon,
    Octagon,
    Decagon,
    Icosagon,
    Hexacontatetragon,
    Chiliaicositetragon
}

public enum UpgradeId
{
    BulletDamage,
    FireRate,
    LightningDamage,
    LightningBounces,
    PoisonDamagePerSec,
    PoisonDuration,
    CurrencyMultiplier
}

[System.Serializable]
public class UpgradeData
{
    public UpgradeId id;
    public string name;
    public int maxLevel;
    public float[] costs; // cost per level (index 0 = cost to go from 0->1)
    public float angle;   // position angle around center (radians)
    public float radius;  // distance from center
}

[System.Serializable]
public class SaveData
{
    public Dictionary<UpgradeId, int> upgradeLevels = new Dictionary<UpgradeId, int>();
    public double currency = 0;
}