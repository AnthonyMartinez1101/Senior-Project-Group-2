
public enum DamageType
{
    Sickle,
    Bullet,
    Explosion,
    Enemy,
    Poison
}

public interface IDamageable
{
    void TakeDamage(float damageDealt, DamageType damageType);
}
