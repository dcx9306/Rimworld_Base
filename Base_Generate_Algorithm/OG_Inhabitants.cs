﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using UnityEngine;   // Always needed
//using VerseBase;   // Material/Graphics handling functions are found here
using RimWorld;      // RimWorld specific functions are found here
using Verse;         // RimWorld universal objects are here
using Verse.AI;       // Needed when you do something with the AI
using Verse.AI.Group; // Needed when you do something with the AI
//using Verse.Sound; // Needed when you do something with the Sound

namespace OutpostGenerator
{
    /// <summary>
    /// OG_Inhabitants class.
    /// </summary>
    /// <author>Rikiki</author>
    /// <permission>Use this code as you want, just remember to add a link to the corresponding Ludeon forum mod release thread.
    /// Remember learning is always better than just copy/paste...</permission>
    public static class OG_Inhabitants
    {
        public const int officersTargetNumber = 1;
        public const int heavyGuardsTargetNumber = 1;
        public const int guardsTargetNumber = 4;
        public const int scoutsTargetNumber = 2;
        public const int techniciansTargetNumber = 4;

        // Predefined colors.
        static Color colorArmyGreenBright = new Color(50f / 255f, 100f / 255f, 0);
        static Color colorArmyGreenDark = new Color(30f / 255f, 80f / 255f, 0);
        static Color colorArmyBrown = new Color(120f / 255f, 70f / 255f, 0);
        
        static Color colorArmyWhite = new Color(220f / 255f, 220f / 255f, 220f / 255f);
        static Color colorArmyGrey = new Color(200f / 255f, 200f / 255f, 200f / 255f);
        
        static Color colorArmyPaleSand = new Color(220f / 255f, 210f / 255f, 150f / 255f);
        static Color colorArmyBrownSand = new Color(215f / 255f, 180f / 255f, 120f / 255f);

        static Color colorCivilGrey = new Color(160f / 255f, 160f / 255f, 160f / 255f);

        // Used colors.
        static Color pantColor;
        static Color shirtColor;
        static Color armorColor;
        static Color helmetColor;
        static bool needParka = false;

        public static void GenerateInhabitants(ref OG_OutpostData outpostData)
        {
            InitializeUniformColorAccordingToBiome();
            if (outpostData.isInhabited == false)
            {
                return;
            }

            // Generate soldiers.
            List<Pawn> guardsList = new List<Pawn>();
            for (int pawnIndex = 0; pawnIndex < OG_Inhabitants.officersTargetNumber; pawnIndex++)
            {
                Pawn pawn = GeneratePawn(OG_Util.OutpostOfficerDef);
                GenSpawn.Spawn(pawn, outpostData.areaSouthWestOrigin + new IntVec3(OG_BigOutpost.areaSideLength / 2 + Rand.RangeInclusive(-5, 5), 0, OG_BigOutpost.areaSideLength / 2 + Rand.RangeInclusive(-5, 5)));
                guardsList.Add(pawn);
            }
            for (int pawnIndex = 0; pawnIndex < OG_Inhabitants.heavyGuardsTargetNumber; pawnIndex++)
            {
                Pawn pawn = GeneratePawn(OG_Util.OutpostHeavyGuardDef);
                GenSpawn.Spawn(pawn, outpostData.areaSouthWestOrigin + new IntVec3(OG_BigOutpost.areaSideLength / 2 + Rand.RangeInclusive(-5, 5), 0, OG_BigOutpost.areaSideLength / 2 + Rand.RangeInclusive(-5, 5)));
                guardsList.Add(pawn);
            }
            for (int pawnIndex = 0; pawnIndex < OG_Inhabitants.guardsTargetNumber; pawnIndex++)
            {
                Pawn pawn = GeneratePawn(OG_Util.OutpostGuardDef);
                GenSpawn.Spawn(pawn, outpostData.areaSouthWestOrigin + new IntVec3(OG_BigOutpost.areaSideLength / 2 + Rand.RangeInclusive(-5, 5), 0, OG_BigOutpost.areaSideLength / 2 + Rand.RangeInclusive(-5, 5)));
                guardsList.Add(pawn);
            }
            for (int pawnIndex = 0; pawnIndex < OG_Inhabitants.scoutsTargetNumber; pawnIndex++)
            {
                Pawn pawn = GeneratePawn(OG_Util.OutpostScoutDef);
                GenSpawn.Spawn(pawn, outpostData.areaSouthWestOrigin + new IntVec3(OG_BigOutpost.areaSideLength / 2 + Rand.RangeInclusive(-5, 5), 0, OG_BigOutpost.areaSideLength / 2 + Rand.RangeInclusive(-5, 5)));
                guardsList.Add(pawn);
            }
            // Generate technicians.
            for (int pawnIndex = 0; pawnIndex < OG_Inhabitants.techniciansTargetNumber; pawnIndex++)
            {
                Pawn pawn = GeneratePawn(OG_Util.OutpostTechnicianDef);
                GenSpawn.Spawn(pawn, outpostData.areaSouthWestOrigin + new IntVec3(OG_BigOutpost.areaSideLength / 2 + Rand.RangeInclusive(-5, 5), 0, OG_BigOutpost.areaSideLength / 2 + Rand.RangeInclusive(-5, 5)));
            }
        }

        public static void InitializeUniformColorAccordingToBiome()
        {
            // Set uniforms color according to biome.
            string biomeDefName = Find.Map.Biome.defName;
            if ((biomeDefName == "IceSheet")
                || (biomeDefName == "Tundra")
                || (biomeDefName == "BorealForest"))
            {
                pantColor = colorArmyGrey;
                shirtColor = colorArmyWhite;
                armorColor = colorArmyWhite;
                helmetColor = colorArmyGrey;
                needParka = true;
            }
            else if ((biomeDefName == "AridShrubland")
                || (biomeDefName == "Desert")
                || (biomeDefName == "ExtremeDesert"))
            {
                pantColor = colorArmyBrownSand;
                shirtColor = colorArmyPaleSand;
                armorColor = colorArmyPaleSand;
                helmetColor = colorArmyBrownSand;
            }
            else // TemperateForest and TropicalRainforest.
            {
                pantColor = colorArmyBrown;
                shirtColor = colorArmyGreenBright;
                armorColor = colorArmyGreenBright;
                helmetColor = colorArmyGreenDark;
            }
        }

        /*private static void AddJoyAndComfortNeed(Pawn pawn)
        {
            // TODO: does not work anymore...
            Log.Message("Try to add joy to " + pawn.Name.ToStringShort);
            if (pawn.needs.TryGetNeed(NeedDefOf.Joy) == null)
            {
                MethodInfo addNeed = typeof(Pawn_NeedsTracker).GetMethod("AddNeed", BindingFlags.Instance | BindingFlags.NonPublic);
                addNeed.Invoke(pawn.needs, new object[]
                {
                    DefDatabase<NeedDef>.GetNamed("Joy", true)
                });
                pawn.needs.joy.CurLevel = Rand.Range(0.5f, 1f);
            }
            if (pawn.needs.comfort == null)
            {
                MethodInfo addNeed = typeof(Pawn_NeedsTracker).GetMethod("AddNeed", BindingFlags.Instance | BindingFlags.NonPublic);
                addNeed.Invoke(pawn.needs, new object[]
                {
                    DefDatabase<NeedDef>.GetNamed("Comfort", true)
                });
                pawn.needs.comfort.CurLevel = Rand.Range(0.75f, 1f);
            }
        }*/

        public static Pawn GeneratePawn(PawnKindDef kindDef)
        {
            Pawn pawn = null;

            // Generate a new pawn until it respects all criteria.
            PawnGenerationRequest request = new PawnGenerationRequest(
                kind: kindDef,
                faction: OG_Util.FactionOfMiningCo,
                forceGenerateNewPawn: true,
                mustBeCapableOfViolence: true,
                canGeneratePawnRelations: false);
            for (int tryIndex = 0; tryIndex <= 20; tryIndex++)
            { 
                pawn = PawnGenerator.GeneratePawn(kindDef, OG_Util.FactionOfMiningCo);
                if (kindDef == OG_Util.OutpostTechnicianDef)
                {
                    if (pawn.story.WorkTagIsDisabled(WorkTags.Hauling)
                        || pawn.story.WorkTagIsDisabled(WorkTags.Cleaning)
                        || pawn.story.WorkTagIsDisabled(WorkTags.ManualDumb)
                        || pawn.story.WorkTagIsDisabled(WorkTags.PlantWork)
                        || pawn.story.WorkTagIsDisabled(WorkTags.Firefighting)
                        || pawn.story.WorkTagIsDisabled(WorkTags.Scary))
                    {
                        pawn.Destroy();
                        pawn = null;
                        if (tryIndex == 20)
                        {
                            Log.Message("MiningCo. OutpostGenerator: cannot generate requested technician pawn.");
                        }
                    }
                }
                else
                {
                    if (pawn.story.WorkTagIsDisabled(WorkTags.Violent)
                        || pawn.story.WorkTagIsDisabled(WorkTags.Firefighting))
                    {
                        pawn.Destroy();
                        pawn = null;
                        if (tryIndex == 20)
                        {
                            Log.Message("MiningCo. OutpostGenerator: cannot generate requested soldier pawn.");
                        }
                    }
                }
                if (pawn != null)
                {
                    break;
                }
            };
            // Generate apparel and weapon.
            GeneratePawnApparelAndWeapon(ref pawn, kindDef);
            
            // TODO: add joy and comfort needs.

            // Enable work settings.
            pawn.workSettings.EnableAndInitialize();
            if (kindDef == OG_Util.OutpostTechnicianDef)
            {
                List<WorkTypeDef> allDefsListForReading = DefDatabase<WorkTypeDef>.AllDefsListForReading;
                for (int workTypeIndex = 0; workTypeIndex < allDefsListForReading.Count; workTypeIndex++)
                {
                    // Ensure doctor, repair and growing are enabled.
                    WorkTypeDef workTypeDef = allDefsListForReading[workTypeIndex];
                    if (workTypeDef.alwaysStartActive
                        || (workTypeDef == WorkTypeDefOf.Doctor)
                        || (workTypeDef == WorkTypeDefOf.Repair)
                        || (workTypeDef == WorkTypeDefOf.Growing))
                    {
                        if (pawn.story.WorkTypeIsDisabled(workTypeDef) == false)
                        {
                            pawn.workSettings.SetPriority(workTypeDef, 3);
                            if (workTypeDef == WorkTypeDefOf.Repair)
                            {
                                pawn.workSettings.SetPriority(workTypeDef, 4); // So dead bodies are hauled away before repairing walls.
                            }
                        }
                    }
                    else
                    {
                        pawn.workSettings.Disable(workTypeDef);
                    }
                }
            }
            // For soldiers, restrict allowed works to only firefighting and doctor (to rescue downed pawns).
            else
            {
                List<WorkTypeDef> allDefsListForReading = DefDatabase<WorkTypeDef>.AllDefsListForReading;
                for (int workTypeIndex = 0; workTypeIndex < allDefsListForReading.Count; workTypeIndex++)
                {
                    WorkTypeDef workTypeDef = allDefsListForReading[workTypeIndex];
                    if ((workTypeDef == WorkTypeDefOf.Firefighter)
                        || (workTypeDef == WorkTypeDefOf.Doctor))
                    {
                        if (pawn.story.WorkTypeIsDisabled(workTypeDef) == false)
                        {
                            pawn.workSettings.SetPriority(workTypeDef, 3);
                        }
                    }
                    else
                    {
                        pawn.workSettings.Disable(workTypeDef);
                    }
                }
            }
            // Set allowed area.
            pawn.playerSettings = new Pawn_PlayerSettings(pawn);
            pawn.playerSettings.AreaRestriction = OG_Util.FindOutpostArea();
            // Add a bonus mood boost.
            pawn.needs.mood.thoughts.memories.TryGainMemoryThought(OG_Util.MiningCoEmployeeThoughtDef);
            return pawn;
        }

        private static void GeneratePawnApparelAndWeapon(ref Pawn pawn, PawnKindDef kindDef)
        {
            if (kindDef == OG_Util.OutpostOfficerDef)
            {
                GeneratePawnApparel(ref pawn, kindDef.itemQuality, ThingDef.Named("Apparel_Pants"), ThingDef.Named("Hyperweave"), pantColor);
                GeneratePawnApparel(ref pawn, kindDef.itemQuality, ThingDef.Named("Apparel_CollarShirt"), ThingDef.Named("Hyperweave"), shirtColor);
                GeneratePawnApparel(ref pawn, kindDef.itemQuality, ThingDef.Named("Apparel_VestPlate"), null, Color.black, false);
                GeneratePawnApparel(ref pawn, kindDef.itemQuality, ThingDef.Named("Apparel_Duster"), ThingDef.Named("Hyperweave"), armorColor);
                GeneratePawnApparel(ref pawn, kindDef.itemQuality, ThingDef.Named("Apparel_CowboyHat"), ThingDef.Named("Hyperweave"), helmetColor);
                GeneratePawnWeapon(ref pawn, kindDef.itemQuality, ThingDef.Named("Gun_SniperRifle"));
            }
            else if ((kindDef == OG_Util.OutpostHeavyGuardDef)
                || (kindDef == OG_Util.OutpostGuardDef))
            {
                GeneratePawnApparel(ref pawn, kindDef.itemQuality, ThingDef.Named("Apparel_Pants"), ThingDef.Named("Synthread"), pantColor);
                GeneratePawnApparel(ref pawn, kindDef.itemQuality, ThingDef.Named("Apparel_BasicShirt"), ThingDef.Named("Synthread"), shirtColor);
                GeneratePawnApparel(ref pawn, kindDef.itemQuality, ThingDef.Named("Apparel_PowerArmor"), null, armorColor);
                GeneratePawnApparel(ref pawn, kindDef.itemQuality, ThingDef.Named("Apparel_PowerArmorHelmet"), null, helmetColor);
                if (kindDef == OG_Util.OutpostHeavyGuardDef)
                {
                    GeneratePawnWeapon(ref pawn, kindDef.itemQuality, ThingDef.Named("Gun_Minigun"));
                }
                else
                {
                    GeneratePawnWeapon(ref pawn, kindDef.itemQuality, ThingDef.Named("Gun_ChargeRifle"));
                }
            }
            else if (kindDef == OG_Util.OutpostScoutDef)
            {
                GeneratePawnApparel(ref pawn, kindDef.itemQuality, ThingDef.Named("Apparel_Pants"), ThingDef.Named("Synthread"), pantColor);
                GeneratePawnApparel(ref pawn, kindDef.itemQuality, ThingDef.Named("Apparel_BasicShirt"), ThingDef.Named("Synthread"), shirtColor);
                GeneratePawnApparel(ref pawn, kindDef.itemQuality, ThingDef.Named("Apparel_VestPlate"), null, Color.black, false);
                GeneratePawnApparel(ref pawn, kindDef.itemQuality, ThingDef.Named("Apparel_KevlarHelmet"), null, helmetColor);
                if (needParka)
                {
                    GeneratePawnApparel(ref pawn, kindDef.itemQuality, ThingDef.Named("Apparel_Parka"), ThingDef.Named("Synthread"), armorColor);
                }
                GeneratePawnWeapon(ref pawn, kindDef.itemQuality, ThingDef.Named("Gun_AssaultRifle"));
            }
            else if (kindDef == OG_Util.OutpostTechnicianDef)
            {
                GeneratePawnApparel(ref pawn, kindDef.itemQuality, ThingDef.Named("Apparel_Pants"), ThingDef.Named("Synthread"), colorCivilGrey);
                GeneratePawnApparel(ref pawn, kindDef.itemQuality, ThingDef.Named("Apparel_CollarShirt"), ThingDef.Named("Synthread"), colorCivilGrey);

                if (OG_Util.IsModActive("MiningCo. MiningHelmet"))
                {
                    GeneratePawnApparel(ref pawn, kindDef.itemQuality, ThingDef.Named("MiningHelmet"), null, Color.black, false);
                }
                else
                {
                    if (Find.MapWorldSquare.temperature < 20f)
                    {
                        // Only give a tuque if temperature is low enough.
                        GeneratePawnApparel(ref pawn, kindDef.itemQuality, ThingDef.Named("Apparel_Tuque"), ThingDef.Named("Synthread"), colorCivilGrey);
                    }
                }
                if (needParka)
                {
                    GeneratePawnApparel(ref pawn, kindDef.itemQuality, ThingDef.Named("Apparel_Parka"), ThingDef.Named("Synthread"), armorColor);
                }
                GeneratePawnWeapon(ref pawn, kindDef.itemQuality, ThingDef.Named("Gun_Pistol"));
            }

        }
        
        private static void GeneratePawnApparel(ref Pawn pawn, QualityCategory apparelQuality, ThingDef apparelDef, ThingDef apparelStuff, Color apparelColor, bool applyColor = true)
        {
            Apparel apparel = ThingMaker.MakeThing(apparelDef, apparelStuff) as Apparel;
            if (applyColor)
            {
                apparel.SetColor(apparelColor);
            }
            apparel.TryGetComp<CompQuality>().SetQuality(apparelQuality, ArtGenerationContext.Outsider);
            pawn.apparel.Wear(apparel);
        }

        private static void GeneratePawnWeapon(ref Pawn pawn, QualityCategory weaponQuality, ThingDef weaponDef)
        {
            ThingWithComps weapon = ThingMaker.MakeThing(weaponDef) as ThingWithComps;
            weapon.TryGetComp<CompQuality>().SetQuality(weaponQuality, ArtGenerationContext.Outsider);
            pawn.equipment.AddEquipment(weapon);
        }
    }
}
