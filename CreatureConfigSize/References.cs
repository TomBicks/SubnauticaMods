﻿using System;
using System.Collections.Generic;
using Nautilus.Extensions;
using UnityEngine.AddressableAssets;

namespace CreatureConfigSize
{
    internal class References
    {
        //NOTE!! Whilst this won't make it more efficient, requiring more checks, surely making the code cleaner and more legible is better (can't imagine this would even remotely tank performance)
        //NOTE!! However, I won't be using it forever until I'm sure the switch case statement isn't more helpful, for things unique to each creature
        public static readonly TechType[][] CreatureSizeClassReference = {
            //Size Class - Small
            new TechType[] {TechType.Biter, TechType.Bladderfish, TechType.Bleeder, TechType.Blighter, TechType.Shuttlebug, TechType.Boomerang, TechType.CaveCrawler, TechType.Crash,
                TechType.Eyeye, TechType.Floater, TechType.GarryFish, TechType.HoleFish, TechType.Hoopfish, TechType.Hoverfish, TechType.LavaBoomerang, TechType.LavaLarva,
                TechType.Mesmer, TechType.Oculus, TechType.Peeper, TechType.LavaEyeye, TechType.Reginald, TechType.Skyray, TechType.Spadefish, TechType.Spinefish, TechType.Jumper},
            //Size Class - Medium
            new TechType[] {TechType.Shocker, TechType.BoneShark, TechType.Crabsnake, TechType.CrabSquid, TechType.Cutefish, TechType.GhostRayRed, TechType.Gasopod, TechType.GhostRayBlue,
                TechType.Jellyray, TechType.LavaLizard, TechType.RabbitRay, TechType.SpineEel, TechType.Sandshark, TechType.SeaEmperorBaby, TechType.Stalker, TechType.Warper},
            //Size Class - Large (Leviathan)
            new TechType[] {TechType.GhostLeviathan, TechType.GhostLeviathanJuvenile, TechType.ReaperLeviathan, TechType.Reefback, TechType.ReefbackBaby, TechType.SeaDragon, TechType.SeaEmperorJuvenile,
                TechType.SeaTreader}
        };

        #region Reference Dictionaries
        //Dictionary used to reference the min and max values a creature can be whilst still able to be picked up by the player
        public static readonly Dictionary<TechType, (float min, float max)> PickupableReference = new Dictionary<TechType, (float, float)>()
        {
            //Small Fish
            { TechType.Biter, (0.0f, 2.0f) }, //Done
            { TechType.Bladderfish, (0.0f, 2.0f) }, //Done
            { TechType.Bleeder, (0.0f, 2.5f) }, //Done
            { TechType.Blighter, (0.0f, 2.0f) }, //Done
            { TechType.Shuttlebug, (0.0f, 0.7f) }, //Blood Crawler TechType, Done
            { TechType.Boomerang, (0.0f, 1.6f) }, //Done
            { TechType.CaveCrawler, (0.0f, 1.4f) }, //Done
            { TechType.Crash,(0.0f, 1.3f) }, //Crashfish TechType, Done
            { TechType.Eyeye, (0.0f, 2.0f) }, //Done
            { TechType.Floater, (0.0f, 2.0f) }, //Done
            { TechType.GarryFish, (0.0f, 2.0f) }, //Done
            { TechType.HoleFish, (0.0f, 2.0f) }, //Done
            { TechType.Hoopfish, (0.0f, 2.0f) }, //Done
            { TechType.Hoverfish, (0.0f, 2.0f) }, //Done
            { TechType.LavaBoomerang, (0.0f, 1.6f) }, //Magmarang TechType, Done
            { TechType.LavaLarva, (0.0f, 1.0f) }, //Done
            { TechType.Mesmer,(0.0f, 1.5f) }, //Done
            { TechType.Oculus, (0.0f, 2.0f) }, //Done
            { TechType.Peeper, (0.0f, 2.0f) }, //Done
            { TechType.LavaEyeye, (0.0f, 2.0f) }, //Done
            { TechType.Reginald, (0.0f, 2.0f) }, //Done
            { TechType.Skyray, (0.0f, 1.7f) }, //Done
            { TechType.Spadefish, (0.0f, 1.5f) }, //Done
            { TechType.Spinefish, (0.0f, 2.0f) }, //Done
            { TechType.Jumper, (0.0f, 1.5f) }, //Shuttlebug TechType, Done

            //Medium Fish
            { TechType.Shocker, (0.0f, 0.1f) }, //Done
            { TechType.BoneShark, (0.0f, 0.3f) }, //Done
            { TechType.Crabsnake, (0.0f, 0.1f) }, //Done
            { TechType.CrabSquid, (0.0f, 0.2f) }, //Done
            { TechType.Cutefish, (0.0f, 1.7f) }, //Done
            { TechType.GhostRayRed, (0.0f, 0.1f) }, //Done
            { TechType.Gasopod, (0.0f, 0.2f) }, //Done
            { TechType.GhostRayBlue, (0.0f, 0.1f) }, //Done
            { TechType.Jellyray, (0.0f, 0.3f) }, //Done
            { TechType.LavaLizard, (0.0f, 0.3f) }, //Done
            { TechType.RabbitRay, (0.0f, 0.7f) }, //Done
            { TechType.SpineEel, (0.0f, 0.2f) }, //River Prowler TechType, Done
            { TechType.Sandshark, (0.0f, 0.2f) }, //Done
            { TechType.SeaEmperorBaby, (0.0f, 0.5f) }, //Done - feels cruel to grab them, but they *are* cute
            { TechType.Stalker, (0.0f, 0.3f) }, //Done
            { TechType.Warper, (0.0f, 0.3f) }, //Done

            //Leviathans
            { TechType.GhostLeviathan, (0.0f, 0.1f) }, //Done - Made pickupable at minimum size, just so it's even possible to put it in containment
            { TechType.GhostLeviathanJuvenile, (0.0f, 0.2f) }, //Done - NOTE!! Ghost Juveniles are approximately 63% the size of adults; making pickup up to 0.2 just because (easier on player to obtain)
            { TechType.ReaperLeviathan, (0.0f, 0.2f) }, //Done
            { TechType.Reefback, (0.0f, 0.1f) }, //Done - TODO!! Flora on back acts weirdly when scaled down, and they still produce large bubbles and tiger plants; want to disable them
            { TechType.ReefbackBaby, (0.0f, 0.2f) }, //Done - Baby seems to be about 25% the size of the adult
            { TechType.SeaDragon, (0.0f, 0.1f) }, //Done, Made pickupable at minimum size, just so it's even possible to put it in containment
            { TechType.SeaEmperorJuvenile, (0.0f, 0.1f) }, //Done - Made pickupable at minimum size, just so it's even possible to put it in containment
            { TechType.SeaTreader, (0.0f, 0.1f) } //Done
        };

        //Dictionary used to reference the min and max values a creature can be whilst still able to placed in an alien containment (big fish tank)
        //NOTE!! Likely need to make sure the range includes the regular size of fish that can be hatched via eggs
        //This does bring up the great question of what occurs with my mod when creatures are hatched from eggs however

        //NOTE 2!! Are there *any* cases where a creature that can be picked up *could not* be added to a WaterPark?
        //YES, there are; instances where a creature can fit in the tank, but can't be picked up, are those that grow too big, e.g. stalkers, where you can pick them up
        //but can't when you place them outside containment; i.e. IsOutsidePickupable = false in WPC data
        public static readonly Dictionary<TechType, (float min, float max)> WaterParkReference = new Dictionary<TechType, (float, float)>()
        {
            //NOTE!! The creature is 60% of its initial size in containment; these ranges are for their initial size! E.g. a 0.6x Ampeel is 0.36x in containment (the max size I'm okay with)
            //NOTE!! Ampeels and Crabsquids hatched from eggs have a different maxSize (0.12 and 0.1 respectively, as opposed to the 0.6 of most small fish); however...
            //...This is not an issue, as creatures hatching from eggs still randomise size. The only issue is their present size is not recognised as their full size outside

            //Small Fish
            { TechType.Biter, (0.0f, 5.0f) }, //Done
            { TechType.Bladderfish, (0.0f, 5.0f) }, //Done
            { TechType.Bleeder, (0.0f, 7.0f) }, //Done
            { TechType.Blighter, (0.0f, 5.0f) }, //Done
            { TechType.Shuttlebug, (0.0f, 0.7f) }, //Blood Crawler TechType, Don
            { TechType.Boomerang, (0.0f, 4.0f) }, //Done
            { TechType.CaveCrawler, (0.0f, 2.0f) }, //Done
            { TechType.Crash, (0.0f, 4.0f) }, //Crashfish TechType, Done
            { TechType.Eyeye, (0.0f, 5.0f) }, //Done
            { TechType.Floater, (0.0f, 1.0f) }, //Done
            { TechType.GarryFish, (0.0f, 5.0f) }, //Done
            { TechType.HoleFish, (0.0f, 5.0f) }, //Done
            { TechType.Hoopfish, (0.0f, 5.0f) }, //Done
            { TechType.Hoverfish, (0.0f, 4.0f) }, //Done
            { TechType.LavaBoomerang, (0.0f, 4.0f) }, //Magmarang TechType, Done
            { TechType.LavaLarva, (0.0f, 1.5f) }, //Done
            { TechType.Mesmer, (0.0f, 2.0f) }, //Done
            { TechType.Oculus, (0.0f, 5.0f) }, //Done
            { TechType.Peeper, (0.0f, 5.0f) }, //Done
            { TechType.LavaEyeye, (0.0f, 5.0f) }, //Done
            { TechType.Reginald, (0.0f, 5.0f) }, //Done
            { TechType.Skyray, (0.0f, 1.7f) }, //Done
            { TechType.Spadefish, (0.0f, 2.5f) }, //Done
            { TechType.Spinefish, (0.0f, 5.0f) }, //Done
            { TechType.Jumper, (0.0f, 3.5f) }, //Shuttlebug TechType, Done

            //Medium Fish
            { TechType.Shocker, (0.0f, 0.6f) }, //Done
            { TechType.BoneShark, (0.0f, 0.5f) }, //Done
            { TechType.Crabsnake, (0.0f, 0.3f) }, //Done
            { TechType.CrabSquid, (0.0f, 0.2f) }, //Done
            { TechType.Cutefish, (0.0f, 2.5f) }, //Done
            { TechType.GhostRayRed, (0.0f, 0.5f) }, //Done
            { TechType.Gasopod, (0.0f, 0.7f) }, //Done
            { TechType.GhostRayBlue, (0.0f, 0.5f) }, //Done
            { TechType.Jellyray, (0.0f, 0.9f) }, //Done
            { TechType.LavaLizard, (0.0f, 1.0f) }, //Done
            { TechType.RabbitRay, (0.0f, 1.5f) }, //Done
            { TechType.SpineEel, (0.0f, 0.5f) }, //River Prowler TechType, Done
            { TechType.Sandshark, (0.0f, 0.6f) }, //Done
            { TechType.SeaEmperorBaby, (0.0f, 1.0f) }, //Done
            { TechType.Stalker, (0.0f, 0.7f) }, //Done
            { TechType.Warper, (0.0f, 1.0f) }, //Done

            //Leviathans
            { TechType.GhostLeviathan, (0.0f, 0.1f) }, //Done - 6% size is honestly the largest it fits without clipping; so only minimum size ghost will fit
            { TechType.GhostLeviathanJuvenile, (0.0f, 0.2f) }, //Done - NOTE!! Ghost Juveniles are approximately 63% the size of adults; 0.2 works good enough in small containment
            { TechType.ReaperLeviathan, (0.0f, 0.2f) }, //Done - Reaper won't fit in the containment at anything larger than 12% size
            { TechType.Reefback, (0.0f, 0.1f) }, //Done - ERROR!! Scaling down the Reefback doesn't scale down the physics of the flora on its back, causing graphical issues
            { TechType.ReefbackBaby, (0.0f, 0.4f) }, //Done
            { TechType.SeaDragon, (0.0f, 0.1f) }, //Done
            { TechType.SeaEmperorJuvenile, (0.0f, 0.1f) }, //Done - The grown up sea emperors; 6% size is honestly the largest it fits without clipping; so only minimum size sea emperor will fit
            //{ TechType.SeaTreader, (0.0f, 0.1f) } //ERROR!! Sea Treader has no locomotion component, instead having a unique component to move on the ground, and thus breaks in containment
        };
        #endregion

        //List of prefab references for creatures that can't normally breed, so we assign them a child prefab
        //Each of the AssetReferences is forcibly validated, ensuring their RuntimeKey is valid
        //NOTE!! Prefab filepaths can be found here: https://github.com/SubnauticaModding/Nautilus/blob/master/Nautilus/Documentation/resources/SN1-PrefabPaths.jsonn 
        public static readonly Dictionary<TechType, AssetReferenceGameObject> AssetPrefabReference = new Dictionary<TechType, AssetReferenceGameObject>()
        {
            //Small Fish - NOTE!! Shuttlebugs (Jumpers) have their own eggs
            { TechType.Biter, new AssetReferenceGameObject("WorldEntities/Creatures/Biter.prefab").ForceValid() },
            { TechType.Bleeder, new AssetReferenceGameObject("WorldEntities/Creatures/Bleeder.prefab").ForceValid() },
            { TechType.Blighter, new AssetReferenceGameObject("WorldEntities/Creatures/Biter_02.prefab").ForceValid() },
            { TechType.Shuttlebug, new AssetReferenceGameObject("WorldEntities/Creatures/CaveCrawler_03.prefab").ForceValid() }, //TODO!! Is this correct?
            { TechType.CaveCrawler, new AssetReferenceGameObject("WorldEntities/Creatures/CaveCrawler.prefab").ForceValid() },
            { TechType.Floater, new AssetReferenceGameObject("WorldEntities/Environment/Floater.prefab").ForceValid() },
            { TechType.LavaLarva, new AssetReferenceGameObject("WorldEntities/Creatures/LavaLarva.prefab").ForceValid() },
            //{ TechType.Skyray, new AssetReferenceGameObject("WorldEntities/Creatures/Skyray.prefab") },
            //{ TechType.Jumper, new AssetReferenceGameObject("WorldEntities/Creatures/Biter.prefab") },

            //Medium Fish
            { TechType.Cutefish, new AssetReferenceGameObject("WorldEntities/Eggs/CuteEgg.prefab").ForceValid() }, //Cutefish have eggs but can't normally lay them
            { TechType.GhostRayRed, new AssetReferenceGameObject("WorldEntities/Creatures/GhostRayRed.prefab").ForceValid() },
            { TechType.GhostRayBlue, new AssetReferenceGameObject("WorldEntities/Creatures/GhostRayBlue.prefab").ForceValid() },
            { TechType.SpineEel, new AssetReferenceGameObject("WorldEntities/Creatures/SpineEel.prefab").ForceValid() },
            //{ TechType.SeaEmperorBaby, new AssetReferenceGameObject("WorldEntities/Creatures/SeaEmperorBaby.prefab") }, //It's a baby; can't breed; probably can't grow up either though? Unless that's what adultPrefab is???
            //{ TechType.Warper, new AssetReferenceGameObject("WorldEntities/Creatures/Warper.prefab") }, //TODO!! Should they breed at all? should they warp more in? WarperSpawner?

            //Leviathans
            { TechType.GhostLeviathan, new AssetReferenceGameObject("WorldEntities/Creatures/GhostLeviathan.prefab").ForceValid() },
            //{ TechType.GhostLeviathanJuvenile, new AssetReferenceGameObject("WorldEntities/Creatures/GhostLeviathanJuvenile.prefab") },
            { TechType.ReaperLeviathan, new AssetReferenceGameObject("WorldEntities/Creatures/ReaperLeviathan.prefab").ForceValid() },
            { TechType.Reefback, new AssetReferenceGameObject("WorldEntities/Creatures/Reefback.prefab").ForceValid() },
            //{ TechType.ReefbackBaby, new AssetReferenceGameObject("WorldEntities/Creatures/ReaperLeviathan.prefab") },
            { TechType.SeaDragon, new AssetReferenceGameObject("WorldEntities/Creatures/SeaDragon.prefab").ForceValid() }, //TODO!! "WorldEntities/Environment/Precursor/LostRiverBase/Precursor_LostRiverBase_SeaDragonEggShell.prefab" works, though missing most proper physics
            { TechType.SeaEmperorJuvenile, new AssetReferenceGameObject("WorldEntities/Creatures/SeaEmperorJuvenile.prefab").ForceValid() },
            //{ TechType.SeaTreader, new AssetReferenceGameObject("WorldEntities/Creatures/SeaTreader.prefab") } //TODO!! Still doesn't work in containment
        };

        //List of charge values each creature generates when added to a Bio Reactor
        public static readonly List<(TechType, float)> BioReactorChargeReference = new List<(TechType, float)>()
        {
            //(TechType.Biter, 350f),
            (TechType.Bleeder, 410f),
            (TechType.Blighter, 410f),
            //(TechType.Shuttlebug, 280f),
            (TechType.CaveCrawler, 260f),
            //(TechType.Floater, 100f),
            (TechType.LavaLarva, 2800f),
            (TechType.GhostRayRed, 630f),
            (TechType.GhostRayBlue, 560f),
            (TechType.SpineEel, 700f),
            (TechType.GhostLeviathan, 8000f),
            (TechType.GhostLeviathanJuvenile, 7000f),
            (TechType.ReaperLeviathan, 6400f),
            //(TechType.Reefback, 820f),
            (TechType.ReefbackBaby, 620f),
            (TechType.SeaDragon, 10500f),
            (TechType.SeaEmperorJuvenile, 9000f),
            (TechType.SeaEmperorBaby, 1200f),
            (TechType.SeaTreader, 1500f),
            (TechType.Skyray, 210f),
            (TechType.Warper, 1100f)
        };
    }
}