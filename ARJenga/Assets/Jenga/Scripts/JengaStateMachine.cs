using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

////////////////////////////////////////////////////////////////////////////////
/// <summary>
/// 
/// Class to control flow of the match.
///
/// </summary>
////////////////////////////////////////////////////////////////////////////////

[RequireComponent(typeof(Animator))]
public class JengaStateMachine : MonoBehaviour 
{
	//== Members ===============================================================

	public MouseBlockHandler mouseHandler;		// Mouse block handler.

	public GameObject playMouse;				// Game Object with all components of mouse handling.

	public GameObject playAR;					// Game Object with all components of AR handling.

	//== UI Elements ===============================================================

	public GameObject panelModel;

	public GameObject buttonWin;

	public GameObject buttonLose;

	public GameObject buttonLost;

	public GameObject panelTurn;

	public Text yourTurn;

	//== Properties ============================================================

	public string sceneBack;						// Scene to load when going back.

	private Rigidbody lastBlockGrabbed = null;		// Last block grabbed in the turn.

	private int tColor;								// Id of color block of current turn.

	private Color turnColor;                        // Color block of current turn.

	private JengaMatch jengaMatch;                  // Socket to control the match.

	private Animator animator;                      // Animator with the state machine.

	private bool blockGrabbed;                      // Block grabbed flag.

	private bool startGame;							// Match started flag.

	//== Methods ===============================================================

	// ---- Unity events ----

	void Start () 
	{
		// Init random color seed.

		Random.InitState(System.DateTime.Now.Millisecond);

		// Initialize variables.

		lastBlockGrabbed = null;
		blockGrabbed = false;
		startGame = false;
		animator = GetComponent<Animator>();
		jengaMatch = GetComponent<JengaMatch>();

		// Check play alone.

		if (animator != null)
			animator.SetBool("Play Alone", PlayerPrefs.GetInt("Play Alone", 1) == 1);
	}

	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
			reloadScene();

		buttonLost.SetActive((Time.time - jengaMatch.lastData) > 5 && startGame);
	}

	// ---- UI Management and control of match flow ----

	public void selectModel(string Model)
    {
		if (string.Equals(Model, "AR"))
        {
			playAR.SetActive(true);
			playMouse.SetActive(false);
		}

		if (string.Equals(Model, "Mouse"))
		{
			playAR.SetActive(false);
			playMouse.SetActive(true);
		}

		panelModel.SetActive(false);
		panelTurn.SetActive(true);
		
		startGame = true;

		if (animator != null)
			animator.SetTrigger("Start Game");

		jengaMatch.lastData = Time.time;
	}

	public void receivedStartTurn() 
	{
		if (animator != null)
			animator.SetTrigger("Start Turn");
	}

	public void startTurn() 
	{
		// Generate Random color.

		tColor = Random.Range(0,3);
		turnColor = new Color(tColor == 0?1:0, tColor == 1?1:0, tColor == 2?1:0, 1);

		// Reset turn variables.

		blockGrabbed = false;
		lastBlockGrabbed = null;
		jengaMatch.isMoving = true;
		if (yourTurn != null) 
		{
			string[] colores = {"red", "green", "blue"};
			yourTurn.text = "Your Turn, color " + colores[tColor];
			yourTurn.color = turnColor;
			yourTurn.gameObject.SetActive(true);
		}

		// Enable and notify handler.

		mouseHandler.enabled = true;
	}

	public void disableHandler() 
	{
		if (mouseHandler.enabled)
		{
			mouseHandler.enabled = false;
		}

		if (yourTurn != null)
			yourTurn.gameObject.SetActive(false);
	}

	public void blockFalls()
	{
		disableHandler();
	}

	public void endTurn() 
	{
		disableHandler();

		if (lastBlockGrabbed != null)
		{
			lastBlockGrabbed.gameObject.SetActive(false);
			lastBlockGrabbed.gameObject.GetComponent<JengaBlock>().e = false;
		}

		lastBlockGrabbed = null;
		jengaMatch.sendEndTurn();
		jengaMatch.isMoving = false;
		Invoke("sentEndTurn", 1.0f);
	}

	public void sentEndTurn() 
	{
		if (animator != null)
			animator.SetTrigger("Start View");
	}

	public void setBlockGrabbed(Rigidbody b) 
	{
		JengaBlock	jb = b.gameObject.GetComponent<JengaBlock>();
		int bColor = -1;
		if (jb != null)
			bColor = jb.color;

		if ((bColor != tColor) || ((lastBlockGrabbed != null) && (lastBlockGrabbed != b))) 
		{
			if (animator != null)
				animator.SetTrigger("Wrong Block Grabbed");
		} 
		else 
		{
			lastBlockGrabbed = b;
		}
		
		blockGrabbed = true;
	}

	public void releaseBlock()
	{
		blockGrabbed = false;
	}

	public void blockTouchesGround(Rigidbody b) 
	{
		if (lastBlockGrabbed == null)
			return;

		JengaBlock jb = b.GetComponent<JengaBlock>();
		if (jb == null)
			return;

		if (jb.floor == 0)
			return;

		if (b != lastBlockGrabbed) 
		{
			if (animator != null)
				animator.SetTrigger("Wrong Block Falls");
		} 
		else
		{
			blockFalls();
			if (animator != null)
				animator.SetTrigger("Block Falls");
		}
	}

	public void lose()
	{
		jengaMatch.sendLost();
		if (buttonLose != null)
			buttonLose.SetActive(true);
	}

	public void win()
	{
		if (buttonWin != null)
			buttonWin.SetActive(true);
	}

	public void reloadScene()
	{
		Scene scene = SceneManager.GetActiveScene(); 
		SceneManager.LoadScene(sceneBack);
	}
}
