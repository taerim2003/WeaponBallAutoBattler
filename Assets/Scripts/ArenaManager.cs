using UnityEngine;

public class ArenaManager : MonoBehaviour
{
    [SerializeField] float width = 16f;
    [SerializeField] float height = 9f;

    void Awake()
    {
        float t = 0.25f;
        CreateWall("Top",    new Vector2(0,  height / 2 + t / 2), new Vector2(width + t * 2, t));
        CreateWall("Bottom", new Vector2(0, -height / 2 - t / 2), new Vector2(width + t * 2, t));
        CreateWall("Left",   new Vector2(-width / 2 - t / 2, 0),  new Vector2(t, height));
        CreateWall("Right",  new Vector2( width / 2 + t / 2, 0),  new Vector2(t, height));
    }

    void CreateWall(string wallName, Vector2 pos, Vector2 size)
    {
        var go = new GameObject("Wall_" + wallName);
        go.transform.SetParent(transform);
        go.transform.position = pos;
        go.transform.localScale = new Vector3(size.x, size.y, 1f);

        go.AddComponent<BoxCollider2D>().sharedMaterial = BouncyMaterial();

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = WhiteSquareSprite();
        sr.color = new Color(0.25f, 0.25f, 0.25f);
    }

    static PhysicsMaterial2D BouncyMaterial() =>
        new PhysicsMaterial2D { bounciness = 1f, friction = 0f };

    static Sprite WhiteSquareSprite()
    {
        var tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 1, 1), Vector2.one * 0.5f, 1f);
    }
}
