using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{

    public GameObject MainMenuPanel;
    public GameObject Instruction;
public void loadselection()
    {


        SceneManager.LoadScene("locationselection");
    }
    public void exitgame()
    {
        Debug.Log("application has been quit");

        Application.Quit();
    }
    public void showinstruction()
    {
        MainMenuPanel.SetActive(false);
        Instruction.SetActive(true);
    }
    public void gobacktoMainmenu()
    {
        Instruction.SetActive(false);
        MainMenuPanel.SetActive(true);
    }

}