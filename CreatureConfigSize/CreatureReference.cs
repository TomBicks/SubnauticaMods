using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace CreatureConfigSize
{
    //Class used to store *all/most* relevant info
    //Might not have eggOrChildPrefab, or CreatureInvInfo???
    //NOTE!! TechType is not mentioned here; these will be in a dictionary, with the value being this class and the key being a TechType
    internal class CreatureReference
    {
        public enum SizeClass { None, Small, Medium, Large }

        internal CreatureInvInfo invInfo { get; private set; } //The collection of values and info a creature needs in the player's inventory. Empty if already exists in-game by default
        SizeClass sizeClass = SizeClass.None; //The SizeClass of the creature (Small, Medium or Large). TODO!! Use this maybe for gestation period, or something else? Or just for simplified random sizes???
        float pickupMin, pickupMax; //The minimum and maximum size values the creature must be within to be able ot be picked up by the player
        float waterparkMin, waterparkMax; //The minimum and maximum size values the creature must be within to be added to Alien Containment
        AssetReferenceGameObject eggOrChildPrefab; //Whether the creature births anything; an egg or live-birth. Empty if doesn't give birth (for some, this means they grow into an adult)
        float bioCharge; //How much charge the creature gives in a bioreactor


        //Create a constructor to streamline defining entries
        internal CreatureReference(SizeClass sizeClass, CreatureInvInfo invInfo)
        {
            this.sizeClass = sizeClass;
            this.invInfo = invInfo;
        }
    }
}
