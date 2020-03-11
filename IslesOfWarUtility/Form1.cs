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
                Console.WriteLine(name);
                sb.AppendLine(name);
                List<long> units = player.Units;
                int i = 0;
                
                foreach(long unitCount in units)
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
                    Console.WriteLine("=" + unitCount.ToString().PadLeft(12, ' '));
                    sb.Append("=" + string.Format("{0:n0}", unitCount).ToString().PadLeft(12, ' ') + Environment.NewLine);
                }
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
                Console.WriteLine(name);
                sb.AppendLine(name);
                List<long> resources = player.Resources;
                int i = 0;

                foreach(long resourceCount in resources)
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

                    long[] squadCount = new long[squadCountCount];

                    List<long> sc = new List<long>();

                    for (int i = 0; i < squadCountCount; i++)
                    {
                        sc = island.SquadCounts[i];
                        int tileNumber = Convert.ToInt32( island.SquadPlans[i][0]);
                        // List blockers & bunkers 
                        string defenses = island.Defenses;
                        char defense = defenses[tileNumber];
                        string blocker = GetBlockerFromChar(defense);
                        string bunkers = GetBunkersFromChar(defense);

                        sb.AppendLine("\tSquad #" + (i + 1).ToString() + " on tile " + tileNumber.ToString() + " with " + blocker + " and " + bunkers);

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

            txtOutput.Text = sb.ToString();
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

    }
}
