using static CreatureConfigSize.CreatureConfigSizePlugin;
using static CreatureConfigSize.CreatureConfigSize;
using Nautilus.Commands;
using UnityEngine;

namespace CreatureConfigSize
{
    public static class CommandsSize
    {
        private static bool GetTarget(out GameObject target)
        {
            Transform cameraTransform = MainCamera.camera.transform;
            //Starting position is a little forward, so we don't detect the Player accidentally instead
            Physics.Raycast(new Ray((cameraTransform.position + cameraTransform.forward * 0.15f), cameraTransform.forward), out RaycastHit hit, 100f);

            if (hit.collider != null)
            {
                target = hit.collider.gameObject;
                return target;
            }

            target = null;
            return false;
        }

        [ConsoleCommand("setsize")]
        public static void SetSizeCommand(float modifier)
        {
            //NOTE!! Need to first confirm if target is null or not before referring to it elsewhere; thus, whole thing has to be incapsulated in that if statement
            if (GetTarget(out GameObject target))
            {
                TechType techType = CraftData.GetTechType(target);

                //NOTE!! Needed to refer to parent objects with Creature component, as some creatures have child objects that interfere with the raycast
                if (target.GetComponentInParent<Creature>())
                {
                    logger.LogError($"{target}");
                    GameObject parent = target.GetComponentInParent<Creature>().gameObject;

                    logger.LogError($"{target}, {parent}, {target.GetComponent<Creature>()}, {target.GetComponentInParent<Creature>()}");

                    if (parent != null)
                    {
                        //Check if this creature's unique id is in the creature size dictionary
                        string creatureId = parent.GetComponent<PrefabIdentifier>().Id;
                        if(creatureSizeInfoList.creatureSizeDictionary.ContainsKey(creatureId))
                        {
                            //Update the localscale of the creature (if setting size of creature in containment, set the size to be 60% of what the user enters, for consistency)
                            ErrorMessage.AddMessage($"Setting size of {techType} to {modifier}");
                            logger.LogMessage($"Setting size of {techType} to {modifier}");
                            if(GetInsideWaterPark(parent)) { modifier = modifier * 0.6f; }
                            SetSize(parent, modifier);

                            //Go through checks it eligible for either pickupable or waterpark components
                            CheckPickupableComponent(parent, modifier);
                            CheckWaterParkCreatureComponent(parent, modifier);

                            //Update the size of this creature in the creature size dictionary
                            ErrorMessage.AddMessage($"Updating listed size of {techType} from {creatureSizeInfoList.creatureSizeDictionary[creatureId]} to {modifier}");
                            logger.LogMessage($"Updating listed size of {techType} from {creatureSizeInfoList.creatureSizeDictionary[creatureId]} to {modifier}");
                            creatureSizeInfoList.creatureSizeDictionary[creatureId] = modifier;
                        }
                        else
                        {
                            ErrorMessage.AddWarning("Creature is not in size dictionary!");
                            logger.LogError("Creature is not in size dictionary!");
                        }
                    }
                }
                else
                {
                    //If we target something like the ground, it'll still return something, but have TechType.None, so this is effectively "no target"
                    if (techType == TechType.None)
                    {
                        ErrorMessage.AddWarning("No Target!");
                        logger.LogError("No Target!");
                    }
                    else
                    {
                        ErrorMessage.AddWarning($"{techType} is not a creature!");
                        logger.LogError($"{techType} is not a creature!");
                    }
                }
            }
            else
            {
                //2nd "no target" if the user manages to look at absolutely nothing, like the sky
                ErrorMessage.AddWarning("No Target!");
                logger.LogError("No Target!");
            }
        }

        //DEBUG!!
        //TODO!! Check if this function is still needed or can be removed safely!
        [ConsoleCommand("fixcreature")]
        public static void FixBadCreature()
        {
            //Debug command used to reanable the creature component of a creature in a waterpark, so they animate better, but still keep them friendly
            //NOTE!! Need to first confirm if target is null or not before referring to it elsewhere; thus, whole thing has to be incapsulated in that if statement
            if (GetTarget(out GameObject target))
            {
                TechType techType = CraftData.GetTechType(target);

                //NOTE!! Needed to refer to parent objects with Creature component, as some creatures have child objects that interfere with the raycast
                if (techType is TechType.CaveCrawler || techType is TechType.Shuttlebug || techType is TechType.SeaDragon)
                {
                    logger.LogError($"{target}");
                    GameObject parent = target.GetComponentInParent<Creature>().gameObject;
                    logger.LogError($"{target.GetComponentInParent<Creature>()}");

                    logger.LogError($"{target}, {parent}, {target.GetComponent<Creature>()}, {target.GetComponentInParent<Creature>()}");

                    if (parent != null)
                    {
                        //Fix the creature!
                        //Renable the creature component, as for these creatures much of the animation is contained there
                        //Seemingly no need to make them friendly to the player, as much of their attack functions seem to get disabled when they're placed in containment
                        ErrorMessage.AddMessage($"Fixing {techType}");
                        parent.GetComponent<CaveCrawler>().enabled = true;
                        //parent.GetComponent<CaveCrawler>().friendlyToPlayer = true;
                    }
                }
                else
                {
                    //If we target something like the ground, it'll still return something, but have TechType.None, so this is effectively "no target"
                    if (techType == TechType.None)
                    {
                        ErrorMessage.AddWarning("No Target!");
                        logger.LogError("No Target!");
                    }
                    else
                    {
                        ErrorMessage.AddWarning($"{techType} is not a Cave Crawler!");
                        logger.LogError($"{techType} is not a Cave Crawler!");
                    }
                }
            }
            else
            {
                //2nd "no target" if the user manages to look at absolutely nothing, like the sky
                ErrorMessage.AddWarning("No Target!");
                logger.LogError("No Target!");
            }
        }
    }
}
