using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Nautilus;

namespace CreatureConfigSize
{
    public class SizeChecker : MonoBehaviour
    {
        public bool SizeChanged { get; private set; } = false;

        void Start()
        {
            //TODO!! Check if this is still needed!
            //Check if the size has been changed before; if not, resize the creature
            //NOTE!! Hopefully this runs before a lot of other stuff!
            ErrorMessage.AddMessage("Component Started");
            ErrorMessage.AddMessage($"LocalScale = {transform.localScale.x}, {transform.localScale.y}, {transform.localScale.z}");
        }
    }
}
