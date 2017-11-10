using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType { none, grass, sand, snow, cloud, preview, maxBlock };
public enum ConstructionMode { none, free, batch, batchHeight, maxMode };

public class Block {
    public BlockType _type;
    public bool _vis;
    public GameObject _block;

    public Block(BlockType type, bool vis, GameObject block) {
        _type = type;
        _vis = vis;
        _block = block;
    }
}

public class Landscape : MonoBehaviour {
    // VR Camera
    public Camera _CameraFacing;
    // x
    public static int _width = 150;
    // y
    public static int _height = 150 + 100;
    // z
    public static int _depth = 150;

    public static Camera main;

    // PerlinNoise Parameters
    public int _heightScale = 20;
    public float _detailScale = 25.0f;
    public int _heightOffset = 100;

    // GameObjects
    public GameObject _grassBlock;
    public GameObject _sandBlock;
    public GameObject _snowBlock;
    public GameObject _cloudBlock;
    public GameObject _previewBlock;
    public GameObject _presetColorPicker;
    public GameObject _modeMenu;

    // GameObjects Mapping array with their enum?

    Block[,,] worldBlocks = new Block[_width, _height, _depth];

    // Mode Variables for Creating and Deleting Blocks
    private ConstructionMode _constructionMode;

    // Batch Mode Variables
    Vector3 _batchStart;
    Vector3 _batchEnd;
    int _xStart, _xEnd;
    int _zStart, _zEnd;
    int _baseHeight;

    //Edit Mode Variables
    public GameObject objMenuPrefab;
    bool select_state = false;
    GameObject hoverGO;
    GameObject objectMenu;

    // Pre-constructed landscape
    LandscapeStructure landscapeStructure;

    // Haptics
    public AudioClip vibeClip_0_05;

    // Touch Controller
    private bool primaryIndexInUse = false;
    private bool primaryHandInUse = false;
    private bool secondaryIndexInUse = false;
    private bool secondaryHandInUse = false;

    // Change Texture/bricktype and color
    private BlockType blockType = BlockType.cloud;
    private Vector4 _color = new Vector4(1.0f, 0f, 0f, 0.55f);

    // Preview Block reference
    private GameObject singlePreviewBlock = null;

    // Use this for initialization
    void Start() {
        _constructionMode = ConstructionMode.free;
        int seed = (int)Network.time * 10;
        for (int z = 0; z < _depth; z++) {
            for (int x = 0; x < _width; x++) {
                // If you want just a flat plane
                // int y = 0;
                // If you want regular randomness:
                // int y = (int)Random.Range(0, 10);
                // If you want smooth randomness:
                int y = (int)(Mathf.PerlinNoise((x + seed) / _detailScale, (z + seed) / _detailScale) * _heightScale) + _heightOffset;
                Vector3 blockPos = new Vector3(x, y, z);

                CreateBlock(blockPos, true);
                while (y > 0) {
                    y--;
                    // blockPos = new Vector3(x, y, z);
                    blockPos.y = y;
                    CreateBlock(blockPos, false);
                }
            }
        }

        CreateCloud(20, 100);

        // Load landscapeStructure
        landscapeStructure = loadLandscapeStructure("Data/tree.data");
        // Debug.Log(tree.blockType);
    }

    // Update is called once per frame
    void Update()
    {

        #region Screen Version
        /*
        if (Input.GetMouseButtonDown(0)) {
            // Debug.Log("Left Botton Detected");
            RaycastHit hit;
            // Ray ray = Camera.allCameras[0].ScreenPointToRay(new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0));
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0));
            // Debug.Log(ray);
            if (Physics.Raycast(ray, out hit, 5000.0f)) {
                Vector3 blockPos = hit.transform.position;

                // this is the bottom block. Don't delete it!!!!
                if ((int)blockPos.y == 0) return;

                worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = null;

                Destroy(hit.transform.gameObject);

                //
                // Show hidden structure underneath
                // Only instantiate hidden blocks when they are on demand!
                //
                for (int x = -1; x <= 1; x++) {
                    for (int y = -1; y <= 1; y++) {
                        for (int z = -1; z <= 1; z++) {
                            if (!(x == 0 && y == 0 && z == 0)) {
                                Vector3 neighbour = new Vector3(blockPos.x + x, blockPos.y + y, blockPos.z + z);
                                DrawBlock(neighbour);
                            }
                        }
                    }
                }
            }
        } else if (Input.GetMouseButtonDown(1)) {
            // Debug.Log("Right Botton Detected");
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0));
            // Debug.Log(ray.direction);
            if (Physics.Raycast(ray, out hit, 5000.0f)) {
                Vector3 blockPos = hit.transform.position;
                Vector3 hitVector = blockPos - hit.point;

                hitVector.x = Mathf.Abs(hitVector.x);
                hitVector.y = Mathf.Abs(hitVector.y);
                hitVector.z = Mathf.Abs(hitVector.z);

                // Debug.Log(ray.direction);

                if (hitVector.x > hitVector.z && hitVector.x > hitVector.y) {
                    // blockPos.x -= (int)Mathf.RoundToInt(ray.direction.x);
                    if (ray.direction.x > 0) {
                        blockPos.x -= 1;
                    } else {
                        blockPos.x += 1;
                    }
                } else if (hitVector.y > hitVector.x && hitVector.y > hitVector.z) {
                    // blockPos.y -= (int)Mathf.RoundToInt(ray.direction.y);
                    if (ray.direction.y > 0) {
                        blockPos.y -= 1;
                    } else {
                        blockPos.y += 1;
                    }
                } else {
                    // blockPos.z -= (int)Mathf.RoundToInt(ray.direction.z);
                    if (ray.direction.z > 0) {
                        blockPos.z -= 1;
                    } else {
                        blockPos.z += 1;
                    }
                }


                CreateBlock((int)blockPos.y, blockPos, true);
                CheckObscuredNeighbours(blockPos);
            }
        }
        */
        #endregion

        #region VR Single Reticle Version
        /*
        if (Input.GetMouseButtonDown(0) || Input.GetButton("Xbox_L1"))
        {
            RaycastHit hit;
            // Ray ray = Camera.allCameras[0].ScreenPointToRay(new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0));
            // Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0));
            // Debug.Log(ray);
            if (Physics.Raycast(new Ray(_CameraFacing.transform.position,
                                     _CameraFacing.transform.rotation * Vector3.forward),
                                     out hit, 5000.0f))
            {
                Vector3 blockPos = hit.transform.position;

                // this is the bottom block. Don't delete it!!!!
                if ((int)blockPos.y == 0) return;

                worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = null;

                Destroy(hit.transform.gameObject);

                //
                // Show hidden structure underneath
                // Only instantiate hidden blocks when they are on demand!
                //
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        for (int z = -1; z <= 1; z++)
                        {
                            if (!(x == 0 && y == 0 && z == 0))
                            {
                                Vector3 neighbour = new Vector3(blockPos.x + x, blockPos.y + y, blockPos.z + z);
                                DrawBlock(neighbour);
                            }
                        }
                    }
                }
            }
        }
        else if (Input.GetMouseButtonDown(1) || Input.GetButton("Xbox_R1"))
        {
            // Debug.Log("Right Botton Detected");
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0));
            // Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);
            // Debug.Log(ray.direction);
            if (Physics.Raycast(new Ray(_CameraFacing.transform.position,
                                     _CameraFacing.transform.rotation * Vector3.forward),
                                     out hit, 5000.0f))
            {
                Vector3 blockPos = hit.transform.position;
                Vector3 hitVector = blockPos - hit.point;

                hitVector.x = Mathf.Abs(hitVector.x);
                hitVector.y = Mathf.Abs(hitVector.y);
                hitVector.z = Mathf.Abs(hitVector.z);

                // Debug.Log(ray.direction);

                if (hitVector.x > hitVector.z && hitVector.x > hitVector.y)
                {
                    // blockPos.x -= (int)Mathf.RoundToInt(ray.direction.x);
                    if (ray.direction.x > 0)
                    {
                        blockPos.x -= 1;
                    }
                    else
                    {
                        blockPos.x += 1;
                    }
                }
                else if (hitVector.y > hitVector.x && hitVector.y > hitVector.z)
                {
                    // blockPos.y -= (int)Mathf.RoundToInt(ray.direction.y);
                    if (ray.direction.y > 0)
                    {
                        blockPos.y -= 1;
                    }
                    else
                    {
                        blockPos.y += 1;
                    }
                }
                else
                {
                    // blockPos.z -= (int)Mathf.RoundToInt(ray.direction.z);
                    if (ray.direction.z > 0)
                    {
                        blockPos.z -= 1;
                    }
                    else
                    {
                        blockPos.z += 1;
                    }
                }


                CreateBlock((int)blockPos.y, blockPos, true);
                CheckObscuredNeighbours(blockPos);
            }
        }
        */
        #endregion

        #region VR Duo Reticle Version
        // Touch Controller Trigger States for Single Click Detection
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) == 0.0f)
        {
            primaryIndexInUse = false;
        }

        if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger) == 0.0f)
        {
            primaryHandInUse = false;
        }

        if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) == 0.0f)
        {
            secondaryIndexInUse = false;
        }

        if (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) == 0.0f)
        {
            secondaryHandInUse = false;
        }

        // Get Construction Mode
        String mode = GameObject.Find("ModeMenu").GetComponent<ShowModeMenu>().getMode();

        switch(mode)
        {
            case "Single":
                // singlePreview();
                singleMode();
                deleteMode();
                break;
            case "Batch":
                batchMode();
                deleteMode();
                break;
            case "Pre":
                preMode();
                deleteMode();
                break;
            case "Edit":
                editMode();
                break;
            default:
                break;
        }
        #endregion
    }

    private void editMode()
    {
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.5f && secondaryIndexInUse == false &&
            !_modeMenu.GetComponent<ShowModeMenu>().Enabled() && select_state == false)
        {
            if (select_state == true)
            {
                editDeleteObjectMenu();
            }

            // create a new instance
            RayCastReturn rayCastAnswer = rayCastBlockDeletion(OVRInput.Controller.RTouch);
            if (rayCastAnswer.valid)
            {
                hoverGO = rayCastAnswer.block;
                objectMenu = Instantiate(objMenuPrefab);
                objectMenu.transform.SetParent(hoverGO.transform);
                objectMenu.name = "ObjectMenu";
                objectMenu.transform.position = rayCastAnswer.blockPos;
                Renderer rend = rayCastAnswer.block.GetComponent<Renderer>();

                if (rend)
                {
                    rend.material.SetColor("_EmissionColor", Color.grey);
                }

                select_state = true;
            }

                secondaryIndexInUse = true;
        }
        else if (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) > 0.5f &&
            secondaryHandInUse == false && select_state == true)
        {
            editDeleteObjectMenu();
            secondaryHandInUse = true;
        }
        else if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick) && select_state == true)
        {
            switch (objectMenu.GetComponent<ObjMenu>().getMaterialState())
            {
                case 0:
                    hoverGO.GetComponent<Renderer>().material = _grassBlock.GetComponent<Renderer>().sharedMaterial;
                    break;
                case 1:
                    hoverGO.GetComponent<Renderer>().material = _sandBlock.GetComponent<Renderer>().sharedMaterial;
                    break;
                case 2:
                    hoverGO.GetComponent<Renderer>().material = _snowBlock.GetComponent<Renderer>().sharedMaterial;
                    break;
                case 3:
                    hoverGO.GetComponent<Renderer>().material = _cloudBlock.GetComponent<Renderer>().sharedMaterial;
                    hoverGO.GetComponent<Renderer>().material.color = _color;
                    break;
                default:
                    break;
            }
            editDeleteObjectMenu();
            secondaryHandInUse = true;
        }
    }

    private void editDeleteObjectMenu()
    {
        Destroy(hoverGO.transform.FindChild("ObjectMenu").gameObject);
        Renderer rend = hoverGO.transform.GetComponent<Renderer>();
        rend.material.SetColor("_EmissionColor", Color.black);
        hoverGO = null;
        select_state = false;
    }

    /*
        private void singlePreview()
        {
            RayCastReturn rayCastAnswer = rayCastBlockCreation(OVRInput.Controller.RTouch);
            if (rayCastAnswer.valid)
            {
                Vector3 blockPos = rayCastAnswer.blockPos;
                // Overflow prevention and clean up
                if (blockPos.x < 0 || blockPos.x >= _width || blockPos.y < 0 || blockPos.y >= _height || blockPos.z < 0 || blockPos.x >= _depth) return;
                if (worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] != null &&
                    worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z]._block == singlePreviewBlock)
                {
                    return;
                }
                else if (worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] == null)
                {
                    // delete the previous singlePreviewBlock
                    if (singlePreviewBlock != null)
                    {
                        Destroy(singlePreviewBlock);
                    }

                    CreateCustomBlock(blockType, blockPos, true, false);
                    singlePreviewBlock = worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z]._block;
                    singlePreviewBlock.transform.GetComponent<BoxCollider>().enabled = false;
                    Renderer rend = singlePreviewBlock.transform.GetComponent<Renderer>();
                    rend.material.SetColor("_EmissionColor", Color.grey);
                }
            }
        }
    */

    private void preMode()
    {
/*
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) > 0.5f &&
            (primaryIndexInUse == false || OVRInput.Get(OVRInput.Touch.PrimaryThumbRest)))
        {
            RayCastReturn rayCastAnswer = rayCastBlockDeletion(OVRInput.Controller.LTouch);
            if (rayCastAnswer.valid)
            {
                Vector3 baseIndex = rayCastAnswer.blockPos - landscapeStructure.center;
                for (int i = 0; i < landscapeStructure.blockType.GetLength(0); i++)
                {
                    for (int j = 0; j < landscapeStructure.blockType.GetLength(1); j++)
                    {
                        for (int k = 0; k < landscapeStructure.blockType.GetLength(2); k++)
                        {
                            Vector3 curIndex = new Vector3(baseIndex.x + i, baseIndex.y + j, baseIndex.z + k);
                            CreateCustomBlock(landscapeStructure.blockType[i, j, k], curIndex, true, false);
                        }
                    }
                }

                if (primaryIndexInUse == false)
                {
                    primaryIndexInUse = true;
                }

                // rayCastAnswer.blockPos;
                // Debug.Log("batchStart: " + _batchStart);
                // Debug.Log("batchEnd: " + _batchEnd);
            }
        }
*/
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.5f &&
            (secondaryIndexInUse == false || OVRInput.Get(OVRInput.Touch.SecondaryThumbRest)))
        {
            RayCastReturn rayCastAnswer = rayCastBlockDeletion(OVRInput.Controller.RTouch);
            if (rayCastAnswer.valid)
            {
                OVRHaptics.Channels[1].Mix(new OVRHapticsClip(vibeClip_0_05));
                Vector3 baseIndex = rayCastAnswer.blockPos - landscapeStructure.center;
                for (int i = 0; i < landscapeStructure.blockType.GetLength(0); i++)
                {
                    for (int j = 0; j < landscapeStructure.blockType.GetLength(1); j++)
                    {
                        for (int k = 0; k < landscapeStructure.blockType.GetLength(2); k++)
                        {
                            Vector3 curIndex = new Vector3(baseIndex.x + i, baseIndex.y + j, baseIndex.z + k);
                            CreateCustomBlock(landscapeStructure.blockType[i, j, k], curIndex, true, false);
                        }
                    }
                }

                secondaryIndexInUse = true;

                // rayCastAnswer.blockPos;
                // Debug.Log("batchStart: " + _batchStart);
                // Debug.Log("batchEnd: " + _batchEnd);
            }
        }
    }

    private void batchMode()
    {
        if (_constructionMode == ConstructionMode.free && OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.5f && secondaryIndexInUse == false)
        {
            // Switch mode to batch
            _constructionMode = ConstructionMode.batch;
            // ray cast to obtain the starting block
            RayCastReturn rayCastAnswer = rayCastBlockDeletion(OVRInput.Controller.RTouch);
            if (rayCastAnswer.valid)
            {
                OVRHaptics.Channels[1].Mix(new OVRHapticsClip(vibeClip_0_05));
                _batchStart = rayCastAnswer.blockPos;
                _batchEnd = _batchStart;
                // Debug.Log("batchStart: " + _batchStart);
                // Debug.Log("batchEnd: " + _batchEnd);
            }
        }
        else if (_constructionMode == ConstructionMode.batch && OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.5f)
        {
            // ray cast to update the ending block
            RayCastReturn rayCastAnswer = rayCastBlockDeletion(OVRInput.Controller.RTouch);
            if (rayCastAnswer.valid)
            {
                _batchEnd = rayCastAnswer.blockPos;
                // Debug.Log("batchEnd: " + _batchEnd);
            }
        }
        else if (_constructionMode == ConstructionMode.batch && OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) < 0.5f)
        {
            // Switch to batchHeight mode for height adjustment when button B is released
            _constructionMode = ConstructionMode.batchHeight;

            OVRHaptics.Channels[1].Mix(new OVRHapticsClip(vibeClip_0_05));

            // Create a rectengular base
            // Get the heigher among the two points and match all the points with that height if possible.
            _baseHeight = (int)Math.Max(_batchStart.y, _batchEnd.y);

            // determine the right incremental starting and ending point
            if (_batchEnd.x - _batchStart.x >= 0)
            {
                _xStart = (int)_batchStart.x;
                _xEnd = (int)_batchEnd.x;
            }
            else
            {
                _xEnd = (int)_batchStart.x;
                _xStart = (int)_batchEnd.x;
            }

            if (_batchEnd.z - _batchStart.z >= 0)
            {
                _zStart = (int)_batchStart.z;
                _zEnd = (int)_batchEnd.z;
            }
            else
            {
                _zEnd = (int)_batchStart.z;
                _zStart = (int)_batchEnd.z;
            }

            // iterate through the book keeping structure
            // Start actually creating blocks when hits the surface
            bool surfaceFlag = false;
            for (int x = _xStart; x <= _xEnd; x++)
            {
                for (int z = _zStart; z <= _zEnd; z++)
                {
                    // Create Actual Blocks for the structure
                    surfaceFlag = false;
                    for (int y = 0; y <= _baseHeight; y++)
                    {
                        if (!surfaceFlag && worldBlocks[x, y, z] != null && worldBlocks[x, y, z]._vis)
                        {
                            surfaceFlag = true;
                        }
                        else if (surfaceFlag)
                        {
                            Vector3 blockPos = new Vector3(x, y, z);
                            CreateCustomBlock(blockType, blockPos, true);
                            // CreateBlock(blockPos, true);
                            // Hide obscured neighbours
                            CheckObscuredNeighbours(blockPos);
                        }
                    }

                    // Create Preview Blocks for batchHeight Mode
                    for (int y = _baseHeight + 1; y <= _height - 50; y++)
                    {
                        Vector3 blockPos = new Vector3(x, y, z);
                        CreateCustomBlock(BlockType.preview, blockPos, true);
                        // Hide obscured neighbours
                        CheckObscuredNeighbours(blockPos);
                    }
                }
            }

            // Create Preview Blocks up to _height - 3 in order to be separated from the cloud.

        }
        else if (_constructionMode == ConstructionMode.batchHeight && OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.5f)
        {
            // Switch back to free mode since this is the last step of batch Mode
            _constructionMode = ConstructionMode.free;

            // Finish Creating the block + clean up preview blocks
            // ray cast to get the intended height of the geometric structure
            RayCastReturn rayCastAnswer = rayCastBlockDeletion(OVRInput.Controller.RTouch);
            Vector3 heightPos = Vector3.zero;
            if (rayCastAnswer.valid)
            {
                OVRHaptics.Channels[1].Mix(new OVRHapticsClip(vibeClip_0_05));
                heightPos = rayCastAnswer.blockPos;
                // Debug.Log("Intended Height: " + _batchEnd);
            }
            int intendedHeight = (int)Math.Max(heightPos.y, _baseHeight);
            for (int x = _xStart; x <= _xEnd; x++)
            {
                for (int z = _zStart; z <= _zEnd; z++)
                {
                    // Create Actual Blocks for the structure up to the intended height
                    for (int y = _baseHeight + 1; y <= intendedHeight; y++)
                    {
                        Vector3 blockPos = new Vector3(x, y, z);
                        CreateCustomBlock(blockType, blockPos, true);
                        // CreateBlock(blockPos, true);
                        // Hide obscured neighbours
                        CheckObscuredNeighbours(blockPos);
                    }

                    // Delete Preview Blocks that are irrelevant
                    for (int y = intendedHeight + 1; y <= _height - 50; y++)
                    {
                        Vector3 blockPos = new Vector3(x, y, z);
                        if (worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] != null)
                        {
                            Destroy(worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z]._block);
                            worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = null;
                        }
                        // Show hidden structure underneath
                        // Only instantiate hidden blocks when they are on demand!
                        showHiddenBlockAround(blockPos);
                    }
                }
            }

            secondaryIndexInUse = true;
        }
    }

    private void deleteMode()
    {
/*
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger) > 0.5f &&
            (primaryHandInUse == false || OVRInput.Get(OVRInput.Touch.PrimaryThumbRest)))
        {
            OVRHaptics.Channels[0].Mix(new OVRHapticsClip(vibeClip_0_05));
            RayCastReturn rayCastAnswer = rayCastBlockDeletion(OVRInput.Controller.LTouch);
            if (rayCastAnswer.valid)
            {
                Vector3 blockPos = rayCastAnswer.blockPos;
                // this is the bottom block. Don't delete it!!!!
                if ((int)blockPos.y == 0) return;
                // Delete from the structure and the Unity world
                worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = null;
                Destroy(rayCastAnswer.block);
                // Show hidden structure underneath
                // Only instantiate hidden blocks when they are on demand!
                showHiddenBlockAround(blockPos);
            }

            primaryHandInUse = true;

        }
*/
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) > 0.5f &&
            (secondaryHandInUse == false || OVRInput.Get(OVRInput.Touch.SecondaryThumbRest)))
        {
            RayCastReturn rayCastAnswer = rayCastBlockDeletion(OVRInput.Controller.RTouch);
            if (rayCastAnswer.valid)
            {
                OVRHaptics.Channels[1].Mix(new OVRHapticsClip(vibeClip_0_05));
                Vector3 blockPos = rayCastAnswer.blockPos;
                // this is the bottom block. Don't delete it!!!!
                if ((int)blockPos.y == 0) return;
                // Delete from the structure and the Unity world
                worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = null;
                Destroy(rayCastAnswer.block);
                // Show hidden structure underneath
                // Only instantiate hidden blocks when they are on demand!
                showHiddenBlockAround(blockPos);
            }
            if (!OVRInput.Get(OVRInput.Touch.SecondaryThumbRest))
            {
                secondaryHandInUse = true;
            }
        }
    }

    private void singleMode()
    {
/*
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) > 0.5f &&
            (primaryIndexInUse == false || OVRInput.Get(OVRInput.Touch.PrimaryThumbRest)))
        {
            Debug.Log(OVRInput.Axis1D.PrimaryIndexTrigger);
            Debug.Log(primaryIndexInUse);

            OVRHaptics.Channels[0].Mix(new OVRHapticsClip(vibeClip_0_05));
            RayCastReturn rayCastAnswer = rayCastBlockCreation(OVRInput.Controller.LTouch);
            if (rayCastAnswer.valid)
            {
                Vector3 blockPos = rayCastAnswer.blockPos;
                CreateCustomBlock(blockType, blockPos, true);
                // CreateBlock(blockPos, true);
                // Hide obscured neighbours
                CheckObscuredNeighbours(blockPos);
            }

            primaryIndexInUse = true;
        }
*/
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.5f &&
            (secondaryIndexInUse == false || OVRInput.Get(OVRInput.Touch.SecondaryThumbRest)))
        {
            // Debug.Log(OVRInput.Axis1D.SecondaryIndexTrigger);
            // Debug.Log(secondaryIndexInUse);

            RayCastReturn rayCastAnswer = rayCastBlockCreation(OVRInput.Controller.RTouch);
            if (rayCastAnswer.valid)
            {
                OVRHaptics.Channels[1].Mix(new OVRHapticsClip(vibeClip_0_05));
                Vector3 blockPos = rayCastAnswer.blockPos;
                CreateCustomBlock(blockType, blockPos, true);
                // CreateBlock(blockPos, true);
                // Hide obscured neighbours
                CheckObscuredNeighbours(blockPos);
            }

            secondaryIndexInUse = true;
        }
    }

    private LandscapeStructure loadLandscapeStructure(string fileName)
    {
        BlockType[,,] data = null;

        try
        {
            using (StreamReader reader = new StreamReader(fileName))
            {
                int width = int.Parse(reader.ReadLine());
                int height = int.Parse(reader.ReadLine());
                int depth = int.Parse(reader.ReadLine());

                int xBase = int.Parse(reader.ReadLine());
                int yBase = int.Parse(reader.ReadLine());
                int zBase = int.Parse(reader.ReadLine());

                data = new BlockType[width, height, depth];

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < depth; j++)
                    {
                        string s = reader.ReadLine();
                        for (int k = 0; k < height; k++)
                        {
                            data[i, k, j] = (s[k] == '0' ? BlockType.none : BlockType.grass);
                        }
                    }
                }

                Debug.Log(width);
                Debug.Log(height);
                Debug.Log(depth);

                LandscapeStructure result = new LandscapeStructure();
                result.blockType = data;
                result.center.x = xBase;
                result.center.y = yBase;
                result.center.z = zBase;
                return result;
            }
        }
        catch (Exception e)
        {
            Debug.Log("File reading failed: ");
            Debug.Log(e.Message);
        }
        return null;
    }

    private void CreateBlock(Vector3 blockPos, bool create) {
        if (blockPos.x < 0 || blockPos.x >= _width || blockPos.y < 0 || blockPos.y >= _height || blockPos.z < 0 || blockPos.x >= _depth) return;
        if (worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] != null)
        {
            Destroy(worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z]._block);
        }
        GameObject newBlock = null;
        int y = (int)blockPos.y;

        if (y > 15 + _heightOffset) {
            if (create)
                newBlock = Instantiate(_snowBlock, blockPos, Quaternion.identity);
            worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new Block(BlockType.snow, create, newBlock);
        } else if (y > 5 + _heightOffset) {
            if (create)
                newBlock = Instantiate(_grassBlock, blockPos, Quaternion.identity);
            worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new Block(BlockType.grass, create, newBlock);
        } else {
            if (create)
                newBlock = Instantiate(_sandBlock, blockPos, Quaternion.identity);
            worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new Block(BlockType.sand, create, newBlock);
        }
    }

    private void CreateCustomBlock(BlockType blockType, Vector3 blockPos, bool create, bool deleteEmpty)
    {
        if (blockType == BlockType.none && deleteEmpty == false)
        {
            return;
        }

        // Overflow prevention and clean up
        if (blockPos.x < 0 || blockPos.x >= _width || blockPos.y < 0 || blockPos.y >= _height || blockPos.z < 0 || blockPos.x >= _depth) return;
        if (worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] != null)
        {
            Destroy(worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z]._block);
        }

        // Start Creating
        GameObject newBlock = null;
        switch (blockType)
        {
            case BlockType.snow:
                if (create)
                    newBlock = Instantiate(_snowBlock, blockPos, Quaternion.identity);
                worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new Block(BlockType.snow, create, newBlock);
                break;
            case BlockType.grass:
                if (create)
                    newBlock = Instantiate(_grassBlock, blockPos, Quaternion.identity);
                worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new Block(BlockType.grass, create, newBlock);
                break;
            case BlockType.sand:
                if (create)
                    newBlock = Instantiate(_sandBlock, blockPos, Quaternion.identity);
                worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new Block(BlockType.sand, create, newBlock);
                break;
            case BlockType.cloud:
                if (create)
                    newBlock = Instantiate(_cloudBlock, blockPos, Quaternion.identity);
                worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new Block(BlockType.cloud, create, newBlock);
                break;
            case BlockType.preview:
                if (create)
                    newBlock = Instantiate(_previewBlock, blockPos, Quaternion.identity);
                worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new Block(BlockType.preview, create, newBlock);
                break;
            default:
                break;
        }

        // Change color according to color bar if block type is preview block
        if (newBlock != null && blockType == BlockType.cloud)
        {
            Renderer rend = newBlock.transform.GetComponent<Renderer>();
            if (rend)
            {
                /*
                Debug.Log(_presetColorPicker.GetComponent<MyColorPicker>().color);
                if (_presetColorPicker.activeInHierarchy)
                {
                    _color = _presetColorPicker.GetComponent<MyColorPicker>().color;
                }
                */
                // Debug.Log(_color);
                rend.material.SetColor("_Color", _color);
            }
        }
    }

    private void CreateCustomBlock(BlockType blockType, Vector3 blockPos, bool create)
    {
        CreateCustomBlock(blockType, blockPos, create, true);
    }

    private void CreateCloud(int numClouds, int cSize) {
        for (int i = 0; i < numClouds; i++) {
            int xpos = UnityEngine.Random.Range(0, _width - 1);
            int zpos = UnityEngine.Random.Range(0, _depth - 1);
            for (int j = 0; j < cSize; j++) {
                Vector3 blockPos = new Vector3(xpos, _height - 1, zpos);
                GameObject newBlock = (GameObject)Instantiate(_cloudBlock, blockPos, Quaternion.identity);
                // Debug.Log(blockPos);
                worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new Block(BlockType.cloud, true, newBlock);
                xpos += UnityEngine.Random.Range(-1, 2);
                zpos += UnityEngine.Random.Range(-1, 2);
                if (xpos < 0 || xpos >= _width) xpos = _width / 2;
                if (zpos < 0 || zpos >= _depth) zpos = _depth / 2;
            }
        }
    }

    void DrawBlock(Vector3 blockPos) {
        //
        // Prevent drawing outside of the permitted space
        //
        if (blockPos.x < 0 || blockPos.x >= _width || blockPos.y < 0 || blockPos.y >= _height || blockPos.z < 0 || blockPos.z >= _depth) return;

        if (worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] == null) return;

        if (!worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z]._vis) {
            GameObject newBlock = null;
            worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z]._vis = true;
            if (worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z]._type == BlockType.grass) {
                newBlock = Instantiate(_grassBlock, blockPos, Quaternion.identity);
            } else if (worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z]._type == BlockType.sand) {
                newBlock = Instantiate(_sandBlock, blockPos, Quaternion.identity);
            } else if (worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z]._type == BlockType.snow) {
                newBlock = Instantiate(_snowBlock, blockPos, Quaternion.identity);
            } else {
                worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z]._vis = false;
            }

            if (newBlock != null)
                worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z]._block = newBlock;
        }
    }

    int NeighbourCount(Vector3 blockPos) {
        int nCount = 0;
        for (int x = -1; x <= 1; x++)
            for (int y = -1; y <= 1; y++)
                for (int z = -1; z <= 1; z++) {
                    if (!(x == 0 && y == 0 && z == 0)) {
                        if (worldBlocks[(int)blockPos.x + x, (int)blockPos.y + y, (int)blockPos.z + z] != null)
                            nCount++;
                    }
                }
        return (nCount);
    }

    void CheckObscuredNeighbours(Vector3 blockPos) {
        for (int x = -1; x <= 1; x++)
            for (int y = -1; y <= 1; y++)
                for (int z = -1; z <= 1; z++) {
                    if (!(x == 0 && y == 0 && z == 0)) {
                        Vector3 neighbour = new Vector3(blockPos.x + x, blockPos.y + y, blockPos.z + z);

                        //if it is outside the map
                        if (neighbour.x < 1 || neighbour.x > _width - 2 ||
                            neighbour.y < 1 || neighbour.y > _height - 2 ||
                            neighbour.z < 1 || neighbour.z > _depth - 2) continue;


                        if (worldBlocks[(int)neighbour.x, (int)neighbour.y, (int)neighbour.z] != null) {
                            if (NeighbourCount(neighbour) == 26) {
                                Destroy(worldBlocks[(int)neighbour.x, (int)neighbour.y, (int)neighbour.z]._block);
                                worldBlocks[(int)neighbour.x, (int)neighbour.y, (int)neighbour.z] = null;
                            }
                        }
                    }
                }
    }

    private void showHiddenBlockAround(Vector3 blockPos)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    if (!(x == 0 && y == 0 && z == 0))
                    {
                        Vector3 neighbour = new Vector3(blockPos.x + x, blockPos.y + y, blockPos.z + z);
                        DrawBlock(neighbour);
                    }
                }
            }
        }
    }

    private RayCastReturn rayCastBlockDeletion(OVRInput.Controller controller)
    {
        RaycastHit hit;
        // Ray ray = Camera.allCameras[0].ScreenPointToRay(new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0));
        // Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0));
        // Debug.Log(ray);
        RayCastReturn ans;
        if (Physics.Raycast(new Ray(_CameraFacing.transform.position + OVRInput.GetLocalControllerPosition(controller),
                    OVRInput.GetLocalControllerRotation(controller) * Vector3.forward),
                    out hit, 5000.0f))
        {
            ans.block = hit.transform.gameObject;
            ans.blockPos = hit.transform.position;
            ans.valid = true;
        }
        else
        {
            ans.block = null;
            ans.blockPos = Vector3.zero;
            ans.valid = false;
        }
        return ans;
    }

    private RayCastReturn rayCastBlockCreation(OVRInput.Controller controller)
    {
        // Debug.Log("Right Botton Detected");
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0));
        // Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);
        // Debug.Log(ray.direction);
        RayCastReturn ans;
        ans.block = null;
        if (Physics.Raycast(new Ray(_CameraFacing.transform.position + OVRInput.GetLocalControllerPosition(controller),
                                 OVRInput.GetLocalControllerRotation(controller) * Vector3.forward),
                                 out hit, 5000.0f))
        {
            Vector3 blockPos = hit.transform.position;
            Vector3 hitVector = blockPos - hit.point;

            hitVector.x = Mathf.Abs(hitVector.x);
            hitVector.y = Mathf.Abs(hitVector.y);
            hitVector.z = Mathf.Abs(hitVector.z);

            // Debug.Log(ray.direction);

            if (hitVector.x > hitVector.z && hitVector.x > hitVector.y)
            {
                // blockPos.x -= (int)Mathf.RoundToInt(ray.direction.x);
                if (ray.direction.x > 0)
                {
                    blockPos.x -= 1;
                }
                else
                {
                    blockPos.x += 1;
                }
            }
            else if (hitVector.y > hitVector.x && hitVector.y > hitVector.z)
            {
                // blockPos.y -= (int)Mathf.RoundToInt(ray.direction.y);
                if (ray.direction.y > 0)
                {
                    blockPos.y -= 1;
                }
                else
                {
                    blockPos.y += 1;
                }
            }
            else
            {
                // blockPos.z -= (int)Mathf.RoundToInt(ray.direction.z);
                if (ray.direction.z > 0)
                {
                    blockPos.z -= 1;
                }
                else
                {
                    blockPos.z += 1;
                }
            }

            ans.blockPos = blockPos;
            ans.valid = true;
        }
        else
        {
            ans.blockPos = Vector3.zero;
            ans.valid = false;
        }
        return ans;
    }

    public struct RayCastReturn
    {
        public GameObject block;
        public Vector3 blockPos;
        public bool valid;
    }

    public class LandscapeStructure
    {
        public BlockType[,,] blockType;
        public Vector3 center;
    }

    public void setBlockType(int blockTypeInt)
    {
        if (blockTypeInt >= 5)
        {
            blockType = BlockType.grass;
        } else if (blockTypeInt <= 0)
        {
            blockType = BlockType.cloud;
        } else
        {
            blockType = (BlockType)blockTypeInt;
        }
    }

    public void setColor(Vector4 color)
    {
        _color = color;
    }

    public bool getObjectMenuSelect()
    {
        return select_state;
    }
}
