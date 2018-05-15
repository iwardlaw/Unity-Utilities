using UnityEngine;
using System;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class Util {
  // Based on the Microsoft .NET method:
  // <http://referencesource.microsoft.com/#mscorlib/system/string.cs,55e241b6143365ef>
  public static bool StrIsNullOrWhiteSpace(string s)
  {
    if(s == null) return true;

    for(int i = 0; i < s.Length; ++i)
      if(!char.IsWhiteSpace(s[i])) return false;

    return true;
  }

  public static void QuitApplication(string message = "")
  {
    if(!StrIsNullOrWhiteSpace(message))
      Debug.LogError(message);
#if UNITY_EDITOR
    EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
  }

  public static void ModInc(ref int value, int modulus)
  {
    ++value;
    if(value >= modulus)
      value = 0;
  }

  public static void ModDec(ref int value, int modulus)
  {
    --value;
    if(value < 0)
      value = modulus - 1;
  }

  /// <summary>
  /// Gets the string corresponding to a Photon client state enum. If arg out of range, returns "???".
  /// </summary>
  /// <param name="state">An enumerated Photon client state (or corresponding int).</param>
  public static string ClientStateToString(int state)
  {
    switch(state) {
      case 0:  return "Uninitialized";
      case 1:  return "PeerCreated";
      case 2:  return "Queued";
      case 3:  return "Authenticated";
      case 4:  return "JoinedLobby";
      case 5:  return "DisconnectingFromMasterserver";
      case 6:  return "ConnectingToGameserver";
      case 7:  return "ConnectedToGameserver";
      case 8:  return "Joining";
      case 9:  return "Joined";
      case 10: return "Leaving";
      case 11: return "DisconnectingFromGameserver";
      case 12: return "ConnectingToMasterserver";
      case 13: return "QueuedComingFromGameserver";
      case 14: return "Disconnecting";
      case 15: return "Disconnected";
      case 16: return "ConnectedToMaster";
      case 17: return "ConnectingToNameServer";
      case 18: return "ConnectedToNameServer";
      case 19: return "DisconnectingFromNameServer";
      case 20: return "Authenticating";
      default: return "???";
    }
  }

  public static string FullArrayToString(object[] arr)
  {
    if(arr == null) { return "null"; }
    string ret = "[";
    if(arr.Length == 0)
      ret += " ";
    else {
      for(int i = 0; i < arr.Length - 1; ++i)
        ret += arr[i] + ", ";
      ret += arr[arr.Length - 1];
    }
    ret += "]";

    return ret;    
  }

  // Inspired by <https://stackoverflow.com/questions/7411438/remove-characters-from-c-sharp-string#7411472>.
  /// <summary>
  /// Removes all instances of a single character from a string.
  /// </summary>
  /// <param name="str">String from which to remove the character.</param>
  /// <param name="charToRemove">Character to remove.</param>
  /// <returns>A copy of the original string with all instances of 'charToRemove' removed.</returns>
  public static string RemoveFromString(string str, char charToRemove)
  {
    return string.Join("", str.Split(charToRemove));
  }

  // Inspired by <https://stackoverflow.com/questions/7411438/remove-characters-from-c-sharp-string#7411472>.
  /// <summary>
  /// Removes all instances of a substring removed.
  /// </summary>
  /// <param name="str">String from which to remove the substring.</param>
  /// <param name="charsToRemove">Substring of characters to remove.</param>
  /// <returns>A copy of the original string will all instances of 'charsToRemove' removed.</returns>
  public static string RemoveFromString(string str, string charsToRemove)
  {
    return string.Join("", str.Split(charsToRemove.ToCharArray()));
  }

  public static string ReplaceFirstInString(string str, string oldValue, string newValue)
  {
    int oldValueIdx = str.IndexOf(oldValue);
    if(oldValueIdx == -1) { return str; }
    return str.Substring(0, oldValueIdx) + newValue + str.Substring(oldValueIdx + oldValue.Length);    
    //return string.Join(newValue, str.Split(oldValue.ToCharArray()), str.IndexOf(oldValue), oldValue.Length);
  }

  public static string SetTextColor(string text, Color color)
  {
    return "<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">" + text + "</color>";
  }

  const BindingFlags StandardBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

  // Courtesy Julien on OverStack:
  // <https://stackoverflow.com/questions/5114469/how-to-check-whether-an-object-has-certain-method-property#5114514>
  /// <summary>
  /// Determines whether the given object contains a method with the given name.
  /// </summary>
  /// <param name="obj">Object to check for a specific method.</param>
  /// <param name="methodName">Name of the method to search for.</param>
  /// <returns>A bool indicating whether the object contains the method.</returns>
  public static bool ObjectHasMethod(object obj, string methodName)
  {
    return GetMethod(obj.GetType(), methodName) != null;
  }

  public static bool TypeHasMethod(Type type, string methodName)
  {
    return GetMethod(type, methodName) != null;
  }

  public static MethodInfo GetMethod(object obj, string methodName)
  {
    return GetMethod(obj.GetType(), methodName);
  }

  public static MethodInfo GetMethod(Type type, string methodName)
  {
    return type.GetMethod(methodName, StandardBindingFlags);
  }

  /// <summary>
  /// Absolute value of a Vector4.
  /// </summary>
  /// <param name="vec">Vector4 from which to take the absolute value.</param>
  /// <returns>A Vector4 containing the absolute value of each element of 'vec', in the same order.</returns>
  public static Vector4 Abs(Vector4 vec)
  {
    Vector4 ret = vec;
    for(int i = 0; i < 4; ++i)
      ret[i] = Mathf.Abs(vec[i]);
    return ret;
  }

  static int logLevel = 1;

  /// <summary>
  /// Prints a message to the screen if Util.logLevel is set high enough. (This member is private and must be manipulated from within Util.cs.)
  /// Differs from Util.LogBasic() in that the entire message is printed in the indicated color.
  /// </summary>
  /// <param name="textColorName">Name of the color in which to print the debug message.</param>
  /// <param name="message">Message to print to the console.</param>
  /// <param name="level">Log level; Util.logLevel must be set to at least this value for the message to print.</param>
  public static void Log(string textColor = "red", string message = "-- Debug --", int level = 1)
  {
    if(logLevel >= level)
      Debug.Log("<color=" + HexExpand(textColorName) + ">" + message + "</color>");
  }

  /// <summary>
  /// Prints a message to the screen if Util.logLevel is set high enough. (This member is private and must be manipulated from within Util.cs.)
  /// Differs from Util.Log() in that no color is assigned.
  /// </summary>
  /// <param name="message">Message to print to the console.</param>
  /// <param name="level">Log level; Util.logLevel must be set to at least this value for the message to print.</param>
  public static void LogBasic(string message = "-- Debug --", int level = 1)
  {
    if(debugLevel >= level)
      Debug.Log(message);
  }

  public static string HexExpand(string hexString)
  {
    if(hexString == "" || hexString == "red")
      return hexString;

    bool includesPound = hexString[0] == '#';
    if((includesPound && hexString.Length != 4) || (!includesPound && hexString.Length != 3))
      return hexString;

    string ret = includesPound ? "#" : "";
    int i = includesPound ? 1 : 0;
    for(/* */; i < hexString.Length; ++i) {
      ret += hexString[i];
      ret += hexString[i];
    }

    return ret;
  }

  public static void Swap<T>(ref T a, ref T b)
  {
    T tmp = a;
    a = b;
    b = tmp;
  }

  public static float MinAngleBetween(float fromAngle, float toAngle)
  {
    float angleA = toAngle - fromAngle;

    toAngle += (toAngle < fromAngle) ? 360f : -360f;
    float angleB = toAngle - fromAngle;

    return (Mathf.Abs(angleA) < Mathf.Abs(angleB)) ? angleA : angleB;
  }

  public static Vector3 MinAngleVectorBetween(Vector3 fromAngles, Vector3 toAngles)
  {
    float x = MinAngleBetween(fromAngles.x, toAngles.x);
    float y = MinAngleBetween(fromAngles.y, toAngles.y);
    float z = MinAngleBetween(fromAngles.z, toAngles.z);
    return new Vector3(x, y, z);
  }

  public static bool VectorsEqual(Vector2 a, Vector2 b, float epsilon = 0.01f)
  {
    return Mathf.Abs(a.x - b.x) <= epsilon && Mathf.Abs(a.y - b.y) <= epsilon;
  }

  public static bool VectorsEqual(Vector3 a, Vector3 b, float epsilon = 0.01f)
  {
    return Mathf.Abs(a.x - b.x) <= epsilon && Mathf.Abs(a.y - b.y) <= epsilon && Mathf.Abs(a.z - b.z) <= epsilon;
  }

  public static bool VectorsEqual(Vector4 a, Vector4 b, float epsilon = 0.01f)
  {
    return Mathf.Abs(a.x - b.x) <= epsilon && Mathf.Abs(a.y - b.y) <= epsilon && Mathf.Abs(a.z - b.z) <= epsilon && Mathf.Abs(a.w - b.w) <= epsilon;
  }

  public static float WrapMod(float a, int b)
  {
    int aInt = (int)a;
    float aDec = a - aInt;

    aInt %= b;
    if(aInt < 0) aInt += b;
    if(aInt == 0 && aDec < 0f) aInt = b;
    return aInt + aDec;
  }

  public static float Modf(float value, float modulus)
  {
    while(value < 0f)
      value += modulus;
    while(value >= modulus)
      value -= modulus;
    return value;
  }

  public static bool And(bool a, bool b) { return a && b; }
  public static bool Nand(bool a, bool b) { return !(a && b); }
  public static bool Or(bool a, bool b) { return a || b; }
  public static bool Nor(bool a, bool b) { return !(a || b); }
  public static bool Xor(bool a, bool b) { return (a && !b) || (!a && b); }
  public static bool Xnor(bool a, bool b) { return (a && b) || (!a && !b); }
}
