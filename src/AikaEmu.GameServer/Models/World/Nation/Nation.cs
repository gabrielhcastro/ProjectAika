using System.Collections.Generic;

namespace AikaEmu.GameServer.Models.World.Nation
{
    public class NationRoleInfo
    {
        public string LordMarechal { get; set; } = "";
        public string Tactician { get; set; } = "";
        public string Judge { get; set; } = "";
        public string Treasurer { get; set; } = "";

        // IDs das Guildas associadas aos cargos (Migrado do Delphi)
        public int LordMarechalGuildId { get; set; }
        public int TacticianGuildId { get; set; }
        public int JudgeGuildId { get; set; }
        public int TreasurerGuildId { get; set; }
    }

    public class Nation
    {
        public byte Id { get; set; }
        public string Name { get; set; } = "";

        public ushort TaxCitizen { get; set; }
        public ushort TaxVisitor { get; set; }
        public long Settlement { get; set; }
        public byte Stabilization { get; set; }
        public long NationGold { get; set; }

        // Campos de Aliança
        public byte NationAlly { get; set; }
        public uint AllyDate { get; set; }
        public string MarshalAllyName { get; set; } = "";
        public byte Rank { get; set; }

        // Estrutura de Governo (Defensores)
        public NationRoleInfo Defenders { get; set; } = new NationRoleInfo();

        // Estrutura de Atacantes (Migrado do Delphi TNationCerco)
        // No Delphi são 4 slots de atacantes (ARRAY [0 .. 3] OF TGuildsAlly)
        public List<NationRoleInfo> Attackers { get; set; } = new List<NationRoleInfo>();

        public Nation()
        {
            for (int i = 0; i < 4; i++)
            {
                Attackers.Add(new NationRoleInfo());
            }
        }
    }
}
