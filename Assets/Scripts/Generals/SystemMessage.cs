using UnityEngine;

public static class SystemMessage
{
    public static string FileNameNotFound(string fileName)
        => $"<Error Code 0x0000cc6> FileName \"{fileName}\" Has Not Found";

    public static string ObjectNameNotFound(string fileName)
     => $"<Error Code 0x0000cc7> FileName \"{fileName}\" Has Not Found";
}
