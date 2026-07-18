using UnityEngine;

public class BloodParticle : MonoBehaviour
{
    [SerializeField] private float lifetime = 1f;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float gravity = 3f;

    private Vector3 velocity;
    private float lifeTimer;
    private SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
            sr = gameObject.AddComponent<SpriteRenderer>();

        float angle = Random.Range(-45f, 135f);
        float angleRad = angle * Mathf.Deg2Rad;
        velocity = new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0) * speed;
        lifeTimer = lifetime;
    }

    private void Update()
    {
        lifeTimer -= Time.deltaTime;

        velocity.y -= gravity * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;

        if (sr != null)
        {
            float alpha = lifeTimer / lifetime;
            sr.color = new Color(1f, 0f, 0f, alpha);
            transform.localScale = Vector3.one * (0.15f + alpha * 0.15f);
        }

        if (lifeTimer <= 0)
            Destroy(gameObject);
    }
}