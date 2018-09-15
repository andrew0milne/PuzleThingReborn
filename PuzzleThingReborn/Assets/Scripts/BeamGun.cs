using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(LineRenderer))]
public class BeamGun : MonoBehaviour 
{
	public GameObject beam_target_parent;
	public GameObject[] beam_targets;

    ParticleSystem ps;
    ParticleSystem.Particle[] particles;

    public GameObject front_section;
	public float spin_speed = 1.0f;

	Transform current_target;

	public Transform begin_pos, start_pos, mid_pos, end_pos;

	public float speed = 1.0f;

	public int max_line_points = 30;
	int bullet_total = 0;

	public bool use_perlin = true;
	public float perlin_scale = 1.0f;
	float perlin_y = 0.0f;
	public float perlin_speed = 1.0f;

	public float perlin_seed = 0.0f;

	bool active = false;

	public GameObject spark_particle;
    public GameObject spark_gun_particle;

    List<GameObject>  particle_list;
	List<GameObject> kill_list;

	LineRenderer beam;

	// Use this for initialization
	void Start () 
	{
		beam = GetComponent<LineRenderer> ();
		beam.positionCount = max_line_points;
		beam_targets =  GameObject.FindGameObjectsWithTag ("BeamTarget"); //beam_target_parent.GetComponentsInChildren<Transform> ();

        ps = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[1]; ps = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[1];

        particle_list = new List<GameObject>();
		kill_list = new List<GameObject>();

	}

	bool CheckRay(Vector3 pos_1, Vector3 pos_2)
	{
		RaycastHit hit;
		bool has_hit = Physics.Raycast (pos_1, pos_2 - pos_1, out hit, Vector3.Distance(pos_1, pos_2));


		if (has_hit) 
		{
			if (hit.collider.isTrigger) 
			{
				return false;
			}
		}

		return has_hit;
	}

	bool CheckBeamCollision(Vector3[] beam)
	{
		for(int i = 1; i < max_line_points; i++)
		{
			
			if (CheckRay (beam[i - 1], beam[i])) 
			{
				return true;
			}

		}
		return false;
	}

	Vector3[] GetBeam()
	{
		Vector3[] beam_points = new Vector3 [max_line_points];
		float t = 0.0f;

		for (int i = 0; i < 3; i++) 
		{
			beam_points[i] = Vector3.Lerp (begin_pos.position, start_pos.position, t);
			t += 1.0f / 3.0f;
		}

		t = 0.0f;

		for (int i = 3; i < max_line_points; i++) 
		{
			beam_points[i] = 
				Vector3.Lerp (
					Vector3.Lerp (start_pos.position, mid_pos.position, t), 
					Vector3.Lerp (mid_pos.position, current_target.position, t), t);

			t += 1.0f / (max_line_points - 3);
		}

		return beam_points;
	}

	// Could also use a combnations of various sin/cos waves
	// or pass audio data in, so it the musics sound wave, would be cool

	Vector3[] PerlinBeam(Vector3[] beam)
	{
		for(int i = 0; i < max_line_points; i++)
		{
			beam [i].x += (Mathf.PerlinNoise (i*0.08f, perlin_seed + perlin_y)-0.5f) * Mathf.Sin(((float)i/max_line_points)*Mathf.PI) * perlin_scale;
			beam [i].y += (Mathf.PerlinNoise (i*0.02f, perlin_seed + perlin_y)-0.5f) * Mathf.Sin(((float)i/max_line_points)*Mathf.PI) * perlin_scale;
			beam [i].z += (Mathf.PerlinNoise (i*0.03f, perlin_seed + perlin_y)-0.5f) * Mathf.Sin(((float)i/max_line_points)*Mathf.PI) * perlin_scale;
		}

		return beam;
	}

	void SpawnParticles(Vector3 pos)
	{
        spark_particle.transform.position = pos;

        //ps

        //if (ps.isPlaying == false)
        //{
        //    ps.Emit(1);
        //    ps.GetParticles(particles);

        //    particles[0].position = pos;

        //    ps.SetParticles(particles, 1);

        //    ps.Play();
        //}
    }

	IEnumerator Shoot()
	{
		Vector3[] temp_beam;
        spark_particle.GetComponent<ParticleSystem>().Play();
        spark_gun_particle.GetComponent<ParticleSystem>().Play();

        while (active) 
		{

			front_section.transform.RotateAround (front_section.transform.position, front_section.transform.up, spin_speed * Time.deltaTime);

			temp_beam = GetBeam ();
			if (CheckBeamCollision (temp_beam))
			{
				active = false;
				break;
			}

			if (use_perlin) 
			{
				temp_beam = PerlinBeam (temp_beam);
			}

			beam.SetPositions(temp_beam);

			SpawnParticles (temp_beam[(int)Random.Range(0, temp_beam.Length-1)]);

			yield return new WaitForSeconds (speed);
		}

        spark_particle.GetComponent<ParticleSystem>().Stop();
        spark_gun_particle.GetComponent<ParticleSystem>().Stop();
        Deactivate ();

		yield return null;
	}

	bool FindTarget()
	{
		Vector2 mid_screen = new Vector2 (0.5f, 0.5f);

		Transform target = beam_targets [1].transform;

		float closest_dist = Vector2.Distance (mid_screen, Camera.main.WorldToViewportPoint (target.position));

		foreach (GameObject g in beam_targets) 
		{
			if (g.tag == "BeamTarget") 
			{
				Vector2 temp_pos = Camera.main.WorldToViewportPoint (g.transform.position);

				float new_dist = Vector2.Distance (mid_screen, Camera.main.WorldToViewportPoint (g.transform.position));

				if (new_dist < closest_dist) 
				{
					target = g.transform;
					closest_dist = new_dist;
				}
			}
		}

		current_target = target;

		Vector2 pos = Camera.main.WorldToViewportPoint (target.position);

		if (pos.x < 0 || pos.x > 1 || pos.y < 0 || pos.y > 1) 
		{
			return false;
		}

		Vector3[] temp_points = GetBeam ();
		if (CheckBeamCollision (temp_points)) 
		{
			return false;
		}

		return true;
	}

	void Activate()
	{
		active = true;

		if (FindTarget ()) 
		{
			current_target.SendMessage ("Activate");
			StartCoroutine (Shoot ());
		}
	}

	void Deactivate()
	{
		active = false;

		current_target.SendMessage ("Deactivate");

		for (int i = 0; i < max_line_points; i++) 
		{
			beam.SetPosition (i, begin_pos.position);
		}

	}

	// Update is called once per frame
	void Update () 
	{
		perlin_y += Time.deltaTime * perlin_speed;
	}
}
