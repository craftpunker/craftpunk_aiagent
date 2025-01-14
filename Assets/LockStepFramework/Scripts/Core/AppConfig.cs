
using static System.Net.WebRequestMethods;

public class AppConfig
{
#if UNITY_EDITOR
    //public static readonly string URL = "http://192.168.1.178:8002";
    //public static readonly string URL = "http://192.168.1.128:8002";

    
    public static readonly string URL = "http://8.138.184.211:8002";
    //public static readonly string URL = "http://43.163.239.99:8002";
    //public static readonly string URL = "https://alpha-login.craftpunk.games";

    public static readonly string Account2SessionURL = URL + "/api/account2Session";
#elif UNITY_WEBGL || UNITY_STANDALONE_WIN
    public static readonly string URL = "https://alpha-login.craftpunk.games";
    public static readonly string Account2SessionURL = "https://1252945775256932403.discordsays.com/.proxy/alpha-login/api/account2Session";
#endif

    public static string Platform = "webgl"; // discord, webgl

    public static string TermsUrl = "https://craftpunk.gitbook.io/terms";
    public static string PrivacyPolicyUrl = "https://craftpunk.gitbook.io/privacy-policy";
}