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
                        //Update the localscale of the creature
                        ErrorMessage.AddMessage($"Setting size of {techType} to {modifier}");
                        logger.LogMessage($"Setting size of {techType} to {modifier}");
                        parent.transform.localScale = new Vector3(modifier, modifier, modifier);

                        //Go through checks it eligible for either pickupable or waterpark components
                        CheckPickupableComponent(parent, modifier);
                        CheckWaterParkCreatureComponent(parent, modifier);
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
        [ConsoleCommand("fixcreature")]
        public static void FixBadCreature()
        {
            //Debug command used to reanable the creature component of a creature in a waterpark, so they animate better, but still keep them friendly
            //NOTE!! Need to first confirm if target is null or not before referring to it elsewhere; thus, whole thing has to be incapsulated in that if statement
            if (GetTarget(out GameObject target))
            {
                TechType techType = CraftData.GetTechType(target);

                //NOTE!! Needed to refer to parent objects with Creature component, as some creatures have child objects that interfere with the raycast
                if (target.GetComponentInParent<Creature>() is CaveCrawler)
                {
                    logger.LogError($"{target}");
                    GameObject parent = target.GetComponentInParent<Creature>().gameObject;
                    logger.LogError($"{target.GetComponentInParent<Creature>()}");

                    logger.LogError($"{target}, {parent}, {target.GetComponent<Creature>()}, {target.GetComponentInParent<Creature>()}");

                    if (parent != null)
                    {
                        //Fix the creature!
                        //Renable the creature component, as for these creatures much of the animation is contained there
                        //Then, MAYBE!!, as a precaution, turn on friendly, so they don't try to attack the player when they're 'fixed'
                        ErrorMessage.AddMessage($"Fixing {techType}");
                        parent.GetComponent<CaveCrawler>().enabled = true;
                        parent.GetComponent<CaveCrawler>().friendlyToPlayer = true;
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

        [ConsoleCommand("addtest")]
        public static void AddTestComponent()
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
                        parent.AddComponent<SizeChecker>();
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
    }
}
