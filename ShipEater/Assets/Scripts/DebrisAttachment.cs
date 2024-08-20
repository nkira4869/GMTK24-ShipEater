using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisAttachment : MonoBehaviour
{
    public float pullSpeed = 2f; // The speed at which debris is pulled towards the ship
    public float minDetectionRange = 2f; // Minimum detection range
    public float maxDetectionRange = 5f; // Maximum detection range
    public LineRenderer lineRenderer; // LineRenderer for visualizing the pull

    public float healthModifier = 1.1f; // Health modifier (multiplicative)
    public float speedModifier = 0.5f; // Speed modifier (additive)
    public GameObject bulletPatternPrefab; // Reference to a bullet pattern that will be activated on attachment

    // Line renderer animation fields
    [SerializeField]
    private Texture2D[] textures;
    private int animationStep;
    [SerializeField]
    private float fps = 30f;
    private float fpsCounter;

    private DynamicHexGrid shipGridSystem;
    private HullManager hullManager;
    private Transform shipTransform;
    private Vector2Int targetGridPosition;
    private bool isBeingPulled = false;
    private bool isAttached = false;
    private float detectionRange; // Randomized detection range for this debris
    private CircleCollider2D circleCollider;

    void Start()
    {
        detectionRange = Random.Range(minDetectionRange, maxDetectionRange);

        circleCollider = GetComponent<CircleCollider2D>();
        if (circleCollider != null)
        {
            circleCollider.radius = detectionRange;
        }

        shipGridSystem = FindObjectOfType<DynamicHexGrid>();
        hullManager = FindObjectOfType<HullManager>();

        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Color.yellow;
            lineRenderer.endColor = Color.yellow;
            lineRenderer.enabled = false;
        }
    }

    void Update()
    {
        // Handle line renderer animation
        AnimateLineRenderer();

        // Continuously update the line renderer positions even after attachment
        if (isBeingPulled || isAttached)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, shipTransform.position);
        }

        if (isBeingPulled && !isAttached)
        {
            Vector3 targetWorldPosition = shipGridSystem.HexToWorldPosition(targetGridPosition) + shipTransform.position;
            transform.position = Vector3.MoveTowards(transform.position, targetWorldPosition, pullSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetWorldPosition) < 0.1f)
            {
                AttachToShip();
            }
        }
    }

    // Animation logic for the line renderer
    private void AnimateLineRenderer()
    {
        if (textures.Length == 0) return; // Ensure textures are assigned

        fpsCounter += Time.deltaTime;
        if (fpsCounter >= 1f / fps)
        {
            animationStep++;
            if (animationStep == textures.Length)
            {
                animationStep = 0;
            }

            lineRenderer.material.SetTexture("_MainTex", textures[animationStep]);
            fpsCounter = 0f;
        }
    }

    void AttachToShip()
    {
        isBeingPulled = false;
        isAttached = true;

        gameObject.layer = LayerMask.NameToLayer("Player");
        gameObject.tag = "Player";

        shipGridSystem.OccupyCell(targetGridPosition);

        circleCollider.radius = 0.2f;

        transform.SetParent(shipTransform);
        transform.position = shipGridSystem.HexToWorldPosition(targetGridPosition) + shipTransform.position; // Align perfectly with the cell center

        // Keep the line renderer enabled and active
        lineRenderer.enabled = true;

        GetComponent<DebrisMovement>().AttachToShip();

        if (hullManager != null)
        {
            Attachment attachment = gameObject.AddComponent<Attachment>();
            attachment.gridPosition = targetGridPosition;
            hullManager.AddAttachment(attachment);

            hullManager.ApplyHealthModifier(healthModifier);
            hullManager.ApplySpeedModifier(speedModifier);
        }

        if (bulletPatternPrefab != null)
        {
            GameObject bulletPattern = Instantiate(bulletPatternPrefab, shipTransform);
            if (bulletPattern.TryGetComponent<Shooter>(out Shooter sh))
            {
                bulletPattern.GetComponent<Shooter>().shootPoint = this.gameObject.transform;
                bulletPattern.GetComponent<Shooter>().bulletPrefab = hullManager.defaultBulletPrefab;
            }

            bulletPattern.transform.SetParent(this.gameObject.transform);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isBeingPulled && !isAttached)
        {
            shipTransform = hullManager.transform;
            targetGridPosition = FindNearestUnoccupiedCellToCenter();

            // Attempt to reserve the cell; if it’s already reserved, find another cell
            if (!shipGridSystem.ReserveCell(targetGridPosition))
            {
                targetGridPosition = FindAnotherUnoccupiedCell();
            }

            isBeingPulled = true;
            lineRenderer.enabled = true;
        }
    }

    Vector2Int FindAnotherUnoccupiedCell()
    {
        List<Vector2Int> allCells = shipGridSystem.GetAllHexCoordinates();

        foreach (Vector2Int cell in allCells)
        {
            if (!shipGridSystem.IsCellOccupied(cell))
            {
                shipGridSystem.ReserveCell(cell);
                return cell;
            }
        }

        return new Vector2Int(0, 0);
    }

    Vector2Int FindNearestUnoccupiedCellToCenter()
    {
        Vector2Int gridCenter = new Vector2Int(0, 0);
        List<Vector2Int> allCells = shipGridSystem.GetAllHexCoordinates();

        allCells.Sort((a, b) => Vector2Int.Distance(a, gridCenter).CompareTo(Vector2Int.Distance(b, gridCenter)));

        foreach (Vector2Int cell in allCells)
        {
            if (!shipGridSystem.IsCellOccupied(cell))
            {
                return cell;
            }
        }

        return gridCenter;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}