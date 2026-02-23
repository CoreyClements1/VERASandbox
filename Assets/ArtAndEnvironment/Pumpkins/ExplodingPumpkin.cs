using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ExplodingPumpkin : MonoBehaviour, I_Explodable
{

    // ExplodingPumpkin is responsible for handling a single pumpkin, with ability to fade in/out and explode.

    #region VARIABLES

    [SerializeField] private Transform[] pumpkinPieces;
    [SerializeField] private ParticleSystem glowParticles;
    [SerializeField] private ParticleSystem explosionParticles;

    [Header("Explosion Settings")]
    [SerializeField] private float explosionForce = 500f;
    [SerializeField] private float explosionRadius = 2f;
    [SerializeField] private float upwardsModifier = 0.5f;
    [SerializeField] private float horizontalForceMultiplier = 1.5f;
    [SerializeField] private Vector3 randomForceRange = new Vector3(200f, 100f, 200f);
    [SerializeField] private float randomTorqueRange = 100f;

    [Header("Fade Settings")]
    [SerializeField] private float timeBeforeFade = 2f;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fadeAwayDuration = 0.5f;

    private bool canExplode = true;
    private Vector3 originalScale;

    public UnityEvent onExplode { get; private set; } = new UnityEvent();

    #endregion

    #region SETUP

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    #endregion

    #region EXPLODE

    public void Explode()
    {
        if (!canExplode) return;
        StartCoroutine(ExplodeCoroutine());
    }

    private IEnumerator ExplodeCoroutine()
    {
        canExplode = false;

        // Notify any listeners that the pumpkin has exploded
        onExplode.Invoke();

        // Disable glow particles
        if (glowParticles != null)
        {
            glowParticles.Stop();
            glowParticles.gameObject.SetActive(false);
        }

        // Get the center of the pumpkin for the explosion origin
        Vector3 explosionCenter = transform.position;

        // Enable physics on each piece and apply explosive force
        Rigidbody[] rigidbodies = new Rigidbody[pumpkinPieces.Length];
        MeshRenderer[] renderers = new MeshRenderer[pumpkinPieces.Length];

        for (int i = 0; i < pumpkinPieces.Length; i++)
        {
            if (pumpkinPieces[i] != null)
            {
                Rigidbody rb = pumpkinPieces[i].GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                    rb.AddExplosionForce(explosionForce, explosionCenter, explosionRadius, upwardsModifier, ForceMode.Impulse);

                    // Add random force for unpredictability
                    Vector3 randomForce = new Vector3(
                        Random.Range(-randomForceRange.x, randomForceRange.x),
                        Random.Range(-randomForceRange.y, randomForceRange.y),
                        Random.Range(-randomForceRange.z, randomForceRange.z)
                    );
                    rb.AddForce(randomForce, ForceMode.Impulse);

                    // Scale horizontal velocity to make pieces spread more
                    Vector3 velocity = rb.linearVelocity;
                    velocity.x *= horizontalForceMultiplier;
                    velocity.z *= horizontalForceMultiplier;
                    rb.linearVelocity = velocity;

                    // Add random torque for spin
                    Vector3 randomTorque = new Vector3(
                        Random.Range(-randomTorqueRange, randomTorqueRange),
                        Random.Range(-randomTorqueRange, randomTorqueRange),
                        Random.Range(-randomTorqueRange, randomTorqueRange)
                    );
                    rb.AddTorque(randomTorque, ForceMode.Impulse);

                    rigidbodies[i] = rb;
                }

                MeshRenderer mr = pumpkinPieces[i].GetComponent<MeshRenderer>();
                if (mr != null)
                {
                    renderers[i] = mr;
                }
            }
        }

        // Play explosion particles
        if (explosionParticles != null)
        {
            explosionParticles.gameObject.SetActive(true);
            explosionParticles.Play();
        }

        // Wait before starting fade
        yield return new WaitForSeconds(timeBeforeFade);

        // Fade out the pumpkin pieces
        yield return StartCoroutine(FadePiecesCoroutine(renderers));

        // Destroy the game object
        Destroy(gameObject);
    }

    #endregion

    #region FADE PIECES

    // Fades out the pumpkin pieces by gradually reducing their material alpha over time
    private IEnumerator FadePiecesCoroutine(MeshRenderer[] renderers)
    {
        float elapsedTime = 0f;

        // Store original materials and create instances for fading
        Material[][] originalMaterials = new Material[renderers.Length][];
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
            {
                // Create material instances to avoid modifying shared materials
                originalMaterials[i] = renderers[i].materials;
                Material[] newMaterials = new Material[originalMaterials[i].Length];
                for (int j = 0; j < originalMaterials[i].Length; j++)
                {
                    newMaterials[j] = new Material(originalMaterials[i][j]);
                    // Enable transparency
                    SetMaterialTransparent(newMaterials[j]);
                }
                renderers[i].materials = newMaterials;
            }
        }

        // Fade over time
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = 1f - (elapsedTime / fadeDuration);

            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null)
                {
                    foreach (Material mat in renderers[i].materials)
                    {
                        if (mat.HasProperty("_BaseColor"))
                        {
                            Color color = mat.GetColor("_BaseColor");
                            color.a = alpha;
                            mat.SetColor("_BaseColor", color);
                        }
                        else if (mat.HasProperty("_Color"))
                        {
                            Color color = mat.GetColor("_Color");
                            color.a = alpha;
                            mat.SetColor("_Color", color);
                        }
                    }
                }
            }

            yield return null;
        }
    }

    // Configures a material to use transparency by setting appropriate shader properties and render queue
    private void SetMaterialTransparent(Material mat)
    {
        // Handle URP/HDRP materials
        if (mat.HasProperty("_Surface"))
        {
            mat.SetFloat("_Surface", 1); // 1 = Transparent
            mat.SetFloat("_Blend", 0); // 0 = Alpha blend
        }

        // Handle Standard shader materials
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }

    #endregion

    #region FADE WHOLE PUMPKIN

    public void FadeIn()
    {
        StartCoroutine(FadeInCoroutine());
    }

    private IEnumerator FadeInCoroutine()
    {
        canExplode = false;
        transform.localScale = Vector3.zero;

        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeInDuration);
            // Quadratic ease-out: t = 1 - (1-t)^2
            float easedT = 1f - Mathf.Pow(1f - t, 2f);
            transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, easedT);
            yield return null;
        }

        transform.localScale = originalScale;
        canExplode = true;
    }

    public void DisableAndFadeAway()
    {
        StartCoroutine(DisableAndFadeAwayCoroutine());
    }

    private IEnumerator DisableAndFadeAwayCoroutine()
    {
        canExplode = false;

        float elapsedTime = 0f;
        Vector3 startScale = transform.localScale;

        while (elapsedTime < fadeAwayDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeAwayDuration;
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            yield return null;
        }

        transform.localScale = Vector3.zero;
        gameObject.SetActive(false);
    }

    #endregion
}
