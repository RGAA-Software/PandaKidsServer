using Microsoft.AspNetCore.Mvc;

namespace PandaKidsServer.Controllers;

public class ControllerError
{
    public const int Ok = 200;
    public const int ErrParamErr = 600;
    public const int ErrNoFile = 601;
    public const int ErrCopyFileFailed = 602;

    public static string Error2String(int code)
    {
        switch (code)
        {
            case Ok: return "Ok";
            case ErrParamErr: return "Error param";
            case ErrNoFile: return "No file";
            case ErrCopyFileFailed: return "Copy file failed";
        }

        return "Unknown Error: " + code;
    }

}