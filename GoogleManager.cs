using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Extensions;

public class GoogleManager : MonoBehaviour
{

    DatabaseManager dbManager;

    public Text infoText;
    public string webClientId = "";

    // Estara disponible despues de login.
    public string email;
    public string username;

    private GoogleSignInConfiguration configuration;

    public string userEmail;

    private void Awake()
    {
        configuration = new GoogleSignInConfiguration { WebClientId = webClientId, RequestEmail = true, RequestIdToken = true };
    }

    public void Start()
    {
        dbManager = GameObject.FindGameObjectWithTag("DatabaseManager").GetComponent<DatabaseManager>();
    }

    public void SignInWithGoogle() { OnSignIn(); }
    public void SignOutFromGoogle() { OnSignOut(); }

    private void OnSignIn()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
    }


    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            using (IEnumerator<Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error = (GoogleSignIn.SignInException)enumerator.Current;
                    AddToInformation("Got Error: " + error.Status + " " + error.Message);
                }
                else
                {
                    AddToInformation("Got Unexpected Exception?!?" + task.Exception);
                }
            }
        }
        else if (task.IsCanceled)
        {
            AddToInformation("Canceled");
        }
        else
        {
            username = task.Result.DisplayName;
            email = task.Result.Email;
            string IdToken = task.Result.IdToken;


            dbManager.writeNewUser(dbManager.createUser(username, email));
        }
    }


    public void displayUser()
    {
        dbManager.getUser(email, (user) =>
        {
            AddToInformation(user.username);
            AddToInformation(user.email);
        }, () =>
        {
            AddToInformation("Did not work");
        });
    }

    private void OnSignOut()
    {
        AddToInformation("Calling SignOut");
        GoogleSignIn.DefaultInstance.SignOut();
    }

    public void OnDisconnect()
    {
        AddToInformation("Calling Disconnect");
        GoogleSignIn.DefaultInstance.Disconnect();
    }

    private void AddToInformation(string str) { infoText.text += "\n" + str; }
}