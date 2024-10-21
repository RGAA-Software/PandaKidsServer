namespace PandaKidsServer.Controllers;

public class ControllerError
{
    public const int Ok = 200;
    public const int ErrParamErr = 600;
    public const int ErrNoFile = 601;
    public const int ErrCopyFileFailed = 602;
    public const int ErrRecordAlreadyExist = 603;
    public const int ErrFileAlreadyExist = 604;
    public const int ErrInsertImageFailed = 605;
    public const int ErrInsertVideoFailed = 606;
    public const int ErrInsertAudioFailed = 607;
    public const int ErrDeleteFailed = 608;

    public static string Error2String(int code) {
        switch (code) {
            case Ok: return "Ok";
            case ErrParamErr: return "Error param";
            case ErrNoFile: return "No file";
            case ErrCopyFileFailed: return "Copy file failed";
            case ErrRecordAlreadyExist: return "Record already exists in db";
            case ErrFileAlreadyExist: return "File already exists";
            case ErrInsertImageFailed: return "Insert image failed";
            case ErrInsertVideoFailed: return "Insert video failed";
            case ErrInsertAudioFailed: return "Insert audio failed";
            case ErrDeleteFailed: return "Delete failed";
        }

        return "Unknown Error: " + code;
    }
}