using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableSelection
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), @"Data\teams.json");
            if (!File.Exists(path))
            {
                Console.WriteLine($"File [{path}] does not exist!");
                return;
            }
            var teams = SerializationHelper.Deserialize<List<dynamic>>(File.ReadAllText(path));
            int teamCount = teams.Count;
            // ramdomize teams
            Random random = new Random();
            var newTeams = teams.Select((t, i) => new { TeamName = (string)t.name, StaffCount = (int)t.count, Tables = new List<int>() })
                .OrderBy(n => random.Next(100)).ToList();
            int startTable = 13;
            int seatRemainsInPreviousTable = 0;
            foreach (var team in newTeams)
            {
                int staffCount = team.StaffCount;
                if (seatRemainsInPreviousTable > 0)
                {
                    team.Tables.Add(startTable);
                    if (team.StaffCount <= seatRemainsInPreviousTable)
                    {
                        seatRemainsInPreviousTable = seatRemainsInPreviousTable - team.StaffCount;
                        if (seatRemainsInPreviousTable == 0)
                        {
                            startTable++;
                        }
                        continue;
                    }
                    startTable++;
                    staffCount = team.StaffCount - seatRemainsInPreviousTable;
                }
                int tabelRequired = (staffCount - 1) / 10 + 1;
                int remains = staffCount % 10;
                int endTable = startTable + tabelRequired;
                for (; startTable < endTable; startTable++)
                {
                    team.Tables.Add(startTable);
                }
                if (remains > 0)
                {
                    seatRemainsInPreviousTable = 10 - remains;
                    startTable--;
                }
                else
                {
                    seatRemainsInPreviousTable = 0;
                }
            }
            Console.WriteLine($"Total staff count:[{newTeams.Sum(t => t.StaffCount)}]");
            foreach (var team in newTeams)
            {
                Console.WriteLine($"Team:[{team.TeamName}]  StaffCount:[{team.StaffCount}]  Tables = [{string.Join(",", team.Tables)}]");
            }
            Console.ReadKey();
        }
    }
}
