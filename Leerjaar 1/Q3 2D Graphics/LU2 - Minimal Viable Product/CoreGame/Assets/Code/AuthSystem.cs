using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class AuthSystem : MonoBehaviour
{
  public TMP_InputField UsernameField;
  public TMP_InputField PasswordField;
  public TMP_Text MessageText;

  [Header("Dependencies")]
  public UserApiClient userApiClient;
  public Environment2DApiClient enviroment2DApiClient;
  public Object2DApiClient object2DApiClient;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
            Quit(); 

        
        if (Input.GetKeyDown(KeyCode.Return))
            Login();

        if (Input.GetKeyDown(KeyCode.L))
            Login();

        if (Input.GetKeyDown(KeyCode.R)) 
            Register();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public async void Register()
  {
        MessageText.text = "";

        User user = new()
    {
      email = UsernameField.text,
      password = PasswordField.text,
    };

    IWebRequestReponse webRequestResponse = await userApiClient.Register(user);

    switch (webRequestResponse)
    {
      case WebRequestData<string> dataResponse:
        {
          Debug.Log("Register succes!");
          // TODO: Handle succes scenario;
          MessageText.color = Color.green;
          MessageText.text = "Register Succesfull!";
          UsernameField.text = "";
          PasswordField.text = "";

          break;
        }
      case WebRequestError errorResponse:
        {
          string errorMessage = errorResponse.ErrorMessage;
          Debug.Log("Register error: " + errorMessage);
          // TODO: Handle error scenario. Show the errormessage to the user.
          MessageText.color = Color.red;
          MessageText.text = errorMessage switch
          {
            "HTTP/1.1 400 Bad Request" => "Invalid Credentials!",
            _ => "Something went wrong!",
          };
          break;
        }
      default:
        {
          throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }
  }

  public async void Login()
  {
        MessageText.text = "";

        User user = new()
    {
      email = UsernameField.text,
      password = PasswordField.text,
    };

    IWebRequestReponse webRequestResponse = await userApiClient.Login(user);

    switch (webRequestResponse)
    {
      case WebRequestData<string> dataResponse:
        Debug.Log("Login succes!");
        // TODO: Todo handle succes scenario.
        MessageText.color = Color.green;
        MessageText.text = "Login Succesfull!";

        await SceneManager.LoadSceneAsync("StartScreen");
        break;
      case WebRequestError errorResponse:
        string errorMessage = errorResponse.ErrorMessage;
        Debug.Log("Login error: " + errorMessage);
                // TODO: Handle error scenario. Show the errormessage to the user.
        MessageText.color = Color.red;
        MessageText.text = errorMessage switch
        {
          "HTTP/1.1 401 Unauthorized" => "Invalid Credentials!",
          _ => "Something went wrong!",
        };
        break;
      default:
        throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
    }
  }
}