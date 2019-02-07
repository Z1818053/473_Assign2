using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Assign2 {
    // The types of items a character can have.
    public enum ItemType {
        Helmet, Neck, Shoulders, Back, Chest, Wrists,
        Gloves, Belt, Pants, Boots, Ring, Trinket
    };

    // The available races for the character.
    public enum Race {
        Orc, Troll, Tauren, Forsaken
    }

    static class Program {
        private static uint MAX_ILVL = 360;             // Max item level.
        private static uint MAX_PRIMARY = 200;          // Max primary level.
        private static uint MAX_STAMINA = 275;          // Max stamina level.
        private static uint MAX_LEVEL = 60;             // Highest level a player can be.
        private static uint GEAR_SLOTS = 14;            // The amount of gear slots a player has.
        private static uint MAX_INVENTORY_SIZE = 20;    // The max amount of objects that can be held in the inventory.

        private static Dictionary<uint, string> guildList = new Dictionary<uint, string>();     // Guild dictionary.
        private static Dictionary<uint, Item> gearList = new Dictionary<uint, Item>();          // Item dictionary.
        private static Dictionary<uint, Player> playerList = new Dictionary<uint, Player>();    // Player dictionary.

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            // Build Guild Dictionary
            using (var reader = new StreamReader(@"..\..\Resources\guilds.txt")) {
                string line;

                while ((line = reader.ReadLine()) != null) {
                    string[] itemTokens = line.Split('\t');

                    guildList.Add(Convert.ToUInt32(itemTokens[0]), itemTokens[1]);
                }
            }

            // Build item dictionary.
            using (var reader = new StreamReader(@"..\..\Resources\equipment.txt")) {
                string line;

                while ((line = reader.ReadLine()) != null) {
                    string[] itemTokens = line.Split('\t');

                    //uint newId, string newName, ItemType newType, uint newIlvl, uint newPrimary, uint newStamina, uint newRequirement, string newFlavor
                    Item item = new Item(
                        Convert.ToUInt32(itemTokens[0]),
                        itemTokens[1],
                        (ItemType)Convert.ToUInt32(itemTokens[2]),
                        Convert.ToUInt32(itemTokens[3]),
                        Convert.ToUInt32(itemTokens[4]),
                        Convert.ToUInt32(itemTokens[5]),
                        Convert.ToUInt32(itemTokens[6]),
                        itemTokens[7]
                        );

                    gearList.Add(Convert.ToUInt32(itemTokens[0]), item);
                }
            }

            // Build Player Dictionary
            using (var reader = new StreamReader(@"..\..\Resources\players.txt")) {
                string line;

                while ((line = reader.ReadLine()) != null) {
                    string[] itemTokens = line.Split('\t');

                    //uint id, string name, Race race, uint level, uint exp, uint guildID, uint[] gear, List<uint> inventory
                    uint[] gear = {
                        Convert.ToUInt32(itemTokens[6]),
                        Convert.ToUInt32(itemTokens[7]),
                        Convert.ToUInt32(itemTokens[8]),
                        Convert.ToUInt32(itemTokens[9]),
                        Convert.ToUInt32(itemTokens[10]),
                        Convert.ToUInt32(itemTokens[11]),
                        Convert.ToUInt32(itemTokens[12]),
                        Convert.ToUInt32(itemTokens[13]),
                        Convert.ToUInt32(itemTokens[14]),
                        Convert.ToUInt32(itemTokens[15]),
                        Convert.ToUInt32(itemTokens[16]),
                        Convert.ToUInt32(itemTokens[17]),
                        Convert.ToUInt32(itemTokens[18]),
                        Convert.ToUInt32(itemTokens[19])
                    };

                    Player player = new Player(
                        Convert.ToUInt32(itemTokens[0]),
                        itemTokens[1],
                        (Race)Convert.ToUInt32(itemTokens[2]),
                        Convert.ToUInt32(itemTokens[3]),
                        Convert.ToUInt32(itemTokens[4]),
                        Convert.ToUInt32(itemTokens[5]),
                        gear,
                        new List<uint>()
                        );

                    playerList.Add(Convert.ToUInt32(itemTokens[0]), player);
                }
            }
        }


        /*
         * Class:       Item
         * Description: Game item that a player can have in their inventory and also equip.
         */
        public class Item : IComparable {
            readonly uint id;   // ID of item.
            string name;        // Name of item.
            ItemType type;      // Type of item.
            uint ilvl;          // Item level.
            uint primary;       // Primary stat.
            uint stamina;       // Stamina of item.
            uint requirement;   // Player level required to equip item.
            string flavor;      // Description of item.

            public Item() {
                this.id = 0;
                Name = "";
                IType = 0;
                ILevel = 0;
                Primary = 0;
                Stamina = 0;
                Requirement = 0;
                Flavor = "";
            }

            public Item(uint newId, string newName, ItemType newType, uint newIlvl, uint newPrimary, uint newStamina, uint newRequirement, string newFlavor) {
                this.id = newId;
                Name = newName;
                IType = newType;
                ILevel = newIlvl;
                Primary = newPrimary;
                Stamina = newStamina;
                Requirement = newRequirement;
                Flavor = newFlavor;
            }

            /*
             *  Property:       Id
             *  
             *  Description:    Gets ID of item.
             */
            public uint Id {
                get { return this.id; }
            }

            /*
             *  Property:       Name
             *  
             *  Description:    Gets and sets name of item.
             */
            public string Name {
                set { this.name = value; }
                get { return this.name; }
            }

            /*
             *  Property:       IType
             *  
             *  Description:    Gets and sets the type of the item.
             */
            public ItemType IType {
                set {
                    // If item type exists, then set item.
                    if ((int)value >= 0 && (int)value <= 12) {
                        this.type = value;
                    } else {
                        this.type = 0;
                    }
                }
                get { return this.type; }
            }

            /*
             *  Property:       ILevel
             *  
             *  Description:    Gets and sets level of item.
             */
            public uint ILevel {
                set {
                    // If item's level is permissible, the set item level.
                    if (value >= 0 && value <= MAX_ILVL) {
                        this.ilvl = value;
                    } else {
                        this.ilvl = 0;
                    }
                }
                get { return this.ilvl; }
            }

            /*
             *  Property:       Primary
             *  
             *  Description:    Gets and sets primary stat of item.
             */
            public uint Primary {
                set {
                    if (value >= 0 && value <= MAX_PRIMARY) {
                        this.primary = value;
                    } else {
                        this.primary = 0;
                    }
                }
                get { return this.primary; }
            }

            /*
            *  Property:       Stamina
            *  
            *  Description:    Gets and sets stamina stat of item.
            */
            public uint Stamina {
                set {
                    if (value >= 0 && value <= MAX_STAMINA) {
                        this.stamina = value;
                    } else {
                        this.stamina = 0;
                    }
                }
                get { return this.stamina; }
            }

            /*
            *  Property:       Requirement
            *  
            *  Description:    Gets and sets requirement stat of item.
            */
            public uint Requirement {
                set {
                    if (value >= 0 && value <= MAX_LEVEL) {
                        this.requirement = value;
                    } else {
                        this.requirement = 0;
                    }
                }
                get { return this.requirement; }
            }

            /*
            *  Property:       Flavor
            *  
            *  Description:    Gets and sets flavor (description) of item.
            */
            public string Flavor {
                set { this.flavor = value; }
                get { return this.flavor; }
            }

            /*
            *  Method:          CompareTo
            *  Arguments:       obj   (Object to compare.)
            *  
            *  Description:     IComparable method that needs to be defined so that Item class
            *                   objects can be ordered.
            */
            public int CompareTo(object obj) {
                // Check for null value.
                if (obj == null) return 1;

                // Convert obj to Item.
                Item rightOp = obj as Item;

                // Make sure conversion of obj to Item was successful.
                if (rightOp != null) {
                    return Name.CompareTo(rightOp.Name);
                } else {
                    throw new ArgumentException("You're comparing apples and oranges!");
                }
            }

            /*
             *  Method: ToString
             *  
             *  Description:    ToString implementation for Item class.
             */
            public override string ToString() {
                return "" + String.Format("{0,-13}", "(" + this.IType + ") ") +
                       "" + String.Format("{0,-30}", this.Name) +
                       "" + String.Format("{0,-5}", "|" + this.ILevel + "| ") +
                       "" + String.Format("{0,-5}", "--" + this.Requirement + "-- ") + "\n" +
                       "    " + "\"" + this.Flavor + "\"" + "";
            }
        }

        /*
         * Class:       Player
         * Description: Game character.
         */
        public class Player : IComparable {
            readonly uint id;        // ID of player.
            readonly string name;    // Name of player.
            readonly Race race;      // Race of player.
            uint level;              // Level of player.
            uint exp;                // Player's accumulated experience.
            uint guildID;            // ID of guild that player is in.
            private uint[] gear;     // Player's gear slots. 
            List<uint> inventory;    // Player's inventory.
            Boolean ring = false;    // Used for determining which ring slot to populate.
            Boolean trinket = false; // Used for determining which ring slot to populate.

            public Player() {
                this.id = 0;
                this.name = "Ben";
                this.race = 0;
                this.level = 3;
                this.exp = 0;
                this.guildID = 0;
                this.gear = new uint[GEAR_SLOTS];
                this.inventory = null;
            }

            public Player(uint id, string name, Race race, uint level, uint exp, uint guildID, uint[] gear, List<uint> inventory) {
                this.id = id;
                this.name = name;
                this.race = race;
                this.level = level;
                this.exp = exp;
                this.guildID = guildID;
                this.gear = gear;
                this.inventory = inventory;
            }

            public uint ID {
                get { return this.id; }
            }

            public string Name {
                get { return this.name; }
            }

            public Race Race {
                get { return this.race; }
            }

            public uint Level {
                set {
                    if (value >= 0 && value <= MAX_LEVEL)
                        this.level = value;
                    else
                        this.level = 0;
                }

                get { return this.level; }
            }

            public uint Exp {
                set {

                    if (Level == MAX_LEVEL) {
                        return;
                    }

                    // Add new experience points to existing experience points.
                    this.exp += value;


                    // Determine what the next level-up experience threshold is.
                    uint nextLevelExp = (this.level * 1000);

                    // While the total character experience is greater than level-up threshold, level up and readjust threshold.
                    while (this.exp >= nextLevelExp) {
                        // Level up character by 1.
                        this.Level += 1;
                        Console.WriteLine("Ding!");

                        //XP = 35000
                        //NextLvL = 10000
                        this.exp = this.exp - nextLevelExp;

                        // Redetermine what the next level-up experience threshold is, now that the character is 1 level higher.
                        nextLevelExp = (this.level * 1000);
                    }
                }

                get { return this.exp; }
            }

            public uint GuildID {
                get { return guildID; }
                set { guildID = value; }
            }

            // Indexer for Player class, which is based on gear.
            public uint this[int i] {
                set { gear[i] = value; }
                get { return gear[i]; }
            }

            public int CompareTo(object newRightOp) {
                if (newRightOp == null) return 1;

                Player rightOp = newRightOp as Player;

                if (rightOp != null)
                    return this.name.CompareTo(rightOp.name);
                else
                    throw new ArgumentException("The argument being compared is not of type Player.");
            }

            public void EquipGear(uint newGearID) {
                if (!gearList.ContainsKey(newGearID)) {
                    return;
                }

                Item item = gearList[newGearID];
                if (this.Level < item.Requirement) {
                    throw new Exception("You are too weak to equip this item.");
                }

                uint slot = (uint)item.IType;

                if (item.IType.Equals(ItemType.Ring)) {
                    // Figure out which slot
                    if (ring) {
                        slot = 11;
                    } else {
                        slot = 10;
                    }
                    ring = !ring;

                }
                if (item.IType.Equals(ItemType.Trinket)) {
                    // Figure out which slot
                    if (trinket) {
                        slot = 13;
                    } else {
                        slot = 12;
                    }
                    trinket = !trinket;
                }
                gear[slot] = newGearID;

                //Console.WriteLine(gear[(uint)item.getType()].ToString());
                //Console.WriteLine(item.getName());
            }

            public void UnequipGear(int gearSlot) {
                if (gear[gearSlot] == 0) {
                    return;
                }

                Item item = gearList[gear[gearSlot]];

                PlaceInInventory(item);
                gear[gearSlot] = 0;

                return;
            }

            public void PlaceInInventory(Item item) {
                if (inventory.Count >= MAX_INVENTORY_SIZE) {
                    throw new Exception("Your inventory is full!");
                }

                // Adds the item into the inv.
                inventory.Add(item.Id);

                return;
            }

            public void PrintGearList() {
                //Console.WriteLine("Player Name: " + Name + " lvl. (" + Level + ")");
                Console.WriteLine(ToString());
                Console.WriteLine("Gear: ");
                String msg = "";
                String type = "";
                Item item;
                for (int i = 0; i < gear.Length; i++) {
                    msg = "empty";
                    if (i < 10) {
                        type = ((ItemType)i).ToString();
                    } else {
                        switch (i) {
                            case 10:
                            case 11:
                                type = "Ring";
                                break;
                            case 12:
                            case 13:
                                type = "Trinket";
                                break;
                            default:
                                type = "You borked it";
                                break;
                        }
                    }
                    // Dic contains id from gear slot (i)
                    if (gearList.ContainsKey(gear[i])) {
                        item = gearList[gear[i]];
                        msg = item.Name;
                        Console.WriteLine(item.ToString());
                    } else {
                        Console.WriteLine("" + String.Format("{0,-20}", "(" + type + ") ") + " " + msg);
                    }
                }
            }

            public void JoinGuild(uint guildID) {
                // Need to leave before you can join another guild
                if (this.GuildID != 0) {
                    LeaveGuild();
                }

                string guildName = "N/A";
                if (guildList.ContainsKey(guildID)) {
                    this.GuildID = guildID;
                    guildName = guildList[this.GuildID];
                    Console.WriteLine(this.Name + " has joined the guild: " + guildName);
                } else {
                    throw new Exception("Unable to find guild with ID: " + guildID);
                }


            }

            public void LeaveGuild() {
                // Need to be in a guild to leave a guild
                if (this.GuildID == 0) {
                    throw new Exception("You must be in a guild to leave a guild...");
                }
                string guildName = "N/A";
                if (guildList.ContainsKey(this.guildID)) {
                    guildName = guildList[this.GuildID];
                }

                this.GuildID = 0;

                Console.WriteLine(this.Name + " has left the guild: " + guildName);

            }

            public override string ToString() {
                string guildName = "None";
                if (guildList.ContainsKey(this.GuildID)) {
                    guildName = guildList[this.GuildID];
                }
                return "" + String.Format("Name: {0,-20}", this.Name) +
                       "" + String.Format("Race: {0,-20}", this.Race) +
                       "" + String.Format("Level: {0,-15}", this.Level) +
                       "" + String.Format("Guild: {0,-20}", guildName) + "";
            }

        }
    }
}
   
