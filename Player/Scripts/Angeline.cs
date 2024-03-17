using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class Angeline : Unit
{
    [SerializeField] private enum angelineActions {move, attack, dodge, hackform, Skill_Mibman, Skill_Cthulhu, Skill_Tetsuo,Death}
    [SerializeField] private angelineActions currentAction;

    [Header("Refs")]
    [SerializeField] private CharacterController charControll;
    [SerializeField] private Hackform angelineHackform;

    [Header("Movement")]
    [SerializeField] private Vector2 moveAxis;
    private bool lookDirection = true;
    [SerializeField] private bool onGround;

    [SerializeField] private float jumpForce;
    [SerializeField] private float currentVerticalMovement;

    [Header("Combat")]
    private int basicComboCount;

    [Header("Animation")]
    [SerializeField] private Animator anim;

    [Header("HUD")]
    [SerializeField] private Image hpBar;

    [SerializeField] private GameObject HudDeath;

    [Header("VFX")]
    [SerializeField] private GameObject hackform_2;

    [Header("SFX")]
    [SerializeField] private AudioMixer inCombatAudio;

    public GameObject hackformSFX;

    [Header("Effects")]
    [SerializeField] private PostProcessVolume PPProfile;
    private ChromaticAberration newAberration;

    public GameObject hackformVFX;

    float deathTimer = 0f;

    private void Start()
    {
        setLife();
        setReferences();
    }

    private void setReferences()
    {
        Cursor.visible = false;
        inCombatAudio.SetFloat("Volume", Mathf.Log10(10f));
    }

    private void Update()
    {
        //Action execution
        switch(currentAction)
        {
            case angelineActions.move:
                baseMovement();
                    break;
            case angelineActions.attack:
                basicAttacks();
                break;
            case angelineActions.dodge:
                dodge();
                break;
            case angelineActions.hackform:
                hackForm();
                break;
            case angelineActions.Skill_Mibman:
                skill_Mibman();
                break;
            case angelineActions.Skill_Cthulhu:
                skill_Cthulhu();
                break;
            case angelineActions.Skill_Tetsuo:
                skill_Tetsuo();
                break;
            case angelineActions.Death:
                DeathAction();
                break;
        }
        //Effects
        regenAberration();
        HUD();

    }

    private void HUD()
    {
        hpBar.fillAmount = getLifePoint() / getTotalLifePoints();
    }

    private void setAction(angelineActions newAction)
    {
        currentAction = newAction;
    }

    private void DeathAction()
    {
        //Plays Animation
        anim.Play("Death_1");

        //Hud Animation
        HudDeath.SetActive(true);
    }

    private void baseMovement()
    {
        //While on Movement Action, Loop idle or Movement 

        if(onGround)
        moveAxis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));


        // While pressing movement keys
        if (moveAxis.magnitude >= 0.1f) 
        {

            //Clamped Movement based on character controller
            #region
            Vector3 desiredPosition = transform.position +
                           (Vector3.right * moveAxis.x * this.getMoveSpeed() * Time.deltaTime) +
                           (Vector3.forward * moveAxis.y * this.getMoveSpeed() * 1.15f * Time.deltaTime);

            // Aplica o clamp à posição desejada apenas no eixo X
            float clampedZ = Mathf.Clamp(desiredPosition.z, -7f, 0f);

            // Define a posição final com o valor clamped para o eixo X
            Vector3 finalPosition = new Vector3(desiredPosition.x, desiredPosition.y, clampedZ);

            // Final movement Clamped
            // stops if its landing animation
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Landing_1")) charControll.Move(finalPosition - transform.position);

            #endregion

            if (moveAxis.x < -0.15f) lookDirection = false;
            if (moveAxis.x > 0.15f) lookDirection = true;



            // Play Walk Animation Loop if ON GROUND
            if (!anim.GetNextAnimatorStateInfo(0).IsName("Move_1") && !anim.GetCurrentAnimatorStateInfo(0).IsName("Landing_1") && onGround)
                anim.Play("Move_1");
        }
        else
        {
            // Play Idle Animation Loop IF ON GROUND
            if (!anim.GetNextAnimatorStateInfo(0).IsName("Idle_1") && !anim.GetCurrentAnimatorStateInfo(0).IsName("Landing_1") && onGround)
                anim.Play("Idle_1");

        }



       

        // Jumping && Raycast
        #region
        if (!Physics.Raycast(transform.position, Vector3.down, 0.1f)) // If its not on ground, pull player towards currentVerticalMovement
        {
            // Raycast NOT Collided

            currentVerticalMovement += Physics.gravity.y * Time.deltaTime;
            onGround = false;                                  // isnt on ground

       
        }

        // Raycast Collided
        Ray rayGround =  new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(rayGround, out RaycastHit rayHit, 0.1f) && rayHit.collider.gameObject.layer !=8)
        {

            // set on ground, play landing animation
            if (onGround == false)
            {
                onGround = true;          // is on ground
                anim.Play("Landing_1");   //Plays Landing Animation
            }

            // Jumping 
            if (Input.GetButtonDown("Jump"))// if jump press, sets a jump force
            {
                currentVerticalMovement = jumpForce;          // Instantly turns veritcal movement into jump force
                if (!anim.GetNextAnimatorStateInfo(0).IsName("Jump_1")) anim.Play("Jump_1");     // Play Jump Animation
                onGround = false;                             // set on ground to false
            }
            

        }

        // Final Vertical movement 
        charControll.Move(Vector3.up * currentVerticalMovement * Time.deltaTime);
        #endregion


        //Rotate to directionEven on Idle
        rotateToDirection();


        // Action Changing - Attack 
        if (Input.GetButtonDown("Attack_1"))
        {
            setAction(angelineActions.attack);
            basicComboCount = 1;
        }
        if(Input.GetButtonDown("Attack_2"))
        {
            if (angelineHackform.getCurrentHackform() == Hackform.hackForm.normalForm)
                return;
            if (angelineHackform.getCurrentHackform() == Hackform.hackForm.MibmanForm) setAction(angelineActions.Skill_Mibman);
            if (angelineHackform.getCurrentHackform() == Hackform.hackForm.CthulhuForm) setAction(angelineActions.Skill_Cthulhu);
            if (angelineHackform.getCurrentHackform() == Hackform.hackForm.TetsuoForm) setAction(angelineActions.Skill_Tetsuo);
        }
        if (Input.GetButtonDown("Action_1")) setAction(angelineActions.hackform);


        //
    }

    // On Trigger take damage
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "EnemyHitbox")
        {
            TakeDamage(other.GetComponent<Hitbox>().getDamage());
            takeDamageEffects(other.GetComponent<Hitbox>());

            if (getLifePoint() <=0) setAction(angelineActions.Death);
        }
    }

    private void regenAberration()
    {
        PPProfile.profile.TryGetSettings(out newAberration);
        if(newAberration.intensity.value >0) newAberration.intensity.value -= Time.deltaTime * 3;

        Mathf.Clamp(newAberration.intensity.value, 0, 5f);


    }

    private void takeDamageEffects(Hitbox otherHitbox)
    {
        //Aberration damage effect;
        PPProfile.profile.TryGetSettings(out newAberration);
        if(newAberration.intensity.value <5f) newAberration.intensity.value += 1;

        //Clamp aberration intensity
        Mathf.Clamp(newAberration.intensity.value, 0, 5f);

        //Camera Shake
        cameraShaker.InduceStress(0.15f);

        //Instantiate the HITBOX SFX 
        instantiateSFX(otherHitbox.getHitSound(), 6f, transform.position);
    }

    // -----// -----// -----// -----// ----- Right to Left Rotation
    private void rotateToDirection()
    {
        // Apply look Direction, Right left and Tangents
        Quaternion lookRot = new Quaternion(0, 0, 0, 0);
        if (lookDirection) lookRot = Quaternion.LookRotation(new Vector3(1, 0, 0.55f * moveAxis.y));
        if (!lookDirection) lookRot = Quaternion.LookRotation(new Vector3(-1, 0, 0.55f * moveAxis.y));

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 15f);
        //
    }



    // -----// -----// -----// -----// ----- Attack Action
    private void basicAttacks()
    {
        // Apply look Direction, Even if its idle
        Quaternion lookRot = new Quaternion(0, 0, 0, 0);
        if (lookDirection) lookRot = Quaternion.LookRotation(new Vector3(1, 0, 0));
        if (!lookDirection) lookRot = Quaternion.LookRotation(new Vector3(-1, 0, 0));

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 15f);
        //

        // play current combo animation attack
        if(basicComboCount ==1 && onGround && !anim.GetNextAnimatorStateInfo(0).IsName("Attack_1"))anim.Play("Attack_1");        // Ground Attack 1
        if(basicComboCount ==2 && onGround && !anim.GetNextAnimatorStateInfo(0).IsName("Attack_2")) anim.Play("Attack_2");      // Ground attack 2

        if (basicComboCount == 1 && !onGround && !anim.GetNextAnimatorStateInfo(0).IsName("AirAttack_1"))anim.Play("AirAttack_1"); //Air Attack 1
        //

        //Continue Combos 
        if (Input.GetButtonDown("Attack_1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >=0.5f)
        {
            basicComboCount++;
        }
        //

        // Vertical movement on Attacks
        if (currentVerticalMovement > 0)    // if is going upwards
        {
            charControll.Move(Vector3.up * (currentVerticalMovement / 3) * Time.deltaTime); // Upwards on air movement
        }
        if(currentVerticalMovement <=0)      // Falling 
        {
            charControll.Move(Vector3.up * (currentVerticalMovement *1.15f) * Time.deltaTime); // Upwards on air movement
        }

        //On Air Movement
        if (!onGround)
        {
            currentVerticalMovement += Physics.gravity.y * Time.deltaTime;

            //Clamped Movement based on character controller In Mid Air (moveAxis freezes)
            #region
            Vector3 desiredPosition = transform.position +
                           (Vector3.right * moveAxis.x * this.getMoveSpeed() * Time.deltaTime) +
                           (Vector3.forward * moveAxis.y * this.getMoveSpeed() * 1.45f * Time.deltaTime);

            // Aplica o clamp à posição desejada apenas no eixo X
            float clampedZ = Mathf.Clamp(desiredPosition.z, -7f, 0f);

            // Define a posição final com o valor clamped para o eixo X
            Vector3 finalPosition = new Vector3(desiredPosition.x, desiredPosition.y, clampedZ);

            // Final movement Clamped
            charControll.Move(finalPosition - transform.position);

            #endregion
        }
        

        //Exit Attack animation if its Air attack
        if (!onGround && anim.GetCurrentAnimatorStateInfo(0).IsName("AirAttack_1"))
        {
            if (moveAxis.x < -0.1f) moveAxis.x = -1.35f;
            if (moveAxis.x > 0.1f) moveAxis.x = 1.35f;
            if (Physics.Raycast(transform.position, Vector3.down, 0.1f)) setAction(angelineActions.move);
        }

     
    }

    // -----// -----// -----// -----// ----- Dodge Action

    private void dodge()
    {

    }

    // -----// -----// -----// -----// ----- HackForm Action
    private void hackForm()
    {
        // Apply look Direction, Even if its idle
        Quaternion lookRot = new Quaternion(0, 0, 0, 0);
        if (lookDirection) lookRot = Quaternion.LookRotation(new Vector3(1, 0, 0));
        if (!lookDirection) lookRot = Quaternion.LookRotation(new Vector3(-1, 0, 0));

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 15f);
        //

        anim.Play("Hackform_1");
            //Instantiate(hackformVFX, transform.position, transform.rotation);
        
    }

    public void hackformHit(int enemyType)
    {
        instantiateSFX(hackform_2, 6f, transform.position);
        instantiateSFX(hackformSFX, 6f, transform.position);

        angelineHackform.setSkillCharge(2);
        print("times");

        if (enemyType == 1) angelineHackform.hittedHackform(Hackform.hackForm.MibmanForm);
        if (enemyType == 2) angelineHackform.hittedHackform(Hackform.hackForm.CthulhuForm);
        if (enemyType == 3) angelineHackform.hittedHackform(Hackform.hackForm.TetsuoForm);
    }
    public void castedSkill()
    {
        angelineHackform.setSkillCharge(-1);
    }
    

    // ------ /------ /------ /------ /------ / Skills

    private void skill_Mibman()
    {
        // Apply look Direction, Even if its idle
        Quaternion lookRot = new Quaternion(0, 0, 0, 0);
        if (lookDirection) lookRot = Quaternion.LookRotation(new Vector3(1, 0, 0));
        if (!lookDirection) lookRot = Quaternion.LookRotation(new Vector3(-1, 0, 0));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 15f);


        anim.Play("Skill_Kamehameha");   
    }
    private void skill_Cthulhu()
    {
        // Apply look Direction, Even if its idle
        Quaternion lookRot = new Quaternion(0, 0, 0, 0);
        if (lookDirection) lookRot = Quaternion.LookRotation(new Vector3(1, 0, 0));
        if (!lookDirection) lookRot = Quaternion.LookRotation(new Vector3(-1, 0, 0));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 15f);

        anim.Play("Skill_Purple");
    }
    private void skill_Tetsuo()
    {
        // Apply look Direction, Even if its idle
        Quaternion lookRot = new Quaternion(0, 0, 0, 0);
        if (lookDirection) lookRot = Quaternion.LookRotation(new Vector3(1, 0, 0));
        if (!lookDirection) lookRot = Quaternion.LookRotation(new Vector3(-1, 0, 0));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 15f);

        anim.Play("Skill_Telekinetic");
    }

    // ------ /------ /------ /------ / Event Manager
    public void endAttack()
    {
        setAction(angelineActions.move);
    }

    // ------ /------ /------ /------ / Setters



}
