using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class UpgradeTree : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject nodeTemplate;
    [SerializeField] private GameObject lineTemplate;
    [SerializeField] private TextMeshProUGUI currencyText;

    [Header("Layout")]
    [SerializeField] private float treeRadius = 200f;
    [SerializeField] private Vector2 treeCenter = Vector2.zero;

    // Colors
    private static readonly Color COL_ACTIVE = new Color(0.2f, 0.8f, 0.2f);
    private static readonly Color COL_AVAILABLE = new Color(0.8f, 0.8f, 0.2f);
    private static readonly Color COL_LOCKED = new Color(0.4f, 0.4f, 0.4f);

    // Upgrade definitions
    private List<UpgradeData> upgrades = new List<UpgradeData>
    {
        new UpgradeData { id = UpgradeId.BulletDamage,      name = "Bullet Damage",    maxLevel = 5, costs = new float[] { 10, 25, 50, 100, 200 }, angle = 0f,                        radius = 200f },
        new UpgradeData { id = UpgradeId.FireRate,          name = "Fire Rate",        maxLevel = 5, costs = new float[] { 10, 25, 50, 100, 200 }, angle = Mathf.PI / 3f,             radius = 200f },
        new UpgradeData { id = UpgradeId.LightningDamage,   name = "Lightning Dmg",    maxLevel = 5, costs = new float[] { 30, 60, 120, 240, 480 },angle = 2f * Mathf.PI / 3f,        radius = 200f },
        new UpgradeData { id = UpgradeId.LightningBounces,  name = "Lightning Bounces",maxLevel = 3, costs = new float[] { 50, 150, 400 },         angle = Mathf.PI,                  radius = 200f },
        new UpgradeData { id = UpgradeId.PoisonDamagePerSec,name = "Poison Dmg/s",     maxLevel = 5, costs = new float[] { 30, 60, 120, 240, 480 },angle = 4f * Mathf.PI / 3f,        radius = 200f },
        new UpgradeData { id = UpgradeId.PoisonDuration,    name = "Poison Duration",  maxLevel = 3, costs = new float[] { 40, 100, 250 },         angle = 5f * Mathf.PI / 3f,        radius = 200f },
        new UpgradeData { id = UpgradeId.CurrencyMultiplier,name = "Currency Mult",    maxLevel = 3, costs = new float[] { 50, 150, 400 },         angle = Mathf.PI / 2f,             radius = 200f },
    };

    private List<RectTransform> nodeTransforms = new List<RectTransform>();
    private List<Image> nodeImages = new List<Image>();
    private List<Text> nodeTexts = new List<Text>();
    private List<GameObject> lineObjects = new List<GameObject>();

    private bool dragging = false;
    private float currentRotation = 0f;
    private float lastMouseAngle = 0f;

    void Start()
    {
        CreateNodes();
        CreateLines();
        UpdateVisuals();
    }

    void Update()
    {
        HandleDrag();
        UpdateCurrencyText();
    }

    private void HandleDrag()
    {
        Vector2 mousePos = Input.mousePosition - new Vector3(canvas.transform.position.x, canvas.transform.position.y);
        float mouseAngle = Mathf.Atan2(mousePos.y - treeCenter.y, mousePos.x - treeCenter.x);

        if (Input.GetMouseButtonDown(0))
        {
            dragging = true;
            lastMouseAngle = mouseAngle;
        }
        if (Input.GetMouseButtonUp(0))
        {
            dragging = false;
        }

        if (dragging)
        {
            float delta = mouseAngle - lastMouseAngle;
            // Avoid wrapping jumps
            if (delta > Mathf.PI) delta -= 2f * Mathf.PI;
            if (delta < -Mathf.PI) delta += 2f * Mathf.PI;
            currentRotation += delta;
            lastMouseAngle = mouseAngle;
            PositionNodes();
        }
    }

    private void CreateNodes()
    {
        for (int i = 0; i < upgrades.Count; i++)
        {
            GameObject node = Instantiate(nodeTemplate, canvas.transform);
            RectTransform rt = node.GetComponent<RectTransform>();
            Image img = node.GetComponent<Image>();
            Text txt = node.GetComponentInChildren<Text>();

            var upd = upgrades[i];
            int lvl = GameManager.Instance.save.upgradeLevels[upd.id];
            txt.text = $"{upd.name}\nLv {lvl}/{upd.maxLevel}";

            node.GetComponent<Button>().onClick.AddListener(() => TryBuy(i));

            nodeTransforms.Add(rt);
            nodeImages.Add(img);
            nodeTexts.Add(txt);
        }
        PositionNodes();
    }

    private void PositionNodes()
    {
        for (int i = 0; i < upgrades.Count; i++)
        {
            float angle = upgrades[i].angle + currentRotation;
            float r = upgrades[i].radius;
            Vector2 pos = treeCenter + new Vector2(Mathf.Cos(angle) * r, Mathf.Sin(angle) * r);
            nodeTransforms[i].anchoredPosition = pos;
        }
        UpdateLines();
    }

    private void CreateLines()
    {
        for (int i = 0; i < upgrades.Count; i++)
        {
            GameObject line = Instantiate(lineTemplate, canvas.transform);
            lineObjects.Add(line);
        }
    }

    private void UpdateLines()
    {
        for (int i = 0; i < upgrades.Count; i++)
        {
            RectTransform lineRT = lineObjects[i].GetComponent<RectTransform>();
            Vector2 start = treeCenter;
            Vector2 end = nodeTransforms[i].anchoredPosition;

            Vector2 diff = end - start;
            float dist = diff.magnitude;
            float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

            lineRT.anchoredPosition = start;
            lineRT.sizeDelta = new Vector2(dist, 4f);
            lineRT.rotation = Quaternion.Euler(0, 0, angle);

            // Color based on state
            var upd = upgrades[i];
            int lvl = GameManager.Instance.save.upgradeLevels[upd.id];
            Image lineImg = lineObjects[i].GetComponent<Image>();

            if (lvl > 0)
                lineImg.color = COL_ACTIVE;
            else if (CanBuy(i))
                lineImg.color = COL_AVAILABLE;
            else
                lineImg.color = COL_LOCKED;
        }
    }

    private void UpdateVisuals()
    {
        for (int i = 0; i < upgrades.Count; i++)
        {
            var upd = upgrades[i];
            int lvl = GameManager.Instance.save.upgradeLevels[upd.id];

            if (lvl >= upd.maxLevel)
                nodeImages[i].color = COL_ACTIVE;
            else if (CanBuy(i))
                nodeImages[i].color = COL_AVAILABLE;
            else
                nodeImages[i].color = COL_LOCKED;

            nodeTexts[i].text = $"{upd.name}\nLv {lvl}/{upd.maxLevel}";
        }
        UpdateLines();
    }

    private bool CanBuy(int index)
    {
        var upd = upgrades[index];
        int lvl = GameManager.Instance.save.upgradeLevels[upd.id];
        if (lvl >= upd.maxLevel) return false;
        return GameManager.Instance.save.currency >= upd.costs[lvl];
    }

    private void TryBuy(int index)
    {
        if (!CanBuy(index)) return;

        var upd = upgrades[index];
        int lvl = GameManager.Instance.save.upgradeLevels[upd.id];
        GameManager.Instance.save.currency -= upd.costs[lvl];
        GameManager.Instance.save.upgradeLevels[upd.id]++;
        GameManager.Instance.SaveGame();
        GameManager.Instance.ApplyUpgrades();
        UpdateVisuals();
    }

    private void UpdateCurrencyText()
    {
        currencyText.text = $"Currency: {GameManager.Instance.save.currency:F0}";
    }
}