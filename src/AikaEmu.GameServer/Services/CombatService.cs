using System;
using AikaEmu.GameServer.Models.Units;
using AikaEmu.GameServer.Models.Units.Character;
using AikaEmu.GameServer.Models.Units.Mob;

namespace AikaEmu.GameServer.Services
{
    public enum DamageType : byte
    {
        Normal = 0,
        Critical = 1,
        Double = 2, // Exemplo, pode ser ajustado conforme o cliente
        Miss = 3,
        Immune = 4,
        Block = 5
    }

    public static class CombatService
    {
        private static readonly Random _random = new Random();

        public static (int damage, DamageType type) CalculateDamage(BaseUnit attacker, BaseUnit target, ushort skillId)
        {
            int finalDamage = 0;
            DamageType damageType = DamageType.Normal;

            bool isPhysical = IsPhysicalAttack(attacker, skillId);

            // Obter atributos de ataque e defesa
            int attackerAttack = 0;
            int targetDefense = 0;
            int attackerPenetration = 0;
            int attackerCritical = 0;
            int targetCriticalResistance = 0;
            int attackerAccuracy = 0;
            int targetEvasion = 0;

            if (attacker is Character charAttacker)
            {
                attackerAttack = isPhysical ? charAttacker.Attributes.Strenght : charAttacker.Attributes.Intelligence; // Simplificado, idealmente seria DNFis/DNMag
                attackerPenetration = isPhysical ? charAttacker.Attributes.FisPenetration : charAttacker.Attributes.MagPenetration;
                attackerCritical = charAttacker.Attributes.Critical;
                attackerAccuracy = charAttacker.Attributes.Accuracy;
            }
            else if (attacker is Mob mobAttacker)
            {
                // TODO: Implementar atributos de ataque para Mob
                attackerAttack = 50; // Valor placeholder para Mob
                attackerPenetration = 0; // Placeholder
                attackerCritical = 5; // Placeholder
                attackerAccuracy = 10; // Placeholder
            }

            if (target is Character charTarget)
            {
                targetDefense = isPhysical ? charTarget.Attributes.Defense : charTarget.Attributes.MagicDefense; // Simplificado, idealmente seria DEFFis/DEFMAG
                targetCriticalResistance = charTarget.Attributes.CritRes;
                targetEvasion = charTarget.Attributes.Evasion;
            }
            else if (target is Mob mobTarget)
            {
                // TODO: Implementar atributos de defesa para Mob
                targetDefense = 20; // Valor placeholder para Mob
                targetCriticalResistance = 0; // Placeholder
                targetEvasion = 5; // Placeholder
            }

            damageType = DetermineHitOrMiss(attackerAccuracy, targetEvasion);
            if (damageType == DamageType.Miss) return (0, DamageType.Miss);

            int effectiveDefense = targetDefense - ((targetDefense / 100) * attackerPenetration);
            if (effectiveDefense < 0) effectiveDefense = 0;

            finalDamage = attackerAttack - (effectiveDefense / 8);
            if (finalDamage <= 0) finalDamage = 1; // Dano mínimo de 1

            DamageType criticalType = DetermineCritical(attackerCritical, targetCriticalResistance);
            if (criticalType == DamageType.Critical)
            {
                finalDamage = (int)(finalDamage * 1.5); // Multiplicador de crítico (exemplo)
                damageType = DamageType.Critical;
            }
            // TODO: Implementar Double Critical se aplicável

            // TODO: Adicionar bônus/reduções de equipamentos, buffs, etc.

            // Random variability no dano
            Random rand = new Random();
            int randomDamage = rand.Next((int)(finalDamage * 0.8), finalDamage);

            return (randomDamage, damageType);
        }

        private static bool IsPhysicalAttack(BaseUnit attacker, ushort skillId)
        {
            // Lógica simplificada: SkillId 0 é ataque básico (físico)
            // Classes 0-3 são sempre físicas (precisaria do tipo de classe do attacker)
            // Para um sistema completo, seria necessário um SkillData para verificar o tipo da skill
            return skillId == 0;
        }

        private static DamageType DetermineHitOrMiss(int attackerAccuracy, int targetEvasion)
        {
            int hitChance = attackerAccuracy - targetEvasion;
            int randomValue = _random.Next(1, 101); // 1 a 100

            if (hitChance >= 0)
            {
                // Atacante tem mais ou igual acerto que esquiva do alvo
                if (randomValue <= 20 && targetEvasion >= 7) // Exemplo de chance de miss mesmo com acerto alto
                {
                    return DamageType.Miss;
                }
            }
            else // hitChance < 0 (alvo tem mais esquiva)
            {
                hitChance = Math.Abs(hitChance);
                if (hitChance > 10) hitChance = 10; // Limite a penalidade

                if (randomValue <= (30 + hitChance) && targetEvasion >= 3) // Exemplo de chance de miss aumentada
                {
                    return DamageType.Miss;
                }
            }
            return DamageType.Normal;
        }

        private static DamageType DetermineCritical(int attackerCritical, int targetCriticalResistance)
        {
            int effectiveCriticalResistance = (int)((targetCriticalResistance + 10) * 1.4);
            int criticalChance = attackerCritical - effectiveCriticalResistance;

            if (criticalChance >= 0)
            {
                int randomValue = _random.Next(1, 101);
                if (criticalChance > 25) criticalChance = 25; // Limite o bônus de chance de crítico

                if ((randomValue + criticalChance) >= 40 && attackerCritical >= 5) // Exemplo de chance de crítico
                {
                    return DamageType.Critical;
                }
            }
            return DamageType.Normal;
        }
    }
}