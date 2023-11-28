using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MyUtils
{
    public static string GetScreenAspectRatio()
    {
        int width = Screen.width;
        int height = Screen.height;

        Log($"Screen Width: {width}, Height: {height}");

        // Calculate the greatest common divisor (GCD) to simplify the ratio
        int gcd = GetGCD(width, height);
        Log($"GCD: {gcd}");

        // Calculate the simplified aspect ratio
        int aspectWidth = width / gcd;
        int aspectHeight = height / gcd;
        Log($"Aspect Ratio: {aspectWidth}:{aspectHeight}");

        return $"{aspectWidth}:{aspectHeight}";
    }

    private static int GetGCD(int a, int b)
    {
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

#if UNITY_EDITOR
    private static readonly string SavePath = Application.dataPath + "/_PlayerData/";
#else
    private static readonly string SavePath = Application.persistentDataPath;
#endif

    public static void SaveData(object data, string jsonFileName)
    {
        string path = Path.Combine(SavePath, $"{jsonFileName}.json");

        using StreamWriter writer = new(path);
        string dataToWrite = JsonUtility.ToJson(data);
        writer.Write(dataToWrite);

        //string jsonData = JsonUtility.ToJson(data);
        //File.WriteAllText(path, jsonData);
    }

    public static T LoadData<T>(string jsonFileName)
    {
        string path = Path.Combine(SavePath, $"{jsonFileName}.json");
        if (File.Exists(path))
        {
            using StreamReader reader = new(path);
            if (reader == null)
            {
                return default;
            }

            string dataToLoad = reader.ReadToEnd();
            return JsonUtility.FromJson<T>(dataToLoad);

            //string jsonData = File.ReadAllText(SavePath);
            //return JsonUtility.FromJson<T>(jsonData);
        }
        else
        {
            Debug.LogWarning("No saved data found.");
            T defaultData = Activator.CreateInstance<T>();
            SaveData(defaultData, jsonFileName); 
            return defaultData;
        }
    }
    public static async Task WaitForSomeTime(float duration)
    {
        await Task.Delay((int)(duration * 1000));
    }
    public static int SceneNameToBuildIndex(string sceneName)
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;

        for (int i = 0; i < sceneCount; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = Path.GetFileNameWithoutExtension(path);

            if (name == sceneName)
            {
                return i;
            }
        }

        LogError("Scene name not found in build settings: " + sceneName);
        return -1;
    }
    public static string GenerateUniqueCode(string characters, int minCodeLength, int maxCodeLength, List<string> existingCodes, Action<string> onSuccessCallback)
    {
        int length = UnityEngine.Random.Range(minCodeLength, maxCodeLength + 1);
        int possibleCombinations = Mathf.RoundToInt(Mathf.Pow(characters.Length, length));
        int totalCodeGenerated = 0;

        string code;

        bool waitingToGenerateCode = true;

        while (waitingToGenerateCode)
        {
            code = GenerateRandomCode(characters, length);

            if(existingCodes.Count > 0)
            {
                if (totalCodeGenerated < possibleCombinations)
                {
                    if (!existingCodes.Contains(code))
                    {
                        onSuccessCallback(code);
                        return code;
                    }
                    else
                    {
                        totalCodeGenerated++;
                    }
                }
                else
                    waitingToGenerateCode = false;
            }
            else
            {
                onSuccessCallback(code);
                return code;
            }
        }
        throw new Exception("Failed to generate a unique code; all possible codes are taken.");
    }

    private static string GenerateRandomCode(string characters, int length)
    {
        string code = "";

        for (int i = 0; i < length; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, characters.Length);
            code += characters[randomIndex];
        }

        return code;
    }

    public static Color HexToColor(string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out Color color))
        {
            return color;
        }
        else
        {
            MyUtils.LogWarning("Invalid hex color code: " + hex);
            return Color.white;
        }
    }
    public static GameObject GetFarthestTarget(Vector3 _origin, List<GameObject> targetList)
    {
        GameObject farthestTarget = null;
        float largestDistance = 0f;

        if (targetList.Count != 0)
        {
            foreach (GameObject _enemy in targetList)
            {
                Vector3 targetPos = new Vector3(_enemy.transform.position.x, _origin.y, _enemy.transform.position.z);
                float distance = (targetPos - _origin).sqrMagnitude;

                if (distance > largestDistance)
                {
                    farthestTarget = _enemy;
                    largestDistance = distance;
                }
            }
        }

        return farthestTarget;
    }

    public static Transform GetNearestTarget(Vector3 _origin, Transform[] targetList)
    {
        Transform nearestTarget = null;
        float smallestDistance = float.MaxValue;
        if (targetList.Length > 0)
        {
            foreach (Transform _enemy in targetList)
            {
                Vector3 targetPos = new(_enemy.position.x, _origin.y, _enemy.position.z);
                float distance = (targetPos - _origin).sqrMagnitude;
                if (distance < smallestDistance)
                {
                    nearestTarget = _enemy;
                    smallestDistance = distance;
                }
            }
        }

        return nearestTarget == null ? null : nearestTarget;
    }

    public static Quaternion LookTowards(Vector3 _origin, Vector3 _target)
    {
        Vector3 MoveDistance = _target - _origin;
        MoveDistance.y = 0f;
        Quaternion targetRotation = Quaternion.LookRotation(MoveDistance);
        return targetRotation;
    }

    public static Vector2 MapPositionInRadius(Vector3 cricketerPosition, Vector2 miniMapCenter, float miniMapRadius)
    {
        //Vector2 miniMapCenter = new Vector2(miniMapCenterX, miniMapCenterY);
        Vector2 cricketerPosition2D = new Vector2(cricketerPosition.x, cricketerPosition.z);

        Vector2 direction = cricketerPosition2D - miniMapCenter;
        float distance = direction.magnitude;

        if (distance > miniMapRadius)
        {
            direction.Normalize();
            cricketerPosition2D = miniMapCenter + direction * miniMapRadius;
        }

        return cricketerPosition2D;
    }

    public static GameObject FindChildByName(this GameObject parent, string name)
    {
        Transform[] children = parent.GetComponentsInChildren<Transform>(true);

        foreach (Transform child in children)
        {
            if (child.name == name)
            {
                return child.gameObject;
            }
        }

        return null;
    }

    public static Color SetColor(int red, int green, int blue, int alpha)
    {
        float normalizedRed = red / 255f;
        float normalizedGreen = green / 255f;
        float normalizedBlue = blue / 255f;
        float normalizedAlpha = alpha / 255f;

        return new Color(normalizedRed, normalizedGreen, normalizedBlue, normalizedAlpha);
    }

    public static void Log(object log, bool showLogOnMobile = false)
    {
        if(Application.isEditor || showLogOnMobile)
            Debug.Log($"{log}\n{StackTraceUtility.ExtractStackTrace()}");
/*#if UNITY_EDITOR
        Debug.Log(log);
#endif*/
    }
    public static void LogError(object log)
    {
#if UNITY_EDITOR
        Debug.LogError($"{log}\n{StackTraceUtility.ExtractStackTrace()}");
#endif
    }
    public static void LogWarning(object log)
    {
#if UNITY_EDITOR
        Debug.LogWarning($"{log}\n{StackTraceUtility.ExtractStackTrace()}");
#endif
    }

    public static int[] GetRandomArrayInRange(int min, int max)
    {
        int length = max - min + 1;
        int[] array = new int[length];

        // Initialize the array with the numbers in the desired range
        for (int i = 0; i < length; i++)
        {
            array[i] = min + i;
        }

        // Perform Fisher-Yates shuffle to randomize the array
        for (int i = 0; i < length - 1; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, length);
            int temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }

        return array;
    }
}
