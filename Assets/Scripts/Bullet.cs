using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int damage = 1;

    public int Damage => damage;

    public void setDamage(int newDamage)
    {
        damage = newDamage;
    }

    void OnCollisionEnter(Collision collision)
    {
        //destroy the bullet
        Destroy(gameObject);
    }
}