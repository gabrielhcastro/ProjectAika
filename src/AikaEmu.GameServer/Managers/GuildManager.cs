using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AikaEmu.GameServer.Models.Guild;
using AikaEmu.Shared.Utils;
using NLog;

namespace AikaEmu.GameServer.Managers
{
    public class GuildManager : Singleton<GuildManager>
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly ConcurrentDictionary<ushort, Guild> _guilds;

        protected GuildManager()
        {
            _guilds = new ConcurrentDictionary<ushort, Guild>();
        }

        public void Init()
        {
            using (var con = DatabaseManager.Instance.GetConnection())
            {
                using (var query = con.CreateCommand())
                {
                    query.CommandText = "SELECT * FROM guilds";
                    using (var reader = query.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var guild = new Guild
                            {
                                Id = reader.GetUInt16("guild_id"),
                                Name = reader.GetString("guild_name"),
                                NationId = reader.GetByte("nation_id"),
                                Level = reader.GetByte("guild_level"),
                                Points = reader.GetUInt32("guild_points"),
                                LogoIndex = reader.GetUInt16("guild_logo"),
                                // Campos adicionais da sua classe Guild
                                Message1 = reader.IsDBNull(reader.GetOrdinal("message1")) ? "" : reader.GetString("message1"),
                                Message2 = reader.IsDBNull(reader.GetOrdinal("message2")) ? "" : reader.GetString("message2"),
                                Message3 = reader.IsDBNull(reader.GetOrdinal("message3")) ? "" : reader.GetString("message3"),
                                Message4 = reader.IsDBNull(reader.GetOrdinal("message4")) ? "" : reader.GetString("message4")
                            };
                            _guilds.TryAdd(guild.Id, guild);
                        }
                    }
                }
            }
            _log.Info("Loaded {0} guilds from database.", _guilds.Count);
        }

        public Guild GetGuild(ushort id)
        {
            return _guilds.TryGetValue(id, out var guild) ? guild : null;
        }

        public string GetGuildName(int id)
        {
            if (id <= 0) return "";
            var guild = GetGuild((ushort)id);
            return guild?.Name ?? "";
        }

        public List<Guild> GetNationGuilds(byte nationId)
        {
            return _guilds.Values.Where(g => g.NationId == nationId).ToList();
        }

        public void SaveGuild(ushort guildId)
        {
            if (!_guilds.TryGetValue(guildId, out var guild)) return;

            using (var con = DatabaseManager.Instance.GetConnection())
            {
                using (var query = con.CreateCommand())
                {
                    query.CommandText = @"UPDATE guilds SET 
                        guild_name = @name, 
                        nation_id = @nation, 
                        guild_level = @level, 
                        guild_points = @points, 
                        guild_logo = @logo,
                        message1 = @m1,
                        message2 = @m2,
                        message3 = @m3,
                        message4 = @m4
                        WHERE guild_id = @id";

                    query.Parameters.AddWithValue("@name", guild.Name);
                    query.Parameters.AddWithValue("@nation", guild.NationId);
                    query.Parameters.AddWithValue("@level", guild.Level);
                    query.Parameters.AddWithValue("@points", guild.Points);
                    query.Parameters.AddWithValue("@logo", guild.LogoIndex);
                    query.Parameters.AddWithValue("@m1", guild.Message1 ?? "");
                    query.Parameters.AddWithValue("@m2", guild.Message2 ?? "");
                    query.Parameters.AddWithValue("@m3", guild.Message3 ?? "");
                    query.Parameters.AddWithValue("@m4", guild.Message4 ?? "");
                    query.Parameters.AddWithValue("@id", guild.Id);

                    query.ExecuteNonQuery();
                }
            }
        }
    }
}