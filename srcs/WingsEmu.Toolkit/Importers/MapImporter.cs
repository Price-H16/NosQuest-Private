// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using OpenNos.Core;
using OpenNos.Core.Logging;
using OpenNos.DAL;
using WingsEmu.DTOs;

namespace WingsEmu.Toolkit.Importers
{
    public class MapImporter
    {
        private readonly ImportConfiguration _configuration;
        public MapImporter(ImportConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void Import()
        {
            string fileMapIdDat = $"{_configuration.Folder}/MapIDData.dat";
            string fileMapIdLang = $"{_configuration.Folder}/_code_{_configuration.Lang}_MapIDData.txt";
            string folderMap = $"{_configuration.Folder}/map";
            IEnumerable<MapDTO> inDbMaps = DaoFactory.Instance.MapDao.LoadAll();
            List<MapDTO> maps = new List<MapDTO>();
            Dictionary<int, string> dictionaryId = new Dictionary<int, string>();
            Dictionary<string, string> dictionaryIdLang = new Dictionary<string, string>();
            Dictionary<int, int> dictionaryMusic = new Dictionary<int, int>();

            string line;
            int i = 0;
            using (var mapIdStream = new StreamReader(fileMapIdDat, Encoding.GetEncoding(1252)))
            {
                while ((line = mapIdStream.ReadLine()) != null)
                {
                    string[] linesave = line.Split(' ');
                    if (linesave.Length <= 1)
                    {
                        continue;
                    }

                    if (!int.TryParse(linesave[0], out int mapid))
                    {
                        continue;
                    }

                    if (!dictionaryId.ContainsKey(mapid))
                    {
                        dictionaryId.Add(mapid, linesave[4]);
                    }
                }
            }

            using (var mapIdLangStream = new StreamReader(fileMapIdLang, Encoding.GetEncoding(1252)))
            {
                while ((line = mapIdLangStream.ReadLine()) != null)
                {
                    string[] linesave = line.Split('\t');
                    if (linesave.Length <= 1 || dictionaryIdLang.ContainsKey(linesave[0]))
                    {
                        continue;
                    }

                    dictionaryIdLang.Add(linesave[0], linesave[1]);
                }
            }

            foreach (string[] linesave in _configuration.Packets.Where(o => o[0].Equals("at")))
            {
                if (linesave.Length <= 7 || linesave[0] != "at")
                {
                    continue;
                }

                if (dictionaryMusic.ContainsKey(int.Parse(linesave[2])))
                {
                    continue;
                }

                dictionaryMusic.Add(int.Parse(linesave[2]), int.Parse(linesave[7]));
            }

            foreach (FileInfo file in new DirectoryInfo(folderMap).GetFiles())
            {
                string name = string.Empty;
                int music = 0;

                if (dictionaryId.ContainsKey(int.Parse(file.Name)) && dictionaryIdLang.ContainsKey(dictionaryId[int.Parse(file.Name)]))
                {
                    name = dictionaryIdLang[dictionaryId[int.Parse(file.Name)]];
                }

                if (dictionaryMusic.ContainsKey(int.Parse(file.Name)))
                {
                    music = dictionaryMusic[int.Parse(file.Name)];
                }

                var map = new MapDTO
                {
                    Name = name,
                    Music = music,
                    MapId = short.Parse(file.Name),
                    Data = File.ReadAllBytes(file.FullName),
                    ShopAllowed = short.Parse(file.Name) == 147
                };
                if (inDbMaps.Any(s => s.MapId == map.MapId)) 
                {
                    continue; // Map already exists in list
                }

                maps.Add(map);
                i++;
            }

            DaoFactory.Instance.MapDao.Insert(maps);
            Logger.Log.Info(string.Format(Language.Instance.GetMessageFromKey("MAPS_PARSED"), i));
        }
    }
}