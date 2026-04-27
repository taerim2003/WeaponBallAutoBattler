using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D), typeof(SpriteRenderer))]
public class BallController : MonoBehaviour
{
    [SerializeField] float speed = 6f;
    [SerializeField] float radius = 0.5f;

    [Header("Weapon")]
    [SerializeField] float weaponDistance = 1.2f;
    [SerializeField] float weaponLength = 1.0f;
    [SerializeField] float weaponWidth = 0.2f;
    [SerializeField] float weaponRotationSpeed = 180f;

    [Header("Stats")]
    [SerializeField] int maxHp = 50;

    public bool IsPlayer = true;
    int currentHp;
    TextMeshProUGUI hpText;
    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.sharedMaterial = new PhysicsMaterial2D { bounciness = 1f, friction = 0f };

        GetComponent<CircleCollider2D>().radius = 0.5f;
        transform.localScale = new Vector3(radius * 2f, radius * 2f, 1f);

        var sr = GetComponent<SpriteRenderer>();
        sr.sprite = CreateCircleSprite();
        sr.color = Color.cyan;

        currentHp = maxHp;
        CreateHpText();
        SpawnWeapon();
    }

    void Start() => Launch();

    public void Launch()
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        rb.linearVelocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;
    }

    public void ResetBall(Vector2 startPos)
    {
        rb.linearVelocity = Vector2.zero;
        transform.position = startPos;
        currentHp = maxHp;
        UpdateHpText();
        Launch();
    }

    public void TakeDamage(int damage)
    {
        currentHp = Mathf.Max(0, currentHp - damage);
        UpdateHpText();
    }

    public void OnLevelUp()
    {
        maxHp += 10;
        currentHp = Mathf.Min(currentHp + 10, maxHp);
        UpdateHpText();
    }

    void FixedUpdate()
    {
        if (rb.linearVelocity.sqrMagnitude > 0.01f)
            rb.linearVelocity = rb.linearVelocity.normalized * speed;
    }

    void CreateHpText()
    {
        var canvasGO = new GameObject("HpCanvas");
        canvasGO.transform.SetParent(transform);
        canvasGO.transform.localPosition = Vector3.zero;
        canvasGO.transform.localRotation = Quaternion.identity;

        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = 5;

        // 캔버스 world 크기를 공 크기(diameter 1 unit)에 고정
        float s = 0.01f / (radius * 2f);
        var rt = canvasGO.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(100f, 100f);
        rt.localScale = new Vector3(s, s, 1f);

        var textGO = new GameObject("HpText");
        textGO.transform.SetParent(canvasGO.transform, false);
        var textRt = textGO.AddComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = Vector2.zero;
        textRt.offsetMax = Vector2.zero;

        hpText = textGO.AddComponent<TextMeshProUGUI>();
        hpText.alignment = TextAlignmentOptions.Center;
        hpText.fontSize = 36;
        hpText.fontStyle = FontStyles.Bold;
        hpText.color = Color.black;
        UpdateHpText();
    }

    void UpdateHpText() { if (hpText != null) hpText.text = $"{currentHp}"; }

    void SpawnWeapon()
    {
        var pivot = new GameObject("WeaponPivot");
        pivot.transform.SetParent(transform);
        pivot.transform.localPosition = Vector3.zero;

        var weaponGO = new GameObject("Weapon");
        weaponGO.transform.SetParent(pivot.transform);
        weaponGO.transform.localPosition = new Vector3(weaponDistance, 0f, 0f);
        weaponGO.transform.localScale = new Vector3(weaponLength, weaponWidth, 1f);

        weaponGO.AddComponent<BoxCollider2D>().isTrigger = true;

        var sr = weaponGO.AddComponent<SpriteRenderer>();
        sr.sprite = WhiteSquareSprite();
        sr.color = Color.white;
        sr.sortingOrder = 1;

        weaponGO.AddComponent<WeaponController>().Init(pivot.transform, weaponRotationSpeed, this);
    }

    static Sprite CreateCircleSprite()
    {
        int res = 128;
        var tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        float r = res / 2f;
        for (int y = 0; y < res; y++)
            for (int x = 0; x < res; x++)
            {
                float dx = x - r + 0.5f, dy = y - r + 0.5f;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, Mathf.Clamp01(r - dist)));
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, res, res), Vector2.one * 0.5f, res);
    }

    static Sprite WhiteSquareSprite()
    {
        var tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 1, 1), Vector2.one * 0.5f, 1f);
    }
}
