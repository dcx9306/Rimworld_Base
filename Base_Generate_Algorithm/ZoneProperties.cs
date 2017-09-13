﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;   // Always needed
using RimWorld;      // RimWorld specific functions are found here
using Verse;         // RimWorld universal objects are here
//using Verse.AI;    // Needed when you do something with the AI
//using Verse.Sound; // Needed when you do something with the Sound

namespace OutpostGenerator
{
    /// <summary>
    /// ZoneProperties class.
    /// </summary>
    /// <author>Rikiki</author>
    /// <permission>Use this code as you want, just remember to add a link to the corresponding Ludeon forum mod release thread.
    /// Remember learning is always better than just copy/paste...</permission>
    public class ZoneProperties
    {
        public ZoneType zoneType;
        public Rot4 rotation;
        public Rot4 linkedZoneRelativeRotation;

        public ZoneProperties(ZoneType zoneType, Rot4 rotation, Rot4 linkedZoneRelativeRotation)
        {
            this.zoneType = zoneType;
            this.rotation = rotation;
            this.linkedZoneRelativeRotation = linkedZoneRelativeRotation;
        }        
    }
}
