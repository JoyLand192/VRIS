using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : ScriptableObject
{
    [field: SerializeField] public string SkillName { get; protected set; }
    [field: SerializeField] public bool MovementBlock { get; protected set; } = true;
    [field: SerializeField] public bool DashBlock { get; protected set; } = true;
    public abstract UniTask Execute(CR cr);
    public virtual void Initialize(CR cr)
    {
        HashSet<Enemy> hitEnemies = new();

        void OnEnemyHitHandler(CollisionDetector collisionDetector)
        {
            if (collisionDetector.Owner is not Enemy enemy) return;
            if (!hitEnemies.Add(enemy)) return;

            HitEnemy(enemy, cr);
        }
        void Dispose()
        {
            cr.Hitbox.OnEntityHit -= OnEnemyHitHandler;
            cr.SkillCaster.OnSkillEnd -= Dispose;
        }
        cr.Hitbox.OnEntityHit += OnEnemyHitHandler;
        cr.SkillCaster.OnSkillEnd += Dispose;
    }
    protected virtual void HitEnemy(Enemy enemy, CR caster)
    {
        enemy.ReceiveDamage(CalculateDamage(caster));
    }
    protected abstract DamageInfo CalculateDamage(CR cr);
    public virtual bool CheckCondition(CR cr) => true;
}
