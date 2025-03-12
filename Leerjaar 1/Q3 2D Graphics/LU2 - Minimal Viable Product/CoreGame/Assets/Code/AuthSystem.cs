using TMPro;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AuthSystem : MonoBehaviour
{
  [Header("UI References")]
  public TMP_InputField UsernameField;
  public TMP_InputField PasswordField;
  public TMP_Text UserMessage;

  [Header("Dependencies")]
  public UserApiClient userApiClient;

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.Escape))
      Quit();


    if (Input.GetKeyDown(KeyCode.Return))
      Login();
  }

  public void Quit()
  {
    Application.Quit();
  }

  private void ShowErrorMessage(string message, Color color)
  {
    if (color == default)
    {
      color = Color.red;
    }

    UserMessage.color = color;
    UserMessage.text = message;
    Debug.LogError(message);
  }

  private void ShowSuccessMessage(string message)
  {
    UserMessage.color = Color.green;
    UserMessage.text = message;
    Debug.Log(message);
  }

  public void ResetFromFields()
  {
    UsernameField.text = "";
    PasswordField.text = "";
  }

  public async void Register()
  {
    UserMessage.text = "";

    User user = new()
    {
      email = UsernameField.text,
      password = PasswordField.text,
    };

    IWebRequestReponse webRequestResponse = await userApiClient.Register(user);

    switch (webRequestResponse)
    {
      case WebRequestData<string>:
        {
          ShowSuccessMessage("Register Successful!");
          ResetFromFields();
          break;
        }
      case WebRequestError webRequestError:
        {
          ShowErrorMessage("Registration failed! \n\n\n Password must be at least 10 characters long and include a digit, a lowercase letter, an uppercase letter, and a special character.", Color.white);
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
    UserMessage.text = "";

    User user = new()
    {
      email = UsernameField.text,
      password = PasswordField.text,
    };

    IWebRequestReponse webRequestResponse = await userApiClient.Login(user);

    switch (webRequestResponse)
    {
      case WebRequestData<string>:
        ShowSuccessMessage("Login Successful!");
        await SceneManager.LoadSceneAsync("MainMenu");
        break;
      case WebRequestError errorResponse:
        var errorMessage = errorResponse.ErrorMessage switch
        {
          "HTTP/1.1 401 Unauthorized" => "Invalid Credentials!",
          _ => "Something went wrong!"
        };

        ShowErrorMessage(errorMessage, Color.red);
        ResetFromFields();
        break;

      default:
        throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
    }
  }
}