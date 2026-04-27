using UnityEngine;

public class WeaponController : MonoBehaviour
{
    Transform pivot;
    float rotationSpeed;
    BallController owner;
    int weaponDamage = 1;

    public bool IsPlayerWeapon { get; private set; }

    public void Init(Transform pivot, float rotationSpeed, BallController owner)
    {
        this.pivot = pivot;
        this.rotationSpeed = rotationSpeed;
        this.owner = owner;
        IsPlayerWeapon = owner.IsPlayer;
    }

    void Update()
    {
        pivot.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 상대 공 본체에 맞은 경우
        var hitBall = other.GetComponent<BallController>();
        if (hitBall != null && hitBall.IsPlayer != owner.IsPlayer)
        {
            Fx.SlowMo();
            Fx.SpawnRing(other.transform.position);
            Fx.SpawnDamage(other.transform.position, weaponDamage);
            hitBall.TakeDamage(weaponDamage);
            weaponDamage++;
            rotationSpeed = -rotationSpeed;
            return;
        }

        // 상대 무기에 맞은 경우
        var hitWeapon = other.GetComponent<WeaponController>();
        if (hitWeapon != null && hitWeapon.IsPlayerWeapon != IsPlayerWeapon)
        {
            rotationSpeed = -rotationSpeed;
            Fx.SpawnRing(transform.position);
        }
    }
}
