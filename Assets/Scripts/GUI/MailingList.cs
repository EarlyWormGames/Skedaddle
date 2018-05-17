using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Xml;

/// <summary>
/// joining the mailing list
/// </summary>
public class MailingList : MonoBehaviour {

    public GameObject m_SubscribePanel;
    public InputField m_nameField;
    public InputField m_emailField;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //---------------------------------------------------------------------//
    //------------------------SUBSCRIBE FUNCTIONS--------------------------//
    //---------------------------------------------------------------------//

    public void Submit()
    {
        if ((m_nameField.text == "" && m_nameField != null) || (m_emailField.text == "" && m_emailField != null))
            return;

        //Start the document
        XmlDocument doc = new XmlDocument();
        //Create the row parent and the row
        XmlElement rowset;

        if (File.Exists(Application.persistentDataPath + "/subscribe.xml"))
        {
            doc.Load(Application.persistentDataPath + "/subscribe.xml");
            rowset = doc.DocumentElement;
        }
        else
        {
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", null, null));
            rowset = doc.CreateElement("ROWSET");
        }

        if (m_nameField != null && m_emailField != null)
        {
            XmlElement row = doc.CreateElement("ROW");
            //Create the name and email and set their values
            XmlElement name = doc.CreateElement("name");

            name.InnerText = m_nameField.text;


            XmlElement email = doc.CreateElement("email");
            email.InnerText = m_emailField.text;

            //Now work backwards up the hierarchy
            row.AppendChild(name);
            row.AppendChild(email);
            rowset.AppendChild(row);
            doc.AppendChild(rowset);
            //And finally save the file
            doc.Save(Application.persistentDataPath + "/subscribe.xml");
        }

        Skip();
    }

    public void Skip()
    {
        EWApplication.LoadLevel("Menu");
    }

    //---------------------------------------------------------------------//
    //----------------------END SUBSCRIBE FUNCTIONS------------------------//
    //---------------------------------------------------------------------//
}
