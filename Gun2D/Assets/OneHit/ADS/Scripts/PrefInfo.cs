using OneHit;
using UnityEngine;

public class PrefInfo : MonoBehaviour
{
    public static void SetTimeShowAds()
    {
        PlayerPrefs.SetString("TimeToShowAds", System.DateTime.Now.ToString());
    }

    public static System.DateTime GetLastSpinTime()
    {
        if (!PlayerPrefs.HasKey("LastSpinTime"))
        {
            SetLastSpinTime(System.DateTime.Now.AddDays(-1));
        }
        return System.DateTime.Parse(PlayerPrefs.GetString("LastSpinTime"));
    }
    public static void SetLastSpinTime(System.DateTime time)
    {
        PlayerPrefs.SetString("LastSpinTime", time.ToString());
    }
    public static void SetTotalExtraLife(int total)
    {
        PlayerPrefs.SetInt("TotalExtraLife", total);
    }
    public static int TotalExtraLife()
    {
        return PlayerPrefs.GetInt("TotalExtraLife", 5);
    }

    //public static float Distance(Vector3 a, Vector3 b)
    //{
    //    return Mathf.Sqrt((a.x - b.x)  (a.x - b.x) + (a.y - b.y)  (a.y - b.y));
    //}

    public static System.DateTime GetLastDieTime()
    {
        return System.DateTime.Parse(PlayerPrefs.GetString("LastDieTime", System.DateTime.Now.ToString()));
    }
    public static void SetLastDieTime(System.DateTime time)
    {
        PlayerPrefs.SetString("LastDieTime", time.ToString());
    }

    public static int GetPlayTime(string id = "")
    {
        return PlayerPrefs.GetInt("PlayTime" + id, 0);
    }
    public static void SetPlayTime(int c, string id = "")
    {
        PlayerPrefs.SetInt("PlayTime" + id, c);
    }
    public static void IncreasePlayTime(int am, string id = "")
    {
        PlayerPrefs.SetInt("PlayTime" + id, GetPlayTime(id) + am);
    }
    //
    public static float GetIQ()
    {
        return PlayerPrefs.GetFloat("IQ", 85);
    }
    public static void SetIQ(float c)
    {
        PlayerPrefs.SetFloat("IQ", c);
    }

    public static int GetHeart()
    {
        if (PrefInfo.IsUnlimited()) return TotalExtraLife();
        return Mathf.Max(0, PlayerPrefs.GetInt("Heart", TotalExtraLife()));
    }
    public static void SetHeart(int c)
    {
        PlayerPrefs.SetInt("Heart", c);
    }
    public static int GetSpin()
    {
        return PlayerPrefs.GetInt("Spin", 0);
    }
    public static void SetSpin(int c)
    {
        PlayerPrefs.SetInt("Spin", c);
    }
    public static int GetGem()
    {
        return PlayerPrefs.GetInt("Gem", 0);
    }
    public static void SetGem(int c)
    {
        PlayerPrefs.SetInt("Gem", c);
    }
    public static void AddGem(int c)
    {
        PlayerPrefs.SetInt("Gem", GetGem() + c);
    }

    public static bool GetItemStatus(int id)
    {
        if (id == 0) return true;
        return PlayerPrefs.GetInt("Equipment_" + id, 0) != 0;
    }
    public static void SetItemStatus(int id, bool status = true)
    {
        PlayerPrefs.SetInt("Equipment_" + id, status ? 1 : 0);
    }


    public static string GetName()
    {
        return PlayerPrefs.GetString("PlayerName", "You");
    }
    public static void SetName(string name)
    {
        PlayerPrefs.SetString("PlayerName", name);
    }
    public static bool IsUnlimited()
    {
        return PlayerPrefs.GetInt("UnlimitedHeart", 0) == 1;
    }
    public static void SetUnlimited(bool active = false)
    {
        PlayerPrefs.SetInt("UnlimitedHeart", active ? 1 : 0);
    }
    public static bool IsUsingAd()
    {
        return PlayerPrefs.GetInt("AdEnabled", 1) == 1;
    }
    public static void SetAd(bool active = false)
    {
        // lưu xem user đã mua chưa
        PlayerPrefs.SetInt("AdEnabled", active ? 1 : 0);
    }
    public static float GetSensitivity()
    {
        return PlayerPrefs.GetFloat("Sensitivity", 0.5f);
    }
    public static void SetSensitivity(float total)
    {
        PlayerPrefs.SetFloat("Sensitivity", total);
    }
    public static int GetCurrentSkin(int type)
    {

        return PlayerPrefs.GetInt("Skin" + type, 0);
    }
    public static void SetCurrentSkin(int c, int type)
    {
        PlayerPrefs.SetInt("Skin" + type, c);
    }
    public static int GetCoin()
    {
        return PlayerPrefs.GetInt("Coin", 0);
    }

    public static void SetCoin(int total)
    {
        PlayerPrefs.SetInt("Coin", total);
    }
    public static void AddCoin(int total)
    {
        PlayerPrefs.SetInt("Coin", GetCoin() + total);
    }
    //public static string GetLevelStats(int level, GameMode mode)
    //{
    //    return PlayerPrefs.GetString("Stats_" + level + "_" + mode, "0" + (level == 1 ? "1" : "0"));
    //}
    //public static void SetLevelStats(int level, GameMode mode, int totalStar, bool isUnlocked)
    //{
    //    PlayerPrefs.SetString("Stats_" + level + "_" + mode, string.Format("{0}{1}", totalStar, isUnlocked ? 1 : 0));
    //}

    //public static int GetLevelStar(int level, GameMode mode)
    //{
    //    return PlayerPrefs.GetInt("Star" + level + "_" + mode, 0);
    //}

    //public static void SetLevelStar(int level, GameMode mode, int star)
    //{
    //    if (star > PlayerPrefs.GetInt("Star" + level + "_" + mode, 0))
    //    {
    //        PlayerPrefs.SetInt("Star" + level + "_" + mode, star);
    //    }
    //}

    public static bool IsUnlocked(int id, int type)
    {
        if ((id == 0 && type == 0)) return true;
        return PlayerPrefs.GetInt("Lock_" + type + "_" + id, 0) == 0 ? false : true;
    }

    public static void SetUnlocked(int id, bool active, int type)
    {
        PlayerPrefs.SetInt("Lock_" + type + "_" + id, active ? 1 : 0);
        FirebaseManager.Instance.LogEvent("CHARACTER_UNLOCK_" + (id + 1));
    }

    public static void SetHint(int currenHint)
    {
        PlayerPrefs.SetInt("Hint", currenHint);
    }

    public static int TotalHint()
    {
        return PlayerPrefs.GetInt("Hint", 1);
    }


    private static string CurrentDailyIndexStr = "CurrentDailyIndex";
    public static void SetCurrentDailyGiftIndex()
    {
        PlayerPrefs.SetInt(CurrentDailyIndexStr, CurrentDailyIndex() + 1);
        if (CurrentDailyIndex() > 6)
        {
            if (Complete7Day() == false)
            {
                SetComplete7Day();

            }
            PlayerPrefs.SetInt(CurrentDailyIndexStr, 0);
        }
        SetDayClaimed();
    }

    private static string DayClaimed = "DayClaimed";
    public static int GetDayClaim()
    {
        return PlayerPrefs.GetInt(DayClaimed, 0);
    }
    public static void SetDayClaimed()
    {
        PlayerPrefs.SetInt(DayClaimed, GetDayClaim() + 1);
    }

    public static int CurrentDailyIndex()
    {
        return PlayerPrefs.GetInt(CurrentDailyIndexStr);
    }

    private static string Complete7DayStr = "Complete7Day";
    public static bool Complete7Day()
    {
        return PlayerPrefs.GetInt(Complete7DayStr, 0) == 0 ? false : true;
    }

    public static void SetComplete7Day()
    {
        Debug.Log("Complete 7 day");
        PlayerPrefs.SetInt(Complete7DayStr, 1);
    }

    private static string LastDayLoginStr = "LastDayLogin";
    public static void SaveTimeLastLogin(string time)
    {
        PlayerPrefs.SetString(LastDayLoginStr, time);
    }

    public static string TimeLastDayLogin()
    {
        return PlayerPrefs.GetString(LastDayLoginStr);
    }
}