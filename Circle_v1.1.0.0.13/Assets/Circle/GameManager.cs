using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [NonSerialized] public string OnlineBaseURL = "http://3.113.4.13/";


    //基本情報

    public int ProjectLocalID;


    //保存データ

    public projectsinformation PsI;
    public projectsinformation DsI;

    public projectdata PD;


    //製作用ののpublic変数

    public int Nowscene;

    public int View_SelectObject;

    public bool PW_IsSend;

    public int Nowexecutemessage;

    public int Nowprogrammingicon;

    public bool IsExecute;
    public float ExecuteAnimation;

    public GameObject EditObjectsPrefab;

    public List<Texture2D> EditCapture = new List<Texture2D>();

    public Dictionary<string, string> ProgrammingType_Lang = new Dictionary<string, string>
    {
        {"Move_Right", "右に動かす"},
        {"Move_Left", "左に動かす"},
        {"Move_Up", "上に動かす"},
        {"Move_Down", "下に動かす"},
        {"Move_Rotate", "回転"},
        {"Move_Random", "ランダムな動き"},
        {"Move_Position", "位置を指定"},
        {"Move_Angle", "角度を指定"},
        {"Text", "テキスト"},
        {"ChangeScene", "シーン変更"},
        {"Pause", "一時停止"},
        {"Tap", "押された時"},
        {"Contact", "接触した時"},
        {"Interval", "間隔調整"},
        {"Forever", "ずっと"},
    };

    public Dictionary<string, Sprite> ProgrammingIcon = new Dictionary<string, Sprite>();


    ///Characterのbyte画像

    public class imageByte
    {
        public byte[][] Character = new byte[12][];
        public byte[] Rectangle;
        public byte[] Circle;
        public byte[] Triangle;
    }

    public imageByte ImageByte = new imageByte();



    //Awake

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);


            //プロジェクト新規作成
            if (!Directory.Exists(Path(1)))
            {
                Directory.CreateDirectory(Path(1));
            }

            if (!Directory.Exists(Path(2)))
            {
                Directory.CreateDirectory(Path(2));
            }

            if (!Directory.Exists(Path(1, "Downloads")))
            {
                Directory.CreateDirectory(Path(1, "Downloads"));
            }

            if (!File.Exists(Path(2, "ProjectsInformation.json")))
            {
                PsI = JsonUtility.FromJson<projectsinformation>(Resources.Load<TextAsset>("Demo/ProjectsInformation").text);

                SaveJson(PsI);

                ProjectLocalID = 0;

                Directory.CreateDirectory(Path(3));

                PD = JsonUtility.FromJson<projectdata>(Resources.Load<TextAsset>("Demo/0/ProjectData").text);

                SaveJson();

                Directory.CreateDirectory(Path(3, "Images"));
                Directory.CreateDirectory(Path(3, "System Images"));

                for (int i = 0; i < 27; i++)
                {
                    SaveImage(Resources.Load<TextAsset>("Demo/0/Images/" + i.ToString()).bytes, Path(3, "Images", i.ToString()));
                }

                for (int i = 0; i < 7; i++)
                {
                    SaveImage(Resources.Load<TextAsset>("Demo/0/System Images/" + i.ToString()).bytes, Path(3, "System Images", i.ToString()));
                }
            }
            else
            {
                PsI = LoadJson<projectsinformation>();
            }

            if (!File.Exists(Path(1, "Downloads/ProjectsInformation.json")))
            {
                SaveJson(DsI, Path(1, "Downloads/ProjectsInformation.json"));
            }
            else
            {
                DsI = LoadJson<projectsinformation>(Path(1, "Downloads/ProjectsInformation.json"));
            }



            Android_Bar_Get();

            if (SceneManager.GetActiveScene().name != "Home")
            {
                SceneManager.LoadScene("Home");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }


    //関数


    //パス取得

    public bool Path_IsDownload;

    public string Path(int index, params string[] Hierarchy)
    {
        string FirstHalf = Application.persistentDataPath;

        for (int i = 0; i < index; i++)
        {
            switch (i)
            {
                case 0:
                    
                    FirstHalf += "/User";

                break;

                case 1:
                    
                    if (Path_IsDownload)
                    {
                        FirstHalf += "/Downloads";
                    }
                    else
                    {
                        FirstHalf += "/Projects";
                    }

                break;

                case 2:
                    
                    FirstHalf += "/" + ProjectLocalID.ToString();

                break;

                case 3:
                    
                    FirstHalf += "/ProjectData.json";

                break;
            }
        }

        return FirstHalf + (Hierarchy.Length == 0 ? "" : "/") + string.Join("/", Hierarchy);
    }


    //Jsonファイルへの保存、読み出し

    public void SaveJson(object JsonType = null, string path = "")
    {
        if (JsonType == null) JsonType = PD;

        if (path == "")
        {
            if (JsonType == PD && path == "") path = Path(4);

            if (JsonType == PsI && path == "") path = Path(2, "ProjectsInformation.json");
        }

        string datastr = JsonUtility.ToJson(JsonType, true);
        File.WriteAllText(path, datastr);
    }

    public T LoadJson<T>(string path = "")
    {
        if (typeof(T) == typeof(projectdata) && path == "") path = Path(4);

        if (typeof(T) == typeof(projectsinformation) && path == "") path = Path(2, "ProjectsInformation.json");


        string datastr = File.ReadAllText(path);

        return JsonUtility.FromJson<T>(datastr);
    }


    //オンライン

    public string OnlineDataErrorMessage;


    public IEnumerator OnlineGetData<T>(Action<float> progress, params string[] path)
    {
        var request = UnityWebRequest.Get(OnlineBaseURL + String.Join("_", path));

        var async = request.SendWebRequest();

        while (true)
        {
            if (request.result != UnityWebRequest.Result.InProgress)
            {
                switch (request.result)
                {
                    case UnityWebRequest.Result.Success:

                        OnlineDataErrorMessage = "";

                        byte[] result = request.downloadHandler.data;

                        if (typeof(T) == typeof(byte[]))
                        {
                            yield return result;
                        }
                        else
                        {
                            string text = (result == null ? "" : Encoding.UTF8.GetString(result));

                            if (typeof(T) == typeof(string))
                            {
                                yield return text;
                            }
                            else
                            {
                                yield return JsonUtility.FromJson<T>(text);
                            }
                        }

                    break;

                    case UnityWebRequest.Result.ConnectionError:

                        OnlineDataErrorMessage = "インターネット環境を確認してください。";

                    break;

                    default:

                        OnlineDataErrorMessage = "サーバーがエラーを返しました。";

                    break;
                }

                yield break;
            }

            if (progress != null) progress(request.downloadProgress);

            yield return null;
        }
    }

    public IEnumerator OnlinePostData<T>(object data, Action<float> progress, params string[] path)
    {
        byte[] prepare;

        if (typeof(T) == typeof(byte[]))
        {
            prepare = (byte[])data;
        }
        else
        {
            string text;

            if (typeof(T) == typeof(string))
            {
                text = data.ToString();
            }
            else
            {
                text = JsonUtility.ToJson(data, true);
            }

            prepare = Encoding.UTF8.GetBytes(text);
        }


        using (var request = new UnityWebRequest(OnlineBaseURL + String.Join("_", path), "POST"))
        {
            Debug.Log(OnlineBaseURL + String.Join("_", path));

            request.uploadHandler = (UploadHandler) new UploadHandlerRaw(prepare);
            request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");

            var async = request.SendWebRequest();

            while (true)
            {
                if (request.result != UnityWebRequest.Result.InProgress)
                {
                    switch (request.result)
                    {
                        case UnityWebRequest.Result.Success:

                            OnlineDataErrorMessage = "";

                        break;

                        case UnityWebRequest.Result.ConnectionError:

                            OnlineDataErrorMessage = "インターネット環境を確認してください。";

                        break;

                        default:

                            OnlineDataErrorMessage = "サーバーがエラーを返しました。";

                        break;
                    }

                    yield break;
                }

                if (progress != null) progress(request.uploadProgress);

                yield return null;
            }
        }
    }



    //Textureファイルの保存、読み出し

    public void SaveImage(Texture2D texture, string path)
    {
        byte[] Data = ConvertImageToByte(texture);

        SaveImage(Data, path);
    }

    public byte[] ConvertImageToByte(Texture2D texture)
    {
        Texture2D FormatTex = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
        FormatTex.SetPixels(texture.GetPixels());

        return FormatTex.GetRawTextureData();
    }

    public void SaveImage(byte[] bytes, string path)
    {
        using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
        {
            fs.Write(bytes, 0, bytes.Length);
        }
    }

    public byte[] LoadBytes(string path)
    {
        byte[] Data;
/*
        using (var fs = new FileStream(path, FileMode.Open))
        {
            Data = new byte[fs.Length];

            Debug.Log(fs.Length);

            fs.Read(Data, 0, Data.Length);
        }
*/
        Data = File.ReadAllBytes(path);

        return Data;
    }

    public Texture2D LoadImage(string path, int width, int height)
    {
        byte[] Data;

        Data = File.ReadAllBytes(path);

        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.LoadRawTextureData(Data);
        tex.Apply();

        return tex;
    }

    public Texture2D LoadImage(byte[] bytes, int width, int height)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.LoadRawTextureData(bytes);
        tex.Apply();

        return tex;
    }

    public IEnumerator UpdateEditCapture()
    {
        yield return new WaitForEndOfFrame();

        var tex = ScreenCapture.CaptureScreenshotAsTexture();

        float res = ((float)Screen.height / (float)Screen.width > 1.5f ? 1800f / Screen.width : 2700f / Screen.height);

        Vector2 size = (SafeAreaSize().Size - new Vector2(0, 220 + 240)) / res;

        var pixel = tex.GetPixels
        (
            (int)((SafeAreaSize().Pos.x + ScreenSize(900).x - SafeAreaSize().Size.x / 2) / res),
            (int)((SafeAreaSize().Pos.y + ScreenSize(900).y - SafeAreaSize().Size.y / 2 + 240) / res),
            (int)(size.x),
            (int)(size.y)
        );


        var NewTexture = new Texture2D((int)(size.x), (int)(size.y));

        NewTexture.SetPixels(pixel);

        NewTexture.Apply();


        if (PD.si.ToList().Contains(PD_NowS().si))
        {
            EditCapture[PD.si.ToList().IndexOf(PD_NowS().si)] = NewTexture;

            PD.SI[PD.si.ToList().IndexOf(PD_NowS().si)] = new Images{ W = (int)(size.x), H = (int)(size.y) };
        }
        else
        {
            EditCapture.Add(NewTexture);

            PD.si = PD.si.Concat(new List<int>(){ PD.si.Length == 0 ? 0 : PD.si.Max() + 1 }).ToArray();

            PD_NowS().si = PD.si.Last();

            PD.SI = PD.SI.Concat(new List<Images>(){ new Images{ W = (int)(size.x), H = (int)(size.y) } }).ToArray();
        }

        SaveJson();

        SaveImage(NewTexture, Path(3, "System Images", PD.si.ToList().IndexOf(PD_NowS().si).ToString()));
    }


    //PDの参照

    public Scene PD_NowS()
    {
        return PD.S[PD.s.ToList().IndexOf(Nowscene)];
    }
    

    public Objects PD_NowO()
    {
        if (PD_NowS().o == null || PD_NowS().o.Length == 0 || !PD_NowS().o.Contains(View_SelectObject))
        {
            return new Objects();
        }
        else
        {
            return PD_NowS().O[PD_NowS().o.ToList().IndexOf(View_SelectObject)];
        }
    }

    public ExecuteMessage PD_NowM()
    {
        if (PW_IsSend)
        {
            return PD_NowO().G.S[PD_NowO().G.s.ToList().IndexOf(Nowexecutemessage)];
        }
        else
        {
            return PD_NowO().G.R[PD_NowO().G.r.ToList().IndexOf(Nowexecutemessage)];
        }
    }

    public ProgrammingIcon PD_NowP()
    {
        return PD_NowM().P[PD_NowM().p.ToList().IndexOf(Nowprogrammingicon)];
    }


    //Jsonの色を変更
    public Color32 ColorIntToColor(ColorInt ColorInt)
    {
        return new Color32((byte)ColorInt.R, (byte)ColorInt.G, (byte)ColorInt.B, (byte)ColorInt.A);
    }


    //アニメーション
    public void Animation(ref float timer, float speed, bool situation)
    {
        if (situation)
        {
            if (timer + Time.deltaTime * speed >= 1)
            {
                timer = 1;
            }
            else
            {
                timer += Time.deltaTime * speed;
            }
        }
        else
        {
            if (timer - Time.deltaTime * speed <= 0)
            {
                timer = 0;
            }
            else
            {
                timer -= Time.deltaTime * speed;
            }
        }
    }

    public void AnimationUpOnly(ref float timer, float speed)
    {
        if (timer < 0)
        {   
            timer = 0;
        }
        
        if (timer + Time.deltaTime * speed >= 1)
        {
            timer = 1;
        }
        else
        {
            timer += Time.deltaTime * speed;
        }
    }


    //マウスの位置
    List<Vector2> LastPos = new List<Vector2>();

    public Vector2 pos(int MouseIndex = 0)
    {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR && !UNITY_WEBGL

        if (MouseIndex >= LastPos.Count)
        {
            for (int i = 0; i < MouseIndex - LastPos.Count + 1; i++)
            {
                LastPos.Add(new Vector2());
            }
        }

        if (Input.touchCount > 0)
        {
            Touch[] touch = Input.touches;

            if (Input.touchCount > MouseIndex)
            {
                LastPos[MouseIndex] = Camera.main.ScreenToWorldPoint(touch[MouseIndex].position) * 100;
            }
        }

        return LastPos[MouseIndex];

#else

        Vector3 click = Input.mousePosition;
        click.z = 10;
        if (LastPos.Count == 0)
        {
            for (int i = 0; i < MouseIndex - LastPos.Count + 1; i++)
            {
                LastPos.Add(new Vector2());
            }

            LastPos.Add(Camera.main.ScreenToWorldPoint(click) * 100);
        }
        else
        {
            LastPos[0] = Camera.main.ScreenToWorldPoint(click) * 100;
        }

        return LastPos[0];

#endif
    }

    public int posCount(int MouseButtonType = 0)
    {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR && !UNITY_WEBGL
        return Input.touchCount;
#else
        return Input.GetMouseButton(MouseButtonType) ? 1 : 0;
#endif
    }


    //Android
    [NonSerialized] public int Android_Status_Bar_Height;
    [NonSerialized] public int Android_Navigation_Bar_Height;
    [NonSerialized] public bool Android_Status_IsShow;
    [NonSerialized] public bool Android_Navigation_IsShow;


    public void Android_Bar_Get()
    {
#if UNITY_ANDROID && !UNITY_EDITOR

        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                using (var context = activity.Call<AndroidJavaObject>("getApplicationContext"))
                {
                    using (var resources = context.Call<AndroidJavaObject>("getResources"))
                    {
                        int resourceId;

                        resourceId = resources.Call<int>("getIdentifier", "status_bar_height", "dimen", "android");
                        if (resourceId > 0) Android_Status_Bar_Height = resources.Call<int>("getDimensionPixelSize", resourceId);

                        resourceId = resources.Call<int>("getIdentifier", "navigation_bar_height", "dimen", "android");
                        if (resourceId > 0) Android_Navigation_Bar_Height = resources.Call<int>("getDimensionPixelSize", resourceId);
                    }
                }
            }
        }

#endif
    }


    public void Android_Bar_Show(bool IsNoLimits, bool IsBarShow, uint StatusBarColor = 0xff000000, uint NavigationBarColor = 0xff000000, bool IsStatusDark = false, bool isNavigationDark = false)
    {

#if UNITY_ANDROID && !UNITY_EDITOR

        Screen.fullScreen = !IsNoLimits && !IsBarShow;

        using (AndroidJavaClass androidBarClass = new AndroidJavaClass("com.mycompany.androidbar.AndroidBarClass"))
        {
            androidBarClass.CallStatic("AndroidBar", IsNoLimits, IsBarShow, (int)StatusBarColor, (int)NavigationBarColor, IsStatusDark, isNavigationDark);
        }

        Android_Status_IsShow = IsBarShow;
        Android_Navigation_IsShow = IsBarShow && IsNoLimits;

#endif
    }

    public void AndroidMessage(string str)
    {
        Debug.Log(str);
    }


    //スクリーンのサイズ
    public Vector2 ScreenSize(float Times)
    {
        if ((float)Screen.height / (float)Screen.width > 1.5f)
        {
            return new Vector2(Times, (float)Screen.height / (float)Screen.width * Times);
        }
        else
        {
            return new Vector2((float)Screen.width / (float)Screen.height * Times, Times) * 1.5f;
        }
    }


    public class PosAndSize
    {
        public Vector2 Pos;
        public Vector2 Size;
    }

    public PosAndSize SafeAreaSize()
    {
        bool IsActive;

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        IsActive = true;
#else
        IsActive = (Screen.currentResolution.refreshRate == 0);
#endif

        Vector2 SafeArea_Top;
        Vector2 SafeArea_Buttom;

        if (IsActive)
        {
            var area = Screen.safeArea;
            var resolition = Screen.currentResolution;

            float yMax;
            float yMin;

#if !UNITY_EDITOR && UNITY_ANDROID

            if (Android_Status_IsShow)
            {
                yMax = 1 - ((float)Android_Status_Bar_Height / resolition.height);
            }
            else
            {
                yMax = area.yMax / resolition.height;
            }

            if (Android_Navigation_IsShow)
            {
                yMin = (float)Android_Navigation_Bar_Height / resolition.height;
            }
            else
            {
                yMin = area.yMin / resolition.height;
            }
            
#else
            yMax = area.yMax / resolition.height;
            yMin = area.yMin / resolition.height;
#endif

            SafeArea_Top = new Vector2(area.xMax / resolition.width, yMax);
            SafeArea_Buttom = new Vector2(area.xMin / resolition.width, yMin);
        }
        else
        {
            SafeArea_Top = new Vector2(1, 1);
            SafeArea_Buttom = new Vector2(0, 0);
        }

        PosAndSize Output = new PosAndSize();

        Output.Pos = ((SafeArea_Top + SafeArea_Buttom) / 2 - new Vector2(0.5f, 0.5f)) * ScreenSize(1800);

        Output.Size = (SafeArea_Top - SafeArea_Buttom) * ScreenSize(1800);

        return Output;
    }


    //編集画面

    ///Json上のオブジェクトをリアルのオブジェクトに反映させる

    public IEnumerator ObjectStartArrange(Transform EditObjectsParent, List<GameObject> UserObjects)
    {
        if (PD_NowS().o == null) PD_NowS().o = new int[]{};

        if (PD_NowS().O == null) PD_NowS().O = new Objects[]{};

        for (int i = 0; i < PD_NowS().o.Length; i++)
        {
            GameObject NewObject = Instantiate(EditObjectsPrefab, EditObjectsParent);
            
            NewObject.GetComponent<ObjectPlacement>().id = PD_NowS().o[i];

            UserObjects.Add(NewObject);

            ReflectJsonToObject(NewObject, PD_NowS().O[i]);

            if (i % 5 == 4) yield return null;
        }
    }

    public void ReflectJsonToObject(GameObject RealObject, Objects JsonObject, bool IsHeavyUpdate = true)
    {
        RealObject.name = JsonObject.N;

        RectTransform RT = RealObject.GetComponent<RectTransform>();
        
        RT.anchoredPosition = JsonObject.P;
        RT.rotation = Quaternion.Euler(0, 0, JsonObject.A);
        RT.localScale = new Vector3(Mathf.Sign(JsonObject.S.x), Mathf.Sign(JsonObject.S.y), 1);
        RT.sizeDelta = new Vector2(Mathf.Abs(JsonObject.S.x), Mathf.Abs(JsonObject.S.y));

        if (JsonObject.T == "Image")
        {
            Image Image;

            if (RealObject.GetComponent<Image>() == null)
            {
                Image = RealObject.AddComponent<Image>();
            }
            else
            {
                Image = RealObject.GetComponent<Image>();
            }

            Image.color = ColorIntToColor(JsonObject.C);
            

            if (IsHeavyUpdate)
            {
                int ImageID = PD.i.ToList().IndexOf(JsonObject.i[JsonObject.Ni]);

                Texture2D NewTexture = LoadImage(Path(3, "Images", ImageID.ToString()), PD.I[ImageID].W, PD.I[ImageID].H);
                NewTexture.wrapMode = TextureWrapMode.Clamp;

                Image.sprite = Sprite.Create(NewTexture, new Rect(0, 0, PD.I[ImageID].W, PD.I[ImageID].H), Vector2.zero);
            }
        }
        
        if (JsonObject.T == "Text")
        {
            TextMeshProUGUI Text;

            if (RealObject.GetComponent<TextMeshProUGUI>() == null)
            {
                Text = RealObject.AddComponent<TextMeshProUGUI>();
            }
            else
            {
                Text = RealObject.GetComponent<TextMeshProUGUI>();
            }

            Text.color = ColorIntToColor(JsonObject.C);

            Text.font = Resources.Load<TMP_FontAsset>("Fonts/03SmartFontUI SDF");

            Text.enableAutoSizing = true;

            Text.fontSizeMax = 720;
            Text.fontSizeMin = 1;

            Text.alignment = TextAlignmentOptions.Center;

            Text.text = JsonObject.O.S[0];
        }
    }


    void Start()
    {
        //Character画像のbyte化

        ImageByte.Character[0] = ConvertImageToByte(Resources.Load<Texture2D>("Character/S-C"));
        //ImageByte.Character[1] = GM.ConvertImageToByte(Resources.Load<Texture2D>("Character/S-L"));
        //ImageByte.Character[2] = GM.ConvertImageToByte(Resources.Load<Texture2D>("Character/S-R"));
        //ImageByte.Character[3] = GM.ConvertImageToByte(Resources.Load<Texture2D>("Character/E-C"));
        //ImageByte.Character[4] = GM.ConvertImageToByte(Resources.Load<Texture2D>("Character/E-L"));
        //ImageByte.Character[5] = GM.ConvertImageToByte(Resources.Load<Texture2D>("Character/E-R"));
        //ImageByte.Character[6] = GM.ConvertImageToByte(Resources.Load<Texture2D>("Character/W-C"));
        //ImageByte.Character[7] = GM.ConvertImageToByte(Resources.Load<Texture2D>("Character/W-L"));
        //ImageByte.Character[8] = GM.ConvertImageToByte(Resources.Load<Texture2D>("Character/W-R"));
        //ImageByte.Character[9] = GM.ConvertImageToByte(Resources.Load<Texture2D>("Character/N-C"));
        //ImageByte.Character[10] = GM.ConvertImageToByte(Resources.Load<Texture2D>("Character/N-L"));
        //ImageByte.Character[11] = GM.ConvertImageToByte(Resources.Load<Texture2D>("Character/N-R"));

        ImageByte.Rectangle = ConvertImageToByte(Resources.Load<Texture2D>("StanderedShapes/Rectangle"));
        ImageByte.Circle = ConvertImageToByte(Resources.Load<Texture2D>("StanderedShapes/Circle"));
        ImageByte.Triangle = ConvertImageToByte(Resources.Load<Texture2D>("StanderedShapes/Triangle"));


        //プログラムアイコンの画像化

        foreach (var key in ProgrammingType_Lang.Keys)
        {
            Texture2D NewTexture = LoadImage(ConvertImageToByte(Resources.Load<Texture2D>("ProgrammingIcon/" + key)), 600, 600);
            NewTexture.wrapMode = TextureWrapMode.Clamp;

            ProgrammingIcon.Add(key, Sprite.Create(NewTexture, new Rect(0, 0, NewTexture.width, NewTexture.height), Vector2.zero));
        }
    }


    void Update()
    {
        Animation(ref ExecuteAnimation, 2, IsExecute);
    }
}
