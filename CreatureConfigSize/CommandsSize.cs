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
            if (GetTarget(out GameObject target) && target.GetComponent<Creature>())
            {
                TechType techType = CraftData.GetTechType(target);
                ErrorMessage.AddMessage($"Setting size of {techType} to {modifier}");
                logger.LogMessage($"Setting size of {techType} to {modifier}");
                target.transform.localScale = new Vector3(modifier, modifier, modifier);

                //TODO!! Break down the pickupable and waterparkcreature checks into functions, then add them in here
                //We want to update whether it should have pickupable and WPC components after the change, to keep it in line with everything
                //Should also take into account whether it's already in containment, as then its initialSize should be taken into account
                //Also, it should *always* be pickupable if in containment; need a wat to remove it, right?

                CheckPickupableComponent(target, modifier);
            }
            else
            {
                if(target!=null)
                {
                    TechType techType = CraftData.GetTechType(target);
                    ErrorMessage.AddWarning($"{target} is not a creature!");
                }
                else
                {
                    ErrorMessage.AddWarning("No Target!");
                }
            }
        }
    }
}
