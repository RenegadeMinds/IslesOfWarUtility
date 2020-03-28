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
using System.Net;

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
            // For the JSON RPC stuff, set the net to mainnet.
            net = mainnet;
        }

        string getCurrentState = "{\"jsonrpc\": \"2.0\", \"id\":\"GetCurrentState\", \"method\": \"getcurrentstate\"}";

        private void btnGetGameState_Click(object sender, EventArgs e)
        {
            // This is old code that used curl.
            //string s = Blah("getcurrentstate.bat");
            //Application.DoEvents();
            //GspRoot gsp = GetGsp();

            // Get GSP without using curl
            string json = GetJsonRpcResponse(getCurrentState);
            Application.DoEvents();
            GspRoot gsp = GetGspFromJson(json);

            Dictionary<string, Player> playersU = gsp.Result.Gamestate.Players;
            SortedDictionary<string, Player> players = new SortedDictionary<string, Player>(playersU);
            
            PopulatePlayerDropDown(gsp);

            StringBuilder sb = new StringBuilder();
            // set up header
            sb.AppendLine(GetHeader());
            // Set up navigation
            sb.AppendLine(GetFloatingMenu());
            // Set up sb with basic CSS
            sb.AppendLine(GetCSS());

            IslesOfWar.UnitNames unitNames = new UnitNames();
            int pad = 20; // used to pad names & stuff

            // Some basic overview stats first
            sb.AppendLine("<p>Player count = " + players.Count.ToString() + "</p>");
            sb.AppendLine("<p>Island count = " + gsp.Result.Gamestate.Islands.Count.ToString() + "</p>");
            sb.AppendLine("<p></p>");

            // Display resource pools.

            sb.AppendLine("<h1><a id=\"Pools\"></a>POOLS</h1>");
            sb.AppendLine("<pre>Oil: ".PadRight(16) + gsp.Result.Gamestate.ResourcePools[0].ToString("n0").PadLeft(20) + "</pre>");
            sb.AppendLine("<pre>Metal: ".PadRight(16) + gsp.Result.Gamestate.ResourcePools[1].ToString("n0").PadLeft(20) + "</pre>");
            sb.AppendLine("<pre>Concrete: ".PadRight(16) + gsp.Result.Gamestate.ResourcePools[2].ToString("n0").PadLeft(20) + "</pre>");
            sb.AppendLine("<pre>Warbux: ".PadRight(16) + gsp.Result.Gamestate.WarbucksPool.ToString("n0").PadLeft(20) + "</pre>");

            // Get combat unit numbers for each player. 
            sb.AppendLine("<h1><a id=\"PlayerCombatUnits\"></a>PLAYER COMBAT UNITS</h1>");
            sb.AppendLine("<p></p>");
            foreach (var p in players)
            {
                string name = p.Key;
                Player player = p.Value;
                sb.AppendLine("<h1>"+ name + "</h1>");
                // TOTAL units
                sb.AppendLine(GetTotalCombatUnits(player, name, pad));
                // AVAILABLE units
                sb.AppendLine(GetPlayerCombatUnits(player));
            } // End getting combat units for each player.

            // Get resources for each player.
            IslesOfWar.ResourceNames resourceNames = new ResourceNames();
            sb.AppendLine("<p></p>");
            sb.AppendLine("<h1><a id=\"PlayerResources\"></a>PLAYER RESOURCES</h1>");
            sb.AppendLine("<p></p>");
            foreach (var p in players)
            {
                string name = p.Key;
                Player player = p.Value;
                sb.AppendLine("<h2>" + name + "</h2>");

                sb.AppendLine( GetPlayerResources(player, resourceNames));
            }

            // What islands have what resources or defenses? Let's ignore resources for now as it's a bunch of arrays - just do defenses. 
            Dictionary<string, Island> islands = gsp.Result.Gamestate.Islands;
            sb.AppendLine("<p></p>");
            sb.AppendLine("<h1><a id=\"IslandsAndTheirDefenses\"></a>ISLANDS AND THEIR DEFENSES</h1>");
            sb.AppendLine("<p></p>");
            sb.AppendLine(GetIslandNumbering());
            sb.AppendLine("<p></p>");

            Dictionary<string, int> playerIslandCount = new Dictionary<string, int>();
            int islandCount = 0;

            foreach (var isle in islands)
            {
                string hexName = isle.Key;
                Island island = isle.Value;
                sb.AppendLine("<pre>" + island.Owner + " owns " + hexName + "</pre>");
                // Add in if there's a player attacking it
                sb.AppendLine(GetAttackingPlayer(hexName, players));

                if (island.SquadCounts != null && island.SquadPlans != null)
                {
                    // Add in defenses for each island
                    int squadPlanCount = island.SquadPlans.Count;
                    int squadCountCount = island.SquadCounts.Count;

                    sb.AppendLine(GetDefenderSquadInfo(island, squadCountCount, unitNames, pad));
                }
                else
                {
                    // Add in for islands with no squads
                    sb.AppendLine("<pre>");
                    sb.AppendLine(GetDefenderNoSquadInfo(island));
                    sb.AppendLine("</pre>");
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

            // Islands targeted for attack.
            sb.AppendLine("<p></p>");
            sb.AppendLine("<h1><a id=\"IslandsTargetedForAttack\"></a>ISLANDS TARGETED FOR ATTACK</h1>");
            sb.AppendLine(GetIslandsTargetedForAttack(players, islands));

            // How many islands each player has
            sb.AppendLine("<p></p>");
            sb.AppendLine("<h1><a id=\"HowManyIslands\"></a>HOW MANY ISLANDS DOES EACH PLAYER OWN?</h1>");
            sb.AppendLine("<h2>" + islandCount.ToString() + " ISLANDS TOTAL</h2>");
            sb.AppendLine("<p></p><pre>");
            
            // Sort by player name
            var l = playerIslandCount.OrderBy(key => key.Key);
            var dic = l.ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);
            playerIslandCount = dic;

            foreach (var v in playerIslandCount)
            {
                sb.AppendLine("" + v.Key + " owns " + v.Value.ToString() + " islands");
            }
            sb.AppendLine("</pre>");
            sb.AppendLine(GetFooter());

            string newtext = sb.ToString();
            wbBrowser.DocumentText = newtext; // "<pre>" + newtext + "</pre>";
        }

        private string GetIslandNumbering()
        {
            StringBuilder sb = new StringBuilder();
            // Add in text diagram of island numbering
            sb.AppendLine("<h3>Island Tile Numbering</h3>");
            sb.AppendLine("<pre>");
            sb.AppendLine(@"11    7    3
10    6    2
9     5    1
8     4    0");
            sb.AppendLine("</pre>");

            return sb.ToString();
        }

        private GspRoot GetGsp()
        {
            // Read in the JSON for the current game state. 
            string text = System.IO.File.ReadAllText(@"isleofwar.json");

            // Fix issue with gamestate being stored as a string and not JSON
            text = text.Replace("\"gamestate\":\"{", "\"gamestate\":{"); // Fix first quote.
            text = text.Replace("}\",\"height\":", "},\"height\":"); // Fix end quote.
            text = text.Replace("\\\"", "\""); // Fix improperly escaped quotes.
            System.IO.File.WriteAllText("isleofwar.json", text);

            GspRoot gsp = new GspRoot();

            // Deserialize the JSON into the GspRoot class as an object.
            gsp = JsonConvert.DeserializeObject<GspRoot>(text);

            return gsp;
        }

        private GspRoot GetGspFromJson(string json)
        {
            GspRoot gsp = new GspRoot();

            // Deserialize the JSON into the GspRoot class as an object.
            gsp = JsonConvert.DeserializeObject<GspRoot>(json);

            return gsp;
        }

        private string GetIslandsTargetedForAttack(SortedDictionary<string, Player> players, Dictionary<string, Island> islands)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
            int count = 0;

            // Loop through all players
            foreach (var p in players)
            {
                count = 0;
                string name = p.Key;
                Player player = p.Value;

                // Get all of the player's islands that are being attacked
                sb2.AppendLine("<h3>" + name + "'s Islands Targeted for Attack</h3>");
                sb2.AppendLine("<pre>");

                // Order players alphabetically in a sorted dictionary
                SortedDictionary<string, string> dic = new SortedDictionary<string, string>();
                foreach (var isle in player.Islands)
                {
                    if ( islands[isle].AttackingPlayers.Count > 0)
                    {
                        dic.Add(islands[isle].AttackingPlayers[0], isle);
                        count++;
                    }
                }
                
                // Add in those players from a nice, sorted dictionary
                foreach (var plyr in dic)
                {
                    string s = plyr.Key + " is targeting ";
                    sb2.AppendLine(s.PadRight(35) + plyr.Value);
                }

                sb2.AppendLine("</pre>");
                if (count > 0)
                {
                    sb.AppendLine(sb2.ToString());
                }
                sb2.Clear();
            }

            return sb.ToString();
        }

        private string GetAttackingPlayer(string island, SortedDictionary<string, Player> players)
        {
            string attack = string.Empty;
            foreach (var p in players)
            {
                if (island == p.Value.AttackableIsland)
                {
                    attack += "<pre class=\"targetedForAttack\">Island targeted for attack by <span class=\"playerName\">" + p.Key + "</span></pre>";
                    break; // I'm assuming only 1 player can attack any given island
                }
            }

            return attack;
        }

        private string GetPlayerCombatUnits(Player player)
        {
            IslesOfWar.UnitNames unitNames = new UnitNames();
            List<long> units = player.Units;
            int i = 0;
            int pad = 20; // used to pad names & stuff
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<h2>UNITS AVAILABLE TO DEPLOY</h2>");
            sb.AppendLine("<p></p><pre>");
            foreach (long unitCount in units)
            {
                sb.Append("&nbsp;&nbsp;&nbsp;&nbsp;");
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
            sb.AppendLine("</pre>");

            return sb.ToString();
        }

        private string GetPlayerResources(Player player, IslesOfWar.ResourceNames resourceNames)
        {
            List<long> resources = player.Resources;
            int i = 0;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<pre>");
            foreach (long resourceCount in resources)
            {
                sb.Append("&nbsp;&nbsp;&nbsp;&nbsp;");
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
            sb.AppendLine("</pre>");

            return sb.ToString();
        }

        private string GetDefenderSquadInfo(Island island, int squadCountCount, UnitNames unitNames, int pad)
        {
            List<long> sc = new List<long>();
            StringBuilder sb = new StringBuilder();

            // Sort the islands with squads so that they display nicely. 
            List<long> squadPlans = new List<long>();
            for (int c = 0; c < island.SquadPlans.Count; c++)
            {
                squadPlans.Add(island.SquadPlans[c][0]);
            }
            squadPlans.Sort();

            sb.AppendLine("<pre>");
            // Add in all tiles with squads on them
            for (int j = 0; j < squadCountCount; j++)
            {
                sc = island.SquadCounts[j];
                int tileNumber = Convert.ToInt32(squadPlans[j]);    // Convert.ToInt32(island.SquadPlans[j][0]);
                // List blockers & bunkers & features (terrain)
                string defenses = island.Defenses;
                string features = island.Features;
                char defense = defenses[tileNumber];
                char feature = features[tileNumber];
                string blocker = GetBlockerFromChar(defense);
                string bunkers = GetBunkersFromChar(defense);
                string terrain = GetTerrainFromChar(feature);

                sb.AppendLine("&nbsp;&nbsp;&nbsp;&nbsp;Squad #" + (j + 1).ToString() + " on tile " + tileNumber.ToString() + "  ==> " + terrain + " with " + blocker + " and " + bunkers);

                sb.AppendLine("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + unitNames.Riflemen.PadRight(pad) + " = " + sc[0].ToString().PadLeft(12, ' '));
                sb.AppendLine("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + unitNames.MachineGunners.PadRight(pad) + " = " + sc[1].ToString().PadLeft(12, ' '));
                sb.AppendLine("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + unitNames.Bazookamen.PadRight(pad) + " = " + sc[2].ToString().PadLeft(12, ' '));
                sb.AppendLine("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + unitNames.LightTanks.PadRight(pad) + " = " + sc[3].ToString().PadLeft(12, ' '));
                sb.AppendLine("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + unitNames.MediumTanks.PadRight(pad) + " = " + sc[4].ToString().PadLeft(12, ' '));
                sb.AppendLine("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + unitNames.HeavyTanks.PadRight(pad) + " = " + sc[5].ToString().PadLeft(12, ' '));
                sb.AppendLine("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + unitNames.LightFighters.PadRight(pad) + " = " + sc[6].ToString().PadLeft(12, ' '));
                sb.AppendLine("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + unitNames.MediumFighters.PadRight(pad) + " = " + sc[7].ToString().PadLeft(12, ' '));
                sb.AppendLine("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + unitNames.Bombers.PadRight(pad) + " = " + sc[8].ToString().PadLeft(12, ' '));
            }

            // Add in all the tiles with no squad on them
            sb.AppendLine(GetDefenderNoSquadInfo(island));

            sb.AppendLine("</pre>");

            return sb.ToString();
        }

        private string GetDefenderNoSquadInfo(Island island)
        {
            StringBuilder sb = new StringBuilder();

            List<long> skipTiles = new List<long>();
            if (island.SquadPlans != null)
            {
                for (int c = 0; c < island.SquadPlans.Count; c++)
                {
                    skipTiles.Add(island.SquadPlans[c][0]);
                }
            }
            else
            {
                skipTiles.Add(-1);
            }

            // Add in all tiles with NO squads on them
            for (int j = 0; j < 12; j++)
            {
                // If there's a squad, continue
                if (skipTiles.Contains(j))
                    continue;

                int tileNumber = j;

                // There's no squad - do blockers, terrain, and bunkers
                // List blockers & bunkers 
                string defenses = island.Defenses;
                string features = island.Features;
                char defense = defenses[tileNumber];
                char feature = features[tileNumber];
                string blocker = GetBlockerFromChar(defense);
                string bunkers = GetBunkersFromChar(defense);
                string terrain = GetTerrainFromChar(feature);

                sb.AppendLine("&nbsp;&nbsp;&nbsp;&nbsp;No squad on tile " + tileNumber.ToString() + "  ==> " + terrain + " with " + blocker + " and " + bunkers);
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

            sb.AppendLine("<h2>TOTAL COMBAT UNITS FOR " + name + "</h2>");
            sb.AppendLine("<p></p>");

            sb.Append("<pre>&nbsp;&nbsp;&nbsp;&nbsp;" + unitNames.Riflemen.PadRight(pad, ' '));
            sb.Append("=" + string.Format("{0:n0}", riflemen).ToString().PadLeft(12, ' ') + "</pre>" + Environment.NewLine);

            sb.Append("<pre>&nbsp;&nbsp;&nbsp;&nbsp;" + unitNames.MachineGunners.PadRight(pad, ' '));
            sb.Append("=" + string.Format("{0:n0}", machinegunners).ToString().PadLeft(12, ' ') + "</pre>" + Environment.NewLine);

            sb.Append("<pre>&nbsp;&nbsp;&nbsp;&nbsp;" + unitNames.Bazookamen.PadRight(pad, ' '));
            sb.Append("=" + string.Format("{0:n0}", bazookamen).ToString().PadLeft(12, ' ') + "</pre>" + Environment.NewLine);

            sb.Append("<pre>&nbsp;&nbsp;&nbsp;&nbsp;" + unitNames.LightTanks.PadRight(pad, ' '));
            sb.Append("=" + string.Format("{0:n0}", lighttanks).ToString().PadLeft(12, ' ') + "</pre>" + Environment.NewLine);

            sb.Append("<pre>&nbsp;&nbsp;&nbsp;&nbsp;" + unitNames.MediumTanks.PadRight(pad, ' '));
            sb.Append("=" + string.Format("{0:n0}", mediumtanks).ToString().PadLeft(12, ' ') + "</pre>" + Environment.NewLine);

            sb.Append("<pre>&nbsp;&nbsp;&nbsp;&nbsp;" + unitNames.HeavyTanks.PadRight(pad, ' '));
            sb.Append("=" + string.Format("{0:n0}", heavytanks).ToString().PadLeft(12, ' ') + "</pre>" + Environment.NewLine);

            sb.Append("<pre>&nbsp;&nbsp;&nbsp;&nbsp;" + unitNames.LightFighters.PadRight(pad, ' '));
            sb.Append("=" + string.Format("{0:n0}", lightfighters).ToString().PadLeft(12, ' ') + "</pre>" + Environment.NewLine);

            sb.Append("<pre>&nbsp;&nbsp;&nbsp;&nbsp;" + unitNames.MediumFighters.PadRight(pad, ' '));
            sb.Append("=" + string.Format("{0:n0}", mediumfighters).ToString().PadLeft(12, ' ') + "</pre>" + Environment.NewLine);

            sb.Append("<pre>&nbsp;&nbsp;&nbsp;&nbsp;" + unitNames.Bombers.PadRight(pad, ' '));
            sb.Append("=" + string.Format("{0:n0}", bombers).ToString().PadLeft(12, ' ') + "</pre>" + Environment.NewLine);

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
                return "<b>Troop bunker</b>";
            }
            if (bunkers == '@' || bunkers == '2' || bunkers == 'c' || bunkers == 'C')
            {
                return "<b>Tank bunker</b>";
            }
            if (bunkers == '#' || bunkers == '3' || bunkers == 'd' || bunkers == 'D')
            {
                return "<b>Air bunker</b>";
            }
            if (bunkers == '$' || bunkers == '4' || bunkers == 'e' || bunkers == 'E')
            {
                return "<b>Troop & Tank bunkers</b>";
            }
            if (bunkers == '%' || bunkers == '5' || bunkers == 'f' || bunkers == 'F')
            {
                return "<b>Troop & Air bunkers</b>";
            }
            if (bunkers == '^' || bunkers == '6' || bunkers == 'g' || bunkers == 'G')
            {
                return "<b>Tank & Air bunkers</b>";
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
                return "<b>Troop blocker</b>";
            }
            if (blocker == 'a' || blocker == 'b' || blocker == 'c' || blocker == 'd' || blocker == 'e' || blocker == 'f' || blocker == 'g' || blocker == 'h')
            {
                // Tank blocker
                return "<b>Tank blocker</b>";
            }
            if (blocker == 'A' || blocker == 'B' || blocker == 'C' || blocker == 'D' || blocker == 'E' || blocker == 'F' || blocker == 'G' || blocker == 'H')
            {
                // Aircraft blocker
                return "<b>Aircraft blocker</b>";
            }

            return "Unknown blocker";
        }

        private string GetTerrainFromChar(char terrain)
        {
            if (terrain == ')' || terrain == '!' || terrain == '@' || terrain == '#' || terrain == '$' || terrain == '%' || terrain == '^' || terrain == '&')
            {
                // No terrain
                return "No terrain";
            }
            if (terrain == '0' || terrain == '1' || terrain == '2' || terrain == '3' || terrain == '4' || terrain == '5' || terrain == '6' || terrain == '7')
            {
                // Troop terrain
                return "Hills (allows all unit types)";
            }
            if (terrain == 'a' || terrain == 'b' || terrain == 'c' || terrain == 'd' || terrain == 'e' || terrain == 'f' || terrain == 'g' || terrain == 'h')
            {
                // Tank terrain
                return "Lakes (<b>blocks tanks</b>)";
            }
            if (terrain == 'A' || terrain == 'B' || terrain == 'C' || terrain == 'D' || terrain == 'E' || terrain == 'F' || terrain == 'G' || terrain == 'H')
            {
                // Aircraft terrain
                return "Mountains (<b>blocks aircraft</b>)";
            }

            return "Unknown terrain";
        }

        // Section to replace using curl
        #region Getting JSON stuff and RPC stuff

        //int testnet = 18396; // testnet = 18396
        int mainnet = 8900; // Mainnet = 8600
        int net = 8900;

        private string GetJsonRpcResponse(string json)
        {
            string result = string.Empty;

            using (var webClient = new WebClient())
            {
                var response = webClient.UploadString("http://localhost:" + net.ToString(), "POST", json);
                result = response.ToString();
            }

            // Fix issue with gamestate being stored as a string and not JSON
            result = result.Replace("\"gamestate\":\"{", "\"gamestate\":{"); // Fix first quote.
            result = result.Replace("}\",\"height\":", "},\"height\":"); // Fix end quote.
            result = result.Replace("\\\"", "\""); // Fix improperly escaped quotes.

            return result;
        }

        private string GetJsonFromObject(object obj)
        {
            string json = "{}";
            json = JsonConvert.SerializeObject(obj);
            return json;
        }

        private dynamic GetObjectFromJson(string json, Type type)
        {
            dynamic result;

            result = JsonConvert.DeserializeObject<Type>(json);
            result = Convert.ChangeType(result, type);

            return result;
        }
        #endregion
        
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

        private void PopulatePlayerDropDown(GspRoot gsp)
        {
            Dictionary<string, Player> players = gsp.Result.Gamestate.Players;

            SortedDictionary<string, Player> playersSorted = new SortedDictionary<string, Player>(players);

            cbxPlayers.DataSource = new BindingSource(playersSorted, null);
            cbxPlayers.DisplayMember = "Key";
            cbxPlayers.ValueMember = "Value";
        }

        bool skip = true;

        private string GetCSS()
        {
            string css = string.Empty;

            try
            {
                string text = System.IO.File.ReadAllText(@"IslesOfWarUtility.css");
                css = "<style>" + Environment.NewLine + text + Environment.NewLine + "</style>";
            }
            catch
            {
            css = @"
<style>
h1 {
	color: #ff0000;
	font-family: ""Courier New"", monospace;
}

h2 {
	color: #0000ff;
	font-family: ""Courier New"", monospace;
}

pre {
	font-family: ""Courier New"", monospace;
	line-height: 100%;
	margin: 0px;
	padding: 0px;
}

p {
	font-family: ""Courier New"", monospace;
}
</style>
";
            }

            return css;
        }

        private string GetFloatingMenu()
        {
            string menu = @"
<div id=""floatdiv"" style=""  
    position:fixed;  
    top:10px;right:10px;  
    z-index:100; 
    width: 200px; 
    height: 250px; 
    margin: auto; 
    padding: 10px;
    border-radius: 10px; 
    border: 3px solid #1c87c9; 
    background-color: white;
"">  
<a href=""#top"">Top</a><br /><br />
<a href=""#Pools"">Pools</a><br /><br />
<a href=""#PlayerCombatUnits"">Player combat units</a><br /><br />
<a href=""#PlayerResources"">Player resources</a><br /><br />
<a href=""#IslandsAndTheirDefenses"">Islands and their defenses</a><br /><br />
<a href=""#IslandsTargetedForAttack"">Islands targeted for attack</a><br /><br />
<a href=""#HowManyIslands"">How many islands?</a><br />
</div>";

            return menu;
        }

        private string GetHeader()
        {
            string header = string.Empty; ;

            header = @"<!doctype html><html><head><title>Isles of War Utility</title></head><body>";

            return header;
        }

        private string GetFooter()
        {
            string footer = string.Empty;

            footer = "</body></html>";

            return footer;
        }

        private void cbxPlayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            // skip 1 time
            if (skip) { skip = false; return; }

            // Old code that used curl
            //GspRoot gsp = GetGsp();

            // Get GSP without using curl
            string json = GetJsonRpcResponse(getCurrentState);
            Application.DoEvents();
            GspRoot gsp = GetGspFromJson(json);


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
            // set up header
            sb.AppendLine(GetHeader());
            // Set up navigation
            //sb.AppendLine(GetFloatingMenu()); // no need for a menu
            // Set up sb with basic CSS
            sb.AppendLine(GetCSS());
            IslesOfWar.UnitNames unitNames = new UnitNames();
            int pad = 20; // used to pad names & stuff

            // Should do TOTAL combat units at the top here
            sb.AppendLine( GetTotalCombatUnits(player, name, pad));
            // Next, available combat units
            sb.AppendLine(GetPlayerCombatUnits(player));

            // resources

            // Get resources for player.
            IslesOfWar.ResourceNames resourceNames = new ResourceNames();
            sb.AppendLine("<h2>PLAYER RESOURCES FOR " + name + "</h2>");
            sb.AppendLine();

            sb.AppendLine(GetPlayerResources(player, resourceNames));

            // islands & defences
            sb.AppendLine("<p></p>");
            sb.AppendLine("<h2>ISLANDS AND THEIR DEFENSES</h2>");
            sb.AppendLine(GetIslandNumbering());

            // What islands have what resources or defenses? Let's ignore resources for now as it's a bunch of arrays - just do defenses. 
            sb.AppendLine("<p></p>");
            sb.AppendLine("<h3>###PLAYERISLANDCOUNT###</h3>"); // place holder to replace

            Dictionary<string, Island> islands = gsp.Result.Gamestate.Islands;
            sb.AppendLine("<p></p>");
            Dictionary<string, int> playerIslandCount = new Dictionary<string, int>();
            int islandCount = 0;

            foreach (var isle in islands)
            {
                if (isle.Value.Owner != name)
                    continue;

                string hexName = isle.Key;
                Island island = isle.Value;
                sb.AppendLine("<pre>" + island.Owner + " owns " + hexName + "</pre>");
                // Add in if there's a player attacking it
                sb.AppendLine(GetAttackingPlayer(hexName, players));

                // This adds in all islands with squads deployed.
                if (island.SquadCounts != null && island.SquadPlans != null)
                {
                    // Add in defenses for each island
                    int squadPlanCount = island.SquadPlans.Count;
                    int squadCountCount = island.SquadCounts.Count;

                    sb.AppendLine( GetDefenderSquadInfo(island, squadCountCount, unitNames, pad));
                }
                else
                {
                    // Add in for islands with no squads
                    sb.AppendLine("<pre>");
                    sb.AppendLine(GetDefenderNoSquadInfo(island));
                    sb.AppendLine("</pre>");
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
            sb.AppendLine("<p></p>");
            sb.Replace("###PLAYERISLANDCOUNT###", name + " OWNS " + islandCount.ToString() + " ISLANDS");
            sb.AppendLine(GetFooter());

            webBrowser1.DocumentText = sb.ToString(); // "<pre>" + sb.ToString() + "</pre>";
        }
    }
}
