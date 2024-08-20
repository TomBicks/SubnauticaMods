using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatureConfigSize
{
    internal class References
    {
        //NOTE!! Whilst this won't make it more efficient, requiring more checks, surely making the code cleaner and more legible is better (can't imagine this would even remotely tank performance)
        //NOTE!! However, I won't be using it forever until I'm sure the switch case statement isn't more helpful, for things unique to each creature
        public static readonly TechType[][] CreatureSizeClassReference = {
            new TechType[] {TechType.Biter, TechType.Bladderfish, TechType.Bleeder, TechType.Blighter, TechType.Shuttlebug, TechType.Boomerang, TechType.CaveCrawler, TechType.Crash,
                TechType.Eyeye, TechType.Floater, TechType.GarryFish, TechType.HoleFish, TechType.Hoopfish, TechType.HoopfishSchool, TechType.Hoverfish, TechType.LavaBoomerang, TechType.LavaLarva, TechType.Mesmer,
                TechType.Oculus, TechType.Peeper, TechType.LavaEyeye, TechType.Reginald, TechType.Skyray, TechType.Spadefish, TechType.Spinefish, TechType.Jumper},
            new TechType[] {TechType.Shocker, TechType.BoneShark, TechType.Crabsnake, TechType.CrabSquid, TechType.Cutefish, TechType.GhostRayRed, TechType.Gasopod, TechType.GhostRayBlue,
                TechType.Jellyray, TechType.LavaLizard, TechType.RabbitRay, TechType.SpineEel, TechType.Sandshark, TechType.SeaEmperorBaby, TechType.Stalker, TechType.Warper},
            new TechType[] {TechType.GhostLeviathan, TechType.GhostLeviathanJuvenile, TechType.ReaperLeviathan, TechType.SeaDragon, TechType.SeaEmperorBaby, TechType.SeaEmperorJuvenile,
                TechType.SeaTreader}
        };

        #region Reference Dictionaries
        //Dictionary used to reference the min and max values a creature can be whilst still able to be picked up by the player
        public static readonly Dictionary<TechType, (float min, float max)> PickupableReference = new Dictionary<TechType, (float, float)>()
        {
            //Small Fish
            { TechType.Peeper, (0.1f, 2.0f) },

            //Medium Fish
            { TechType.CrabSquid, (0.1f, 0.2f) },
            { TechType.SeaEmperorBaby, (0.1f, 0.5f) }, //The baby sea emperor newly hatched; feels cruel to grab them, but they cute

            //Leviathans
            { TechType.GhostLeviathan, (0.1f,0.1f) }, ////Made pickupable at minimum size, just so it's even possible to put it in containment
            //{ TechType.GhostLeviathanJuvenile, (0.1f,0.1f) }, //NOTE!! Is Juvenile different to adult in terms of default size? Different TechTypes either way means I have to check for both likely
            { TechType.ReaperLeviathan, (0.1f,0.2f) },
            //{ TechType.SeaDragon, (0.1f,0.1f) }, //UNTESTED!! But likely this is max size, like the sea emperor
            { TechType.SeaEmperorJuvenile, (0.1f,0.1f) }, //The grown up sea emperors; made pickupable at minimum size, just so it's even possible to put it in containment
            { TechType.SeaTreader, (0.1f,0.1f) }
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
            //NOTE!! Ampeels and Crabsquids hatched from eggs have a different maxSize (0.12 and 0.1 respectively, as opposed to the 0.6 of most small fish); should I worry?
            //Small Fish
            { TechType.Crash, (0.1f, 4.0f) },
            { TechType.Peeper, (0.1f, 5.0f) },

            //Medium Fish
            { TechType.Shocker, (0.1f, 4.0f) },

            //Leviathans
            { TechType.GhostLeviathan, (0.1f,0.1f) }, //6% size is honestly the largest it fits without clipping; so only minimum size ghost will fit
            //{ TechType.GhostLeviathanJuvenile, (0.1f,0.1f) }, //NOTE!! Is Juvenile different to adult in terms of default size? Different TechTypes either way means I have to check for both likely
            { TechType.ReaperLeviathan, (0.1f,0.2f) }, //Reaper won't fit in the containment at anything larger than 12% size
            //{ TechType.SeaDragon, (0.1f,0.1f) }, //UNTESTED!! But likely this is max size, like the sea emperor
            { TechType.SeaEmperorJuvenile, (0.1f,0.1f) }, //The grown up sea emperors; 6% size is honestly the largest it fits without clipping; so only minimum size sea emperor will fit
            //{ TechType.SeaTreader, (0.1f,0.1f) } //ERROR!! Sea Treader has no locomotion component, instead having a unique component to move on the ground, and thus breaks in containment
        };
        #endregion
    }
}