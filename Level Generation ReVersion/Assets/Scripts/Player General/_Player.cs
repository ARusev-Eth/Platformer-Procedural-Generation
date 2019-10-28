 using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class _Player : MonoBehaviour {
		
	// Publics
	public int lives;				// Player continue attemps
	public float maxHP;  			// Player maximum hit points
	public float maxMP;				// Player maximum mana points
	public float mpTime;			// Time before MPs begin decaying
	public Animator anim;			// Main character animation controller
	public float regenVal;			// How fast the player's HP regenerates 
	public int defPerHitMP;			// The default mp gained per successful hit
	public float mpDecayRate;		// Speed at which MP decays
	public float hpDecayRate;		// How fast the hp bar drops
	public float mpRegenRate;		// How fast the mp bar climbs
	public float inCombatFor;		// How long after attacking or taking damage is the player in combat for
	public RectTransform iniHPPos;  // Initial HP bar position 
	public RectTransform iniMPPos;	// Initial MP bar position


	// GUI
	public Image hpBar;				// The moving part of the bar
	public Image mpBar;				// Not to be used yet
	public Text mpLeft;				// Written values of MP
	public Text healthLeft;			// Written values of HP


	// Privates
	private float curHP;			// Player's current hit points
	private float curMP;			// Player's current mana points
	private float percHP;			// Percentage health remaining
	private float percMP;			// Percentage health remaining
	private float hpTemp;			// Used for damaging the player
	private float mpTemp;			// Used for boosting/reducing the player's MP
	private float casheYMP;			// Easy access MP bar Y
	private float casheYHP;			// Easy access HP bar Y
	private float timeTemp;			// Used for invincibility frames
	private bool invincible;		// Used for invincibility frames
	private float mpDecayTime;		// Time before MP begins decaying 
	private float maxXHPValue;		// Represents a full HP bar's X transform 
	private float minXHPValue;		// Represents an empty HP bar's X transform
	private float maxXMPValue;		// Represents a full MP bar's X transform
	private float minXMPValue;		// Represents an empty MP bar's X transform
	private bool CombatStatus; 		// Is the player in combat or not
	private float combatExitTime;	// How long it takes to leave combat
	private bool permenentCombat;	// If true player cannot leave combat (boss fight etc.)


	private void Start () 
	{ 
		SetUp ();
	}
		
	private void Update ()
	{
		percHP = (curHP / maxHP) * 100;
		percMP = (curMP / maxMP) * 100;
			healthLeft.text = Mathf.RoundToInt (percHP) + "%";
		mpLeft.text = Mathf.RoundToInt (percMP) + "%";

		hpBar.transform.localPosition = new Vector2 (GetBarX (curHP, 0, maxHP, minXHPValue, maxXHPValue), casheYHP);
		mpBar.transform.localPosition = new Vector2 (GetBarX (curMP, 0, maxMP, minXMPValue, maxXMPValue), casheYMP);
	
		SetFight ();
		DecayMP ();
		RegenHP ();
		Invincible ();
		DamagePlayer ();
		ErrorCheck ();

		// An example of getting hit:
		// 20 points of damage
		// Invulnrable for 1 sec
		// Mana points gained: 8 - 11
		if (Input.GetKeyDown (KeyCode.X)){
			Hurt (20,1);
			BoostMP (8); 
			InCombat ();
		}
	}

	// Getters / setters
	public float GetCurHP () { return curHP; }
	public void SetCurHP (float x) { curHP = x; }
	public float GetCurMP () { return curMP; }
	public void SetCurMP (float x) { curMP = x; }
	public void SetCombatStatus (bool x) { CombatStatus = x; }
	public bool GetCombatStatus () { return CombatStatus; }

	private void SetUp ()
	{
		curHP = maxHP;
		curMP = 0;
		hpTemp = curHP;

		maxXHPValue = iniHPPos.localPosition.x;
		maxXMPValue = iniMPPos.localPosition.x;

		minXHPValue = iniHPPos.localPosition.x - iniHPPos.rect.width;
		minXMPValue = iniMPPos.localPosition.x - iniMPPos.rect.width;

		casheYHP = iniHPPos.localPosition.y;
		casheYMP = iniMPPos.localPosition.y;

	}
	
	private float GetBarX (float x, float inMin, float inMax, float outMin, float outMax)
	{
		float curBarX = (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
		return curBarX;
	}

	private void DecayMP ()
	{
		if (mpDecayTime > 0) {
			mpDecayTime -= Time.deltaTime;
		}
		if (mpDecayTime <= 0 && curMP > 0) {
			curMP -= mpDecayRate * Time.deltaTime;
			mpTemp = curMP;
		}
		if (curMP < mpTemp && mpTemp > 0) {
			curMP += mpRegenRate * Time.deltaTime;
		}
	}

	private void RegenHP ()
	{
		if (curHP < maxHP && !CombatStatus) {
			curHP += regenVal * Time.deltaTime;
			hpTemp = curHP;
		}
	}

	public void InCombat ()
	{
		combatExitTime = inCombatFor;
	}

	// Move messy overloads
	private void SetFight ()
	{
		if (combatExitTime > 0 && !permenentCombat) {
			CombatStatus = true;
			combatExitTime -= Time.deltaTime;
		}
		if (combatExitTime <= 0 && !permenentCombat) {
			CombatStatus = false;
		}
	}

	public void SetFight (bool x)
	{
		permenentCombat = x;
		CombatStatus = x;
	}

	// Some messy overloading going on

	public void BoostMP ()
	{
		if (curMP < 0) {
			curMP = 0;
		}
		if (mpTemp < 0) {
			mpTemp = 0;
		}
		mpTemp += defPerHitMP + Random.Range (0, 3);
		mpDecayTime = mpTime;
	}

	public void BoostMP (float x)
	{
		if (curMP < 0) {
			curMP = 0;
		}
		if (mpTemp < 0) {
			mpTemp = 0;
		}
		mpTemp += x;
		mpDecayTime = mpTime;
	}

	// Overload used for pick ups
	// the player would be out of combat and decay would begin immediately otherwise
	public void BoostMP (float x, float y)
	{
		if (curMP < 0) {
			curMP = 0;
		}
		if (mpTemp < 0) {
			mpTemp = 0;
		}
		mpTemp += x;
		mpDecayTime = y;
	}
	
	private void DamagePlayer ()
	{
		if (curHP > hpTemp) {
			curHP -= hpDecayRate * Time.deltaTime;
		} else if (curHP < hpTemp) {
			curHP = hpTemp;
		}
	}

	private void Invincible ()
	{
		if (timeTemp > Time.time) {
			invincible = true;
			anim.SetBool("Invincible", true);
		} else {
			anim.SetBool("Invincible", false);
			invincible = false;
		}
	}

	public void Hurt (float x, float y)
	{
		if (!invincible) {
			hpTemp -= x;
			timeTemp = y + Time.time;
		} 
		if (hpTemp < 0) {
			hpTemp = 0;
		}

	}
	

	// Make sure bars dont go crazy
	private void ErrorCheck ()
	{
		if (curHP > maxHP) {
			curHP = maxHP;
		}
		if (curHP < 0) {
			curHP = 0;
		}
	
		if (curMP > maxMP) {
			curMP = maxMP;
		}
		if (curMP < 0) {
			curMP = 0;
		}
	}
}
