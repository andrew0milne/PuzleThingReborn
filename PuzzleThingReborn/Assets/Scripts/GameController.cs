using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance = null;

    public GameObject player;
    public GameObject pickUps;

    public GameObject[] lasers;
    public GameObject[] enemies;
    public GameObject[] warning_lights;

    public Image data_point;
    public float data_point_scale = 70.0f;

    int max_pickups = 0;

    public Image black_screen;
    public float flash_speed = 2.0f;

    public Text prog_text;
    public Text speed_text;
    public Text prox_text;
    public Text guard_text;
    public Text lives_text;

    public float lockdown_start = 0.5f;
    public float hunting_start = 0.8f;

    bool LOCKDOWN = false;
    bool HUNTING = false;

    public AudioSource motion;
    public AudioSource alarm_voice;
    public AudioSource alarm;

    
    [Header("-Variables for the Music Controller-")]
    [Header("Intensity")]
    public float intensity = 0.0f;
    float intensity_target = 0.0f;
    public float intensity_change_speed = 2.0f;
    [Space]
    Movement_State move_state = Movement_State.STILL;
    float movement_factor = 0.0f;
    public float movement_weighting = 1.0f;
    public float movement_factor_change_speed = 0.5f;   
    [Space]
    float guard_proximity_factor = 0.0f;
    public float guard_proximity_weighting = 1.0f;
    public float max_distnce_for_enemies = 30.0f;
    public float min_distnce_for_enemies = 5.0f;
    [Space]
    int score = 0;
    

    [Header("Valence")]
    public float valence = 0.0f;
    float valence_target = 0.0f;
    public float valence_change_speed = 2.0f;
    [Space]
    float lives_factor = 0.0f;
    public float lives_weighting = 1.0f;

    int max_lives = 3;

    [Header("Both")]
    float progression_factor = 0.0f;
    public float progression_I_weighting = 1.0f;
    public float progression_V_weighting = 1.0f;
    float guard_state_factor = 0.0f;
    public float guard_state_I_weighting = 1.0f;
    public float guard_state_V_weighting = 1.0f;



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start ()
    {
        black_screen.color = Color.black;

        StartCoroutine(ScreenFlashToColour(flash_speed, Color.black, false, false));

        Transform[] temp = pickUps.GetComponentsInChildren<Transform>();

        max_pickups = temp.Length - 1;

        Debug.Log(max_pickups);// max_pickups);

        lasers = GameObject.FindGameObjectsWithTag("Laser");

        Debug.Log("LockDown at " + max_pickups * lockdown_start);
        Debug.Log("Hunting at " + max_pickups * hunting_start);

        max_lives = player.GetComponent<PlayerController>().max_lives;
    }

    // Turns the screen a solid colour, if dir is false, does opposite
    IEnumerator ScreenFlashToColour(float speed, Color flash_color, bool dir, bool dead)
    {
        float t = 0.0f;

        Color transparent = new Color(0.0f, 0.0f, 0.0f, 0.0f);

        Color color1;
        Color color2;

        if(dir)
        {
            color1 = transparent;
            color2 = flash_color;
        }
        else
        {
            color2 = transparent;
            color1 = flash_color;
        }

        // Screen flash to black
        while (t < 1.1f)
        {
            black_screen.color = Color.Lerp(color1, color2, t);
            t += speed * Time.deltaTime;

            yield return null;
        }

        black_screen.color = color2;

        if(dead)
        {
            
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            SceneManager.LoadScene("Menu");
            
        }

        yield return null;
    }

    void UpdateGameVariables()
    {
        progression_factor = (float)score / max_pickups;
        prog_text.text = "Progression: " + progression_factor.ToString();

        switch(player.GetComponent<PlayerController>().move_state)
        {
            case Movement_State.STILL:
                {
                    if (movement_factor > -1.0f)
                    {
                        movement_factor -= Time.deltaTime * movement_factor_change_speed;
                    }

                    break;
                }
            case Movement_State.WALK:
                {
                    if (movement_factor > 0.0f)
                    {
                        movement_factor -= Time.deltaTime * movement_factor_change_speed;
                    }
                    else if(movement_factor < 0.0f)
                    {
                        movement_factor += Time.deltaTime * movement_factor_change_speed;
                    }
                    break;
                }
            case Movement_State.RUN:
                {
                    if (movement_factor < 1.0f)
                    {
                        movement_factor += Time.deltaTime * movement_factor_change_speed;
                    }
                    break;
                }
        }

        speed_text.text = "Speed: " + movement_factor.ToString();

        float closest_dist = 10000.0f;
        int highest_enemy_state = 0;

        int num = 0;
        for(int i = 0; i < enemies.Length; i++)
        {
            float distance = Vector3.Distance(player.transform.position, enemies[i].GetComponent<EnemyController>().enemy.transform.position);

            if(distance < closest_dist)
            {
                closest_dist = distance;
                num = i;
            }

            int state = (int)enemies[i].GetComponent<EnemyController>().current_state;
            if(state > highest_enemy_state)
            {
                highest_enemy_state = state;
            }
        }     

        guard_proximity_factor = 1.0f - Mathf.Clamp01((closest_dist - min_distnce_for_enemies) / (max_distnce_for_enemies - min_distnce_for_enemies));
        prox_text.text = "Proximity: " + guard_proximity_factor.ToString();

        guard_state_factor = highest_enemy_state / 3.0f;
        guard_text.text = "Guards: " + guard_state_factor.ToString();

        lives_text.text = "Lives: " + lives_factor.ToString();

        intensity_target = ((movement_factor * movement_weighting) + (guard_proximity_factor * guard_proximity_weighting) + (progression_factor * progression_I_weighting) + (guard_state_factor * guard_state_I_weighting))/4.0f;
        valence_target = ((progression_factor * progression_V_weighting) + (lives_factor * lives_weighting) + (guard_state_factor * guard_state_V_weighting))/3.0f;     
    }

    public void UpdateScore(int num)
    {
        score = num;

        print(score + ", " + max_pickups);

        if(score >= max_pickups * lockdown_start && !LOCKDOWN)
        {
            LockDown();
        }
        else if(score >= max_pickups * hunting_start && !HUNTING)
        {
            StartHunting();
        }

        if (score >= max_pickups)
        {
            print("hello");
            EndGame();
        }
    }

    IEnumerator Alarm()
    {
        alarm_voice.Play();

        while (alarm_voice.isPlaying)
        {
            yield return null;
        }

        alarm.Play();

        yield return null;
    }

    void StartHunting()
    {
        if (!HUNTING)
        {
            HUNTING = true;
            StartCoroutine(Alarm());

            foreach (GameObject e in enemies)
            {
                e.GetComponent<EnemyController>().StartHunting();
            }

            foreach (GameObject l in warning_lights)
            {
                l.GetComponent<WarningLight>().UpdateState();
            }
        }
    }

    public void PlayerDead(int lives)
    {
        if(lives > 0)
        {
            lives_factor = ((float)lives / max_lives);
        }
        else
        {
            EndGame();
        }

        foreach(GameObject g in enemies)
        {
            g.GetComponent<EnemyController>().ResetPosition();
        }
    }

    public void EndGame()
    {
        StartCoroutine(ScreenFlashToColour(2.0f, Color.black, true, true));
    }

    void LockDown()
    {
        Debug.Log("LOCKDOWN");

        motion.Play();

        LOCKDOWN = true;

        foreach (GameObject l in lasers)
        {
            l.SendMessage("LockDown");
        }

        foreach (GameObject l in warning_lights)
        {
            l.GetComponent<WarningLight>().UpdateState();
        }

        
    }

    float RoundToDecimanl(float number, float deciml, bool can_be_zero)
    {
        float d = 1 / deciml;
        float num = Mathf.Round(number * d) / d;
        if (num < deciml && !can_be_zero)
        {
            num = deciml;
        }

        return num;
    }

    private void Update()
    {
        UpdateGameVariables();

        if(intensity > intensity_target)
        {
            intensity -= Time.deltaTime * intensity_change_speed;
        }
        else
        {
            intensity += Time.deltaTime * intensity_change_speed;
        }

        if (valence > valence_target)
        {
            valence -= Time.deltaTime * valence_change_speed;
        }                               
        else                            
        {                               
            valence += Time.deltaTime * valence_change_speed;
        }

        intensity = RoundToDecimanl(intensity, 0.01f, true);
        valence = RoundToDecimanl(valence, 0.01f, true);

        intensity = Mathf.Clamp(intensity, -1.0f, 1.0f);
        valence = Mathf.Clamp(valence, -1.0f, 1.0f);

        data_point.rectTransform.localPosition = new Vector2(data_point_scale * valence, data_point_scale * intensity);

        MusicController.instance.intensity = intensity;
        MusicController.instance.valence = valence;
    }
}
