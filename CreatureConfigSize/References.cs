using System;
using System.Collections.Generic;

namespace CreatureConfigSize
{
    internal class References
    {
        //NOTE!! Whilst this won't make it more efficient, requiring more checks, surely making the code cleaner and more legible is better (can't imagine this would even remotely tank performance)
        //NOTE!! However, I won't be using it forever until I'm sure the switch case statement isn't more helpful, for things unique to each creature
        public static readonly TechType[][] CreatureSizeClassReference = {
            //Size Class - Small
            new TechType[] {TechType.Biter, TechType.Bladderfish, TechType.Bleeder, TechType.Blighter, TechType.Shuttlebug, TechType.Boomerang, TechType.CaveCrawler, TechType.Crash,
                TechType.Eyeye, TechType.Floater, TechType.GarryFish, TechType.HoleFish, TechType.Hoopfish, TechType.HoopfishSchool, TechType.Hoverfish, TechType.LavaLarva, TechType.LavaBoomerang,
                TechType.LavaEyeye, TechType.Mesmer, TechType.Oculus, TechType.Peeper, TechType.Reginald, TechType.Jumper, TechType.Skyray, TechType.Spadefish, TechType.Spinefish},
            //Size Class - Medium
            new TechType[] {TechType.Shocker, TechType.BoneShark, TechType.Crabsnake, TechType.CrabSquid, TechType.Cutefish, TechType.GhostRayRed, TechType.Gasopod, TechType.GhostRayBlue,
                TechType.Jellyray, TechType.LavaLizard, TechType.RabbitRay, TechType.SpineEel, TechType.Sandshark, TechType.SeaEmperorBaby, TechType.Stalker, TechType.Warper},
            //Size Class - Large (Leviathan)
            new TechType[] {TechType.GhostLeviathan, TechType.GhostLeviathanJuvenile, TechType.ReaperLeviathan, TechType.Reefback, TechType.SeaDragon, TechType.SeaEmperorJuvenile,
                TechType.SeaTreader}
        };

        #region Reference Dictionaries
        //Dictionary used to reference the min and max values a creature can be whilst still able to be picked up by the player
        public static readonly Dictionary<TechType, (float min, float max)> PickupableReference = new Dictionary<TechType, (float, float)>()
        {
            //Small Fish
            { TechType.Biter, (0.1f, 2.0f) }, //Done
            { TechType.Bladderfish, (0.1f, 2.0f) }, //Done
            { TechType.Bleeder, (0.1f, 2.5f) }, //Done
            { TechType.Blighter, (0.1f, 2.0f) }, //Done
            { TechType.Shuttlebug, (0.1f, 0.7f) }, //Blood Crawler TechType, Done
            { TechType.Boomerang, (0.1f, 1.6f) }, //Done
            { TechType.CaveCrawler, (0.1f, 1.4f) }, //Done
            { TechType.Crash,(0.1f, 1.3f) }, //Crashfish TechType, Done
            { TechType.Eyeye, (0.1f, 2.0f) }, //Done
            { TechType.Floater, (0.1f, 2.0f) }, //Done
            { TechType.GarryFish, (0.1f, 2.0f) }, //Done
            { TechType.HoleFish, (0.1f, 2.0f) }, //Done
            { TechType.Hoopfish, (0.1f, 2.0f) }, //Done
            { TechType.Hoverfish, (0.1f, 2.0f) }, //Done
            { TechType.LavaBoomerang, (0.1f, 1.6f) }, //Magmarang TechType, Done
            { TechType.LavaLarva, (0.1f, 1.0f) }, //Done
            { TechType.Mesmer,(0.1f, 1.5f) }, //Done
            { TechType.Oculus, (0.1f, 2.0f) }, //Done
            { TechType.Peeper, (0.1f, 2.0f) }, //Done
            { TechType.LavaEyeye, (0.1f, 2.0f) }, //Done
            { TechType.Reginald, (0.1f, 2.0f) }, //Done
            { TechType.Skyray, (0.1f, 1.7f) }, //Done
            { TechType.Spadefish, (0.1f, 1.5f) }, //Done
            { TechType.Spinefish, (0.1f, 2.0f) }, //Done
            { TechType.Jumper, (0.1f, 1.5f) }, //Shuttlebug TechType, Done

            //Medium Fish
            { TechType.Shocker, (0.1f, 0.1f) }, //Done
            { TechType.BoneShark, (0.1f, 0.3f) }, //Done
            { TechType.Crabsnake, (0.1f, 0.1f) }, //Done
            { TechType.CrabSquid, (0.1f, 0.2f) }, //Done
            { TechType.Cutefish, (0.1f, 1.7f) }, //Done
            { TechType.GhostRayRed, (0.1f, 0.1f) }, //Done
            { TechType.Gasopod, (0.1f, 0.2f) }, //Done
            { TechType.GhostRayBlue, (0.1f, 0.1f) }, //Done
            { TechType.Jellyray, (0.1f, 0.3f) }, //Done
            { TechType.LavaLizard, (0.1f, 0.3f) }, //Done
            { TechType.RabbitRay, (0.1f, 0.7f) }, //Done
            { TechType.SpineEel, (0.1f, 0.2f) }, //River Prowler TechType, Done
            { TechType.Sandshark, (0.1f, 0.2f) }, //Done
            { TechType.SeaEmperorBaby, (0.1f, 0.5f) }, //Done - feels cruel to grab them, but they *are* cute
            { TechType.Stalker, (0.1f, 0.3f) }, //Done
            { TechType.Warper, (0.1f, 0.3f) }, //Done

            //Leviathans
            { TechType.GhostLeviathan, (0.1f, 0.1f) }, //Done - Made pickupable at minimum size, just so it's even possible to put it in containment
            { TechType.GhostLeviathanJuvenile, (0.1f, 0.2f) }, //Done - NOTE!! Ghost Juveniles are approximately 63% the size of adults; making pickup up to 0.2 just because (easier on player to obtain)
            { TechType.ReaperLeviathan, (0.1f, 0.2f) }, //Done
            { TechType.Reefback, (0.1f, 0.1f) }, //Done - TODO!! Flora on back acts weirdly when scaled down, and they still produce large bubbles and tiger plants; want to disable them
            { TechType.ReefbackBaby, (0.1f, 0.2f) }, //Done - Baby seems to be about 25% the size of the adult; TODO!! Reefback and ReefbackBaby both share the same techtype of Reefback, despite being different models, different base sizes, and despite ReefbackBaby techtype existing
            { TechType.SeaDragon, (0.1f, 0.1f) }, //Done, Made pickupable at minimum size, just so it's even possible to put it in containment
            { TechType.SeaEmperorJuvenile, (0.1f, 0.1f) }, //Done - Made pickupable at minimum size, just so it's even possible to put it in containment
            { TechType.SeaTreader, (0.1f, 0.1f) } //Done
        };

        //Dictionary used to reference the min and max values a creature can be whilst still able to placed in an alien containment (big fish tank)
        //NOTE!! Likely need to make sure the range includes the regular size of fish that can be hatched via eggs
        //This does bring up the great question of what occurs with my mod when creatures are hatched from eggs however
        
        //NOTE 2!! Are there *any* cases where a creature that can be picked up *could not* be added to a WaterPark?
        //YES, there are; instances where a creature can fit in the tank, but can't be picked up, are those that grow too big, e.g. stalkers, where you can pick them up
        //but can't when you place them outside containment; i.e. IsOutsidePickupable = false in WPC data
        public static readonly Dictionary<TechType, (float min, float max)> WaterParkReference = new Dictionary<TechType, (float, float)>()
        {
            //NOTE!! The creature is 60% of its initial size in containment; these ranges are for their initial size! E.g. a 4x Ampeel is 2.4x in containment (the max size I'm okay with)
            //NOTE!! Ampeels and Crabsquids hatched from eggs have a different maxSize (0.12 and 0.1 respectively, as opposed to the 0.6 of most small fish); should I worry? No;...
            //...it just means creatures hatched from eggs might be smaller on average grown up, or larger on average when first hatched

            //Small Fish
            { TechType.Biter, (0.1f, 5.0f) }, //Done
            { TechType.Bladderfish, (0.1f, 5.0f) }, //Done
            { TechType.Bleeder, (0.1f, 7.0f) }, //Done
            { TechType.Blighter, (0.1f, 5.0f) }, //Done
            { TechType.Shuttlebug, (0.1f, 1.0f) }, //Blood Crawler TechType, Done - TODO!! They don't animate in the tank, and largely don't move all that much; they look awful  xD
            { TechType.Boomerang, (0.1f, 4.0f) }, //Done
            { TechType.CaveCrawler, (0.1f, 2.0f) }, //Done - TODO!! Cave Crawlers mostly work in containment, but just spam their walking animation, even when still
            { TechType.Crash, (0.1f, 4.0f) }, //Crashfish TechType, Done
            { TechType.Eyeye, (0.1f, 5.0f) }, //Done
            { TechType.Floater, (0.1f, 1.0f) }, //Done
            { TechType.GarryFish, (0.1f, 5.0f) }, //Done
            { TechType.HoleFish, (0.1f, 5.0f) }, //Done
            { TechType.Hoopfish, (0.1f, 5.0f) }, //Done
            { TechType.Hoverfish, (0.1f, 4.0f) }, //Done
            { TechType.LavaBoomerang, (0.1f, 4.0f) }, //Magmarang TechType, Done
            { TechType.LavaLarva, (0.1f, 1.5f) }, //Done
            { TechType.Mesmer, (0.1f, 2.0f) }, //Done
            { TechType.Oculus, (0.1f, 5.0f) }, //Done
            { TechType.Peeper, (0.1f, 5.0f) }, //Done
            { TechType.LavaEyeye, (0.1f, 5.0f) }, //Done
            { TechType.Reginald, (0.1f, 5.0f) }, //Done
            { TechType.Skyray, (0.1f, 1.7f) }, //Done, TODO!! Figure out how to have it start drowning if placed in Alien Containment
            //For reference, how it works is that the drowning component makes the BirdSml animated play the drowning animation; trouble is, it's part of the SkyRay component, which is disabled...
            //...when placed in Alien Containment; maybe just make it that if SkyRay is placed in there, just don't disable it's component, because it's not like it will be in there long
            { TechType.Spadefish, (0.1f, 2.5f) }, //Done
            { TechType.Spinefish, (0.1f, 5.0f) }, //Done
            { TechType.Jumper, (0.1f, 3.5f) }, //Shuttlebug TechType, Done

            //Medium Fish
            { TechType.Shocker, (0.1f, 4.0f) }, //Done
            { TechType.BoneShark, (1.0f, 1.0f) },
            { TechType.Crabsnake, (1.0f, 1.0f) },
            { TechType.CrabSquid, (1.0f, 1.0f) },
            { TechType.Cutefish, (1.0f, 1.0f) },
            { TechType.GhostRayRed, (1.0f, 1.0f) },
            { TechType.Gasopod, (1.0f, 1.0f) },
            { TechType.GhostRayBlue, (1.0f, 1.0f) },
            { TechType.Jellyray, (1.0f, 1.0f) },
            { TechType.LavaLizard, (1.0f, 1.0f) },
            { TechType.RabbitRay, (1.0f, 1.0f) },
            { TechType.SpineEel, (0.1f, 0.5f) }, //River Prowler TechType, Done
            { TechType.Sandshark, (0.1f, 0.6f) }, //Done - NOTE!! Sandshark seems to rapidly shrink and expand when up against the walls of containment...just, what?
            { TechType.SeaEmperorBaby, (0.1f, 1.0f) }, //Done
            { TechType.Stalker, (1.0f, 1.0f) },
            { TechType.Warper, (1.0f, 1.0f) }, //TODO Make the Warper eventually leave the tank, as well as maybe kill any infected fish? Or is this just mean?

            //Leviathans
            { TechType.GhostLeviathan, (0.1f, 0.1f) }, //Done - 6% size is honestly the largest it fits without clipping; so only minimum size ghost will fit
            { TechType.GhostLeviathanJuvenile, (0.1f, 0.2f) }, //Done - NOTE!! Ghost Juveniles are approximately 63% the size of adults; 0.2 works good enough in small containment
            { TechType.ReaperLeviathan, (0.1f, 0.2f) }, //Done - Reaper won't fit in the containment at anything larger than 12% size
            { TechType.Reefback, (0.1f, 0.1f) }, //Done - ERROR!! Scaling down the Reefback doesn't scale down the flora on its back, causing graphical issues
            { TechType.ReefbackBaby, (0.1f, 0.4f) }, //Done - NOTE!! Reefback and ReefbackBaby both share the same techtype of Reefback, despite being different models, different base sizes, and despite ReefbackBaby techtype existing
            { TechType.SeaDragon, (0.1f, 0.1f) }, //Done - ERROR!! Sea Dragon does not appear to swim around in the tank without Sea Dragon component being enabled
            { TechType.SeaEmperorJuvenile, (0.1f, 0.1f) }, //Done - The grown up sea emperors; 6% size is honestly the largest it fits without clipping; so only minimum size sea emperor will fit
            //{ TechType.SeaTreader, (0.1f, 0.1f) } //ERROR!! Sea Treader has no locomotion component, instead having a unique component to move on the ground, and thus breaks in containment
        };
        #endregion
    }
}