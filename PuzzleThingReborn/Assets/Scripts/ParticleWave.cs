using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleWave : MonoBehaviour
{
    ParticleSystem ps;
    ParticleSystem.Particle[] particles;
    Vector3[] particles_start_rot;
    float[] particle_distance;
    bool[] particle_player_active;

    

    public int size_x = 25;
    public int size_y = 25;
    public float min = 4.0f;
    public float max = 10.0f;

    float start_size = 0.0f;

    float time = 0.0f;

    GameObject player;

    Vector3 player_position;

    public bool move_to_player = false;

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        ps = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[size_x * size_y];
        particles_start_rot = new Vector3[size_x * size_y];
        particle_player_active = new bool[size_x * size_y];
        particle_distance = new float[size_x * size_y];

        for (int i = 0; i < size_y; i++)
        {
            for (int j = 0; j < size_x; j++)
            {
                particle_distance[i * size_x + j] = 100.0f;
            }
        }

        ParticleSystem.MainModule main = ps.main;
        main.maxParticles = size_x * size_y;
        main.startLifetime = Mathf.Infinity;
        main.playOnAwake = false;

        start_size = ps.main.startSize.constant;

        ps.Emit(size_x * size_y);
        ps.GetParticles(particles);

        for (int i = 0; i < size_x * size_y; i++)
        {

            particles_start_rot[i] = particles[i].rotation3D;

        }
    }

    void OnParticleTrigger()
    {
        List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();
        List<ParticleSystem.Particle> exit = new List<ParticleSystem.Particle>();

        int num_enter = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);
        int num_exit = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Exit, exit);

        for (int i = 0; i < num_enter; i++)
        {
            ParticleSystem.Particle p = enter[i];

            Vector3 pos = p.position;

            int num = Mathf.RoundToInt(p.position.x * size_x + p.position.z);

            if (num >= size_x * size_y)
            {
                //Debug.Log(num);
            }
            else
            {
                particle_player_active[num] = true;
            }

        }

        for (int i = 0; i < num_exit; i++)
        {
            ParticleSystem.Particle p = exit[i];

            Vector3 pos = p.position;

            int num = Mathf.RoundToInt(p.position.x * size_x + p.position.z);

            if (num >= size_x * size_y)
            {
                //Debug.Log(num);
            }
            else
            {
                particle_player_active[num] = false;
            }

        }

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < size_y; i++)
        {
            for (int j = 0; j < size_x; j++)
            {
                float y_pos = Mathf.PerlinNoise((i + transform.position.x) / 10.0f + time , ((j + transform.position.z )/ 10.0f + time ));
                y_pos += Mathf.PerlinNoise((i + transform.position.x) / 10.0f - time, ((j + transform.position.z) / 10.0f - time));

                //y_pos /= 2.0f;

                particles[i * size_x + j].startColor = Color.Lerp(Color.black, Color.red, y_pos);

                //if (move_to_player && player.GetComponent<PlayerController>().moveScale == 1.0f)
                //{
                    if (particle_player_active[i * size_x + j] == true)
                    {
                        float distance = Vector3.Distance(player.transform.position, new Vector3(i + transform.position.x, y_pos, j + transform.position.z));

                        if (((distance - min) / max) < particle_distance[i * size_x + j])
                        {
                            particle_distance[i * size_x + j] = ((distance - min) / max);
                        }

                        float num = particle_distance[i * size_x + j];

                        y_pos = Mathf.Lerp(1.5f, y_pos, num - 0.1f);
                        
                        particles[i * size_x + j].startColor = Color.Lerp(Color.blue, particles[i * size_x + j].startColor, num);
                        particles[i * size_x + j].rotation3D = Vector3.Lerp(Vector3.zero, particles_start_rot[i * size_x + j], num + 0.3f);
                        particles[i * size_x + j].size = Mathf.Lerp(1.0f, start_size, num);
                    }
                //}

                particles[i * size_x + j].position = new Vector3(i, (y_pos * 2.0f) - 2.0f, j);
                
            }
        }

        time += Time.deltaTime * 0.5f;
        ps.SetParticles(particles, particles.Length);
    }
}
