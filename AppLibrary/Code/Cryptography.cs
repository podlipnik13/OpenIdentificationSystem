using System.Security.Cryptography;
using System.Text;

namespace AppLibrary.Code;

public sealed class Cryptography {
    public static string HashThis(string str){ //string result = Encoding.UTF8.GetString(encoded byte[])

        return BitConverter.ToString(SHA256.HashData(Encoding.UTF8.GetBytes(str))).Replace("-","").ToLower();
    }

    public static string StringToBase64(string str){
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
    }

    public static string Base64ToString(string str){
        return Encoding.UTF8.GetString(Convert.FromBase64String(str));
    }
}


