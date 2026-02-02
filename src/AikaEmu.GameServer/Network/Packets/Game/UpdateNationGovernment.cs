using AikaEmu.GameServer.Models.World.Nation;
using AikaEmu.GameServer.Network.GameServer;
using AikaEmu.Shared.Network;

namespace AikaEmu.GameServer.Network.Packets.Game
{
    /// <summary>
    /// Pacote de Informações de Governo e Nação
    /// Migrado de TGuildsGradePacket do AikaDelphi (Opcode $936)
    /// Tamanho Total: 160 bytes (incluindo Header de 12 bytes)
    /// </summary>
    public class UpdateNationGovernment : GamePacket
    {
        private readonly Nation _nation;

        public UpdateNationGovernment(ushort conId, Nation nation)
        {
            _nation = nation;
            Opcode = (ushort)GameOpcode.UpdateNationGovernment;
            SenderId = conId;
        }

        public override PacketStream Write(PacketStream stream)
        {
            // O Header (12 bytes) é escrito automaticamente

            // TGuildsAlly (LordMarechal, Estrategista, Juiz, Tesoureiro) - 20 bytes cada
            // Aqui usamos os nomes reais que devem ser preenchidos pelo NationManager/GuildManager
            stream.Write(_nation.Defenders.LordMarechal ?? "", 20);
            stream.Write(_nation.Defenders.Tactician ?? "", 20);
            stream.Write(_nation.Defenders.Judge ?? "", 20);
            stream.Write(_nation.Defenders.Treasurer ?? "", 20);

            // GuildsId: ARRAY [0 .. 03] OF WORD (8 bytes total)
            // Agora usando os IDs reais do banco de dados
            stream.Write((ushort)_nation.Defenders.LordMarechalGuildId);
            stream.Write((ushort)_nation.Defenders.TacticianGuildId);
            stream.Write((ushort)_nation.Defenders.JudgeGuildId);
            stream.Write((ushort)_nation.Defenders.TreasurerGuildId);

            // Nation: DWORD (4 bytes)
            stream.Write((uint)_nation.Id);

            // RegisterBonus: Int64 (8 bytes) - No Delphi é o Settlement
            stream.Write((long)_nation.Settlement);

            // Null_0: Byte (1 byte)
            stream.Write((byte)0);

            // CitizenTax: Byte (1 byte)
            stream.Write((byte)_nation.TaxCitizen);

            // NoCitizenTax: Byte (1 byte)
            stream.Write((byte)_nation.TaxVisitor);

            // NationAlly: Byte (1 byte)
            stream.Write(_nation.NationAlly);

            // AllyanceDate: DWORD (4 bytes)
            stream.Write(_nation.AllyDate);

            // Null_1: DWORD (4 bytes)
            stream.Write((uint)0);

            // MarshalAlly: Array [0 .. 15] of AnsiChar (16 bytes)
            stream.Write(_nation.MarshalAllyName ?? "", 16);

            // UnkBytes: Array [0 .. 11] of Byte (12 bytes)
            stream.Write(new byte[12]);

            // RankNation: Byte (1 byte)
            stream.Write(_nation.Rank);

            // Estabilization: Byte (1 byte)
            stream.Write(_nation.Stabilization);

            // UnkBytes2: Array [0 .. 5] of Byte (6 bytes)
            stream.Write(new byte[6]);

            return stream;
        }
    }
}