using System;
using Assets;
using UnityEngine;

public class GravitySimulation : MonoBehaviour
{
    public VisualParticle VisualParticlePrefab;

    public float Gravity;
    public float StartSpeed;
    public float SpawnRadius;
    public float MaxMass;
    public int Count;
    public ComputeShader GravityShader;

    private ComputeBuffer gravityParticlesBuffer;
    private GravityParticle[] particles;
    private VisualParticle[] visualParticles;

    // Use this for initialization
    void Start()
    {
        ResetSimulation();
    }

    private VisualParticle[] GenerateVisualParticles(GravityParticle[] particles)
    {
        if (visualParticles != null)
        {
            foreach (var visualParticle in visualParticles)
            {
                if (visualParticle)
                {
                    Destroy(visualParticle.gameObject);
                }
            } 
        }

        var result = new VisualParticle[particles.Length];

        for (int i = 0; i < result.Length; i++)
        {
            result[i] = Instantiate(
                VisualParticlePrefab,
                particles[i].position,
                Quaternion.identity,
                transform);
        }

        return result;
    }

    private GravityParticle[] GenerateParticles()
    {
        var result = new GravityParticle[Count];

        for (int i = 0; i < result.Length; i++)
        {
            GravityParticle particle = new GravityParticle()
            {
                alive = 1,
                collided = -1,
                mass = MaxMass * (UnityEngine.Random.value + 0.5f),
                speed = UnityEngine.Random.insideUnitSphere * StartSpeed / 10000f,
                position = UnityEngine.Random.insideUnitSphere * SpawnRadius
            };

            particle.radius = Mathf.Pow(particle.mass * 3.0f / (4f * Mathf.PI), 0.33333333f) * 0.05f;

            result[i] = particle;
        }

        return result;
    }

    public void DispatchToGpu()
    {
        gravityParticlesBuffer.SetData(particles);

        GravityShader.SetFloat("deltaT", Time.deltaTime);

        uint x, y, z;

        var kernelId = GravityShader.FindKernel("CalculateSpeed");
        GravityShader.SetBuffer(kernelId, "particles", gravityParticlesBuffer);
        GravityShader.GetKernelThreadGroupSizes(kernelId, out x, out y, out z);
        int numberOfGroups = Mathf.CeilToInt(particles.Length / x);
        GravityShader.Dispatch(kernelId, numberOfGroups, numberOfGroups, 1);

        kernelId = GravityShader.FindKernel("CalculatePositions");
        GravityShader.SetBuffer(kernelId, "particles", gravityParticlesBuffer);
        GravityShader.GetKernelThreadGroupSizes(kernelId, out x, out y, out z);
        GravityShader.Dispatch(kernelId, numberOfGroups, 1, 1);

    }

    // UpdateParams is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.KeypadPlus))
        {
            Gravity *= 1.2f;
            GravityShader.SetFloat("G", Gravity);
        }

        if (Input.GetKey(KeyCode.KeypadMinus))
        {
            Gravity *= 0.8f;
            GravityShader.SetFloat("G", Gravity);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetSimulation();
            return;
        }

        DispatchToGpu();
        UpdateVisualParticles();
        gravityParticlesBuffer.GetData(particles);
    }

    private void ResetSimulation()
    {
        GravityShader.SetFloat("G", Gravity);

        particles = GenerateParticles();
        visualParticles = GenerateVisualParticles(particles);

        var stride = sizeof(float) * 6 + sizeof(int) * 2;
        gravityParticlesBuffer = new ComputeBuffer(Count, stride);
        gravityParticlesBuffer.SetData(particles);
    }

    private void UpdateVisualParticles()
    {
        for (int i = 0; i < visualParticles.Length; i++)
        {
            var p = particles[i];
            var v = visualParticles[i];

            v.UpdateParams(p);
        }
    }

    public struct GravityParticle
    {
        public Vector2 speed;
        public Vector2 position;
        public float mass;
        public float radius;
        public int collided;
        public int alive;
    }
}
