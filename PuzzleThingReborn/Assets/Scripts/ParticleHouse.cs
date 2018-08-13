using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(ParticleSystem))]
public class ParticleHouse : MonoBehaviour
{
    ParticleSystem ps;
    ParticleSystem.Particle[] particles;

    public GameObject house;
    public MeshRenderer mesh;

    public bool reverse = false;

    bool active = false;

    // Use this for initialization
    void Start ()
    {
        ps = GetComponent<ParticleSystem>();
        house = transform.parent.gameObject;

        ParticleSystem.ShapeModule sm = ps.shape;

        mesh = house.GetComponent<MeshRenderer>();

        sm.meshRenderer = mesh;

        mesh.enabled = reverse;

        
        particles = new ParticleSystem.Particle[ps.main.maxParticles];
        ps.GetParticles(particles);
    }
	
    IEnumerator DestroyEffect()
    {
        Texture2D tex;
        tex = mesh.material.GetTexture("_MainTex") as Texture2D;

        ps.Play();

        bool finished = false;

        float time = 0.0f;

        house.GetComponent<MeshCollider>().enabled = true;

        RaycastHit hit;

        int num_of_particles;

        while (finished == false)
        {
            num_of_particles = ps.GetParticles(particles);

            for (int i = 0; i < num_of_particles; i++)
            {
                //Debug.Log("" + Time.deltaTime + "   " + (particles[i].startLifetime - particles[i].remainingLifetime));
                //Debug.DrawRay(particles[i].position, mesh.bounds.center - particles[i].position);
                if ((particles[i].startLifetime - particles[i].remainingLifetime) <= Time.deltaTime * 2.0f)
                {
                    if (Physics.Raycast(particles[i].position, mesh.bounds.center - particles[i].position, out hit, 100.0f))
                    {
                        //Debug.Log(hit.collider.name);

                        if (hit.collider.tag == "ParticleHouse")
                        {
                            //Debug.Log("hit");
                            Vector2 tex_coord = hit.textureCoord;
                            Color color = tex.GetPixel((int)(tex.width * tex_coord.x), (int)(tex.height * tex_coord.y));
                            float h, s, v;
                            Color.RGBToHSV(color, out h, out s, out v);
                            s *= 2.0f;
                            color = Color.HSVToRGB(h, s, v);
                            particles[i].startColor = color;
                        }
                    }
                }
            }

            time += Time.deltaTime;

            if (time >= 2.0f)
            {
                finished = true;
            }

            ps.SetParticles(particles, num_of_particles);
            yield return null;
        }

        mesh.enabled = !reverse;
        house.GetComponent<MeshCollider>().enabled = !reverse;

        ParticleSystem.MainModule mm = ps.main;
        ParticleSystem.EmissionModule em = ps.emission;
        //mm.gravityModifier = 0.4f;
        em.rateOverTime = 0.0f;

        yield return new WaitForSeconds(1.0f);

        num_of_particles = ps.GetParticles(particles);

        Vector3[] start_pos = new Vector3[num_of_particles];
        float[] start_size = new float[num_of_particles];

        for(int i = 0; i < num_of_particles; i++)
        {
            start_pos[i] = particles[i].position;
            start_size[i] = particles[i].startSize;
        }

        finished = false;

        float t = 0.0f;

        ParticleSystem.NoiseModule nm = ps.noise;

        nm.strength = 0.0f;
        house.GetComponent<MeshCollider>().enabled = false;
        while (t <= 1.0f)
        {
            nm.strength = t;

            t += Time.deltaTime * 0.5f;

            yield return null;
        }

        yield return null;
    }

    void Activate()
    {
        StartCoroutine(DestroyEffect());
    }


}
