using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Text;
using UnityEngine.VR;

public class CameraControl : MonoBehaviour {
    public bool DEBUG = false;
    public Camera _camera;
    public GameObject previewMenuController;
    public GameObject landscape;

    #region GPS and Serial Variables
    private string DEVICE;
    private long TIME_STAMP;
    private bool STATUS;
    public float LAT;
    public float LON;
    private float ALT;
    private int SAT;
    private int PREC;
    private long CHARS;
    private int SENTENCES;
    private int CSUM_ERR;
    private float iniLAT;
    private float iniLON;
    private float iniALT;

    private int init_x = -1;
    private int init_y = -1;


    //set gimbal control
    [SerializeField]
    VRNode m_VRNode = VRNode.Head;

    // Set the COM Port (COM4) and the BAUD Rate (9600 for XBee Connection).
    public static string COMPort = "COM3";
    private static int BAUDRate = 115200;
    SerialPort streamCable = new SerialPort(COMPort, BAUDRate);

    private bool readDoneFlg;
    private bool firstGPSFlg;
    StringBuilder jsonBuffer = new StringBuilder();
    #endregion

    #region Player Control Variables
    public float speed = 6.0F;
    public float jumpSpeed = 8.0F;
    public float gravity = 20.0F;
    private Vector3 moveDirection = Vector3.zero;

    public int playerControlSpeed = 10;
    public bool playerControlFlg;
    #endregion

    // Use this for initialization
    void Start() {
        Debug.Log("Start  ");
        //start reading headset
        StartCoroutine(EndOfFrameUpdate());
        while (!streamCable.IsOpen)
        {
            streamCable.Open();
        }

        firstGPSFlg = true;
        readDoneFlg = false;
        playerControlFlg = true;
        //streamCable.DataReceived += new SerialDataReceivedEventHandler(StreamCable_DataReceived);
        streamCable.ReadTimeout = 100;
        if (!streamCable.IsOpen)
        {
            streamCable.Open(); //Open the Serial Stream.
        }
    }

    private IEnumerator EndOfFrameUpdate()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            LogRotation("EndOfFrame");
        }
    }

    private int find_diff(int new_v, int init_v)
    {
        if (new_v >= init_v)
        {
            if (new_v - init_v <= 180)
            {
                return new_v - init_v;
            }else
            {
                return 0 - ((360 - new_v) + init_v);
            }
        }else
        {
            if(new_v - init_v >= -180)
            {
                return new_v - init_v;
            }else
            {
                return (360 - init_v) + new_v;
            }
        }
    }

    private void LogRotation(string id)
    {
        var quaternion = InputTracking.GetLocalRotation(m_VRNode);
        var euler = quaternion.eulerAngles;
        if (init_x == -1 || init_y == -1)
        {
            init_x = System.Convert.ToInt32(euler.x);
            init_y = System.Convert.ToInt32(euler.y);
            return;
        }
        //Debug.Log(string.Format("{0} {1}, ({2}) Quaternion {3} Euler {4}", logPrefix, id, m_VRNode, quaternion.ToString("F2"), euler.ToString("F2")));
        int new_x = System.Convert.ToInt32(euler.x);
        int new_y = System.Convert.ToInt32(euler.y);

        if (streamCable.IsOpen)
        {
            streamCable.WriteLine('X' + (90 - find_diff(new_x, init_x)).ToString());
            streamCable.WriteLine('Y' + (90 + find_diff(new_y, init_y)).ToString());
            // Debug.Log('X' + (90 + find_diff(new_x, init_x)).ToString() + ' ' + 'Y' + (90 + find_diff(new_y, init_y)).ToString());
            // Debug.Log(euler.ToString());
        }
    }

    private void StreamCable_DataReceived(object sender, SerialDataReceivedEventArgs e) {
        if (readDoneFlg == true) {
            // wait for main program done with the current buffer
            return;
        }
        SerialPort sp = (SerialPort)sender;
        char indata = (char)sp.ReadChar();
        if (indata == '\n') {
            readDoneFlg = true;
        } else {
            if (DEBUG) {
                Debug.Log("read: " + indata);
            }

            jsonBuffer.Append(indata);
        }
    }

    // Update is called once per frame
    void Update() {
        if (!playerControlFlg) {
            if (readGPS()) {
                move();
            }
            if (Input.GetKeyDown(KeyCode.C)) {
                playerControlFlg = true;
                LAT = 0;
                LON = 0;
                iniLAT = 0;
                iniLON = 0;
            }
        } else {
            // ccmove();
            // playerControl();
            if (!previewMenuController.GetComponent<PreviewMenuController>().instantiationState() &&
                !landscape.GetComponent<Landscape>().getObjectMenuSelect())
            {
                playerControl();
            }

            if (Input.GetKeyDown(KeyCode.C)) {
                playerControlFlg = false;
            }
        }
        LogRotation("Update");
    }

    bool readGPS() {
        if (!streamCable.IsOpen) {
            Debug.Log("Serial port it not open!");
        }
        char indata;
        while (true) {
            try {
                indata = (char)streamCable.ReadChar();
                if (indata == '\n') {
                    readDoneFlg = true;
                    if (DEBUG) {
                        Debug.Log("read: " + jsonBuffer);
                    }
                    break;
                } else {

                    jsonBuffer.Append(indata);
                }
            } catch (System.Exception) {
            }
        }
        if (readDoneFlg) {

            try {
                JsonUtility.FromJsonOverwrite(jsonBuffer.ToString(), this);
                jsonBuffer.Remove(0, jsonBuffer.Length);        // clear the buffer for the next reading
                readDoneFlg = false;
                if (DEBUG) {
                    Debug.Log("device = " + this.DEVICE);
                    Debug.Log("time_stamp = " + this.TIME_STAMP);
                    Debug.Log("status = " + this.STATUS);
                    Debug.Log("lat = " + this.LAT);
                    Debug.Log("lon = " + this.LON);
                    Debug.Log("alt = " + this.ALT);
                    Debug.Log("sat = " + this.SAT);
                    Debug.Log("prec = " + this.PREC);
                }
                return true;
            } catch (System.Exception) {
                Debug.Log("fail to Json.");
                jsonBuffer.Remove(0, jsonBuffer.Length);
                readDoneFlg = false;
                throw;
            }
        } else {
            Debug.Log("No  ");
            return false;
        }
    }

    void ccmove() {
        CharacterController controller = GetComponent<CharacterController>();
        if (controller.isGrounded) {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;
            if (Input.GetButton("Jump"))
                moveDirection.y = jumpSpeed;

        }
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
    }

    void playerControl() {
        // forward/backward control
        Vector3 verticalNormalized = Vector3.Normalize(new Vector3(_camera.transform.forward.x, 0, _camera.transform.forward.z));
        Vector3 vertical = verticalNormalized * Input.GetAxis("Vertical") * playerControlSpeed * Time.deltaTime;
        transform.Translate(vertical);

        // left/right control
        Vector3 horizontalNormalized = Vector3.Normalize(new Vector3(_camera.transform.right.x, 0, _camera.transform.right.z));
        Vector3 horizontal = horizontalNormalized * Input.GetAxis("Horizontal") * playerControlSpeed * Time.deltaTime;
        transform.Translate(horizontal);
        
        // up/down control
        transform.Translate(new Vector3(0, Input.GetAxis("Xbox_R_Vertical") * playerControlSpeed * Time.deltaTime, 0));

        if (Input.GetKey(KeyCode.Q))
        {
            transform.Translate(new Vector3(0, -playerControlSpeed * Time.deltaTime, 0));
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.Translate(new Vector3(0, playerControlSpeed * Time.deltaTime, 0));
        }
    }

    void move() {
        if (firstGPSFlg == true && LON != 0 && LAT != 0 && ALT != 0) {
            iniLON = LON;
            iniLAT = LAT;
            iniALT = ALT;
            firstGPSFlg = false;
        } else if (LON != 0 && LAT != 0) {
            float la = (float)((LAT - 53.178469) / 0.00001 * 0.12179047095976932582726898256213);
            float lo = (float)((LON - 53.178469) / 0.00001 * 0.12179047095976932582726898256213);
            float ila = (float)((iniLAT - 53.178469) / 0.00001 * 0.12179047095976932582726898256213);
            float ilo = (float)((iniLON - 53.178469) / 0.00001 * 0.12179047095976932582726898256213);
            //transform.position = Quaternion.AngleAxis(LON-iniLON, -Vector3.up) * Quaternion.AngleAxis(LAT-iniLAT, -Vector3.right) * new Vector3(0, 0, 1);
            transform.position = new Vector3((lo - ilo), 0.5f, (la - ila));
        }
    }
}