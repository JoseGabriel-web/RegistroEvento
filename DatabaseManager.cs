using Firebase.Database;
using UnityEngine;
using System.Collections;
using System;
using Firebase.Extensions;

public class DatabaseManager : MonoBehaviour
{

    DatabaseReference dbReference;

    // Start is called before the first frame update
    void Start()
    {
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    private string getUserID(string email)
    {
        return email.Substring(0, email.IndexOf("@"));
    }

    public User createUser(string name, string email)
    {
        return new User(name, email);
    }

    public void writeNewUser(User user)
    {
        string json = JsonUtility.ToJson(user);

        // Insert User with userid being string before @ in email.
        string userid = user.email.Substring(0, user.email.IndexOf("@"));
        dbReference.Child("users").Child(userid).SetRawJsonValueAsync(json);
    }

    public void getUser(string email, Action<User> onCallback, Action onFailed)
    {        
        dbReference.Child("users").Child(getUserID(email)).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if(task.IsCompleted)
            {
                string json = task.Result.GetRawJsonValue();
                User user = JsonUtility.FromJson<User>(json);
                onCallback(user);
                return;
            }

            onFailed();
            Debug.LogError("Something went wrong!");
        });
    }


}
