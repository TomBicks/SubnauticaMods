namespace CreatureConfigSize
{
    internal class CreatureInvInfo
    {
        internal TechType techType { get; set; } //The TechType of the creature
        internal int invSize; //How big the creature is in the inventory (e.g. 4 is 4*4 large in inventory)
        internal string iconName; //The filename of the icon file
        internal string name; //The displayed name of the creature in the inventory
        internal string tooltip; //The displayed tooltip of the creature in the inventory

        //Create a constructor to streamline defining entries
        internal CreatureInvInfo(TechType techType, int invSize, string iconName, string name, string tooltip)
        {
            this.techType = techType;
            this.invSize = invSize;
            this.iconName = iconName;
            this.name = name;
            this.tooltip = tooltip;
        }

        //Create a deconstructor to streamline retrieving entries
        public void Deconstruct(out TechType techType, out int invSize, out string iconName, out string name, out string tooltip)
        {
            techType = this.techType;
            invSize = this.invSize;
            iconName = this.iconName;
            name = this.name;
            tooltip = this.tooltip;
        }
    }
}
