using UnityEngine;
using TMPro;
using System.Collections;

public static class Fx
{
    static FxRunner _runner;
    static Coroutine _slowMoRoutine;

    static FxRunner Runner
    {
        get
        {
            if (_runner != null) return _runner;
            var go = new GameObject("FxRunner");
            Object.DontDestroyOnLoad(go);
            _runner = go.AddComponent<FxRunner>();
            return _runner;
        }
    }

    public static void SlowMo(float timeScale = 0.05f, float realDuration = 0.18f)
    {
        if (_slowMoRoutine != null)
            Runner.StopCoroutine(_slowMoRoutine);
        _slowMoRoutine = Runner.StartCoroutine(SlowMoRoutine(timeScale, realDuration));
    }

    static IEnumerator SlowMoRoutine(float timeScale, float realDuration)
    {
        Time.timeScale = timeScale;
        yield return new WaitForSecondsRealtime(realDuration);
        Time.timeScale = 1f;
        _slowMoRoutine = null;
    }

    public static void SpawnRing(Vector2 pos)
    {
        var go = new GameObject("FxRing");
        go.transform.position = pos;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = CreateRingSprite();
        sr.color = Color.yellow;
        sr.sortingOrder = 10;
        go.AddComponent<FxRing>();
    }

    public static void SpawnDamage(Vector2 pos, int damage)
    {
        var canvasGO = new GameObject("FxDmg");
        canvasGO.transform.position = pos;

        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = 11;

        var rt = canvasGO.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(200f, 80f);
        rt.localScale = new Vector3(0.01f, 0.01f, 1f);

        var textGO = new GameObject("Text");
        textGO.transform.SetParent(canvasGO.transform, false);
        var textRt = textGO.AddComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = Vector2.zero;
        textRt.offsetMax = Vector2.zero;

        var tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = $"{damage}!!";
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize = 70;
        tmp.fontStyle = FontStyles.Bold;
        tmp.color = new Color(1f, 0.85f, 0.1f);

        canvasGO.AddComponent<FxDamageText>();
    }

    static Sprite CreateRingSprite()
    {
        int res = 128;
        var tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        float r = res / 2f, innerR = r * 0.72f;
        for (int y = 0; y < res; y++)
            for (int x = 0; x < res; x++)
            {
                float dx = x - r + 0.5f, dy = y - r + 0.5f;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                float alpha = Mathf.Clamp01(r - dist) * Mathf.Clamp01(dist - innerR);
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, res, res), Vector2.one * 0.5f, res);
    }
}

// 코루틴 실행용 빈 MonoBehaviour
public class FxRunner : MonoBehaviour { }

public class FxRing : MonoBehaviour
{
    void Start() => StartCoroutine(Play());

    IEnumerator Play()
    {
        var sr = GetComponent<SpriteRenderer>();
        float t = 0f, dur = 0.4f;
        while (t < dur)
        {
            t += Time.unscaledDeltaTime; // 슬로우모션 영향 없음
            float f = t / dur;
            transform.localScale = Vector3.one * Mathf.Lerp(0.3f, 2.5f, f);
            var c = sr.color; c.a = 1f - f; sr.color = c;
            yield return null;
        }
        Destroy(gameObject);
    }
}

public class FxDamageText : MonoBehaviour
{
    void Start() => StartCoroutine(Play());

    IEnumerator Play()
    {
        var tmps = GetComponentsInChildren<TextMeshProUGUI>();
        float t = 0f, dur = 0.8f;
        Vector3 startPos = transform.position;
        while (t < dur)
        {
            t += Time.unscaledDeltaTime; // 슬로우모션 영향 없음
            float f = t / dur;
            transform.position = startPos + Vector3.up * (f * 1.5f);
            foreach (var tmp in tmps) { var c = tmp.color; c.a = 1f - f; tmp.color = c; }
            yield return null;
        }
        Destroy(gameObject);
    }
}
