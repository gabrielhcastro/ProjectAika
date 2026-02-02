using AikaEmu.GameServer.Models.Guild;
using AikaEmu.GameServer.Network.GameServer;
using AikaEmu.Shared.Network;

namespace AikaEmu.GameServer.Network.Packets.Game
{
    /// <summary>
    /// Pacote de Informações da Guilda
    /// Migrado de TGuildInfoPacket do AikaDelphi (Opcode $965)
    /// </summary>
    public class SendGuildInfo : GamePacket
    {
        private readonly Guild _guild;

        public SendGuildInfo(ushort conId, Guild guild)
        {
            _guild = guild;
            Opcode = (ushort)GameOpcode.SendGuildInfo;
            SenderId = conId;
        }

        public override PacketStream Write(PacketStream stream)
        {
            // Header (12 bytes) é automático

            // GuildIndex: WORD
            stream.Write(_guild.Id);

            // Null_1: Byte, Nation: Byte
            stream.Write((byte)0);
            stream.Write(_guild.NationId);

            // GuildName: ARRAY [0 .. 17] OF AnsiChar (18 bytes)
            stream.Write(_guild.Name ?? "", 18);

            // Notices: ARRAY [0 .. 02] OF TGuildNotice (3 * 36 bytes = 108 bytes)
            // TGuildNotice = Null (WORD) + Text (34 bytes)
            // Usando as mensagens da sua classe Guild
            WriteNotice(stream, _guild.Message1);
            WriteNotice(stream, _guild.Message2);
            WriteNotice(stream, _guild.Message3);

            // Null_2: WORD
            stream.Write((ushort)0);

            // Site: ARRAY [0 .. 37] OF AnsiChar (38 bytes)
            stream.Write(_guild.Message4 ?? "", 38); // Usando Message4 como site/info adicional

            // NULL_BYTES: ARRAY [0 .. 5] OF Byte
            stream.Write(new byte[6]);

            // GuildIndex_1: WORD
            stream.Write(_guild.Id);

            // Null_3: DWORD
            stream.Write((uint)0);

            // GuildsAlly: ARRAY [0 .. 03] OF TGuildAllyItem (4 * 20 bytes = 80 bytes)
            // TGuildAllyItem = Index (WORD) + Name (18 bytes)
            for (int i = 0; i < 4; i++)
            {
                stream.Write((ushort)0); // Index do aliado
                stream.Write("", 18);    // Nome do aliado
            }

            // Null_4: WORD
            stream.Write((ushort)0);

            // Exp: DWORD
            stream.Write(_guild.Points);

            // Level: Byte
            stream.Write(_guild.Level);

            // Unk_1: ARRAY [0 .. 2] OF Byte
            stream.Write(new byte[3]);

            // RanksConfig: ARRAY [0 .. 4] OF Byte
            stream.Write(new byte[5]);

            // MsgTaxa: ARRAY [0 .. 127] OF AnsiChar (128 bytes)
            stream.Write("", 128);

            // NOT_USE: ARRAY [0 .. 2] OF Byte
            stream.Write(new byte[3]);

            // BravePoints: DWORD
            stream.Write((uint)0);

            // Promote: Boolean (1 byte)
            stream.Write(false);

            // SkillPoints: Byte
            stream.Write((byte)0);

            // Null_5: ARRAY [0 .. 45] OF Byte
            stream.Write(new byte[46]);

            return stream;
        }

        private void WriteNotice(PacketStream stream, string text)
        {
            stream.Write((ushort)0); // Null WORD
            stream.Write(text ?? "", 34); // Text AnsiChar[34]
        }
    }
}
