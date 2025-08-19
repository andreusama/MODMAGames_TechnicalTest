public interface IDamageable
{
    void TakeDamage(float amount, bool allyFire = false);
    bool IsAlive { get; }
}