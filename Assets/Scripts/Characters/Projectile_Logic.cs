using UnityEngine;

public abstract class Projectile_Logic : MonoBehaviour
{
    protected float destroyTime = 3f;
    protected float maxDistance;
    protected int damage { get; set; }
    protected damageT typeDamage;
    protected CanBeWeapon canBeWeapon = new CanBeWeapon();
    [SerializeField] protected EffectData effectBul { get; set; }
    [SerializeField] protected Sprite[] sprites;                      //Разные спрайты пуль, если нужны
    [SerializeField] protected TypeParticle typeParticle;             //Частицы, если нужны

    protected SpriteRenderer spRen;
    protected Vector2 startPosition;   // Стартовая позиция снаряда
    private float sqrMaxDistance; // Кэшируем квадрат дистанции
    protected virtual void Awake()
    {
        if(sprites != null)
            spRen = GetComponent<SpriteRenderer>();
    }
    protected virtual void Start()
    {
        if (sprites != null && sprites.Length > 0)
        {
            spRen.sprite = sprites[Random.Range(0, sprites.Length)];
        }
        startPosition = transform.position; // Сохраняем начальную позицию снаряда

        Invoke(nameof(DestroyP), destroyTime);
        //Destroy(gameObject, destroyTime);
    }
    public virtual void SetStats(float _maxDistance = 7f, int _damage = 1, EffectData _effectBul = null, damageT _typeDamage = damageT.Physical, bool _CanBeMissed = true)
    {
        maxDistance = _maxDistance;
        damage = _damage;
        effectBul = _effectBul;
        typeDamage = _typeDamage;
        canBeWeapon.canBeMissed = _CanBeMissed;
        sqrMaxDistance = maxDistance * maxDistance;
    }
    protected virtual void Update()
    {
        if (((Vector2)transform.position - startPosition).sqrMagnitude > sqrMaxDistance)
        {
            DestroyP();
            //Destroy(gameObject);
        }
    }
    protected abstract void OnTriggerEnter2D(Collider2D collider);
    protected virtual void DestroyP()
    {
        if (typeParticle != TypeParticle.None) Instantiate(ResourcesData.GetParticalPrefab(typeParticle), transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
