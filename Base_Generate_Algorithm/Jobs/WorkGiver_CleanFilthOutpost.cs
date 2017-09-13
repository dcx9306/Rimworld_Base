﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;   // Always needed
//using VerseBase;   // Material/Graphics handling functions are found here
using RimWorld;      // RimWorld specific functions are found here
using Verse;         // RimWorld universal objects are here
using Verse.AI;      // Needed when you do something with the AI
//using Verse.Sound; // Needed when you do something with the Sound

namespace OutpostGenerator
{
    /// <summary>
    /// WorkGiver_CleanFilthOutpost class.
    /// </summary>
    /// <author>Rikiki</author>
    /// <permission>Use this code as you want, just remember to add a link to the corresponding Ludeon forum mod release thread.
    /// Remember learning is always better than just copy/paste...</permission>
    public class WorkGiver_CleanFilthOutpost : WorkGiver_Scanner
    {
        private int MinTicksSinceThickened = 600;

        // This workgiver is specific to MiningCo. employees.
        public override bool ShouldSkip(Pawn pawn)
        {
            if ((pawn.Faction != null)
                && (pawn.Faction == OG_Util.FactionOfMiningCo))
            {
                return base.ShouldSkip(pawn);
            }
            return true;
        }

        public override ThingRequest PotentialWorkThingRequest
        {
            get
            {
                return ThingRequest.ForGroup(ThingRequestGroup.Filth);
            }
        }

        public override PathEndMode PathEndMode
        {
            get
            {
                return PathEndMode.OnCell;
            }
        }
        
        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            List<Thing> filthAroundPawnList = new List<Thing>();
            foreach (IntVec3 cell in GenRadial.RadialCellsAround(pawn.Position, 5f, true))
            {
                foreach (Thing thing in cell.GetThingList())
                {
                    if (thing is Filth)
                    {
                        filthAroundPawnList.Add(thing);
                    }
                }
            }
            return filthAroundPawnList;
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t)
        {
            Area outpostArea = OG_Util.FindOutpostArea();
            Filth filth = t as Filth;
            return filth != null
                && (outpostArea != null)
                && outpostArea.ActiveCells.Contains(t.Position)
                && pawn.CanReserveAndReach(t, PathEndMode.ClosestTouch, pawn.NormalMaxDanger(), 1)
                && filth.TicksSinceThickened >= this.MinTicksSinceThickened;
        }

        public override Job JobOnThing(Pawn pawn, Thing t)
        {
            return new Job(DefDatabase<JobDef>.GetNamed(OG_Util.JobDefName_CleanOutpost), t);
        }
    }
}
