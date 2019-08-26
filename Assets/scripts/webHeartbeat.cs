using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class webHeartbeat : MonoBehaviour
{
    public class LoginCreds
    {
        public string email;
        public string password;
    }

    public float pollingRate = 30.0f;
    float pollingThreshold;
    public string loginUrl; // "http://squidfall-connect.herokuapp.com/users/sign_in.json";
    public string pollingUrl; // "http://squidfall-connect.herokuapp.com/sites.json";
    public string email;
    public string password;
    bool loggedIn = false;

    static string BuildLoginData(string email, string password){
        return string.Format("{{ \"user\": {{ \"email\": \"{0}\", \"password\": \"{1}\" }} }}", email, password);
    }

    void resetPoll(){
        pollingThreshold = Time.time + Time.timeSinceLevelLoad;
    } //end restPoll

    // Start is called before the first frame update
    void Start(){
        resetPoll();
    } // end Start

    // Update is called once per frame
    void Update(){
        if( Time.time > pollingThreshold ){
            if( loggedIn ) {
                StartCoroutine( heartbeat() );
            } // end loggedIn
            else{
                print( "not loggedIn, logging in now" );
                StartCoroutine( logIn() );
            } // end login
        } // end time to Poll
    } // end update
    IEnumerator heartbeat(){
        UnityWebRequest www = UnityWebRequest.Get( pollingUrl ); 
        yield return www.SendWebRequest();
        if( www.isNetworkError || www.isHttpError ){
            print(www.error);
        } // end error
        else{
            print( "heartbeat beated: " + www.downloadHandler.text );
        } // end heartbeat beated
        resetPoll();
    } //end Heartbeat

    IEnumerator logIn(){
        UploadHandler uh = new UploadHandlerRaw( System.Text.Encoding.ASCII.GetBytes(BuildLoginData(email, password))){
            contentType = "application/json"
        };
        DownloadHandler dh = new DownloadHandlerBuffer();
        // Uri uri = new Uri(ApiAccess.base_uri, ApiAccess.sign_in_relative_uri);
        UnityWebRequest www = new UnityWebRequest(loginUrl, UnityWebRequest.kHttpVerbPOST, dh, uh);

        // WWWForm form = new WWWForm();
        // // log in info
        // form.AddField( "user", jsonCreds );
        // UnityWebRequest www = UnityWebRequest.Post( loginUrl, form );
        yield return www.SendWebRequest();
        if( www.isNetworkError || www.isHttpError ){
            print( www.error );
            loggedIn = false;
        } // end error
        else{
            print( "logged in: " + www.downloadHandler.text );
            loggedIn = true;
        } // end loggedInt
    } // end logIn
} // end class
