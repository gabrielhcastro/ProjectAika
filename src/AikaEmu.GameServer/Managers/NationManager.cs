using System;
using System.Collections.Generic;
using AikaEmu.GameServer.Models.World.Devir;
using AikaEmu.GameServer.Models.World.Nation;
using AikaEmu.Shared.Utils;
using NLog;

namespace AikaEmu.GameServer.Managers
{
    public class NationManager : Singleton<NationManager>
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<NationId, Dictionary<DevirId, Devir>> _devirsDictionary;
        private readonly Dictionary<byte, Nation> _nations;

        public NationManager()
        {
            // Reliques
            _devirsDictionary = new Dictionary<NationId, Dictionary<DevirId, Devir>>();
            foreach (NationId nationId in Enum.GetValues(typeof(NationId)))
            {
                if (nationId == NationId.None) continue;
                _devirsDictionary.Add(nationId, new Dictionary<DevirId, Devir>());
                foreach (DevirId devirId in Enum.GetValues(typeof(DevirId)))
                    _devirsDictionary[nationId].Add(devirId, new Devir(nationId, devirId));
            }

            _nations = new Dictionary<byte, Nation>();
        }

        public void Init()
        {
            LoadNations();

            // Reliques
            using (var con = DatabaseManager.Instance.GetConnection())
            {
                using (var query = con.CreateCommand())
                {
                    query.CommandText = "SELECT * FROM devir_slots";
                    query.Prepare();
                    using (var reader = query.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var nationId = (NationId) reader.GetByte("nation_id");
                            var slotId = reader.GetByte("slot_id");
                            var devirId = (DevirId) reader.GetByte("devir_id");
                            var devirslot = new DevirSlot(reader.GetUInt16("item_id"))
                            {
                                Slot = slotId,
                                IsActive = reader.GetBoolean("is_active"),
                                Name = reader.GetString("put_name"),
                                Time = reader.GetUInt32("put_time")
                            };
                            _devirsDictionary[nationId][devirId].Slots.Add(slotId, devirslot);
                        }
                    }
                }
            }

            _log.Info("Loaded {0} nations and their devirs.", _devirsDictionary.Count);
        }

        private void LoadNations()
        {
            using (var con = DatabaseManager.Instance.GetConnection())
            {
                using (var query = con.CreateCommand())
                {
                    query.CommandText = "SELECT * FROM nations";
                    using (var reader = query.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var nation = new Nation
                            {
                                Id = reader.GetByte("nation_id"),
                                Name = reader.GetString("nation_name"),
                                Rank = reader.GetByte("nation_rank"),
                                TaxCitizen = (ushort)reader.GetInt32("citizen_tax"),
                                TaxVisitor = (ushort)reader.GetInt32("visitor_tax"),
                                Settlement = reader.GetInt64("settlement"),
                                NationGold = reader.GetInt64("nation_gold"),
                                NationAlly = reader.GetByte("nation_ally"),
                                MarshalAllyName = reader.GetString("marechal_ally"),
                                AllyDate = (uint)reader.GetInt32("ally_date"),
                                Stabilization = reader.GetByte("stabilization")
                            };

                            // IDs dos Defensores
                            nation.Defenders.LordMarechalGuildId = reader.GetInt32("guild_id_marshal");
                            nation.Defenders.TacticianGuildId = reader.GetInt32("guild_id_tactician");
                            nation.Defenders.JudgeGuildId = reader.GetInt32("guild_id_judge");
                            nation.Defenders.TreasurerGuildId = reader.GetInt32("guild_id_treasurer");

                            // Resolver nomes das guildas automaticamente
                            ResolveGovernmentNames(nation);

                            _nations.Add(nation.Id, nation);
                        }
                    }
                }
            }
            _log.Info("Loaded {0} nations data from database.", _nations.Count);
        }

        /// <summary>
        /// Resolve os nomes das guildas do governo a partir dos IDs
        /// </summary>
        public void ResolveGovernmentNames(Nation nation)
        {
            nation.Defenders.LordMarechal = GuildManager.Instance.GetGuildName(nation.Defenders.LordMarechalGuildId);
            nation.Defenders.Tactician = GuildManager.Instance.GetGuildName(nation.Defenders.TacticianGuildId);
            nation.Defenders.Judge = GuildManager.Instance.GetGuildName(nation.Defenders.JudgeGuildId);
            nation.Defenders.Treasurer = GuildManager.Instance.GetGuildName(nation.Defenders.TreasurerGuildId);
        }

        public void SaveNation(byte nationId)
        {
            if (!_nations.ContainsKey(nationId)) return;
            var nation = _nations[nationId];

            using (var con = DatabaseManager.Instance.GetConnection())
            {
                using (var query = con.CreateCommand())
                {
                    query.CommandText = @"UPDATE nations SET 
                        nation_rank = @rank, 
                        citizen_tax = @ctax, 
                        visitor_tax = @vtax, 
                        settlement = @settlement, 
                        nation_gold = @gold, 
                        nation_ally = @ally, 
                        marechal_ally = @mally, 
                        ally_date = @adate, 
                        stabilization = @stab,
                        guild_id_marshal = @g1,
                        guild_id_tactician = @g2,
                        guild_id_judge = @g3,
                        guild_id_treasurer = @g4
                        WHERE nation_id = @id";

                    query.Parameters.AddWithValue("@rank", nation.Rank);
                    query.Parameters.AddWithValue("@ctax", nation.TaxCitizen);
                    query.Parameters.AddWithValue("@vtax", nation.TaxVisitor);
                    query.Parameters.AddWithValue("@settlement", nation.Settlement);
                    query.Parameters.AddWithValue("@gold", nation.NationGold);
                    query.Parameters.AddWithValue("@ally", nation.NationAlly);
                    query.Parameters.AddWithValue("@mally", nation.MarshalAllyName);
                    query.Parameters.AddWithValue("@adate", nation.AllyDate);
                    query.Parameters.AddWithValue("@stab", nation.Stabilization);
                    query.Parameters.AddWithValue("@g1", nation.Defenders.LordMarechalGuildId);
                    query.Parameters.AddWithValue("@g2", nation.Defenders.TacticianGuildId);
                    query.Parameters.AddWithValue("@g3", nation.Defenders.JudgeGuildId);
                    query.Parameters.AddWithValue("@g4", nation.Defenders.TreasurerGuildId);
                    query.Parameters.AddWithValue("@id", nation.Id);

                    query.ExecuteNonQuery();
                }
            }
            
            // Após salvar, garantir que os nomes estão atualizados
            ResolveGovernmentNames(nation);
        }

        public Nation GetNation(byte id)
        {
            return _nations.ContainsKey(id) ? _nations[id] : null;
        }

        public Dictionary<DevirId, Devir> GetNationDevirs(NationId nationId)
        {
            return _devirsDictionary.ContainsKey(nationId) ? _devirsDictionary[nationId] : null;
        }

        public List<ushort> GetReliquesList(NationId nationId)
        {
            var list = new List<ushort>();
            if (_devirsDictionary.ContainsKey(nationId))
            {
                foreach (var devir in _devirsDictionary[nationId].Values)
                foreach (var devirSlot in devir.Slots.Values)
                    if (devirSlot.ItemId != 0)
                        list.Add(devirSlot.ItemId);
            }

            return list;
        }
    }
}
