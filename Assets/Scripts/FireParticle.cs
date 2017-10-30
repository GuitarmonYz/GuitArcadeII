using UnityEngine;
using System.Collections;

namespace FireParticle
{
    public enum AlphaFalloff { NONE, LINEAR, SQRT };   // TODO: Consider other falloffs, like Square Root?

    public class FireParticle : MonoBehaviour {
    	
        // This only runs ONCE -- not on every Spawn
        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            originalColor = spriteRenderer.color;

        }

    	void OnEnable () {
            velocity = new Vector2( Random.Range(MinVelocity.x, MaxVelocity.x), Random.Range( MinVelocity.y, MaxVelocity.y) );

            actualLifeSpan = LifeSpan * Random.Range(0.9f, 1.1f);

            timeAlive = 0;

            spriteRenderer.color = originalColor;
    	}

        public Vector2 MinVelocity = new Vector2( -0.05f, 0.1f );
        public Vector2 MaxVelocity = new Vector2( 0.05f, 0.2f );
        public float LifeSpan = 2f;

        public bool DestroysSelf = true;

        public AlphaFalloff AlphaFalloff;

        float actualLifeSpan;
        float timeAlive;

        SpriteRenderer spriteRenderer;
        Color originalColor;

        Vector2 velocity;

    	// Update is called once per frame
    	void Update () {
    	    
            timeAlive += Time.deltaTime;

            if(DestroysSelf && timeAlive >= actualLifeSpan)
            {
                SimplePool.Despawn(gameObject);
                return;
            }

            if(AlphaFalloff == AlphaFalloff.LINEAR)
            {
                // As the particle gets older, it fades out

                float alpha = Mathf.Clamp01( 1.0f - (timeAlive / actualLifeSpan) );

                Color newColor = originalColor;
                newColor.a *= alpha;
                spriteRenderer.color = newColor;
            }
            else if(AlphaFalloff == AlphaFalloff.SQRT)
            {
                // As the particle gets older, it fades out

                float alpha = Mathf.Clamp01( 1.0f - (timeAlive / actualLifeSpan) );

                alpha = Mathf.Sqrt( alpha );

                Color newColor = originalColor;
                newColor.a *= alpha;
                spriteRenderer.color = newColor;
            }

            this.transform.Translate( velocity * Time.deltaTime );

    	}
    }
}