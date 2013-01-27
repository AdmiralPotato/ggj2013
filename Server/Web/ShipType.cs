using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProtoBuf;

namespace WebGame
{
    public enum ShipType
    {
        Spearhead,
        Skirmisher,
        Beserker,
        Gunboat,
        Capital,
    }

    public enum BeamType
    {
        HullPiercing,
        StandardPhaser,
        SuppresionPulse,
        ShieldDampener,
        PlasmaVent,
        TractorBeam,
        RepulsorBeam,
        Tether,
        Barrage,
        SelfDestruct,
        ShadowTether
    }

    public enum ProjectileType
    {
        Torpedo,
        Skattershot,
        Hardshell,
        Knockshot,
        Nuke,
    }
}