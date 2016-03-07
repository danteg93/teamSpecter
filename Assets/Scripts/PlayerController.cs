using UnityEngine;
using System;
using System.Collections;
//TODO dash CD

public class PlayerController : MonoBehaviour {

  // Basic player initialization.
  public int PlayerNumber = 0;
  //made this private because it automatically gets mapped by the input controller
  //this way, we only have to set a player number
  //Once we do automatic scene population we can rethink this. But this makes the thing look cleaner for now
  private bool UseKeyboardControl = false;
  private string inputMapping;

  // Base attributes of the player object.
  public float Speed = 0f;
  public float AccelerationFactor = 0.55f;

  // Primary ability initialization.
  public GameObject PrimaryAbility;
  private bool isUsingPrimaryAbility = false;
  private float primaryAbilityCooldownTimer = 0;

  // Blocking ability initialization.
  public GameObject Shield;
  private bool isUsingSecondaryAbility = false;
  private GameObject[] secondaryAbilityCastList = new GameObject[1];
  private bool shieldOn = false;
  private float shieldCooldownTimer = 0;

  // Particle Effects
  public GameObject explosionParticle;
  // Dash ability initialization.
  public float DashCooldown = 1.0f;
  public float DashPower = 15.0f;
  private bool isPressingDash = false;
  private float dashCooldownTimer = 0;
  private bool initializedByGamemode = false;

  //Delta related variables
  private Vector2 previousVelocity = Vector2.zero; //changed to vector, figured its more valuable than just the magnitude
  private Vector2 movementDirection = Vector2.zero;
  private Vector2 initialPosition = Vector2.zero;

  //Ability toogle variables
  private bool dying = false;
  private bool playerShouldRespawn = false;
  private bool movementAndShootingAllowed = true;
  private bool invincible = false;

  // Process inputs that do not rely on physics updates.
  void Start() {
    inputMapping = InputController.inputController.GetPlayerMapping(PlayerNumber);
    initialPosition = gameObject.transform.position;
    if (inputMapping == "k") {
      UseKeyboardControl = true;
    }
    else {
      UseKeyboardControl = false;
    }
  }
  void Update() {
    if (!movementAndShootingAllowed) { return; }
    processPrimaryAbilityInput();
    processBlockInput();
  }

  // Process all other actions that do rely on physics updates.
  void FixedUpdate() {
    if (dying) { return; }
    if (playerShouldRespawn) { executeRespawn(); }
    // If the player is using the keyboard and mouse, execute that movement. Otherwise,
    // find the appropriate joystick for the player
    if (UseKeyboardControl) {
      executeMouseRotation();
    } else {
      executeJoyStickRotation();
    }
    // Anything below this line will not be executed until the game countdown hits 0.
    if (!movementAndShootingAllowed) { return; }
    // Execute movement of the player.
    executeMovement();
    executeDash();
  }

  public void ShieldDestroyed() {
    if (shieldOn) { shieldOn = false; }
  }

  public bool IsShieldOn() {
    return shieldOn;
  }

  public void Kill() {
    if(invincible){return;}
    Cameraman.cameraman.CameraShake(0.5f, 0.1f);
    //So that this can work without gamemode in the scene
    if (initializedByGamemode) { Gamemode.gamemode.playerDied(PlayerNumber, PlayerNumber); }
    StartCoroutine(deathAnimation());
  }

  public void Kill(int killedBy) {
    if (invincible){return;}
    Cameraman.cameraman.CameraShake(0.5f, 0.1f);
    //So that this can work without gamemode in the scene
    if (initializedByGamemode) { Gamemode.gamemode.playerDied(PlayerNumber, killedBy); }
    StartCoroutine(deathAnimation());
  }
  // TODO: Play death animation here.
  // Coroutine for killing the player. This will paly a death animation, stop all player
  // movement, and play a sound on death. The object is not destroyed so that the player
  // can respawn.
  IEnumerator deathAnimation() {
    dying = true;
    SetPlayerInvincibility(true);
    SetPlayerMoveAndShoot(false);
    playAudioDeath();
    //Added this so crossHair would also not show up
    SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    foreach (SpriteRenderer spriteRenderer  in spriteRenderers) {
      spriteRenderer.enabled = false;
    }
    GetComponent<CircleCollider2D>().enabled = false;
    GetComponent<Rigidbody2D>().isKinematic = true;
    //Particle initiation
    GameObject tempBoom = Instantiate(explosionParticle, transform.position, transform.rotation) as GameObject;
    tempBoom.transform.parent = transform;
    yield return new WaitForSeconds(2.0f);
    dying = false;
    Destroy(tempBoom);
    if (!playerShouldRespawn) {
      gameObject.SetActive(false);
    }
  }
  //sets respawn flag
  public void respawn() {
    playerShouldRespawn = true;
  }
  //Respawn at initial position
  public void executeRespawn() {
    //Added this so crossHair would also not show up
    SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    foreach (SpriteRenderer spriteRenderer  in spriteRenderers) {
      spriteRenderer.enabled = true;
    }
    GetComponent<CircleCollider2D>().enabled = true;
    GetComponent<Rigidbody2D>().isKinematic = false;
  //  GetComponent<crossHair>().enabled = true;
    SetPlayerMoveAndShoot(true);
    playerShouldRespawn = false;
    gameObject.transform.position = initialPosition;
    StartCoroutine(respawnInvincibility());
  }
  IEnumerator respawnInvincibility() {
    //TODO graphical que that dude is invincible
    yield return new WaitForSeconds(2.0f);
    SetPlayerInvincibility(false);
  }
  //This function gets called by game mode to allow players to do stuff once the timer ends
  public void SetPlayerMoveAndShoot(bool allowMoveAndShoot) {
    movementAndShootingAllowed = allowMoveAndShoot;
  }

  public void SetPlayerInvincibility(bool invincibility) {
    invincible = invincibility;
  }
  public void setInitializedByGamemode(bool gamemodeInitialized) {
    initializedByGamemode = gamemodeInitialized;
  }

  private void executeMovement() {
    //Vector calculated from the input, it gives a direction of where to move next.
    Vector2 newVelocity;
    if (UseKeyboardControl) {
      //Getting the axis raw (lawl) stops unity from smoothing out key presses (reason why we had sliding with keyboard and not with sticks)
      newVelocity = new Vector2(Input.GetAxisRaw("HorizontalMovementK") * Speed, Input.GetAxisRaw("VerticalMovementK") * Speed);
    }
    else {
      newVelocity = new Vector2(Input.GetAxis(inputMapping + "_HorizontalMovement") * Speed, -Input.GetAxis(inputMapping + "_VerticalMovement") * Speed);
    }
    //if the direction of the movement has changed and you are not static then you will want to "deaccelerate"
    if (newVelocity.magnitude - previousVelocity.magnitude < 0.0f && GetComponent<Rigidbody2D>().velocity.magnitude != 0.0f) {
      newVelocity = Vector2.zero;
    }
    //change the direction towards the direction of the new velocity
    GetComponent<Rigidbody2D>().velocity = Vector2.MoveTowards(GetComponent<Rigidbody2D>().velocity, newVelocity, AccelerationFactor);
    //set previous velocity for delta calculations;
    previousVelocity = GetComponent<Rigidbody2D>().velocity;
    //kept newVeloctiy because these methods are asynced, so other functions that might use movementDirection
    //could potentially access it while being updated.
    movementDirection = newVelocity.normalized;
  }

  // Calculate the angle from the player to the mouse and set the
  // rotation of the player to that angle.
  private void executeMouseRotation() {
    Vector3 objectToMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
    float angleToMouse = (Mathf.Atan2(objectToMouse.y, objectToMouse.x) * Mathf.Rad2Deg) + 90.0f; //The 90 assumes that the front of the sprite is the bottom part.
    transform.rotation = Quaternion.Euler(0f, 0f, angleToMouse);
  }

  private void executeJoyStickRotation() {
    //Vector2 that says where the joystick is
    Vector2 joyStickLocation = new Vector2(Input.GetAxis(inputMapping + "_HorizontalRotation"), Input.GetAxis(inputMapping + "_VerticalRotation"));
    if (joyStickLocation.magnitude != 0) { //this wont reset your rotation if you randomly let go off the controller
      joyStickLocation.Normalize();
      float angleToStick = (Mathf.Atan2(joyStickLocation.x, joyStickLocation.y) * Mathf.Rad2Deg);
      transform.rotation = Quaternion.Euler(0f, 0f, angleToStick);
    }
  }
  private void executeDash() {
    Vector2 dashVector;
    //get the value for the dash input
    float dashInput;
    if (UseKeyboardControl) {
      dashInput = Input.GetAxis("DashK");
    }
    else {
      dashInput = Input.GetAxis(inputMapping + "_Dash");
    }
    //makes sure that this doesnt get called a million times
    if (dashInput != 0 && !isPressingDash && dashCooldownTimer == DashCooldown) {
      isPressingDash = true;
      dashCooldownTimer -= Time.deltaTime;
      playAudioDash();
      //movement Direction gets calculated in the executeMovement function.
      //If you are moving, the dash will take you in that direction.
      if (movementDirection.magnitude != 0.0f) {
        //Set the vector of the dash to be the movementDirection (normalized by trig) times the dash factor
        dashVector = movementDirection * DashPower;
      }
      //If you are not moving, the dash will take you in the direction you are facing
      else {
        //Get the adjusted rotation (in rads) of the current rotation of the sprite.
        float adjustedRotationRadians = (transform.rotation.eulerAngles.z - 90.0f) * Mathf.Deg2Rad;
        //Using the magic of trig, calculate a vector (comes out normalized) that will move you in the direction
        //of the angle you are facing. Do you even unit circle bro?
        Vector2 facingDirection = new Vector2(Mathf.Cos(adjustedRotationRadians), Mathf.Sin(adjustedRotationRadians));
        //Multiply it by the dash factor
        dashVector = facingDirection * DashPower;
      }
      //Set the veloctiy
      GetComponent<Rigidbody2D>().velocity += dashVector;
    }
    else if(dashInput == 0) {
      isPressingDash = false;
    }
    //Decrease the timer
    if (dashCooldownTimer < DashCooldown && dashCooldownTimer > 0) {
      dashCooldownTimer -= Time.deltaTime;
    }
    else if (dashCooldownTimer <= 0) {
      dashCooldownTimer = DashCooldown;
    }
  }

  // Fire a bullet if the player is not performing other actions and
  // the cooldown is off. Reduce the cooldown if it is on.
  private void processPrimaryAbilityInput() {
    if (shieldOn) { return; }

    if (UseKeyboardControl && Input.GetAxis("ShootProjectileK") != 0 || !UseKeyboardControl && Input.GetAxis(inputMapping + "_Primary") > 0) {
      if (!isUsingPrimaryAbility && primaryAbilityCooldownTimer <= 0) {
        isUsingPrimaryAbility = true;
        PrimaryAbility.GetComponent<AbstractAbility>().Cast(this);
        primaryAbilityCooldownTimer = PrimaryAbility.GetComponent<AbstractAbility>().Cooldown;
      }
    } else if (isUsingPrimaryAbility) {
      isUsingPrimaryAbility = false;
    }

    if (primaryAbilityCooldownTimer > 0) { primaryAbilityCooldownTimer -= Time.deltaTime; }
  }

  // Deactivate the shield if the player already has an active shield
  // on them. Activate a shield if the cooldown is off. Reduce the
  // cooldown if it is on.
  private void processBlockInput() {
    if (UseKeyboardControl && Input.GetAxis("BlockK") != 0 || !UseKeyboardControl && Input.GetAxis(inputMapping + "_Secondary") > 0) {
      if (!isUsingSecondaryAbility) {
        isUsingSecondaryAbility = true;
        if (shieldOn) {
          secondaryAbilityCastList[0].GetComponent<AbstractAbility>().Uncast();
          shieldOn = false;
        } else if (shieldCooldownTimer <= 0) {
          secondaryAbilityCastList[0] = Shield.GetComponent<AbstractAbility>().Cast(this);
          shieldOn = true;
          shieldCooldownTimer = Shield.GetComponent<AbstractAbility>().Cooldown;
        }
      }
    } else if (isUsingSecondaryAbility) {
      isUsingSecondaryAbility = false;
    }

		if (shieldCooldownTimer > 0) {
			shieldCooldownTimer -= Time.deltaTime;
			if (PlayerNumber == 1) {this.GetComponent<SpriteRenderer> ().color = Color.black;
			} else if (PlayerNumber == 2) { GetComponent<SpriteRenderer> ().color = Color.black;
			} else if (PlayerNumber == 3) { GetComponent<SpriteRenderer> ().color = Color.black;
			} else if (PlayerNumber == 4) { GetComponent<SpriteRenderer> ().color = Color.black;
			}

		} else {
			if (PlayerNumber == 1) { GetComponent<SpriteRenderer> ().color = Color.red;
			} else if (PlayerNumber == 2) { GetComponent<SpriteRenderer> ().color = Color.blue;
			} else if (PlayerNumber == 3) { GetComponent<SpriteRenderer> ().color = Color.yellow;
			} else if (PlayerNumber == 4) { GetComponent<SpriteRenderer> ().color = Color.green;
			}
		}
  }

  private void playAudioDeath() {
    AudioClip deathSound = Resources.Load<AudioClip>("Audio/SFX/Player/PlayerHit");
    GetComponent<AudioSource>().PlayOneShot(deathSound, 0.5f);
  }

  private void playAudioDash() {
    AudioClip dashSound = Resources.Load<AudioClip>("Audio/SFX/Player/DashActivate");
    GetComponent<AudioSource>().PlayOneShot(dashSound, 0.5f);
  }
}
