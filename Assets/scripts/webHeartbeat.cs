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

    public string loginUrl; // "http://squidfall-connect.herokuapp.com/users/sign_in.json";
    public string pollingUrl; // "http://squidfall-connect.herokuapp.com/sites.json";
    public string email;
    public string password;
    int autoTriggers;
    int motionTriggers;
    public string postUrl;

    static string BuildLoginData(string email, string password){
        return string.Format("{{ \"user\": {{ \"email\": \"{0}\", \"password\": \"{1}\" }} }}", email, password);
    }

    // Start is called before the first frame update
    void Start(){
        print( "bootin' up and loggin' in, yo" );
        StartCoroutine( logIn() );
    } // end Start

    // Update is called once per frame
    void Update(){
        /// - test by button push
        if( Input.GetKeyDown( "l" ) ){
            StartCoroutine( logIn() );
        }
        if( Input.GetKeyDown( "s" ) ){
            StartCoroutine( getSettings() );
        }
        if( Input.GetKeyDown( "r" ) ){
            StartCoroutine( sendReport() );
        }
    } // end update

    IEnumerator getSettings(){
        UnityWebRequest www = UnityWebRequest.Get( pollingUrl ); 
        yield return www.SendWebRequest();
        if( www.isNetworkError || www.isHttpError ){
            print( www.error );
        } // end error
        else{
            print( "settings received: " + www.downloadHandler.text );
        } // end heartbeat beated
    } //end Heartbeat

    IEnumerator logIn(){
        print( "logging in" );
        UploadHandler uh = new UploadHandlerRaw( System.Text.Encoding.ASCII.GetBytes(BuildLoginData(email, password))){
            contentType = "application/json"
        };
        DownloadHandler dh = new DownloadHandlerBuffer();
        UnityWebRequest www = new UnityWebRequest(loginUrl, UnityWebRequest.kHttpVerbPOST, dh, uh);
        yield return www.SendWebRequest();
        if( www.isNetworkError || www.isHttpError ){
            print( www.error );
        } // end error
        else{
            print( "logged in: " + www.downloadHandler.text );
        } // end loggedInt
    } // end logIn
    
    IEnumerator sendReport(){
        WWWForm dataToPost = new WWWForm();
        /// - temp/test  
        dataToPost.AddField("autoTriggers", 2 );
        dataToPost.AddField("motionTrigger", 3 );
        UnityWebRequest www = UnityWebRequest.Post( postUrl, dataToPost );
        yield return www.SendWebRequest();
        if( www.isNetworkError || www.isHttpError ){
            print( www.error );
        } // end error
        else{
            print( "back from POST with: " + www.downloadHandler.text );
            // clear for next report
            autoTriggers = 0;
            motionTriggers = 0;
        } // end POST
    } //end post

    void autoTrigger(){
        autoTriggers++;
    }
    
    void motionTrigger(){
        motionTriggers++;
    }
} // end class
