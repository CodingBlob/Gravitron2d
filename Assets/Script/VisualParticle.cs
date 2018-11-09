using UnityEngine;

namespace Assets
{
    public class VisualParticle : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void SetColor(Color color)
        {
            spriteRenderer.material.SetColor("_Color", color);
        }

        public void UpdateParams(GravitySimulation.GravityParticle p)
        {
            if (p.alive < -1)
            {
                return;
            }

            if (p.alive == -1)
            {
                p.alive = -2;
                Destroy(GetComponent<TrailRenderer>());
                GetComponent<Renderer>().enabled = false;
            }

            var radius = p.radius;

            transform.position = p.position;
            transform.localScale = new Vector3(radius, radius, radius);

            var ct = p.speed.magnitude / 10f;

            var color = ct <= 1 ? Color.Lerp(Color.blue, Color.red, ct) : Color.Lerp(Color.red, Color.white, ct - 1);

            SetColor(color);
        }
    }
}
