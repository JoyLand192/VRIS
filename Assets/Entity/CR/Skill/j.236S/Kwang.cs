using Cinemachine;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Kwang", menuName = "VRIS/CR/Skills/The-New/j.236S")]
public class Kwang : Skill
{
    [SerializeField] private LineEffect reachLineEffectPrefab;
    [SerializeField] private ParticleEffect reachParticleEffectPrefab;
    [SerializeField] private LayerMask targetHitboxLayers;
    [SerializeField] private CinemachineImpulseDefinition cameraImpulseDefinition;
    private const string intervalAnimationStateName = "j_236SInterval";
    private const string animationStateName = "j_236S";
    private const float maxYDistance = 50f;
    private const float hitStopDuration = 0.1f;
    public override UniTask Execute(CR cr)
    {
        if (!CanReachPlatform(cr, out _))
        {
            cr.SkillCaster.CancelSkill();
            return UniTask.CompletedTask;
        }

        cr.Movement.Rb.velocity = Vector2.zero;
        cr.Movement.DisableGravity();
        cr.Animator.PlayState(intervalAnimationStateName);

        return UniTask.CompletedTask;
    }
    public override void Initialize(CR cr)
    {
        HashSet<Enemy> hitEnemies = new();

        void OnEnemyHit(CollisionDetector collisionDetector)
        {
            if (collisionDetector.Owner is not Enemy enemy) return;
            if (!hitEnemies.Add(enemy)) return;

            HitEnemy(enemy, cr);
        }
        void Dispose() => cr.SkillCaster.OnSkillEnd -= Dispose;
        void ActivateAttack()
        {
            var originPosition = cr.transform.position;

            cr.Animator.OnSkillIntervalEnd -= ActivateAttack;
            cr.Movement.RestoreGravity();

            var reachable = CanReachPlatform(cr, out var reachPos);
            if (!reachable)
            {
                cr.SkillCaster.CancelSkill();
                return;
            }

            var distance = reachPos - originPosition;
            var hitEnemyHitboxes = Physics2D.BoxCastAll(originPosition, Vector2.one, 0, distance.normalized, distance.magnitude, targetHitboxLayers);
            foreach (var enemy in hitEnemyHitboxes)
            {
                if (!enemy.transform.TryGetComponent<CollisionDetector>(out var hitbox)) continue;
                OnEnemyHit(hitbox);
            }

            cameraImpulseDefinition.CreateEvent(reachPos, Vector2.up);

            cr.VFX.GenerateSkillEffect(new EffectData(reachParticleEffectPrefab, reachPos));
            cr.VFX.GenerateSkillEffect(new EffectData(reachLineEffectPrefab, originPosition, reachPos));
            cr.transform.position = reachPos;
            cr.Animator.PlayState(animationStateName);
            if (hitEnemyHitboxes.Length != 0) TimeManager.Instance.HitStop(hitStopDuration);

            cr.SkillCaster.OnSkillEnd += Dispose;
        }
        cr.Animator.OnSkillIntervalEnd += ActivateAttack;
    }
    protected bool CanReachPlatform(CR cr, out Vector3 pos)
    {
        float direction = Mathf.Sign(cr.Movement.MoveRatioFixed);
        var raycast = Physics2D.Raycast(cr.transform.position, new Vector2(direction, -1).normalized, maxYDistance, cr.Movement.SurfaceLayerMaskSettings.PlatformLayer);

        Debug.Log($"{raycast == true}");

        if (raycast) pos = raycast.point;
        else pos = default;

        return raycast;
    }
    protected override DamageInfo CalculateDamage(CR cr)
    {
        return new DamageInfo(cr, 45);
    }
}
