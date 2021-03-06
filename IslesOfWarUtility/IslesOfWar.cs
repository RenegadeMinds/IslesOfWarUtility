﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// <auto-generated />
//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using IslesOfWar;
//
//    var gsp = GspRoot.FromJson(jsonString);

namespace IslesOfWar
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class GspRoot
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("jsonrpc")]
        public string Jsonrpc { get; set; }

        [JsonProperty("result")]
        public Result Result { get; set; }
    }

    public partial class Result
    {
        [JsonProperty("blockhash")]
        public string Blockhash { get; set; }

        [JsonProperty("chain")]
        public string Chain { get; set; }

        [JsonProperty("gameid")]
        public string Gameid { get; set; }

        [JsonProperty("gamestate")]
        public Gamestate Gamestate { get; set; }

        [JsonProperty("height")]
        public long Height { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }
    }

    public partial class Gamestate
    {
        [JsonProperty("players")]
        public Dictionary<string, Player> Players { get; set; }

        [JsonProperty("islands")]
        public Dictionary<string, Island> Islands { get; set; }

        [JsonProperty("resourceContributions")]
        public ResourceContributions ResourceContributions { get; set; }

        [JsonProperty("depletedContributions")]
        public DepletedContributions DepletedContributions { get; set; }

        [JsonProperty("resourceMarket")]
        public ResourceMarket ResourceMarket { get; set; }

        [JsonProperty("resourcePools")]
        public List<double> ResourcePools { get; set; }

        [JsonProperty("warbucksPool")]
        public double WarbucksPool { get; set; }

        [JsonProperty("currentConstants")]
        public CurrentConstants CurrentConstants { get; set; }

        [JsonProperty("debugBlockData")]
        public string DebugBlockData { get; set; }
    }

    public partial class CurrentConstants
    {
        [JsonProperty("version")]
        public List<long> Version { get; set; }

        [JsonProperty("isInMaintenanceMode")]
        public bool IsInMaintenanceMode { get; set; }

        [JsonProperty("recieveAddress")]
        public string RecieveAddress { get; set; }

        [JsonProperty("resourcePackCost")]
        public long ResourcePackCost { get; set; }

        [JsonProperty("resourcePackAmount")]
        public List<long> ResourcePackAmount { get; set; }

        [JsonProperty("marketFeePrecent")]
        public List<double> MarketFeePrecent { get; set; }

        [JsonProperty("minMarketFee")]
        public List<long> MinMarketFee { get; set; }

        [JsonProperty("islandSearchCost")]
        public List<long> IslandSearchCost { get; set; }

        [JsonProperty("islandModifierExponent")]
        public double IslandModifierExponent { get; set; }

        [JsonProperty("attackCostPercent")]
        public double AttackCostPercent { get; set; }

        [JsonProperty("undiscoveredPercent")]
        public double UndiscoveredPercent { get; set; }

        [JsonProperty("islandSearchReplenishTime")]
        public double IslandSearchReplenishTime { get; set; }

        [JsonProperty("islandSearchOptions")]
        public List<string> IslandSearchOptions { get; set; }

        [JsonProperty("squadHealthLimit")]
        public long SquadHealthLimit { get; set; }

        [JsonProperty("unitCosts")]
        public List<List<long>> UnitCosts { get; set; }

        [JsonProperty("blockerCosts")]
        public List<List<long>> BlockerCosts { get; set; }

        [JsonProperty("bunkerCosts")]
        public List<List<long>> BunkerCosts { get; set; }

        [JsonProperty("collectorCosts")]
        public List<List<long>> CollectorCosts { get; set; }

        [JsonProperty("unitDamages")]
        public List<double> UnitDamages { get; set; }

        [JsonProperty("unitHealths")]
        public List<long> UnitHealths { get; set; }

        [JsonProperty("unitOrderProbabilities")]
        public List<double> UnitOrderProbabilities { get; set; }

        [JsonProperty("unitCombatModifiers")]
        public List<List<double>> UnitCombatModifiers { get; set; }

        [JsonProperty("minMaxResources")]
        public List<List<long>> MinMaxResources { get; set; }

        [JsonProperty("extractRates")]
        public List<long> ExtractRates { get; set; }

        [JsonProperty("freeResourceRates")]
        public List<long> FreeResourceRates { get; set; }

        [JsonProperty("extractPeriod")]
        public long ExtractPeriod { get; set; }

        [JsonProperty("freeResourcePeriod")]
        public long FreeResourcePeriod { get; set; }

        [JsonProperty("assumedDailyBlocks")]
        public long AssumedDailyBlocks { get; set; }

        [JsonProperty("tileProbabilities")]
        public List<double> TileProbabilities { get; set; }

        [JsonProperty("resourceProbabilities")]
        public List<double> ResourceProbabilities { get; set; }

        [JsonProperty("purchaseToPoolPercents")]
        public List<List<double>> PurchaseToPoolPercents { get; set; }

        [JsonProperty("poolRewardBlocks")]
        public long PoolRewardBlocks { get; set; }

        [JsonProperty("warbucksRewardBlocks")]
        public long WarbucksRewardBlocks { get; set; }
    }

    public partial class DepletedContributions
    {
        [JsonProperty("Cairo Denji")]
        public List<string> CairoDenji { get; set; }

        [JsonProperty("tyki")]
        public List<string> Tyki { get; set; }

        [JsonProperty("Chainsaw")]
        public List<string> Chainsaw { get; set; }

        [JsonProperty("Ryumaster")]
        public List<string> Ryumaster { get; set; }

        [JsonProperty("Anubis")]
        public List<string> Anubis { get; set; }
    }

    public partial class Island
    {
        [JsonProperty("owner")]
        public string Owner { get; set; }

        [JsonProperty("features")]
        public string Features { get; set; }

        [JsonProperty("collectors")]
        public string Collectors { get; set; }

        [JsonProperty("defenses")]
        public string Defenses { get; set; }

        [JsonProperty("resources")]
        public List<List<long>> Resources { get; set; }

        [JsonProperty("squadPlans")]
        public List<List<long>> SquadPlans { get; set; }

        [JsonProperty("squadCounts")]
        public List<List<long>> SquadCounts { get; set; }

        [JsonProperty("attackingPlayers")]
        public List<string> AttackingPlayers { get; set; }
    }

    public partial class Player
    {
        [JsonProperty("nationCode")]
        public string NationCode { get; set; }

        [JsonProperty("units")]
        public List<long> Units { get; set; }

        [JsonProperty("resources")]
        public List<long> Resources { get; set; }

        [JsonProperty("islands")]
        public List<string> Islands { get; set; }

        [JsonProperty("attackableIsland")]
        public string AttackableIsland { get; set; }
    }

    public partial class ResourceContributions
    {
        [JsonProperty("Cairo Denji")]
        public List<List<long>> CairoDenji { get; set; }

        [JsonProperty("tyki")]
        public List<List<long>> Tyki { get; set; }

        [JsonProperty("Ryumaster")]
        public List<List<long>> Ryumaster { get; set; }

        [JsonProperty("Chainsaw")]
        public List<List<long>> Chainsaw { get; set; }

        [JsonProperty("Anubis")]
        public List<List<long>> Anubis { get; set; }

        [JsonProperty("liquid")]
        public List<List<long>> Liquid { get; set; }

        [JsonProperty("AutoTxXx")]
        public List<List<long>> AutoTxXx { get; set; }
    }

    public class UnitNames
    {
        public string Riflemen = "Riflemen";
        public string MachineGunners = "Machine Gunners";
        public string Bazookamen = "Bazookamen";
        public string LightTanks = "Light Tanks";
        public string MediumTanks = "Medium Tanks";
        public string HeavyTanks = "Heavy Tanks";
        public string LightFighters = "Light Figters";
        public string MediumFighters = "Medium Fighters";
        public string Bombers = "Bombers";
    }

    public class ResourceNames
    {
        public string Warbux = "Warbux";
        public string Metal = "Metal";
        public string Concrete = "Concrete";
        public string Oil = "Oil";
    }

    public partial class ResourceMarket
    {
    }

    public partial class GspRoot
    {
        public static GspRoot FromJson(string json) => JsonConvert.DeserializeObject<GspRoot>(json, IslesOfWar.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this GspRoot self) => JsonConvert.SerializeObject(self, IslesOfWar.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}

