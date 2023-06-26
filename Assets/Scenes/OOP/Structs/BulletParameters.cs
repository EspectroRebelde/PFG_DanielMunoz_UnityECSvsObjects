using System;
using Scenes.Test;
using Unity.Mathematics;
using UnityEngine;

/// <summary> 
/// Struct that contains all the parameters needed to spawn a bullet. 
/// <param name="type">The type of the bullet (<see cref="BulletType"/>)</param>
/// <param name="attackArcRange">The range of the attack arc</param>
/// <param name="amountDistributed">How many bullets are distributed in the attack arc</param>
/// <param name="delayBetweenAttacks">The delay between each attack</param> 
/// </summary>
[Serializable]
public struct BulletSpawnParameters
{
    [SerializeReference]
    public IBulletBehavior type;
    [SerializeReference]
    public IBulletSpecifics specificsHolder;
    
    public int2 attackArcRange;
    public int amountDistributed; // How many bullets are distributed in the attack arc
    
    public float speed;
    public float lifeTime;
    public int damage;
    public int pierce;
    
    public float cooldown;
    
    // A ToString method to print the parameters
    public override string ToString()
    {
        return "BulletSpawnParameters:\n" +
               "type: " + type + "\n" +
               "attackArcRange: " + attackArcRange + "\n" +
               "amountDistributed: " + amountDistributed + "\n" +
               "speed: " + speed + "\n" +
               "lifeTime: " + lifeTime + "\n" +
               "damage: " + damage + "\n" + 
               "pierce: " + pierce + "\n" +
               "delayBetweenAttacks: " + cooldown + "\n";
    }
    
    public static BulletSpawnParameters GetDefault()
    {
        return new BulletSpawnParameters
        {
            type = new BulletBehavior_FixedAngle(),
            specificsHolder = new BulletSpecifics_None(),
            attackArcRange = new int2(0, 0),
            amountDistributed = 1,
            cooldown = 0f,
            speed = 1f,
            lifeTime = 1f,
            damage = 1,
            pierce = 1
        };
    }
}

/// <summary>
/// Enum that contains all the bullet behaviours.
/// Explanation of each bullet behaviour:
/// <list type="bulletBehaviour">
/// <item><description><see cref="FixedAngle"/>: The bullet is spawned with a fixed angle</description></item>
/// <item><description><see cref="RandomRange"/>: The bullet is spawned with a random angle within a range</description></item>
/// <item><description><see cref="ChangingAngle"/>: The bullet is spawned with a random angle within a range and the angle changes over time.<br/>
/// This is done by changing the spawner's forward point</description></item>
/// <item><description><see cref="Aimed"/>: The bullet is spawned with an angle aimed at the player.<br/>
/// This is done by aiming the spawner's forward to the player</description></item>
/// <item><description><see cref="Directed"/>: The bullet is homing towards the player</description></item>
/// </list>
/// </summary>
/// <deprecated>Implemented with IBulletBehaviour SerializeReference instead</deprecated>
[Obsolete("Implemented with IBulletBehaviour SerializeReference instead", false)]
public enum BulletType
{
    FixedAngle,
    RandomRange,
    ChangingAngle, // Rotate the forward 
    Aimed, // Forward is aimed at the player
    Directed
}

/// <summary>
/// A IBulletSpecifics interface
/// </summary>
/// <deprecated>Implemented with IBulletSpecific SerializeReference instead</deprecated>
[Obsolete("Implemented with IBulletSpecific SerializeReference instead", false)]
public enum BulletSpecifics
{
    None,
    Void,
    Sword,
    Shock
}