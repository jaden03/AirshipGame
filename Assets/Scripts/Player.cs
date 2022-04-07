using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Player : MonoBehaviour
{
    Rigidbody rb;
    public float speed;
    public float jumpForce;
    float currentSpeed;
    bool grounded;
    public Image crosshair;
    public bool inShip;

    GameObject[] shipColliderObjects;
    GameObject currentShip;

    GameObject mainShipCollider;

    float maxSpeed = 10;
    float moveSpeed = 4500;

    float counterMovement = 0.175f;
    float threshold = 0.01f;

    int lastFootstepSound = 2;

    bool playingFootstepSounds;

    float horizontal;
    float vertical;

    GameObject currentPeg;
    GameObject currentLever;
    bool interacting;

    public string lookingAt;

    public LayerMask ignoreRaycast;
    public LayerMask ignoreRaycastAll;
    public LayerMask playerLayer;

    DiscordThing discord;

    GameObject fader;

    SaveManager saveManager;
    LoadManager loadManager;

    AudioSource gFootstep1;
    AudioSource gFootstep2;
    AudioSource sFootstep1;
    AudioSource sFootstep2;
    AudioSource stoneFootstep1;
    AudioSource stoneFootstep2;
    AudioSource woodFootstep1;
    AudioSource woodFootstep2;

    LineRenderer lr;
    bool holding;
    GameObject itemHeld;

    Coroutine footStepCoroutine;

    bool canGround = true;
    bool canPlayLandSound = true;

    public string itemEquipped = "None";
    string lastItemEquipped = "None";
    GameObject itemWheelCursorThing, itemWheelTop, itemWheelBottom, itemWheelLeft, itemWheelRight, itemWheel, wheelRotate;
    GameObject itemWheelTop2, itemWheelBottom2, itemWheelLeft2, itemWheelRight2, itemWheelPage1, itemWheelPage2;
    public bool inventoryOpen, inventoryUIOpen;
    bool inventoryUICan = true;
    string inventoryDirection = "None";
    string inventoryDirectionLastFrame = "None";

    public bool leftUnlocked, topUnlocked, rightUnlocked;
    public bool leftUnlocked2, topUnlocked2, rightUnlocked2;
    float itemPage = 1;

    public GameObject grappler;
    public GameObject hook;
    public GameObject grappleRopePoint;
    LineRenderer grapplerLR;

    public GameObject shovel;
    GameObject shovelHoldPoint;
    bool digging;
    Vector3 shovelHoldPointPos;
    Vector3 shovelHoldPointPosLocal;
    float shovelRotation;
    bool canDig;
    string digMaterial;
    bool canShovel = true;
    GameObject currentDigSpot;

    public GameObject rangeFinder;
    public Text rangeDistance;
    public Text rangeHeight;

    public GameObject cameraObject;
    GameObject cameraObjectObject;
    public int screenShotCounter;
    public MeshRenderer cameraRenderer;
    RenderTexture cameraRenderTexture;
    bool canTakeImage = true;
    public Camera cameraCam;
    int cameraZoom = 90;

    public GameObject hookPrefab;

    GameObject newHook;

    Vector3 grapplePoint;

    PauseMenu pauseMenu;

    GameObject airshipObject;

    GameObject holdPoint;
    Animator holdAnim;

    bool canSwitchItems = true;
    Text itemNameField;
    Text itemDescField;

    public Stopwatch stopWatch;
    public string timeElapsed;
    public int addedTime;
    Text timer;

    public int shipBlueprintAmount;

    string processedDigSpots = "";
    bool dead;

    string[] devCode;
    bool devMode;
    int codeIndex;
    float devSprintSpeed = 2500;

    GameObject hatchPos;

    Sprite[] loadingScreens;
    public Sprite loadingScreen1, loadingScreen2, loadingScreen3, loadingScreen4, loadingScreen5;

    IEnumerator footStepSound()
    {
        if (horizontal != 0 || vertical != 0)
        {
            Physics.Raycast(transform.position, -transform.up, out RaycastHit ground, 10, ~playerLayer);

            if (ground.transform && ground.transform.GetComponent<MeshRenderer>())
            {
                Renderer renderer = ground.transform.GetComponent<MeshRenderer>();
                if (renderer.material.mainTexture != null)
                {
                    Texture2D texture2D = renderer.material.mainTexture as Texture2D;

                    Vector2 pCoord = ground.textureCoord;
                    pCoord.x *= texture2D.width;
                    pCoord.y *= texture2D.height;

                    var material = getMaterialFromUV(pCoord);

                    if (material == "Wood" && ground.transform.CompareTag("Dig"))
                        material = "Grass";

                    if (grounded)
                    {
                        if (material == "Grass")
                        {
                            if (lastFootstepSound == 2)
                            {
                                gFootstep1.Play();
                                lastFootstepSound = 1;
                            }
                            else
                            {
                                gFootstep2.Play();
                                lastFootstepSound = 2;
                            }
                        }
                        else if (material == "Sand")
                        {
                            if (lastFootstepSound == 2)
                            {
                                sFootstep1.Play();
                                lastFootstepSound = 1;
                            }
                            else
                            {
                                sFootstep2.Play();
                                lastFootstepSound = 2;
                            }
                        }
                        else if (material == "Stone")
                        {
                            if (lastFootstepSound == 2)
                            {
                                stoneFootstep1.Play();
                                lastFootstepSound = 1;
                            }
                            else
                            {
                                stoneFootstep2.Play();
                                lastFootstepSound = 2;
                            }
                        }
                        else if (material == "Wood")
                        {
                            if (lastFootstepSound == 2)
                            {
                                woodFootstep1.Play();
                                lastFootstepSound = 1;
                            }
                            else
                            {
                                woodFootstep2.Play();
                                lastFootstepSound = 2;
                            }
                        }
                    }
                }
            }
        }

        if (currentSpeed != speed)
        {
            yield return new WaitForSecondsRealtime(0.3f);
            if (horizontal != 0 || vertical != 0)
                footStepCoroutine = StartCoroutine(footStepSound());
            else
                playingFootstepSounds = false;
        }
        else
        {
            yield return new WaitForSecondsRealtime(0.4f);
            if (horizontal != 0 || vertical != 0)
                footStepCoroutine = StartCoroutine(footStepSound());
            else
                playingFootstepSounds = false;
        }
    }

    IEnumerator changeScene(string sceneName)
    {
        saveManager.currentLoadingScreen = loadingScreens[Random.Range(1, 5)];
        fader.GetComponent<Image>().sprite = saveManager.currentLoadingScreen;

        stopWatch.Stop();
        LeanTween.alpha(fader.GetComponent<Image>().rectTransform, 1, 0.2f);
        LeanTween.alpha(fader.transform.Find("Text").GetComponent<Text>().rectTransform, 1, 0.2f);
        yield return new WaitForSecondsRealtime(0.2f);
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    IEnumerator canGroundCooldown()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        canGround = true;
    }

    IEnumerator landSoundDelay()
    {
        yield return new WaitForSecondsRealtime(0.25f);
        canPlayLandSound = true;
    }

    IEnumerator inventoryUICooldown()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        inventoryUICan = true;
    }

    IEnumerator setActiveAfterX(bool value, float x, GameObject _object)
    {
        yield return new WaitForSecondsRealtime(x);
        _object.SetActive(value);
    }

    IEnumerator changeItem()
    {
        canSwitchItems = false;
        holdAnim.SetBool("Equipped", false);
        yield return new WaitForSecondsRealtime(1);
        holdAnim.SetBool("Equipped", true);
        canSwitchItems = true;
    }

    IEnumerator holdAnimCooldownCompleteFunctionThingThatMakesSense()
    {
        canSwitchItems = false;
        yield return new WaitForSecondsRealtime(1);
        canSwitchItems = true;
    }

    IEnumerator dig()
    {
        digging = true;
        var position = (Camera.main.transform.forward * 0.5f) + shovelHoldPointPos;
        position = shovelHoldPoint.transform.InverseTransformPoint(position);
        LeanTween.moveLocal(shovelHoldPoint, position, 0.5f);

        yield return new WaitForSecondsRealtime(0.5f);

        if (digMaterial == "Sand")
        {
            shovel.transform.Find("sand").gameObject.SetActive(true);
            sFootstep1.Play();
        }
        if (digMaterial == "Dirt")
        {
            shovel.transform.Find("dirt").gameObject.SetActive(true);
            gFootstep1.Play();
        }
        shovelRotation = -225f;
        LeanTween.moveLocal(shovelHoldPoint, shovelHoldPointPosLocal, 0.5f);

        var name = currentDigSpot.name.Split(' ');

        if (int.Parse(name[0]) > 1)
        {
            currentDigSpot.name = (int.Parse(name[0]) - 1).ToString() + " " + name[1] + " " + name[2] + " " + name[3];
            LeanTween.scale(currentDigSpot, currentDigSpot.transform.localScale * 0.8f, 0.5f).setEaseInOutBounce();
            yield return new WaitForSecondsRealtime(0.5f);
        }
        else
        {
            currentDigSpot.name = "0 " + name[1] + " " + name[2] + " " + name[3];
            LeanTween.scale(currentDigSpot, new Vector3(0, 0, 0), 0.5f).setEaseInOutBounce();
            yield return new WaitForSecondsRealtime(0.5f);

            if (name[1] == "ship")
            {
                shipBlueprintAmount++;
                pauseMenu.StartCoroutine(pauseMenu.collectShipBlueprint(shipBlueprintAmount));

                if (shipBlueprintAmount == 3 && loadManager.saveSlot == "Blueprint")
                {
                    stopWatch.Stop();

                    string fastestTime;

                    if (File.Exists(Application.persistentDataPath + $"/speedruns_{Application.version}"))
                        fastestTime = saveManager.getSpeedruns().blueprint;
                    else
                        fastestTime = "99:99:99:999";

                    if (fastestTime == "00:00:00:000")
                        fastestTime = "99:99:99:999";

                    int hours = int.Parse(fastestTime.Split(':')[0]);
                    int minutes = int.Parse(fastestTime.Split(':')[1]);
                    int seconds = int.Parse(fastestTime.Split(':')[2]);
                    int miliseconds = int.Parse(fastestTime.Split(':')[3]);
                    System.TimeSpan ts = new System.TimeSpan(0, hours, minutes, seconds, miliseconds);

                    if (stopWatch.ElapsedMilliseconds + addedTime < ts.TotalMilliseconds)
                        saveManager.SaveSpeedrun("00:00:00:000", timeElapsed);

                    StartCoroutine(changeScene("Menu"));
                }
            }

            GameObject[] digSpots = GameObject.FindGameObjectsWithTag("Dig");
            string[] spotStrings = new string[digSpots.Length];
            foreach (GameObject spot in digSpots)
            {
                string spotDug = spot.name.Split(' ')[0];
                int spotNumber = int.Parse(spot.name.Split(' ')[2]);
                spotStrings[spotNumber] = spotDug;
            }

            foreach (string s in spotStrings)
            {
                processedDigSpots = (processedDigSpots + " " + s);
            }

            processedDigSpots = processedDigSpots.Substring(1);

            saveManager.CreateSaveData(loadManager.saveSlot, saveManager.getSaveData(loadManager.saveSlot).saveName,
                                        "World", transform.position, transform.eulerAngles.y,
                                        Camera.main.transform.eulerAngles.x, mainShipCollider.transform.position,
                                        mainShipCollider.transform.eulerAngles, itemEquipped,
                                        leftUnlocked, topUnlocked, rightUnlocked,
                                        (stopWatch.ElapsedMilliseconds + addedTime).ToString(),
                                        shipBlueprintAmount, processedDigSpots, hatchPos.transform.position,
                                        hatchPos.transform.eulerAngles.y,
                                        Camera.main.transform.GetComponent<CameraRotation>().cameraValue,
                                        leftUnlocked2, topUnlocked2, rightUnlocked2);
        }

        if (digMaterial == "Sand")
            shovel.transform.Find("sand").gameObject.SetActive(false);
        if (digMaterial == "Dirt")
            shovel.transform.Find("dirt").gameObject.SetActive(false);
        digging = false;

        yield return new WaitForSecondsRealtime(0.5f);

        canShovel = true;

        processedDigSpots = "";
    }

    IEnumerator reloadPlayerPos()
    {
        SaveManager.SaveData data = saveManager.getSaveData(loadManager.saveSlot);
        yield return new WaitForEndOfFrame();
        transform.position = new Vector3(data.playerX, data.playerY, data.playerZ);
        transform.rotation = Quaternion.Euler(0, data.playerRotY, 0);
    }

    IEnumerator flashCameraWhite()
    {
        cameraRenderer.material.color = Color.white;
        cameraRenderer.material.mainTexture = null;
        yield return new WaitForSecondsRealtime(0.5f);
        canTakeImage = true;
    }

    void Start()
    {
        devCode = new string[]
        {
            "j", "a", "d", "e", "n"
        };

        stopWatch = new Stopwatch();

        lr = GetComponent<LineRenderer>();

        saveManager = GameObject.FindGameObjectWithTag("SaveManager").GetComponent<SaveManager>();
        loadManager = GameObject.FindGameObjectWithTag("LoadManager").GetComponent<LoadManager>();

        discord = GameObject.FindGameObjectWithTag("DiscordManager").GetComponent<DiscordThing>();

        // Initialize loading screens \\

        loadingScreens = new Sprite[]
        {
            loadingScreen1, loadingScreen2, loadingScreen3, loadingScreen4, loadingScreen5
        };

        // ---------------------------- \\

        if (loadManager.saveSlot == "Shovel")
            discord.UpdateActivity("Rushing to the shovel!", "Shovel Speedrun");
        else
            discord.UpdateActivity("In Game", "Chillin");

        rb = GetComponent<Rigidbody>();
        currentSpeed = speed;

        if (SceneManager.GetActiveScene().name == "World")
        {
            shipColliderObjects = GameObject.FindGameObjectsWithTag("Ship");
            foreach (GameObject _object in shipColliderObjects)
            {
                if (_object.name == "MainShipCollision")
                {
                    mainShipCollider = _object;
                    hatchPos = mainShipCollider.GetComponent<Airship>().airshipObject.transform.Find("HatchPos").gameObject;
                }
            }
        }

        foreach (AudioSource source in GetComponents<AudioSource>())
        {
            if (source.clip.name == "GFootstep1")
            {
                source.volume = 0.25f;
                gFootstep1 = source;
            }
            if (source.clip.name == "GFootstep2")
            {
                source.volume = 0.25f;
                gFootstep2 = source;
            }
            if (source.clip.name == "SFootstep1")
            {
                source.volume = 0.25f;
                sFootstep1 = source;
            }
            if (source.clip.name == "SFootstep2")
            {
                source.volume = 0.25f;
                sFootstep2 = source;
            }
            if (source.clip.name == "StoneFootstep1")
            {
                source.volume = 0.1f;
                stoneFootstep1 = source;
            }
            if (source.clip.name == "StoneFootstep2")
            {
                source.volume = 0.1f;
                stoneFootstep2 = source;
            }
            if (source.clip.name == "WoodFootstep1")
            {
                source.volume = 0.15f;
                woodFootstep1 = source;
            }
            if (source.clip.name == "WoodFootstep2")
            {
                source.volume = 0.15f;
                woodFootstep2 = source;
            }
        }

        itemWheel = GameObject.FindGameObjectWithTag("ItemWheel");
        itemWheelCursorThing = itemWheel.transform.Find("WheelSelect").gameObject;
        itemWheelTop = itemWheel.transform.Find("1/TopOfWheel").gameObject;
        itemWheelBottom = itemWheel.transform.Find("1/BottomOfWheel").gameObject;
        itemWheelLeft = itemWheel.transform.Find("1/LeftOfWheel").gameObject;
        itemWheelRight = itemWheel.transform.Find("1/RightOfWheel").gameObject;
        wheelRotate = itemWheel.transform.Find("Rotate").gameObject;

        itemWheelPage1 = itemWheel.transform.Find("1").gameObject;
        itemWheelPage2 = itemWheel.transform.Find("2").gameObject;

        itemWheelTop2 = itemWheel.transform.Find("2/TopOfWheel").gameObject;
        itemWheelBottom2 = itemWheel.transform.Find("2/BottomOfWheel").gameObject;
        itemWheelLeft2 = itemWheel.transform.Find("2/LeftOfWheel").gameObject;
        itemWheelRight2 = itemWheel.transform.Find("2/RightOfWheel").gameObject;

        timer = GameObject.FindGameObjectWithTag("ItemWheel").transform.parent.Find("TimerPanel/Timer/Text").GetComponent<Text>();

        itemNameField = itemWheel.transform.Find("BG").Find("MidBar").Find("ItemName").GetComponent<Text>();
        itemDescField = itemWheel.transform.Find("BG").Find("MidBar").Find("ItemDesc").GetComponent<Text>();

        if (leftUnlocked)
        {
            itemWheelLeft.transform.Find("Lock").gameObject.SetActive(false);
            itemWheelLeft.transform.Find("Grappler").gameObject.SetActive(true);
        }

        if (topUnlocked)
        {
            itemWheelTop.transform.Find("Lock").gameObject.SetActive(false);
            itemWheelTop.transform.Find("Shovel").gameObject.SetActive(true);
        }

        if (rightUnlocked)
        {
            itemWheelRight.transform.Find("Lock").gameObject.SetActive(false);
            itemWheelRight.transform.Find("RangeFinder").gameObject.SetActive(true);
        }

        if (leftUnlocked2)
        {
            itemWheelLeft2.transform.Find("Lock").gameObject.SetActive(false);
            itemWheelLeft2.transform.Find("Camera").gameObject.SetActive(true);
        }

        holdPoint = Camera.main.transform.Find("HoldPoint").gameObject;
        holdAnim = holdPoint.GetComponent<Animator>();

        if (itemEquipped != "None")
        {
            //equip the item bro
            if (itemEquipped == "Grappler")
            {
                grappler.SetActive(true);
            }

            if (itemEquipped == "Shovel")
            {
                shovel.SetActive(true);
            }

            if (itemEquipped == "RangeFinder")
            {
                rangeFinder.SetActive(true);
            }

            if (itemEquipped == "Camera")
            {
                cameraObject.SetActive(true);
            }

            lastItemEquipped = itemEquipped;
            holdAnim.SetBool("Equipped", true);
        }
        else
        {
            grappler.SetActive(false);
            shovel.SetActive(false);
            rangeFinder.SetActive(false);
            cameraObject.SetActive(false);
        }

        grapplerLR = grappler.GetComponent<LineRenderer>();
        airshipObject = GameObject.FindGameObjectWithTag("Airship");
        pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu").GetComponent<PauseMenu>();
        shovelHoldPoint = shovel.transform.parent.gameObject;

        cameraObjectObject = cameraObject.transform.Find("default").gameObject;

        if (!Directory.Exists(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures) + "/WLA"))
        {
            Directory.CreateDirectory(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures) + "/WLA");
            screenShotCounter = (Directory.GetFiles(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures) + "/WLA")).Length;
        }
        else
            screenShotCounter = (Directory.GetFiles(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures) + "/WLA")).Length;

        cameraRenderTexture = new RenderTexture(1920, 1080, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
        cameraCam.targetTexture = cameraRenderTexture;

        fader = GameObject.FindGameObjectWithTag("Fader");

        StartCoroutine(reloadPlayerPos());
    }

    void Update()
    {
        // DEV MODE \\

        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(devCode[codeIndex]))
                codeIndex++;
            else
            {
                codeIndex = 0;
            }
        }

        if (codeIndex == devCode.Length)
        {
            codeIndex = 0;

            devMode = !devMode;
            rb.useGravity = !rb.useGravity;
        }

        // ---------------- \\

        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        if (horizontal != 0 || vertical != 0)
        {
            if (!playingFootstepSounds)
            {
                footStepCoroutine = StartCoroutine(footStepSound());
                playingFootstepSounds = true;
            }
        }

        if (Input.GetAxisRaw("Jump") != 0 && grounded)
        {
            grounded = false;
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            canGround = false;
            StartCoroutine(canGroundCooldown());
        }

        if (Input.GetKey(KeyCode.LeftShift))
            maxSpeed = 10;
        else
            maxSpeed = 8;


        Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 3, ~ignoreRaycastAll);

        if (hit.transform)
            lookingAt = hit.transform.tag;

        if (hit.transform && hit.transform.CompareTag("Lever") && currentPeg == null && !holding)
        {
            currentLever = hit.transform.parent.gameObject;
            crosshair.color = Color.yellow;
        }
        else if (hit.transform && hit.transform.CompareTag("Peg") && currentLever == null && !holding)
        {
            currentPeg = hit.transform.parent.gameObject;
            crosshair.color = Color.yellow;
        }
        else if (hit.transform && hit.transform.CompareTag("SceneTransitioner") && !holding)
        {
            crosshair.color = Color.green;
            if (Input.GetKeyDown(KeyCode.E))
            {
                SaveManager.SaveData data = saveManager.getSaveData(loadManager.saveSlot);

                if (SceneManager.GetActiveScene().name == "World")
                {
                    GameObject[] digSpots = GameObject.FindGameObjectsWithTag("Dig");
                    string[] spotStrings = new string[digSpots.Length];
                    foreach (GameObject spot in digSpots)
                    {
                        string spotDug = spot.name.Split(' ')[0];
                        int spotNumber = int.Parse(spot.name.Split(' ')[2]);
                        spotStrings[spotNumber] = spotDug;
                    }

                    foreach (string s in spotStrings)
                    {
                        processedDigSpots = (processedDigSpots + " " + s);
                    }

                    processedDigSpots = processedDigSpots.Substring(1);

                    saveManager.CreateSaveData(loadManager.saveSlot, data.saveName,
                                                hit.transform.name.Split(' ')[0],
                                                new Vector3(float.Parse(hit.transform.name.Split(' ')[1]),
                                                float.Parse(hit.transform.name.Split(' ')[2]),
                                                float.Parse(hit.transform.name.Split(' ')[3])),
                                                float.Parse(hit.transform.name.Split(' ')[4]), 0,
                                                mainShipCollider.transform.position,
                                                mainShipCollider.transform.eulerAngles, itemEquipped,
                                                leftUnlocked, topUnlocked, rightUnlocked,
                                                (stopWatch.ElapsedMilliseconds + addedTime).ToString(),
                                                shipBlueprintAmount, processedDigSpots, hatchPos.transform.position,
                                                hatchPos.transform.eulerAngles.y, 0, leftUnlocked2, topUnlocked2,
                                                rightUnlocked2);
                }
                else
                {
                    if (hit.transform.name != "World ShipHatch")
                    {
                        saveManager.CreateSaveData(loadManager.saveSlot, data.saveName,
                                                    hit.transform.name.Split(' ')[0],
                                                    new Vector3(float.Parse(hit.transform.name.Split(' ')[1]),
                                                    float.Parse(hit.transform.name.Split(' ')[2]),
                                                    float.Parse(hit.transform.name.Split(' ')[3])),
                                                    float.Parse(hit.transform.name.Split(' ')[4]), 0,
                                                    new Vector3(data.shipX, data.shipY, data.shipZ),
                                                    new Vector3(data.shipRotX, data.shipRotY, data.shipRotZ),
                                                    itemEquipped, leftUnlocked, topUnlocked, rightUnlocked,
                                                    (stopWatch.ElapsedMilliseconds + addedTime).ToString(),
                                                    shipBlueprintAmount, data.digSpots, new Vector3(data.hatchX,
                                                    data.hatchY, data.hatchZ), data.hatchR, 0, leftUnlocked2, topUnlocked2,
                                                    rightUnlocked2);
                    }
                    else
                    {
                        saveManager.CreateSaveData(loadManager.saveSlot, data.saveName,
                                                    "World", new Vector3(data.hatchX,
                                                    data.hatchY, data.hatchZ), data.hatchR, 0,
                                                    new Vector3(data.shipX, data.shipY, data.shipZ),
                                                    new Vector3(data.shipRotX, data.shipRotY, data.shipRotZ),
                                                    itemEquipped, leftUnlocked, topUnlocked, rightUnlocked,
                                                    (stopWatch.ElapsedMilliseconds + addedTime).ToString(),
                                                    shipBlueprintAmount, data.digSpots, new Vector3(data.hatchX,
                                                    data.hatchY, data.hatchZ), data.hatchR, 0, leftUnlocked2, topUnlocked2,
                                                    rightUnlocked2);
                    }
                }

                processedDigSpots = "";

                StartCoroutine(changeScene(hit.transform.name.Split(' ')[0]));
            }
        }
        else if (hit.transform && hit.transform.CompareTag("Holdable") && !holding)
        {
            crosshair.color = new Color(0, 0, 255);
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                holding = true;
                itemHeld = hit.transform.gameObject;
            }
        }
        else if (hit.transform && hit.transform.CompareTag("GrapplingHook") && !holding)
        {
            crosshair.color = new Color(0, 255, 0);
            if (Input.GetKeyDown(KeyCode.E))
            {
                leftUnlocked = true;
                itemEquipped = "Grappler";
                Destroy(hit.transform.gameObject);
                itemWheelLeft.transform.Find("Lock").gameObject.SetActive(false);
                itemWheelLeft.transform.Find("Grappler").gameObject.SetActive(true);
                swapItem(true, grappler, lastItemEquipped);
            }
        }
        else if (hit.transform && hit.transform.CompareTag("Shovel") && !holding)
        {
            crosshair.color = new Color(0, 255, 0);
            if (Input.GetKeyDown(KeyCode.E))
            {
                topUnlocked = true;
                itemEquipped = "Shovel";
                Destroy(hit.transform.gameObject);
                itemWheelTop.transform.Find("Lock").gameObject.SetActive(false);
                itemWheelTop.transform.Find("Shovel").gameObject.SetActive(true);
                swapItem(true, shovel, lastItemEquipped);

                if (loadManager.saveSlot == "Shovel")
                {
                    stopWatch.Stop();

                    string fastestTime;

                    if (File.Exists(Application.persistentDataPath + $"/speedruns_{Application.version}"))
                        fastestTime = saveManager.getSpeedruns().shovel;
                    else
                        fastestTime = "99:99:99:999";

                    if (fastestTime == "00:00:00:000")
                        fastestTime = "99:99:99:999";

                    int hours = int.Parse(fastestTime.Split(':')[0]);
                    int minutes = int.Parse(fastestTime.Split(':')[1]);
                    int seconds = int.Parse(fastestTime.Split(':')[2]);
                    int miliseconds = int.Parse(fastestTime.Split(':')[3]);
                    System.TimeSpan ts = new System.TimeSpan(0, hours, minutes, seconds, miliseconds);

                    if (stopWatch.ElapsedMilliseconds + addedTime < ts.TotalMilliseconds)
                        saveManager.SaveSpeedrun(timeElapsed, "00:00:00:000");

                    StartCoroutine(changeScene("Menu"));
                }
            }
        }
        else if (hit.transform && hit.transform.CompareTag("RangeFinder") && !holding)
        {
            crosshair.color = new Color(0, 255, 0);
            if (Input.GetKeyDown(KeyCode.E))
            {
                rightUnlocked = true;
                itemEquipped = "RangeFinder";
                Destroy(hit.transform.gameObject);
                itemWheelRight.transform.Find("Lock").gameObject.SetActive(false);
                itemWheelRight.transform.Find("RangeFinder").gameObject.SetActive(true);
                swapItem(true, rangeFinder, lastItemEquipped);
            }
        }
        else if (hit.transform && hit.transform.CompareTag("Camera") && !holding)
        {
            crosshair.color = new Color(0, 255, 0);
            if (Input.GetKeyDown(KeyCode.E))
            {
                leftUnlocked2 = true;
                itemEquipped = "Camera";
                Destroy(hit.transform.gameObject);
                itemWheelLeft2.transform.Find("Lock").gameObject.SetActive(false);
                itemWheelLeft2.transform.Find("Camera").gameObject.SetActive(true);
                swapItem(true, cameraObject, lastItemEquipped);
            }
        }
        else if (hit.transform && hit.transform.CompareTag("UpgradeTerminal"))
        {
            crosshair.color = new Color(0, 255, 0);
            if (Input.GetKeyDown(KeyCode.E))
            {
                hit.transform.parent.Find("Canvas/Image").GetComponent<Island7BuildingUI>().Interact(shipBlueprintAmount);
            }
        }    
        else if (hit.transform && hit.transform.CompareTag("TestTag"))
        {
            // TEST TAG \\

            crosshair.color = new Color(0, 255, 0);
            if (Input.GetKeyDown(KeyCode.E))
            {
                hit.transform.GetComponentInParent<GateTest>().swapBoolean(hit.transform.name);
            }
        }
        else if (!interacting && !holding && !canDig)
        {
            lr.enabled = false;
            currentPeg = null;
            currentLever = null;
            crosshair.color = Color.white;
        }

        if (hit.transform && hit.transform.CompareTag("Dig"))
        {
            crosshair.color = Color.yellow;
            currentDigSpot = hit.transform.gameObject;
            canDig = true;
            if (currentDigSpot.name.Split(' ')[3] == "g")
                digMaterial = "Dirt";
            if (currentDigSpot.name.Split(' ')[3] == "s")
                digMaterial = "Sand";
        }
        else
            canDig = false;

        if (holding)
        {
            crosshair.color = new Color(0, 0, 255);
            var point = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 4));
            var itemRB = itemHeld.GetComponent<Rigidbody>();
            lr.enabled = true;
            lr.SetPosition(0, point);
            lr.SetPosition(1, itemHeld.transform.position);
            var velocityModifierThing = (point - itemHeld.transform.position); ;
            itemRB.velocity = velocityModifierThing * 5f;
            if (!Input.GetKey(KeyCode.Mouse0))
            {
                holding = false;
                lr.enabled = false;
            }
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (currentLever != null)
            {
                interacting = true;

                Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hitPos, 1000, ~ignoreRaycast);

                if (hitPos.transform)
                {
                    var localHitPos = currentLever.transform.InverseTransformPoint(hitPos.point);
                    Vector3 localLeverPosition = new Vector3(0, 0, Mathf.Clamp(localHitPos.z, -0.36f, 0.06f));
                    var worldLeverPosition = currentLever.transform.TransformPoint(localLeverPosition);

                    currentLever.transform.position = worldLeverPosition;
                    if (currentLever.transform.localPosition.z < -0.36f)
                        currentLever.transform.localPosition = new Vector3(0, 0, -0.36f);
                    if (currentLever.transform.localPosition.z > 0.06f)
                        currentLever.transform.localPosition = new Vector3(0, 0, 0.06f);
                }
            }
            if (currentPeg != null)
            {
                interacting = true;

                Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hitPos, 1000, ~ignoreRaycast);

                if (hitPos.transform)
                {
                    var direction = (currentPeg.transform.position - hitPos.point).normalized;

                    currentPeg.transform.rotation = Quaternion.LookRotation(direction, -currentPeg.transform.forward);
                    currentPeg.transform.localEulerAngles = new Vector3(0, currentPeg.transform.localEulerAngles.y, 0);

                    if (currentPeg.transform.localEulerAngles.y > 85 && currentPeg.transform.localEulerAngles.y < 180)
                        currentPeg.transform.localEulerAngles = new Vector3(0, 85, 0);
                    if (currentPeg.transform.localEulerAngles.y < 275 && currentPeg.transform.localEulerAngles.y > 180)
                        currentPeg.transform.localEulerAngles = new Vector3(0, 275, 0);
                }
            }
        }

        // GRAPPLING HOOK \\

        if (itemEquipped == "Grappler")
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && !interacting && !holding && !inventoryOpen && !BBoolean.checkBoolean(pauseMenu.hasControl, "Paused"))
            {
                hook.SetActive(false);
                newHook = Instantiate(hookPrefab, hook.transform.position, hook.transform.rotation);
                newHook.GetComponentInChildren<Rigidbody>().velocity = (-newHook.transform.right * 50f);
            }
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                hook.SetActive(true);
                Destroy(newHook);
                grapplePoint = new Vector3(0, 0, 0);
            }

            if (hook.activeSelf)
            {
                var lookRot = Quaternion.Euler(0, 90, 0);
                grappler.transform.localRotation = Quaternion.Slerp(grappler.transform.localRotation, lookRot, Time.deltaTime * 5);
            }
            else
            {
                if (!Input.GetKey(KeyCode.Mouse0) && newHook != null)
                {
                    hook.SetActive(true);
                    Destroy(newHook);
                    grapplePoint = new Vector3(0, 0, 0);
                }
            }
        }
        else
        {
            grapplePoint = new Vector3(0, 0, 0);
        }

        // SHOVEL \\

        if (itemEquipped == "Shovel" && canSwitchItems)
        {
            if (!digging)
            {
                var lookRot = Quaternion.Euler(0, 0, 0);
                lookRot *= transform.rotation;
                shovel.transform.rotation = Quaternion.Slerp(shovel.transform.rotation, lookRot, Time.deltaTime * 5);

                if (Input.GetKeyDown(KeyCode.Mouse0) && canDig && canShovel)
                {
                    canDig = false;
                    canShovel = false;
                    shovelHoldPointPos = shovelHoldPoint.transform.position;
                    shovelHoldPointPosLocal = shovelHoldPoint.transform.localPosition;
                    shovelRotation = -75f;
                    StartCoroutine(dig());
                }
            }
            else
            {
                var lookRot = Quaternion.Euler(shovelRotation, 0, 0);
                if (shovelRotation == -225)
                    shovel.transform.localRotation = Quaternion.Slerp(shovel.transform.localRotation, lookRot, Time.deltaTime * 3);
                else
                    shovel.transform.localRotation = Quaternion.Slerp(shovel.transform.localRotation, lookRot, Time.deltaTime * 2);
            }
        }

        // RANGEFINDER \\

        if (itemEquipped == "RangeFinder" && canSwitchItems)
        {
            Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hitPos, 5000);

            if (hitPos.transform)
            {
                rangeDistance.text = $"{Mathf.RoundToInt(Vector3.Distance(hitPos.point, transform.position))}m";
                rangeHeight.text = $"{Mathf.RoundToInt(hitPos.point.y + 1300)}m";
            }
        }

        // CAMERA \\

        if (itemEquipped == "Camera" && canSwitchItems)
        {
            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                cameraZoom = 90;
                cameraCam.fieldOfView = cameraZoom;
            }

            if (Input.GetKey(KeyCode.Mouse1))
            {
                if (Input.GetAxisRaw("Mouse ScrollWheel") != 0)
                {
                    if (Input.GetAxisRaw("Mouse ScrollWheel") < 0 && cameraZoom + 5 < 120)
                    {
                        cameraZoom += 5;
                    }
                    if (Input.GetAxisRaw("Mouse ScrollWheel") > 0 && cameraZoom - 5 > 5)
                    {
                        cameraZoom -= 5;
                    }

                    cameraCam.fieldOfView = cameraZoom;
                }

                cameraCam.enabled = true;

                cameraObjectObject.transform.position = Vector3.Lerp(cameraObjectObject.transform.position,
                    Camera.main.ViewportToWorldPoint(new Vector3(0.44f, 0.5f, 0.65f)), Time.deltaTime * 5);

                if (Vector3.Distance(cameraObjectObject.transform.position, Camera.main.ViewportToWorldPoint(new Vector3(0.44f, 0.5f, 0.65f))) < 0.1f)
                {
                    if (canTakeImage)
                    {
                        cameraRenderer.material.mainTexture = cameraRenderTexture;
                        cameraRenderer.material.color = Color.white;
                    }

                    if (Input.GetKeyDown(KeyCode.Mouse0) && canTakeImage)
                    {
                        canTakeImage = false;

                        StartCoroutine(flashCameraWhite());

                        RenderTexture rt = RenderTexture.active;
                        RenderTexture.active = cameraCam.targetTexture;

                        cameraCam.Render();

                        Texture2D image = new Texture2D(cameraCam.targetTexture.width, cameraCam.targetTexture.height, TextureFormat.RGB24, false, true);
                        image.ReadPixels(new Rect(0, 0, cameraCam.targetTexture.width, cameraCam.targetTexture.height), 0, 0);
                        image.Apply();
                        RenderTexture.active = rt;

                        var bytes = image.EncodeToPNG();
                        Destroy(image);

                        File.WriteAllBytes(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures) + "/WLA/" + screenShotCounter + ".png", bytes);
                        screenShotCounter++;
                    }
                }
                else
                {
                    if (canTakeImage)
                    {
                        cameraRenderer.material.mainTexture = null;
                        cameraRenderer.material.color = Color.black;
                    }
                }
            }
            else
            {
                cameraCam.enabled = false;

                if (canTakeImage)
                {
                    cameraRenderer.material.mainTexture = null;
                    cameraRenderer.material.color = Color.black;
                }

                cameraObjectObject.transform.localPosition = Vector3.Lerp(cameraObjectObject.transform.localPosition,
                    Vector3.zero, Time.deltaTime * 5);
            }
        }
        else
        {
            cameraCam.enabled = false;
        }

        // If not in ship and below -1000, kill player \\
        if (transform.position.y < -750 && !inShip && !dead)
        {
            dead = true;
            SaveManager.SaveData data = saveManager.getSaveData(loadManager.saveSlot);

            GameObject[] digSpots = GameObject.FindGameObjectsWithTag("Dig");
            string[] spotStrings = new string[digSpots.Length];
            foreach (GameObject spot in digSpots)
            {
                string spotDug = spot.name.Split(' ')[0];
                int spotNumber = int.Parse(spot.name.Split(' ')[2]);
                spotStrings[spotNumber] = spotDug;
            }

            foreach (string s in spotStrings)
            {
                processedDigSpots = (processedDigSpots + " " + s);
            }

            processedDigSpots = processedDigSpots.Substring(1);

            saveManager.CreateSaveData("Dead", data.saveName,
                                        "World", mainShipCollider.transform.Find("Spawnpoint").transform.position,
                                        mainShipCollider.transform.eulerAngles.y - 180,
                                        mainShipCollider.transform.eulerAngles.x, mainShipCollider.transform.position,
                                        mainShipCollider.transform.eulerAngles, itemEquipped,
                                        leftUnlocked, topUnlocked, rightUnlocked,
                                        (stopWatch.ElapsedMilliseconds + addedTime).ToString(),
                                        shipBlueprintAmount, processedDigSpots, hatchPos.transform.position,
                                        hatchPos.transform.eulerAngles.y, 0, leftUnlocked2, topUnlocked2, rightUnlocked2);

            StartCoroutine(changeScene("Death"));
        }


        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (currentLever != null)
            {
                if (currentLever.gameObject.name == "AltitudeLever")
                {
                    LeanTween.moveLocalZ(currentLever, -0.155f, 0.1f);
                }
            }
            if (currentPeg != null)
            {
                LeanTween.rotateLocal(currentPeg, Vector3.zero, 0.1f);
            }

            interacting = false;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (canSwitchItems && !digging)
            {
                inventoryOpen = true;
                crosshair.enabled = false;
                wheelRotate.transform.localPosition = new Vector3(0, -0.1f, 0);
                itemPage = 1;
                itemWheelPage1.SetActive(true);
                itemWheelPage2.SetActive(false);
            }
        }

        if (Input.GetKeyUp(KeyCode.Tab) && inventoryOpen == true)
        {
            swapItem(false, null, lastItemEquipped);
        }

        if (inventoryOpen && !inventoryUIOpen && inventoryUICan)
        {
            LeanTween.scale(itemWheel, new Vector3(1, 1, 1), 0.2f);
            inventoryUIOpen = true;
            inventoryUICan = false;
            StartCoroutine(inventoryUICooldown());
        }
        if (!inventoryOpen && inventoryUIOpen && inventoryUICan)
        {
            LeanTween.scale(itemWheel, new Vector3(0, 0, 0), 0.2f);
            inventoryUIOpen = false;
            inventoryUICan = false;
            StartCoroutine(inventoryUICooldown());
        }

        if (inventoryOpen)
        {
            if (Input.GetAxisRaw("Mouse ScrollWheel") != 0)
            {
                if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
                {
                    if (itemPage > 1)
                        itemPage--;
                    else
                        itemPage = 2;
                }
                else
                {
                    if (itemPage < 2)
                        itemPage++;
                    else
                        itemPage = 1;
                }


                if (itemPage == 1)
                {
                    itemWheelPage1.SetActive(true);
                    itemWheelPage2.SetActive(false);
                }
                if (itemPage == 2)
                {
                    itemWheelPage1.SetActive(false);
                    itemWheelPage2.SetActive(true);
                }

                inventoryDirectionLastFrame = "ChangedPages";
            }

            var mouseX = Input.GetAxisRaw("Mouse X");
            var mouseY = Input.GetAxisRaw("Mouse Y");

            wheelRotate.transform.localPosition += new Vector3(mouseX, mouseY, 0);
            if (wheelRotate.transform.localPosition.x > 25)
                wheelRotate.transform.localPosition = new Vector3(25, wheelRotate.transform.localPosition.y);
            if (wheelRotate.transform.localPosition.x < -25)
                wheelRotate.transform.localPosition = new Vector3(-25, wheelRotate.transform.localPosition.y);
            if (wheelRotate.transform.localPosition.y > 25)
                wheelRotate.transform.localPosition = new Vector3(wheelRotate.transform.localPosition.x, 25);
            if (wheelRotate.transform.localPosition.y < -25)
                wheelRotate.transform.localPosition = new Vector3(wheelRotate.transform.localPosition.x, -25);

            var rotatePos = new Vector3(wheelRotate.transform.localPosition.x, wheelRotate.transform.localPosition.y, transform.position.z - Camera.main.transform.position.z);
            itemWheelCursorThing.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2((rotatePos.y), (rotatePos.x)) * Mathf.Rad2Deg - 90);

            var angle = itemWheelCursorThing.transform.eulerAngles.z;
            if (angle < 40 && angle > 0 || angle > 317 && angle < 360)
                inventoryDirection = "Top";
            else if (angle < 312 && angle > 228)
                inventoryDirection = "Right";
            else if (angle < 222 && angle > 137)
                inventoryDirection = "Bottom";
            else if (angle < 130 && angle > 48)
                inventoryDirection = "Left";
            else
                inventoryDirection = "None";

            if (inventoryDirection != inventoryDirectionLastFrame && itemPage == 1)
            {
                if (inventoryDirection == "Top")
                {
                    LeanTween.scale(itemWheelTop, new Vector3(1.1f, 1.1f, 1.1f), 0.1f);
                    LeanTween.scale(itemWheelBottom, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelRight, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelLeft, new Vector3(1, 1, 1), 0.1f);

                    LeanTween.scale(itemWheelTop2, new Vector3(1.1f, 1.1f, 1.1f), 0.1f);
                    LeanTween.scale(itemWheelBottom2, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelRight2, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelLeft2, new Vector3(1, 1, 1), 0.1f);
                    if (topUnlocked)
                    {
                        itemNameField.text = "Shovel";
                        itemDescField.text = "Allows you to dig up\ndig spots around the map!";
                    }
                    else
                    {
                        itemNameField.text = "Locked";
                        itemDescField.text = "Find this item\nsomewhere in the\nworld to unlock it!";
                    }
                }
                if (inventoryDirection == "Bottom")
                {
                    LeanTween.scale(itemWheelBottom, new Vector3(1.1f, 1.1f, 1.1f), 0.1f);
                    LeanTween.scale(itemWheelTop, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelRight, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelLeft, new Vector3(1, 1, 1), 0.1f);

                    LeanTween.scale(itemWheelBottom2, new Vector3(1.1f, 1.1f, 1.1f), 0.1f);
                    LeanTween.scale(itemWheelTop2, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelRight2, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelLeft2, new Vector3(1, 1, 1), 0.1f);
                    itemNameField.text = "Unequip";
                    itemDescField.text = "Unequip your current item!";
                }
                if (inventoryDirection == "Right")
                {
                    LeanTween.scale(itemWheelRight, new Vector3(1.1f, 1.1f, 1.1f), 0.1f);
                    LeanTween.scale(itemWheelBottom, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelTop, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelLeft, new Vector3(1, 1, 1), 0.1f);

                    LeanTween.scale(itemWheelRight2, new Vector3(1.1f, 1.1f, 1.1f), 0.1f);
                    LeanTween.scale(itemWheelBottom2, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelTop2, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelLeft2, new Vector3(1, 1, 1), 0.1f);
                    if (rightUnlocked)
                    {
                        itemNameField.text = "Range\nFinder";
                        itemDescField.text = "Shows the distance to\nobjects around the world!";
                    }
                    else
                    {
                        itemNameField.text = "Locked";
                        itemDescField.text = "Find this item\nsomewhere in the\nworld to unlock it!";
                    }
                }
                if (inventoryDirection == "Left")
                {
                    LeanTween.scale(itemWheelLeft, new Vector3(1.1f, 1.1f, 1.1f), 0.1f);
                    LeanTween.scale(itemWheelBottom, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelRight, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelTop, new Vector3(1, 1, 1), 0.1f);

                    LeanTween.scale(itemWheelLeft2, new Vector3(1.1f, 1.1f, 1.1f), 0.1f);
                    LeanTween.scale(itemWheelBottom2, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelRight2, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelTop2, new Vector3(1, 1, 1), 0.1f);
                    if (leftUnlocked)
                    {
                        itemNameField.text = "Grappling\nHooke";
                        itemDescField.text = "Allows you to swing\naround the world!";
                    }
                    else
                    {
                        itemNameField.text = "Locked";
                        itemDescField.text = "Find this item\nsomewhere in the\nworld to unlock it!";
                    }
                }
                if (inventoryDirection == "None")
                {
                    LeanTween.scale(itemWheelLeft, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelBottom, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelRight, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelTop, new Vector3(1, 1, 1), 0.1f);

                    LeanTween.scale(itemWheelLeft2, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelBottom2, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelRight2, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelTop2, new Vector3(1, 1, 1), 0.1f);
                    itemNameField.text = "";
                    itemDescField.text = "Nothing is selected\nhover over an item\nto view its details!";
                }
            }
            if (inventoryDirection != inventoryDirectionLastFrame && itemPage == 2)
            {
                if (inventoryDirection == "Top")
                {
                    LeanTween.scale(itemWheelTop, new Vector3(1.1f, 1.1f, 1.1f), 0.1f);
                    LeanTween.scale(itemWheelBottom, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelRight, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelLeft, new Vector3(1, 1, 1), 0.1f);

                    LeanTween.scale(itemWheelTop2, new Vector3(1.1f, 1.1f, 1.1f), 0.1f);
                    LeanTween.scale(itemWheelBottom2, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelRight2, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelLeft2, new Vector3(1, 1, 1), 0.1f);
                    if (topUnlocked2)
                    {
                        itemNameField.text = "NAME";
                        itemDescField.text = "DESC";
                    }
                    else
                    {
                        itemNameField.text = "Locked";
                        itemDescField.text = "Find this item\nsomewhere in the\nworld to unlock it!";
                    }
                }
                if (inventoryDirection == "Bottom")
                {
                    LeanTween.scale(itemWheelBottom, new Vector3(1.1f, 1.1f, 1.1f), 0.1f);
                    LeanTween.scale(itemWheelTop, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelRight, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelLeft, new Vector3(1, 1, 1), 0.1f);

                    LeanTween.scale(itemWheelBottom2, new Vector3(1.1f, 1.1f, 1.1f), 0.1f);
                    LeanTween.scale(itemWheelTop2, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelRight2, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelLeft2, new Vector3(1, 1, 1), 0.1f);
                    itemNameField.text = "Unequip";
                    itemDescField.text = "Unequip your current item!";
                }
                if (inventoryDirection == "Right")
                {
                    LeanTween.scale(itemWheelRight, new Vector3(1.1f, 1.1f, 1.1f), 0.1f);
                    LeanTween.scale(itemWheelBottom, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelTop, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelLeft, new Vector3(1, 1, 1), 0.1f);

                    LeanTween.scale(itemWheelRight2, new Vector3(1.1f, 1.1f, 1.1f), 0.1f);
                    LeanTween.scale(itemWheelBottom2, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelTop2, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelLeft2, new Vector3(1, 1, 1), 0.1f);
                    if (rightUnlocked2)
                    {
                        itemNameField.text = "NAME";
                        itemDescField.text = "DESC";
                    }
                    else
                    {
                        itemNameField.text = "Locked";
                        itemDescField.text = "Find this item\nsomewhere in the\nworld to unlock it!";
                    }
                }
                if (inventoryDirection == "Left")
                {
                    LeanTween.scale(itemWheelLeft, new Vector3(1.1f, 1.1f, 1.1f), 0.1f);
                    LeanTween.scale(itemWheelBottom, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelRight, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelTop, new Vector3(1, 1, 1), 0.1f);

                    LeanTween.scale(itemWheelLeft2, new Vector3(1.1f, 1.1f, 1.1f), 0.1f);
                    LeanTween.scale(itemWheelBottom2, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelRight2, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelTop2, new Vector3(1, 1, 1), 0.1f);
                    if (leftUnlocked2)
                    {
                        itemNameField.text = "Camera";
                        itemDescField.text = "Allows you to take\nlit pictures!";
                    }
                    else
                    {
                        itemNameField.text = "Locked";
                        itemDescField.text = "Find this item\nsomewhere in the\nworld to unlock it!";
                    }
                }
                if (inventoryDirection == "None")
                {
                    LeanTween.scale(itemWheelLeft, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelBottom, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelRight, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelTop, new Vector3(1, 1, 1), 0.1f);

                    LeanTween.scale(itemWheelLeft2, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelBottom2, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelRight2, new Vector3(1, 1, 1), 0.1f);
                    LeanTween.scale(itemWheelTop2, new Vector3(1, 1, 1), 0.1f);
                    itemNameField.text = "";
                    itemDescField.text = "Nothing is selected\nhover over an item\nto view its details!";
                }
            }

            inventoryDirectionLastFrame = inventoryDirection;
        }

        string MS = (new System.TimeSpan(0, 0, 0, 0, (int)stopWatch.ElapsedMilliseconds + addedTime).Milliseconds).ToString();
        if (MS.Length == 1)
            MS = ($"00{MS}");
        else if (MS.Length == 2)
            MS = ($"0{MS}");
        string S = (new System.TimeSpan(0, 0, 0, 0, (int)stopWatch.ElapsedMilliseconds + addedTime).Seconds).ToString();
        if (S.Length == 1)
            S = ($"0{S}");
        string M = (new System.TimeSpan(0, 0, 0, 0, (int)stopWatch.ElapsedMilliseconds + addedTime).Minutes).ToString();
        if (M.Length == 1)
            M = ($"0{M}");
        string H = (new System.TimeSpan(0, 0, 0, 0, (int)stopWatch.ElapsedMilliseconds + addedTime).Hours).ToString();
        if (H.Length == 1)
            H = ($"0{H}");
        timeElapsed = $"{H}:{M}:{S}:{MS}";
        timer.text = timeElapsed;

        // Dev Speed \\

        if (Input.GetAxisRaw("Mouse ScrollWheel") != 0)
            devSprintSpeed += Input.GetAxisRaw("Mouse ScrollWheel") * 5000;

        if (Input.GetKeyDown(KeyCode.Mouse2))
            devSprintSpeed = 2500;
    }

    void swapItem(bool pickedUp, GameObject item, string last)
    {
        inventoryOpen = false;
        crosshair.enabled = true;
        if (!pickedUp)
        {
            if (inventoryDirection == "Bottom")
            {
                itemEquipped = "None";
                StartCoroutine(setActiveAfterX(false, 1, grappler));
                StartCoroutine(setActiveAfterX(false, 1, shovel));
                StartCoroutine(setActiveAfterX(false, 1, rangeFinder));
                StartCoroutine(setActiveAfterX(false, 1, cameraObject));
            }

            // Page 1 \\

            if (inventoryDirection == "Left" && leftUnlocked && itemPage == 1)
            {
                if (itemEquipped == "None")
                {
                    itemEquipped = "Grappler";
                    grappler.SetActive(true);
                }
                else
                {
                    itemEquipped = "Grappler";
                    StartCoroutine(setActiveAfterX(true, 1, grappler));
                }
            }
            else
                StartCoroutine(setActiveAfterX(false, 1, grappler));

            if (inventoryDirection == "Top" && topUnlocked && itemPage == 1)
            {
                if (itemEquipped == "None")
                {
                    itemEquipped = "Shovel";
                    shovel.SetActive(true);
                }
                else
                {
                    itemEquipped = "Shovel";
                    StartCoroutine(setActiveAfterX(true, 1, shovel));
                }
            }
            else
                StartCoroutine(setActiveAfterX(false, 1, shovel));

            if (inventoryDirection == "Right" && rightUnlocked && itemPage == 1)
            {
                if (itemEquipped == "None")
                {
                    itemEquipped = "RangeFinder";
                    rangeFinder.SetActive(true);
                }
                else
                {
                    itemEquipped = "RangeFinder";
                    StartCoroutine(setActiveAfterX(true, 1, rangeFinder));
                }
            }
            else
                StartCoroutine(setActiveAfterX(false, 1, rangeFinder));

            // Page 2 \\

            if (inventoryDirection == "Left" && leftUnlocked2 && itemPage == 2)
            {
                if (itemEquipped == "None")
                {
                    itemEquipped = "Camera";
                    cameraObject.SetActive(true);
                }
                else
                {
                    itemEquipped = "Camera";
                    StartCoroutine(setActiveAfterX(true, 1, cameraObject));
                }
            }
            else
                StartCoroutine(setActiveAfterX(false, 1, cameraObject));
        }


        // Animation \\
        if (itemEquipped != lastItemEquipped)
        {
            if (itemEquipped == "None")
            {
                holdAnim.SetBool("Equipped", false);
                StartCoroutine(holdAnimCooldownCompleteFunctionThingThatMakesSense());
            }
            else if (lastItemEquipped == "None")
            {
                holdAnim.SetBool("Equipped", true);
                StartCoroutine(holdAnimCooldownCompleteFunctionThingThatMakesSense());
                if (pickedUp)
                {
                    item.SetActive(true);
                    if (last != "None")
                    {
                        if (last == "Grappler")
                            grappler.SetActive(false);
                        if (last == "Shovel")
                            shovel.SetActive(false);
                        if (last == "RangeFinder")
                            rangeFinder.SetActive(false);
                        if (last == "Camera")
                            cameraObject.SetActive(false);
                    }
                }
            }
            else
            {
                if (pickedUp)
                {
                    StartCoroutine(setActiveAfterX(true, 1, item));
                    if (last != "None")
                    {
                        if (last == "Grappler")
                            StartCoroutine(setActiveAfterX(false, 1, grappler));
                        if (last == "Shovel")
                            StartCoroutine(setActiveAfterX(false, 1, shovel));
                        if (last == "RangeFinder")
                            StartCoroutine(setActiveAfterX(false, 1, rangeFinder));
                        if (last == "Camera")
                            StartCoroutine(setActiveAfterX(false, 1, cameraObject));
                    }
                }
                StartCoroutine(changeItem());
            }
        }

        lastItemEquipped = itemEquipped;
    }
    private void FixedUpdate()
    {
        if (devMode == false)
        {
            //rb.AddForce(Vector3.down * Time.deltaTime * 10);
            Vector2 mag = FindVelRelativeToLook();
            float xMag = mag.x, yMag = mag.y;
            if (grounded)
            {
                if (Mathf.Abs(mag.x) > threshold && Mathf.Abs(horizontal) < 0.05f || (mag.x < -threshold && horizontal > 0) || (mag.x > threshold && horizontal < 0))
                {
                    rb.AddForce(moveSpeed * transform.right * Time.deltaTime * -mag.x * counterMovement);
                }
                if (Mathf.Abs(mag.y) > threshold && Mathf.Abs(vertical) < 0.05f || (mag.y < -threshold && vertical > 0) || (mag.y > threshold && vertical < 0))
                {
                    rb.AddForce(moveSpeed * transform.forward * Time.deltaTime * -mag.y * counterMovement);
                }
                if (Mathf.Sqrt((Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2))) > maxSpeed)
                {
                    float fallspeed = rb.velocity.y;
                    Vector3 n = rb.velocity.normalized * maxSpeed;
                    rb.velocity = new Vector3(n.x, fallspeed, n.z);
                }
            }
            if (horizontal > 0 && xMag > maxSpeed) horizontal = 0;
            if (horizontal < 0 && xMag < -maxSpeed) horizontal = 0;
            if (vertical > 0 && yMag > maxSpeed) vertical = 0;
            if (vertical < 0 && yMag < -maxSpeed) vertical = 0;

            float multiplier = 1f, multiplierV = 1f;

            if (!grounded)
            {
                multiplier = 0.5f;
                multiplierV = 0.5f;
            }

            rb.AddForce(transform.forward * vertical * moveSpeed * Time.deltaTime * multiplier * multiplierV);
            rb.AddForce(transform.right * horizontal * moveSpeed * Time.deltaTime * multiplier);
        }
        else
        {
            // Dev Flying \\

            float up_down = 0;
            if (Input.GetKey(KeyCode.LeftControl))
                up_down = -1;
            if (Input.GetKey(KeyCode.Space))
                up_down = 1;

            if (!Input.GetKey(KeyCode.LeftShift))
            {
                var vector1 = (transform.forward * vertical * 750 * Time.deltaTime);
                var vector2 = (transform.right * horizontal * 750 * Time.deltaTime);
                var vector3 = (transform.up * up_down * 750 * Time.deltaTime);
                rb.velocity = vector1 + vector2 + vector3;
            }
            else
            {
                var vector1 = (transform.forward * vertical * devSprintSpeed * Time.deltaTime);
                var vector2 = (transform.right * horizontal * devSprintSpeed * Time.deltaTime);
                var vector3 = (transform.up * up_down * devSprintSpeed * Time.deltaTime);
                rb.velocity = vector1 + vector2 + vector3;
            }
        }
    }

    private void LateUpdate()
    {
        if (itemEquipped == "Grappler")
        {
            grapplerLR.SetPosition(0, grappleRopePoint.transform.position);
            if (newHook)
            {
                grapplerLR.enabled = true;
                grapplerLR.SetPosition(1, newHook.transform.Find("default").transform.Find("Point").transform.position);
            }
            else
                grapplerLR.enabled = false;
        }
    }

    public Vector2 FindVelRelativeToLook()
    {
        float lookAngle = transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitue = rb.velocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }

    public void updateGrapplePosition(Vector3 pos)
    {
        grapplePoint = pos;

        float distanceFromPoint = Vector3.Distance(transform.position, grapplePoint);

        var force = 0.25f * distanceFromPoint;

        if (force > 2f)
            force = 2f;

        rb.AddForce((grapplePoint - transform.position) * force);

        var dir = (pos - grappler.transform.position);
        var lookRot = Quaternion.LookRotation(dir);
        lookRot *= Quaternion.Euler(0, 90, 0);
        grappler.transform.rotation = Quaternion.Slerp(grappler.transform.rotation, lookRot, Time.deltaTime * 5);
    }

    void playSingleFootstep()
    {
        playingFootstepSounds = false;
        if (footStepCoroutine != null)
            StopCoroutine(footStepCoroutine);

        Physics.Raycast(transform.position, -transform.up, out RaycastHit ground, 10, ~playerLayer);

        if (ground.transform && ground.transform.GetComponent<MeshRenderer>())
        {
            Renderer renderer = ground.transform.GetComponent<MeshRenderer>();
            if (renderer.material.mainTexture != null)
            {
                Texture2D texture2D = renderer.material.mainTexture as Texture2D;

                Vector2 pCoord = ground.textureCoord;
                pCoord.x *= texture2D.width;
                pCoord.y *= texture2D.height;

                var material = getMaterialFromUV(pCoord);

                if (material == "Wood" && ground.transform.CompareTag("Dig"))
                    material = "Grass";

                if (material == "Grass")
                {
                    if (lastFootstepSound == 2)
                    {
                        gFootstep1.Play();
                        lastFootstepSound = 1;
                    }
                    else
                    {
                        gFootstep2.Play();
                        lastFootstepSound = 2;
                    }
                }
                else if (material == "Sand")
                {
                    if (lastFootstepSound == 2)
                    {
                        sFootstep1.Play();
                        lastFootstepSound = 1;
                    }
                    else
                    {
                        sFootstep2.Play();
                        lastFootstepSound = 2;
                    }
                }
                else if (material == "Stone")
                {
                    if (lastFootstepSound == 2)
                    {
                        stoneFootstep1.Play();
                        lastFootstepSound = 1;
                    }
                    else
                    {
                        stoneFootstep2.Play();
                        lastFootstepSound = 2;
                    }
                }
                else if (material == "Wood")
                {
                    if (lastFootstepSound == 2)
                    {
                        woodFootstep1.Play();
                        lastFootstepSound = 1;
                    }
                    else
                    {
                        woodFootstep2.Play();
                        lastFootstepSound = 2;
                    }
                }
            }
        }
    }

    public void afterLoaded()
    {
        if (SceneManager.GetActiveScene().name == "World")
        {
            if (topUnlocked)
            {
                Destroy(GameObject.FindGameObjectWithTag("Shovel").transform.parent.gameObject);
            }
            if (rightUnlocked)
            {
                Destroy(GameObject.FindGameObjectWithTag("RangeFinder").transform.parent.gameObject);
            }
            if (leftUnlocked2)
            {
                Destroy(GameObject.FindGameObjectWithTag("Camera").transform.parent.gameObject);
            }
        }
        if (SceneManager.GetActiveScene().name == "Island1Dungeon")
        {
            if (leftUnlocked)
            {
                Destroy(GameObject.FindGameObjectWithTag("GrapplingHook").transform.gameObject);
            }
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if (!other.transform.CompareTag("DontJump") && !grounded && canGround)
        {
            grounded = true;
            if (canPlayLandSound)
            {
                canPlayLandSound = false;
                playSingleFootstep();
                StartCoroutine(landSoundDelay()); 
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        grounded = false;
    }

    string getMaterialFromUV(Vector2 pos)
    {
        if (pos.x < 1)
        {
            if (pos.y < 1)
                return "Wood";
            else if (pos.y < 2)
                return "Stone";
            else if (pos.y < 3)
                return "Wood";
            else
                return "Grass";
        }
        else if (pos.x < 2)
        {
            if (pos.y < 1)
                return "Red";
            else if (pos.y < 2)
                return "Blue";
            else if (pos.y < 3)
                return "Sand";
            else
                return "Grass";
        }
        else if (pos.x < 3)
        {
            if (pos.y < 1)
                return "Black";
            else if (pos.y < 2)
                return "Blue";
            else if (pos.y < 3)
            {
                // White
                return "Sand";
            }
            else
                return "Wood";
        }
        else
        {
            if (pos.y < 1)
                return "Stone";
            else if (pos.y < 2)
                return "Blue";
            else if (pos.y < 3)
                return "Stone";
            else
                return "Wood";
        }
    }
}
