﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;
using RimWorld;
//using RimWorld.Planet;


namespace OutpostGenerator
{
    /// <summary>
    /// Order a pawn to go and try to capture the outpost command console.
    /// </summary>
    public class JobDriver_TryToCaptureOutpost : JobDriver
    {
        public TargetIndex outpostCommandConsoleTarget = TargetIndex.A;
        
        public enum HackingResult
        {
            MajorFail = 0,
            MinorFail = 1,
            MinorSuccess = 2,
            MajorSuccess = 3
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Reserve.Reserve(outpostCommandConsoleTarget);

            yield return Toils_Goto.GotoCell(outpostCommandConsoleTarget, PathEndMode.InteractionCell).FailOnDestroyedOrNull(outpostCommandConsoleTarget);

            yield return Toils_General.Wait(800).FailOnDestroyedOrNull(outpostCommandConsoleTarget).WithProgressBarToilDelay(outpostCommandConsoleTarget);

            Toil outpostCaptureResultToil = new Toil()
            {
                initAction = () =>
                {
                    const float chanceToSucceedPerSkillLevel = 6f;
                    string eventTitle = "Outpost capture";
                    string eventText = "";
                    LetterType letterType = LetterType.Good;
                    Faction turretsNewFaction = null;
                    bool deactivateTurrets = false;
                    Faction doorsNewFaction = null;
                    bool deactivateDoors = false;
                    PawnKindDef securityForcesDef = null;
                    int dropPodsNumber = 0;

                    Building_OutpostCommandConsole outpostCommandConsole = this.TargetThingA as Building_OutpostCommandConsole;

                    eventText = "   MiningCo. security systems report\n\n" +
                        "Coralie here.\n" +
                        "I have treated your security parameters change request. Here is the result:\n\n";
                    HackingResult hackingresult = ComputeHackResult(this.pawn, SkillDefOf.Research, SkillDefOf.Crafting, chanceToSucceedPerSkillLevel);
                    
                    if (hackingresult == HackingResult.MajorFail)
                    {
                        eventText += "   - defense systems:     ACCESS DENIED => set mode to aggressive.\n" +
                                     "   - door access control: ACCESS DENIED => security locks engaged.\n" +
                                     "   - request checksum:    FORMAT ERROR  => Security systems hacking detected...\n" +
                                     "   => Shock support team requested!\n\n" +
                                     "--- End of transmission ---\n\n\n\n";
                        eventText += this.pawn.Name.ToStringShort + " failed to hack the outpost command console and triggered the internal securities.\n";
                        letterType = LetterType.BadUrgent;
                        turretsNewFaction = OG_Util.FactionOfMiningCo;
                        deactivateTurrets = false;
                        doorsNewFaction = OG_Util.FactionOfMiningCo;
                        deactivateDoors = false;
                        if (OG_Util.FindOrbitalRelay(OG_Util.FactionOfMiningCo) == null)
                        {
                            eventText += "Be prepared to welcome the incoming MiningCo. shock security forces!";
                            securityForcesDef = OG_Util.OutpostGuardDef;
                            dropPodsNumber = 8;
                        }
                    }
                    else if (hackingresult == HackingResult.MinorFail)
                    {
                        eventText += "   - defense systems:     ACCESS DENIED => set mode to aggressive.\n" +
                                     "   - door access control: ERROR => emergency open activated.\n" +
                                     "   - request checksum:    WRONG CRC  => Security systems hacking detected...\n" +
                                     "   => Patrol support team requested!\n\n" +
                                     "--- End of transmission ---\n\n\n\n";
                        eventText += this.pawn.Name.ToStringShort + " has some knowledge in the field of security system but did not managed to hack properly the outpost command console.\n\n";
                        letterType = LetterType.BadUrgent;
                        turretsNewFaction = OG_Util.FactionOfMiningCo;
                        deactivateTurrets = false;
                        doorsNewFaction = null;
                        deactivateDoors = true;
                        if (OG_Util.FindOrbitalRelay(OG_Util.FactionOfMiningCo) == null)
                        {
                            eventText += "Be prepared to welcome the incoming MiningCo. patrol security forces.";
                            securityForcesDef = OG_Util.OutpostScoutDef;
                            dropPodsNumber = 4;
                        }
                    }
                    else if (hackingresult == HackingResult.MinorSuccess)
                    {
                        eventText += "   - defense systems:     ERROR => systems deactivated.\n" +
                                     "   - door access control: ACCESS GRANTED\n" +
                                     "   - request checksum:    CRC OK\n" +
                                     "   => Unexpected error, technician team requested.\n\n" +
                                     "--- End of transmission ---\n\n\n\n";
                        eventText += "Your colonist managed to bypass most of the command console securities. However, " + this.pawn.Name.ToStringShort + " was not able to avoid the sending of a maintenance status report to the nearby MiningCo. comms satellite!\n\n";
                        letterType = LetterType.BadNonUrgent;
                        turretsNewFaction = null;
                        deactivateTurrets = true;
                        doorsNewFaction = Faction.OfPlayer;
                        deactivateDoors = true;
                        if (OG_Util.FindOrbitalRelay(OG_Util.FactionOfMiningCo) == null)
                        {
                            eventText += "You will soon have to welcome a technician team.";
                            securityForcesDef = OG_Util.OutpostTechnicianDef;
                            dropPodsNumber = 2;
                        }
                    }
                    else if (hackingresult == HackingResult.MajorSuccess)
                    {
                        eventText += "   - defense systems:     ACCESS GRANTED\n" +
                                     "   - door access control: ACCESS GRANTED\n" +
                                     "   - request checksum:    CRC OK\n" +
                                     "   => All parameters are valid.\n\n" +
                                     "--- End of transmission ---\n\n\n\n";
                        eventText += "Hacking the outpost command console was a child's play for " + this.pawn.Name.ToStringShort + ".\n The outpost is now fully under your control!";
                        letterType = LetterType.Good;
                        turretsNewFaction = Faction.OfPlayer;
                        deactivateTurrets = false;
                        doorsNewFaction = Faction.OfPlayer;
                        deactivateDoors = true;
                        if (OG_Util.FindOrbitalRelay(OG_Util.FactionOfMiningCo) == null)
                        {
                            securityForcesDef = null;
                            dropPodsNumber = 0;
                        }
                    }
                    outpostCommandConsole.TryToCaptureOutpost(eventTitle, eventText, letterType, turretsNewFaction, deactivateTurrets, doorsNewFaction, deactivateDoors, dropPodsNumber, securityForcesDef);
                    outpostCommandConsole.SetFaction(Faction.OfPlayer);
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            yield return outpostCaptureResultToil;

            yield return Toils_Reserve.Release(outpostCommandConsoleTarget);
        }

        public static HackingResult ComputeHackResult(Pawn hacker, SkillDef mainSkill, SkillDef secondarySkill, float chanceToSucceedPerSkillLevel)
        {
            int mainSkillLevel = 0;
            if (hacker.skills.GetSkill(mainSkill).TotallyDisabled == false)
            {
                mainSkillLevel = hacker.skills.GetSkill(mainSkill).level;
            }
            int secondarySkillLevel = 0;
            if ((secondarySkill != null)
                && (hacker.skills.GetSkill(secondarySkill).TotallyDisabled == false))
            {
                secondarySkillLevel = hacker.skills.GetSkill(secondarySkill).level;
            }
            int bestSkillLevel = Math.Max(mainSkillLevel, secondarySkillLevel);

            if (bestSkillLevel < 3)
            {
                return HackingResult.MajorFail;
            }
            else if (bestSkillLevel >= 17)
            {
                return HackingResult.MajorSuccess;
            }
            else
            {
                float luck = Rand.Value * 100;
                if (luck < bestSkillLevel * chanceToSucceedPerSkillLevel)
                {
                    // Hacking successful.
                    return HackingResult.MinorSuccess;
                }
                else
                {
                    // Bad luck.
                    return HackingResult.MinorFail;
                }
            }
        }
    }
}
