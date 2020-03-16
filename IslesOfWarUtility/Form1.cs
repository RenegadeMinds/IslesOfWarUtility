using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using IslesOfWar;
using System.Diagnostics;

namespace IslesOfWarUtility
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnGetGameState_Click(object sender, EventArgs e)
        {
            string s = Blah("getcurrentstate.bat");
            Application.DoEvents();

            GspRoot gsp = GetGsp();

            Dictionary<string, Player> playersU = gsp.Result.Gamestate.Players;
            SortedDictionary<string, Player> players = new SortedDictionary<string, Player>(playersU);

            PopulatePlayerDropDown(gsp);

            StringBuilder sb = new StringBuilder();
            IslesOfWar.UnitNames unitNames = new UnitNames();
            int pad = 20; // used to pad names & stuff

            // Some basic overview stats first
            sb.AppendLine("Player count = " + players.Count.ToString());
            sb.AppendLine("Island count = " + gsp.Result.Gamestate.Islands.Count.ToString());
            sb.AppendLine();

            // Get combat unit numbers for each player. 
            sb.AppendLine("PLAYER COMBAT UNITS");
            sb.AppendLine();
            foreach (var p in players)
            {
                string name = p.Key;
                Player player = p.Value;
                sb.AppendLine(name);
                // TOTAL units
                sb.AppendLine(GetTotalCombatUnits(player, name, pad));
                // AVAILABLE units
                sb.AppendLine(GetPlayerCombatUnits(player));
            } // End getting combat units for each player.

            // Get resources for each player.
            IslesOfWar.ResourceNames resourceNames = new ResourceNames();
            sb.AppendLine();
            sb.AppendLine("PLAYER RESOURCES");
            sb.AppendLine();
            foreach (var p in players)
            {
                string name = p.Key;
                Player player = p.Value;
                sb.AppendLine(name);

                sb.AppendLine( GetPlayerResources(player, resourceNames));
            }

            // What islands have what resources or defenses? Let's ignore resources for now as it's a bunch of arrays - just do defenses. 
            Dictionary<string, Island> islands = gsp.Result.Gamestate.Islands;
            sb.AppendLine();
            sb.AppendLine("ISLANDS AND THEIR DEFENSES");
            sb.AppendLine();
            Dictionary<string, int> playerIslandCount = new Dictionary<string, int>();
            int islandCount = 0;

            foreach (var isle in islands)
            {
                string hexName = isle.Key;
                Island island = isle.Value;
                sb.AppendLine(island.Owner + " owns " + hexName);

                if (island.SquadCounts != null && island.SquadPlans != null)
                {
                    // Add in defenses for each island
                    int squadPlanCount = island.SquadPlans.Count;
                    int squadCountCount = island.SquadCounts.Count;

                    sb.AppendLine(GetDefenderSquadInfo(island, squadCountCount, unitNames, pad));
                }
                

                // There are 12 hexes per island. Each one can have a squad.
                if (playerIslandCount.Keys.Contains<string>(island.Owner))
                {
                    playerIslandCount[island.Owner]++;
                    islandCount++;
                }
                else
                {
                    playerIslandCount.Add(island.Owner, 1);
                    islandCount++;
                }
            }
            sb.AppendLine();
            sb.AppendLine("HOW MANY ISLANDS DOES EACH PLAYER OWN?");
            sb.AppendLine(islandCount.ToString() + " ISLANDS TOTAL");
            sb.AppendLine();
            foreach (var v in playerIslandCount)
            {
                sb.AppendLine(v.Key + " owns " + v.Value.ToString() + " islands");
            }

            string newtext = sb.ToString(); 
            wbBrowser.DocumentText = "<pre>" + newtext + "</pre>";
        }

        private GspRoot GetGsp()
        {
            // Read in the JSON for the current game state. 
            string text = System.IO.File.ReadAllText(@"isleofwar.json");

            text = text.Replace("\"gamestate\":\"{", "\"gamestate\":{"); // Fix first quote.
            text = text.Replace("}\",\"height\":", "},\"height\":"); // Fix end quote.
            text = text.Replace("\\\"", "\""); // Fix improperly escaped quotes.
            System.IO.File.WriteAllText("isleofwar.json", text);

            GspRoot gsp = new GspRoot();

            // Deserialize the JSON into the GspRoot class as an object.
            gsp = JsonConvert.DeserializeObject<GspRoot>(text);

            return gsp;
        }

        private string GetPlayerCombatUnits(Player player)
        {
            IslesOfWar.UnitNames unitNames = new UnitNames();
            List<long> units = player.Units;
            int i = 0;
            int pad = 20; // used to pad names & stuff
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("UNITS AVAILABLE TO DEPLOY");
            sb.AppendLine();
            foreach (long unitCount in units)
            {
                sb.Append("\t");
                switch (i)
                {
                    case 0:
                        sb.Append(unitNames.Riflemen.PadRight(pad, ' '));
                        break;
                    case 1:
                        sb.Append(unitNames.MachineGunners.PadRight(pad, ' '));
                        break;
                    case 2:
                        sb.Append(unitNames.Bazookamen.PadRight(pad, ' '));
                        break;
                    case 3:
                        sb.Append(unitNames.LightTanks.PadRight(pad, ' '));
                        break;
                    case 4:
                        sb.Append(unitNames.MediumTanks.PadRight(pad, ' '));
                        break;
                    case 5:
                        sb.Append(unitNames.HeavyTanks.PadRight(pad, ' '));
                        break;
                    case 6:
                        sb.Append(unitNames.LightFighters.PadRight(pad, ' '));
                        break;
                    case 7:
                        sb.Append(unitNames.MediumFighters.PadRight(pad, ' '));
                        break;
                    case 8:
                        sb.Append(unitNames.Bombers.PadRight(pad, ' '));
                        break;
                    default:
                        sb.Append("Unknown");
                        break;
                }
                i++;

                sb.Append("=" + string.Format("{0:n0}", unitCount).ToString().PadLeft(12, ' ') + Environment.NewLine);
            }

            return sb.ToString();
        }

        private string GetPlayerResources(Player player, IslesOfWar.ResourceNames resourceNames)
        {
            List<long> resources = player.Resources;
            int i = 0;
            StringBuilder sb = new StringBuilder();

            foreach (long resourceCount in resources)
            {
                sb.Append("\t");
                switch (i)
                {
                    case 0:
                        sb.Append(resourceNames.Warbux.PadRight(16, ' '));
                        break;
                    case 1:
                        sb.Append(resourceNames.Oil.PadRight(16, ' '));
                        break;
                    case 2:
                        sb.Append(resourceNames.Metal.PadRight(16, ' '));
                        break;
                    case 3:
                        sb.Append(resourceNames.Concrete.PadRight(16, ' '));
                        break;
                    default:
                        break;
                }
                i++;
                sb.Append("=" + string.Format("{0:n0}", resourceCount).ToString().PadLeft(16, ' ') + Environment.NewLine);
            }

            return sb.ToString();
        }

        private string GetDefenderSquadInfo(Island island, int squadCountCount, UnitNames unitNames, int pad)
        {
            List<long> sc = new List<long>();
            StringBuilder sb = new StringBuilder();

            for (int j = 0; j < squadCountCount; j++)
            {
                sc = island.SquadCounts[j];
                int tileNumber = Convert.ToInt32(island.SquadPlans[j][0]);
                // List blockers & bunkers 
                string defenses = island.Defenses;
                char defense = defenses[tileNumber];
                string blocker = GetBlockerFromChar(defense);
                string bunkers = GetBunkersFromChar(defense);

                sb.AppendLine("\tSquad #" + (j + 1).ToString() + " on tile " + tileNumber.ToString() + " with " + blocker + " and " + bunkers);

                sb.AppendLine("\t\t" + unitNames.Riflemen.PadRight(pad) + " = " + sc[0].ToString().PadLeft(12, ' '));
                sb.AppendLine("\t\t" + unitNames.MachineGunners.PadRight(pad) + " = " + sc[1].ToString().PadLeft(12, ' '));
                sb.AppendLine("\t\t" + unitNames.Bazookamen.PadRight(pad) + " = " + sc[2].ToString().PadLeft(12, ' '));
                sb.AppendLine("\t\t" + unitNames.LightTanks.PadRight(pad) + " = " + sc[3].ToString().PadLeft(12, ' '));
                sb.AppendLine("\t\t" + unitNames.MediumTanks.PadRight(pad) + " = " + sc[4].ToString().PadLeft(12, ' '));
                sb.AppendLine("\t\t" + unitNames.HeavyTanks.PadRight(pad) + " = " + sc[5].ToString().PadLeft(12, ' '));
                sb.AppendLine("\t\t" + unitNames.LightFighters.PadRight(pad) + " = " + sc[6].ToString().PadLeft(12, ' '));
                sb.AppendLine("\t\t" + unitNames.MediumFighters.PadRight(pad) + " = " + sc[7].ToString().PadLeft(12, ' '));
                sb.AppendLine("\t\t" + unitNames.Bombers.PadRight(pad) + " = " + sc[8].ToString().PadLeft(12, ' '));
            }

            return sb.ToString();
        }

        private string GetTotalCombatUnits(Player player, string name, int pad)
        {
            long riflemen, machinegunners, bazookamen, lighttanks, mediumtanks, heavytanks, lightfighters, mediumfighters, bombers;
            riflemen = 0;
            machinegunners = 0;
            bazookamen = 0;
            lighttanks = 0;
            mediumtanks = 0;
            heavytanks = 0;
            lightfighters = 0;
            mediumfighters = 0;
            bombers = 0;

            // get all free units counted
            riflemen += player.Units[0];
            machinegunners += player.Units[1];
            bazookamen += player.Units[2];
            lighttanks += player.Units[3];
            mediumtanks += player.Units[4];
            heavytanks += player.Units[5];
            lightfighters += player.Units[6];
            mediumfighters += player.Units[7];
            bombers += player.Units[8];

            // get all islands into a dictionary

            GspRoot gsp = GetGsp();
            Dictionary<string, Island> dic = gsp.Result.Gamestate.Islands; 

            foreach (var isle in dic)
            {
                if (isle.Value.Owner == name)
                {
                    if (isle.Value.SquadCounts == null)
                        continue;

                    foreach (List<long> v in isle.Value.SquadCounts)
                    {
                        riflemen += v[0];
                        machinegunners += v[1];
                        bazookamen += v[2];
                        lighttanks += v[3];
                        mediumtanks += v[4];
                        heavytanks += v[5];
                        lightfighters += v[6];
                        mediumfighters += v[7];
                        bombers += v[8];
                    }
                }
            }

            // assemble string of TOTAL units - free and deployed
            StringBuilder sb = new StringBuilder();
            IslesOfWar.UnitNames unitNames = new UnitNames();

            sb.AppendLine("TOTAL COMBAT UNITS FOR " + name);
            sb.AppendLine("");

            sb.Append("\t" + unitNames.Riflemen.PadRight(pad, ' '));
            sb.Append("=" + string.Format("{0:n0}", riflemen).ToString().PadLeft(12, ' ') + Environment.NewLine);

            sb.Append("\t" + unitNames.MachineGunners.PadRight(pad, ' '));
            sb.Append("=" + string.Format("{0:n0}", machinegunners).ToString().PadLeft(12, ' ') + Environment.NewLine);

            sb.Append("\t" + unitNames.Bazookamen.PadRight(pad, ' '));
            sb.Append("=" + string.Format("{0:n0}", bazookamen).ToString().PadLeft(12, ' ') + Environment.NewLine);

            sb.Append("\t" + unitNames.LightTanks.PadRight(pad, ' '));
            sb.Append("=" + string.Format("{0:n0}", lighttanks).ToString().PadLeft(12, ' ') + Environment.NewLine);

            sb.Append("\t" + unitNames.MediumTanks.PadRight(pad, ' '));
            sb.Append("=" + string.Format("{0:n0}", mediumtanks).ToString().PadLeft(12, ' ') + Environment.NewLine);

            sb.Append("\t" + unitNames.HeavyTanks.PadRight(pad, ' '));
            sb.Append("=" + string.Format("{0:n0}", heavytanks).ToString().PadLeft(12, ' ') + Environment.NewLine);

            sb.Append("\t" + unitNames.LightFighters.PadRight(pad, ' '));
            sb.Append("=" + string.Format("{0:n0}", lightfighters).ToString().PadLeft(12, ' ') + Environment.NewLine);

            sb.Append("\t" + unitNames.MediumFighters.PadRight(pad, ' '));
            sb.Append("=" + string.Format("{0:n0}", mediumfighters).ToString().PadLeft(12, ' ') + Environment.NewLine);

            sb.Append("\t" + unitNames.Bombers.PadRight(pad, ' '));
            sb.Append("=" + string.Format("{0:n0}", bombers).ToString().PadLeft(12, ' ') + Environment.NewLine);



            return sb.ToString();
        }


        private string GetBunkersFromChar(char bunkers)
        {
            if (bunkers == ')' || bunkers == '0' || bunkers == 'a' || bunkers == 'A')
            {
                return "No bunkers";
            }
            if (bunkers == '!' || bunkers == '1' || bunkers == 'b' || bunkers == 'B')
            {
                return "Troop bunker";
            }
            if (bunkers == '@' || bunkers == '2' || bunkers == 'c' || bunkers == 'C')
            {
                return "Tank bunker";
            }
            if (bunkers == '#' || bunkers == '3' || bunkers == 'd' || bunkers == 'D')
            {
                return "Air bunker";
            }
            if (bunkers == '$' || bunkers == '4' || bunkers == 'e' || bunkers == 'E')
            {
                return "Troop & Tank bunkers";
            }
            if (bunkers == '%' || bunkers == '5' || bunkers == 'f' || bunkers == 'F')
            {
                return "Troop & Air bunkers";
            }
            if (bunkers == '^' || bunkers == '6' || bunkers == 'g' || bunkers == 'G')
            {
                return "Tank & Air bunkers";
            }
            if (bunkers == '&' || bunkers == '7' || bunkers == 'h' || bunkers == 'H')
            {
                return "Invalid bunker";
            }

            return "Unknown bunker";
        }

        private string GetBlockerFromChar(char blocker)
        {
            if (blocker == ')' || blocker == '!' || blocker == '@' || blocker == '#' || blocker == '$' || blocker == '%' || blocker == '^' || blocker == '&')
            {
                // No blocker
                return "No blocker";
            }
            if (blocker == '0' || blocker == '1' || blocker == '2' || blocker == '3' || blocker == '4' || blocker == '5' || blocker == '6' || blocker == '7')
            {
                // Troop blocker
                return "Troop blocker";
            }
            if (blocker == 'a' || blocker == 'b' || blocker == 'c' || blocker == 'd' || blocker == 'e' || blocker == 'f' || blocker == 'g' || blocker == 'h')
            {
                // Tank blocker
                return "Tank blocker";
            }
            if (blocker == 'A' || blocker == 'B' || blocker == 'C' || blocker == 'D' || blocker == 'E' || blocker == 'F' || blocker == 'G' || blocker == 'H')
            {
                // Aircraft blocker
                return "Aircraft blocker";
            }

            return "Unknown blocker";
        }



        private string Blah(string file)
        {
            // https://stackoverflow.com/questions/206323/how-to-execute-command-line-in-c-get-std-out-results
            // Start the child process.
            Process p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.FileName = file;
            p.Start();
            // Do not wait for the child process to exit before
            // reading to the end of its redirected stream.
            // p.WaitForExit();
            // Read the output stream first and then wait.
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            return output;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Control && e.KeyCode == Keys.F))
            {
                // Do search
                Search();
            }
        }

        private void Search()
        {
            
        }

        private void btnFindPlayers_Click(object sender, EventArgs e)
        {
            

            string s = Blah("getcurrentstate.bat");
            Application.DoEvents();

            // Read in the JSON for the current game state. 
            string text = System.IO.File.ReadAllText(@"isleofwar.json");

            text = text.Replace("\"gamestate\":\"{", "\"gamestate\":{"); // Fix first quote.
            text = text.Replace("}\",\"height\":", "},\"height\":"); // Fix end quote.
            text = text.Replace("\\\"", "\""); // Fix improperly escaped quotes.
            System.IO.File.WriteAllText("isleofwar.json", text);

            GspRoot gsp = new GspRoot();

            // Deserialize the JSON into the GspRoot class as an object.
            gsp = JsonConvert.DeserializeObject<GspRoot>(text);

            Dictionary<string, Player> players = gsp.Result.Gamestate.Players;

            cbxPlayers.DataSource = new BindingSource(players, null);
            cbxPlayers.DisplayMember = "Key";
            cbxPlayers.ValueMember = "Value";

            // Get combobox selection (in handler)
            // Player player = ((KeyValuePair<string, Player>)cbxPlayers.SelectedItem).Value;

        }

        private void PopulatePlayerDropDown(GspRoot gsp)
        {
            Dictionary<string, Player> players = gsp.Result.Gamestate.Players;

            SortedDictionary<string, Player> playersSorted = new SortedDictionary<string, Player>(players);

            cbxPlayers.DataSource = new BindingSource(playersSorted, null);
            cbxPlayers.DisplayMember = "Key";
            cbxPlayers.ValueMember = "Value";
        }

        bool skip = true;

        private void cbxPlayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            // skip 1 time
            if (skip) { skip = false; return; }

            GspRoot gsp = GetGsp();

            SortedDictionary<string, Player> players = new SortedDictionary<string, Player>( gsp.Result.Gamestate.Players);

            // get ComboBox from sender
            ComboBox comboBox = (ComboBox)sender;

            // get selected KVP
            KeyValuePair<string, Player> selectedEntry
                = (KeyValuePair<string, Player>)comboBox.SelectedItem;

            // get selected Key
            string name = selectedEntry.Key;

            Player player = players[name];

            StringBuilder sb = new StringBuilder();
            IslesOfWar.UnitNames unitNames = new UnitNames();
            int pad = 20; // used to pad names & stuff

            // Should do TOTAL combat units at the top here
            sb.AppendLine( GetTotalCombatUnits(player, name, pad));
            // Next, available combat units
            sb.AppendLine(GetPlayerCombatUnits(player));

            // resources

            // Get resources for player.
            IslesOfWar.ResourceNames resourceNames = new ResourceNames();
            sb.AppendLine("PLAYER RESOURCES FOR " + name);
            sb.AppendLine();

            sb.AppendLine(GetPlayerResources(player, resourceNames));

            // islands & defences
            sb.AppendLine("ISLANDS AND THEIR DEFENSES");

            // What islands have what resources or defenses? Let's ignore resources for now as it's a bunch of arrays - just do defenses. 
            sb.AppendLine();
            sb.AppendLine("###PLAYERISLANDCOUNT###"); // place holder to replace

            Dictionary<string, Island> islands = gsp.Result.Gamestate.Islands;
            sb.AppendLine();
            Dictionary<string, int> playerIslandCount = new Dictionary<string, int>();
            int islandCount = 0;

            foreach (var isle in islands)
            {
                if (isle.Value.Owner != name)
                    continue;

                string hexName = isle.Key;
                Island island = isle.Value;
                sb.AppendLine(island.Owner + " owns " + hexName);
                 
                if (island.SquadCounts != null && island.SquadPlans != null)
                {
                    // Add in defenses for each island
                    int squadPlanCount = island.SquadPlans.Count;
                    int squadCountCount = island.SquadCounts.Count;

                    sb.AppendLine( GetDefenderSquadInfo(island, squadCountCount, unitNames, pad));
                }


                // There are 12 hexes per island. Each one can have a squad.
                if (playerIslandCount.Keys.Contains<string>(island.Owner))
                {
                    playerIslandCount[island.Owner]++;
                    islandCount++;
                }
                else
                {
                    playerIslandCount.Add(island.Owner, 1);
                    islandCount++;
                }
            }

            // island counts
            sb.AppendLine();
            sb.Replace("###PLAYERISLANDCOUNT###", name + " OWNS " + islandCount.ToString() + " ISLANDS");

            webBrowser1.DocumentText = "<pre>" + sb.ToString() + "</pre>";
        }
    }
}
