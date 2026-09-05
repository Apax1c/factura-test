using UnityEngine;

namespace Factura.Enemies
{
    [RequireComponent(typeof(Animator))]
    public sealed class EnemyAnimator : MonoBehaviour
    {
        private static readonly int RunningId = Animator.StringToHash("Running");
        private static readonly int AttackId = Animator.StringToHash("Attack");
        private static readonly int DeadId = Animator.StringToHash("Dead");
        private static readonly int AttackStateId = Animator.StringToHash("Attack");

        [SerializeField] private EnemyAgent _agent;

        [Tooltip("Point in the attack clip where the strike connects, as a fraction of its length. " +
                 "Read from the state's own progress, so changing the state speed keeps it in step.")]
        [SerializeField, Range(0f, 1f)] private float _attackImpactPoint = 0.4f;

        private Animator _animator;
        private bool _impactPending;

        private void Awake() => _animator = GetComponent<Animator>();

        private void OnEnable()
        {
            if (!_agent)
                return;

            _agent.Attacked += OnAttacked;
            _agent.Killed += OnKilled;

            _animator.Rebind();
            _animator.SetBool(DeadId, false);
            _animator.SetBool(RunningId, false);
            _impactPending = false;
        }

        private void OnDisable()
        {
            if (!_agent)
                return;

            _agent.Attacked -= OnAttacked;
            _agent.Killed -= OnKilled;
        }

        private void Update()
        {
            if (!_agent)
                return;

            _animator.SetBool(RunningId, _agent.State == EnemyState.Chase);
            TickAttackImpact();
        }

        private void TickAttackImpact()
        {
            if (!_impactPending)
                return;

            AnimatorStateInfo state = _animator.GetCurrentAnimatorStateInfo(0);
            if (state.shortNameHash != AttackStateId)
                return;

            if (state.normalizedTime < _attackImpactPoint)
                return;

            _impactPending = false;
            _agent.DeliverAttack();
        }

        private void OnAttacked()
        {
            _animator.SetTrigger(AttackId);
            _impactPending = true;
        }

        private void OnKilled()
        {
            _impactPending = false;
            _animator.SetBool(DeadId, true);
        }
    }
}
