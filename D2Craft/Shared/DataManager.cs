﻿using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static D2Craft.Shared.Constants;

namespace D2Craft.Shared
{
    public class DataManager
    {

        public List<CubeMain> Recipes { get; set; }
        public List<ItemStatCost> ISC { get; set; }
        public List<Properties> Properties { get; set; }
        public List<MagicAffix> Prefixes { get; set; }
        public List<MagicAffix> Suffixes { get; set; }
        public List<StringTbl> Strings { get; set; }
        public Dictionary<string, string> TypeMap { get; set; }
        public StateContainer StateContainer { get; set; }

        public DataManager(StateContainer stateContainer)
        {
            StateContainer = stateContainer;
            Recipes = ReadCsv<CubeMain>(stateContainer.DataFiles[DataFileTypes.CubeMain]);
            ConvertRecipes();
            ISC = ReadCsv<ItemStatCost>(stateContainer.DataFiles[DataFileTypes.ItemStatCost]);
            Properties = ReadCsv<Properties>(stateContainer.DataFiles[DataFileTypes.Properties]);
            Prefixes = ReadCsv<MagicAffix>(stateContainer.DataFiles[DataFileTypes.MagicPrefix]);
            Suffixes = ReadCsv<MagicAffix>(stateContainer.DataFiles[DataFileTypes.MagicSuffix]);
            var strList1 = ReadCsv<StringTbl>(stateContainer.DataFiles[DataFileTypes.Strings]);
            var strList2 = ReadCsv<StringTbl>(stateContainer.DataFiles[DataFileTypes.ExpStrings]);
            var strList3 = ReadCsv<StringTbl>(stateContainer.DataFiles[DataFileTypes.PatchStrings]);
            Strings = strList1.Union(strList2).Union(strList3).ToList();

        }

        public static List<T> ReadCsv<T>(string file, bool hasHeader = true)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = hasHeader,
                Delimiter = "\t"
            };
            byte[] byteArray = Encoding.ASCII.GetBytes(file);
            MemoryStream stream = new MemoryStream(byteArray);
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, config);
            var records = csv.GetRecords<T>();
            return records.ToList();
        }

        public void InitTypeMap()
        {
            TypeMap = new Dictionary<string, string>
            {
                { "amul", "Amulet" },
                { "belt", "Belt" },
                { "boot", "Boots" },
                { "glov", "Gloves" },
                { "helm", "Helm" },
                { "ring", "Ring" },
                { "shld", "Shield" },
                { "tors", "Chest" },
                { "weap", "Weapon" },
            };
        }

        public void ConvertRecipes()
        {
            if (TypeMap == null)
            {
                InitTypeMap();
            }
            foreach (var recipe in Recipes)
            {
                recipe.ItemType = TypeMap[recipe.Input1];
                recipe.InputTypes = new string[3];
                recipe.InputTypes[0] = recipe.Input2;
                recipe.InputTypes[1] = recipe.Input3;
                recipe.InputTypes[2] = recipe.Input4;
            }
        }
    }
}
