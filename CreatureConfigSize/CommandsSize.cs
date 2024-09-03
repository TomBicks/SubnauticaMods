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
            //NOTE!! Needed to refer to parent objects with Creature component, as some creatures have child objects that interfere with the raycast
            //ERROR!! As a result of my fix however, when there is no target, an error occurs; not sure what
            if (GetTarget(out GameObject target))
            {
                GameObject parent = target.GetComponentInParent<Creature>().gameObject;
                TechType techType = CraftData.GetTechType(parent);

                ErrorMessage.AddWarning($"{target}, {parent}, {target.GetComponent<Creature>()}, {target.GetComponentInParent<Creature>()}");

                if (parent != null) 
                {
                    ErrorMessage.AddMessage($"Setting size of {techType} to {modifier}");
                    logger.LogMessage($"Setting size of {techType} to {modifier}");
                    parent.transform.localScale = new Vector3(modifier, modifier, modifier);

                    CheckPickupableComponent(parent, modifier);

                    CheckWaterParkCreatureComponent(parent, modifier);
                }
                else
                {
                    ErrorMessage.AddWarning($"{techType} is not a creature!");
                }
            }
            else
            {
                ErrorMessage.AddWarning("No Target!");
            }

            ErrorMessage.AddWarning($"{target}, {target.GetComponent<Creature>()}");
        }
    }
}
