using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTombstones : MonoBehaviour
{

    GameObject player;
    bool end_scene = false;

    public float leave_speed = 1.0f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void EndScene()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();

        ParticleSystem.Particle[] particles;

        particles = new ParticleSystem.Particle[ps.main.maxParticles];

        int num = ps.GetParticles(particles);

        for(int i = 0; i < num; i++)
        {
            Vector3 speed = Vector3.Normalize(particles[i].position - player.transform.position);

            speed.y = 0.0f;

            particles[i].velocity = speed * leave_speed;
        }

        ps.SetParticles(particles, num);

        ParticleSystem.EmissionModule em = ps.emission;
        em.rateOverTime = 0.0f;

        end_scene = true;

        //ps.emission

    }

    void OnParticleTrigger()
    {
        if (end_scene == false)
        {
            ParticleSystem ps = GetComponent<ParticleSystem>();

            // particles
            List<ParticleSystem.Particle> exit = new List<ParticleSystem.Particle>();

            List<ParticleSystem.Particle> inside = new List<ParticleSystem.Particle>();

            // get
            int numExit = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Exit, exit);
            int numInside = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, inside);

            for (int i = 0; i < numExit; i++)
            {
                ParticleSystem.Particle p = exit[i];

                p.velocity = new Vector3(0.0f, 0.0f, 0.0f);
                exit[i] = p;
            }

            for (int i = 0; i < numInside; i++)
            {
                ParticleSystem.Particle p = inside[i];
                Vector3 speed = Vector3.Normalize(p.position - player.transform.position);

                speed.y = 0.0f;

                p.velocity = speed * 5.0f;

                inside[i] = p;

            }

            //// set
            ps.SetTriggerParticles(ParticleSystemTriggerEventType.Exit, exit);
            ps.SetTriggerParticles(ParticleSystemTriggerEventType.Inside, inside);
        }
    }
}
