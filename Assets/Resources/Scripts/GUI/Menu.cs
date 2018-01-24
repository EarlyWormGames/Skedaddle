//using UnityEngine;
//using UnityEngine.UI;
//using System.Collections;

//public class Menu : MonoBehaviour
//{
//    //------------------ Public Variables -------------------//
//    public GameObject[] ObjectsToHide;
//    public GameObject[] CanvasGroups;
//    public GameObject[] SignLights;
//    public int[] SingLightNumbers;

//    public GameObject m_KeyPanel;
//    public GameObject m_ButtonPanel;

//    public Sprite[] m_Sprites;
//    public Dropdown m_dropdown;

//    public int[] m_EnumIndex;
//    public bool m_lightsOn = false;

//    //----------------- Private Variables -------------------//
//    private ButtonEnum[] m_beKeyDisplays;

//    private GameObject m_CurrentKeyButton;
//    private GameObject m_CurrentGPadButton;

//    private bool m_bSecondaryKey = false;

//    private KeyActionName m_kAction = KeyActionName.MoveLeft;

//    private float m_fLightTimer = 0;
//    private float m_fMaxLightTimer = 1.0f;
//    private bool m_bEvenLightOn = true;

//    // Use this for initialization
//    void Start()
//    {
//        //if (EWDebug.isDebug && m_SubscribePanel != null)
//        //{
//        //    m_SubscribePanel.SetActive(false);
//        //}
//        //else if (m_SubscribePanel != null)
//        //{
//        //    m_SubscribePanel.SetActive(true);
//        //}

//        m_fLightTimer = m_fMaxLightTimer;
//        // Disables any aditional menu panels/GameObjects on launch
//        for (int i = 0; i < ObjectsToHide.Length; i++)
//        {
//            ObjectsToHide[i].SetActive(false);
//        }

//        // Sets all canvas group Alpahs back to 1 on launch
//        for (int i = 0; i < CanvasGroups.Length; i++)
//        {
//            //CanvasGroups[i].SetActive(true);
//            CanvasGroups[i].GetComponent<CanvasGroup>().alpha = 1;
//        }

//        // Makes sure Menu panel is always active on start
//        //ObjectsToHide[4].SetActive(true);
//    }

//    void Update()
//    {
//        m_fLightTimer -= (1 * Time.deltaTime);

//        if (m_fLightTimer < 0)
//        {
//            m_fLightTimer = m_fMaxLightTimer;

//            for (int i = 0; i < SingLightNumbers.Length; i++)
//            {
//                if (m_bEvenLightOn)
//                {

//                    SignLights[SingLightNumbers[i]].SetActive(false);
//                }
//                else
//                {

//                    SignLights[SingLightNumbers[i]].SetActive(true);
//                }
//            }
//            m_bEvenLightOn = m_bEvenLightOn ? false : true;
//        }


//        // This statment is used for detecting Controller buttons for setting controls
//        if (m_CurrentGPadButton != null)
//        {
//            // Listen for the Gamepad button press
//            GPadControls GPad = Keybinding.ListenForGPadButton();
//            Sprite spr = GetGpadButton(GPad);
//            if (spr != null)
//            {
//                m_CurrentGPadButton.GetComponent<Image>().sprite = spr;
//                m_CurrentGPadButton.GetComponent<ButtonEnum>().Check();
//                Keybinding.SetGPKey(m_kAction, GPad);
//                m_CurrentGPadButton = null;
//            }
//            if (m_CurrentGPadButton == null)
//            {
//                m_ButtonPanel.SetActive(false);
//            }
//        }

//        //for(int i = 0; i < SignLights.Length; i++)
//        //{
//        //    SignLights[i].SetActive(m_lightsOn);
//        //}


//    }

//    public Sprite GetGpadButton(GPadControls GPad)
//    {
//        if (m_EnumIndex[(int)GPad] > -1)
//        {
//            return m_Sprites[m_EnumIndex[(int)GPad]];
//        }
//        return null;
//    }

//    // Closes Application
//    public void Quit()
//    {
//        Application.Quit();
//    }

//    /// <summary>
//    /// Loads the desired scene
//    /// </summary>
//    /// <param name="a_string">Scene Name</param>
//    public void LoadScene(string a_string)
//    {
//        EWApplication.LoadLevel(a_string);
//    }

//    //---------------------------------------------------------------------//
//    //-----------------------OPTIONS FUNCTIONS-----------------------------//
//    //---------------------------------------------------------------------//

//    /// <summary>
//    /// Detects the actual keycode from pressed key
//    /// </summary>
//    /// <param name="a_GameObject">Self</param>
//    public void GetKeyInput(GameObject a_GameObject)
//    {
//        m_CurrentKeyButton = a_GameObject;
//        m_kAction = m_CurrentKeyButton.GetComponent<ButtonEnum>().m_kKey;
//        m_bSecondaryKey = m_CurrentKeyButton.GetComponent<ButtonEnum>().m_bIsSecondary;
//    }

//    /// <summary>
//    /// Detects the actual button pressed on controller
//    /// </summary>
//    /// <param name="a_GameObject">The button pressed in scene</param>
//    public void GPadInput(GameObject a_GameObject)
//    {
//        m_CurrentGPadButton = a_GameObject;
//        m_kAction = m_CurrentGPadButton.GetComponent<ButtonEnum>().m_kKey;
//    }
//    /// <summary>
//    /// Populates all control configuration buttons
//    /// </summary>
//    public void PopulateInputArray()
//    {
//        m_beKeyDisplays = GetComponentsInChildren<ButtonEnum>();
//    }

//    /// <summary>
//    /// Resetes the button icons
//    /// </summary>
//    public void ResetInputGraphics()
//    {
//        foreach (ButtonEnum child in m_beKeyDisplays)
//        {
//            child.Check();
//        }
//    }

//    /// <summary>
//    /// Resetes the primary keys
//    /// </summary>
//    public void ResetPrim()
//    {
//        Keybinding.Instance.ResetPrimary();
//    }

//    /// <summary>
//    /// Resetes the secondary keys
//    /// </summary>
//    public void ResetSecondary()
//    {
//        //Keybinding.Instance.ResetSecondary();
//    }

//    /// <summary>
//    /// Resetes the Gpad buttons
//    /// </summary>
//    public void ResetGPad()
//    {
//        Keybinding.Instance.ResetGamePad();
//    }

//    /// <summary>
//    /// Resetes all keys
//    /// </summary>
//    public void ResetAll()
//    {
//        Keybinding.Instance.ResetAll();
//    }

//    /// <summary>
//    /// OnGUI must be used in order to use the key event, this is basicly the lister for inputs
//    /// </summary>
//    void OnGUI()
//    {
//        //if (m_CurrentKeyButton != null)
//        //{
//        //    Event e = Event.current;
//        //    if (e != null)
//        //    {
//        //        if (e.isKey)
//        //        {
//        //            EWDebug.Log("Key Pressed: " + e.keyCode);
//        //            m_CurrentKeyButton.GetComponentInChildren<Text>().text = e.keyCode.ToString();
//        //            Keybinding.SetKey(m_kAction, e.keyCode, m_bSecondaryKey);
//        //            m_CurrentKeyButton.GetComponent<ButtonEnum>().Check();
//        //            m_CurrentKeyButton = null;
//        //        }
//        //        else if (e.shift)
//        //        {
//        //            if (Input.GetKey(KeyCode.LeftShift))
//        //            {
//        //                EWDebug.Log("Key Pressed: " + "LeftShift");
//        //                m_CurrentKeyButton.GetComponentInChildren<Text>().text = "L Shift";
//        //                Keybinding.SetKey(m_kAction, KeyCode.LeftShift, m_bSecondaryKey);
//        //            }
//        //            else if (Input.GetKey(KeyCode.RightShift))
//        //            {
//        //                EWDebug.Log("Key Pressed: " + "RightShift");
//        //                m_CurrentKeyButton.GetComponentInChildren<Text>().text = "R Shift";
//        //                Keybinding.SetKey(m_kAction, KeyCode.RightShift, m_bSecondaryKey);
//        //            }
//        //            m_CurrentKeyButton.GetComponent<ButtonEnum>().Check();
//        //            m_CurrentKeyButton = null;
//        //        }
//        //        if (m_CurrentKeyButton == null)
//        //        {
//        //            m_KeyPanel.SetActive(false);
//        //        }
//        //    }
//        //}
//    }
//    //---------------------------------------------------------------------//
//    //-----------------------END OPTIONS FUNCTIONS-------------------------//
//    //---------------------------------------------------------------------//

    
//}
