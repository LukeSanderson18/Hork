using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditorInternal;
using UnityEngine;
using UnityEditor;
using Random = UnityEngine.Random;

[CustomEditor(typeof(TerrainEditor2D))]
public class TerrainEditor2DIns: Editor
{
    #region Hotkeys
    private static Dictionary<string, KeyCode> _hotKeys = new Dictionary<string, KeyCode>
    {
        {"SwitchEditMode", KeyCode.E},
        {"DigMode", KeyCode.D},
        {"EditModeUndo", KeyCode.Z},
        {"EditModeRedo", KeyCode.Y},
        {"ShowEditTab", KeyCode.F1},
        {"ShowCapTab", KeyCode.F2},
        {"ShowDeformTab", KeyCode.F3},
        {"ShowMainTab", KeyCode.F4},
    };
    #endregion

    #region Editor settings fields
    //Brush settings
    public float BrushSize = 5;
    public float BrushHardness = 0.25f;
    public float BrushNoise = 0;
    public float BrushHeightLimit = 50;
    public bool BrushHeightLimitEnabled = false;

    private bool _showEditTab;
    private bool _showCapTab;
    private bool _showDeformTab;
    private bool _showMainTab;

    #endregion

    #region Operational fields
    private TerrainEditor2D _myTerrainEditor2D;

    private bool _editMode;
    private bool _playMode;
    private bool _startEdit;
    private bool _digMode;
    private bool _brushSizeMode;
    private bool _brushLockMode;
    private Vector2 _brushHandleLockPos;

    private bool _hotkeyEnteredInEditMode;
    private Vector2 _mousePos;
    private Vector2 _handleLocalPos;

    private List<RecordedTerrainVerts> _recordedVerts = new List<RecordedTerrainVerts>();
    private RecordedTerrainVerts _lastRecordedVerts;
    private int _curUndoRecordedVertsId;

    private Tool _previousUsingTool;

    private Dictionary<string, Texture> _inspectorTextures = new Dictionary<string, Texture>();
    private GUIStyle _guiButtonStyle;
    private Texture2D _guiDefaultButton;

    #endregion

    [MenuItem("GameObject/2D Object/Terrain 2D")]
    private static void CreateTerrain2D()
    {
        GameObject terrain2D = new GameObject("New Terrain 2D");

        terrain2D.AddComponent<TerrainEditor2D>();
        terrain2D.GetComponent<TerrainEditor2D>().SetInstanceId(terrain2D.GetInstanceID());
        terrain2D.GetComponent<TerrainEditor2D>().Create();

        Selection.activeGameObject = terrain2D.gameObject;

        Undo.RegisterCompleteObjectUndo(terrain2D, "Terrain 2D object");
    }

    void OnEnable()
    {
        LoadInspector();
        
        _myTerrainEditor2D = target as TerrainEditor2D;

        if (_myTerrainEditor2D == null)
            return;

        if (PrefabUtility.GetPrefabType(_myTerrainEditor2D.gameObject) == PrefabType.Prefab)
            return;

        if (_myTerrainEditor2D.InstanceId != _myTerrainEditor2D.GetInstanceID())
        {
            CopySharedMesh(_myTerrainEditor2D.GetComponent<MeshFilter>());
            if (_myTerrainEditor2D.CapObj != null)
                CopySharedMesh(_myTerrainEditor2D.CapObj.GetComponent<MeshFilter>());
            if (_myTerrainEditor2D.Collider3DObj != null)
                CopySharedMesh(_myTerrainEditor2D.Collider3DObj.GetComponent<MeshFilter>());

            _myTerrainEditor2D.SetInstanceId(_myTerrainEditor2D.GetInstanceID());
        }

        _recordedVerts.Add(new RecordedTerrainVerts(_myTerrainEditor2D.GetVertsPos()));
    }

    public override void OnInspectorGUI()
    {
        #region Check inspector
        Undo.RecordObject(_myTerrainEditor2D, "2D Terrain Editor");

        if (_myTerrainEditor2D == null)
        {
            EditorGUILayout.HelpBox("Missing object reference", MessageType.Error);
            return;
        }

        if (PrefabUtility.GetPrefabType(_myTerrainEditor2D.gameObject) == PrefabType.Prefab)
        {
            EditorGUILayout.HelpBox("Terrain settings is not avaliable in project view. Please, place this prefab on the scene.", MessageType.Warning);
            return;
        }

        _guiButtonStyle = new GUIStyle(GUI.skin.button);
        _guiDefaultButton = _guiButtonStyle.normal.background;

        string[] sortingLayers = GetSortingLayerNames();
        #endregion

        #region Tab controls
        
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        _guiButtonStyle.overflow.right = 4;
        _guiButtonStyle.overflow.left = 4;

        _guiButtonStyle.normal.background = _guiDefaultButton;
        if (_showEditTab)
            _guiButtonStyle.normal.background = _guiButtonStyle.onActive.background;
        if (GUILayout.Button(new GUIContent(_inspectorTextures["Edit settings"], "Edit mode"), _guiButtonStyle, GUILayout.Height(25), GUILayout.Width(35)) 
            || (Event.current.keyCode == _hotKeys["ShowEditTab"]))
            SwitchTab("Edit settings");

        _guiButtonStyle.normal.background = _guiDefaultButton;
        if (_showCapTab)
            _guiButtonStyle.normal.background = _guiButtonStyle.onActive.background;
        if (GUILayout.Button(new GUIContent(_inspectorTextures["Cap settings"], "Cap settings"), _guiButtonStyle, GUILayout.Height(25), GUILayout.Width(35))
            || (Event.current.keyCode == _hotKeys["ShowCapTab"]))
            SwitchTab("Cap settings");

        _guiButtonStyle.normal.background = _guiDefaultButton;
        if (_showDeformTab)
            _guiButtonStyle.normal.background = _guiButtonStyle.onActive.background;
        if (GUILayout.Button(new GUIContent(_inspectorTextures["Deform settings"], "Realtime deformation settings"), _guiButtonStyle, GUILayout.Height(25), GUILayout.Width(35))
            || (Event.current.keyCode == _hotKeys["ShowDeformTab"]))
            SwitchTab("Deform settings");

        _guiButtonStyle.normal.background = _guiDefaultButton;
        if (_showMainTab)
            _guiButtonStyle.normal.background = _guiButtonStyle.onActive.background;
        if (GUILayout.Button(new GUIContent(_inspectorTextures["Main settings"], "Main settings"), _guiButtonStyle, GUILayout.Height(25), GUILayout.Width(35))
            || (Event.current.keyCode == _hotKeys["ShowMainTab"]))
            SwitchTab("Main settings");
        
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        _guiButtonStyle = new GUIStyle(GUI.skin.button);

        #endregion

        #region Edit tab

        if (_showEditTab)
        {
            //Brush settings
            GUI.skin.label.fontStyle = FontStyle.Bold;
            GUILayout.Label("Brush", GUI.skin.label);
            GUI.skin.label.fontStyle = FontStyle.Normal;

            BrushSize = EditorGUILayout.Slider("Size", BrushSize, 0.1f, 50);
            BrushHardness = EditorGUILayout.Slider("Hardness", BrushHardness, 0.01f, 1);
            BrushNoise = EditorGUILayout.Slider("Noise", BrushNoise, 0, 1);

            EditorGUILayout.BeginHorizontal();

            BrushHeightLimitEnabled = EditorGUILayout.Toggle(BrushHeightLimitEnabled, GUILayout.MaxWidth(15));
            
            EditorGUI.BeginDisabledGroup(!BrushHeightLimitEnabled);

            BrushHeightLimit = EditorGUILayout.Slider("Limit height", BrushHeightLimit, 0, _myTerrainEditor2D.Height);
            
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (_editMode)
                _guiButtonStyle.normal.background = _guiButtonStyle.onActive.background;

            if (GUILayout.Button("EDIT MODE", _guiButtonStyle, GUILayout.Height(40)))
            {
                SwitchEditMode();
            }

            if (_editMode)
                EditorGUILayout.HelpBox(
                    "Hold [LMB] - raise terrain\n" +
                    "Hold [" + _hotKeys["DigMode"] + " + LMB] - dig mode\n" +
                    "Hold [ALT+Mouse Wheel] - change brush size\n" +
                    "Hold [SHIFT] - horizontal lock\n" +
                    "[SHIFT+" + _hotKeys["EditModeUndo"] + " / " + _hotKeys["EditModeRedo"] + "] - undo / redo",
                    MessageType.None);
            else
            {
                if (GetSceneViewCamera() != null)
                {
                    if (!GetSceneViewCamera().orthographic)
                        EditorGUILayout.HelpBox("Set scene view camera to orthographic or 2D mode to enter edit mode.", MessageType.Warning);
                    else EditorGUILayout.HelpBox("Use [Shift+" + _hotKeys["SwitchEditMode"] + "] to enter Edit Mode", MessageType.None);
                }
            }

            EditorGUILayout.Space();

            //Randomizer
            GUI.skin.label.fontStyle = FontStyle.Bold;
            GUILayout.Label("Random generation", GUI.skin.label);
            GUI.skin.label.fontStyle = FontStyle.Normal;
            
            _myTerrainEditor2D.RndHeight = EditorGUILayout.Slider("Average height", _myTerrainEditor2D.RndHeight, 0.1f, _myTerrainEditor2D.Height);
            _myTerrainEditor2D.RndHillsCount = EditorGUILayout.IntSlider("Hills count", _myTerrainEditor2D.RndHillsCount, 1, _myTerrainEditor2D.Width / 2);
            _myTerrainEditor2D.RndAmplitude = EditorGUILayout.Slider("Amplitude", _myTerrainEditor2D.RndAmplitude, 0.1f, _myTerrainEditor2D.Height / 2f);

            if (GUILayout.Button("Generate"))
            {
                _myTerrainEditor2D.RandomizeTerrain(true);
            }

            //Check transform
            if (_myTerrainEditor2D.transform.rotation.eulerAngles != Vector3.zero || _myTerrainEditor2D.transform.localScale != Vector3.one)
            {
                EditorGUILayout.HelpBox("Object rotation or scale is different from default. Edit mode may not work properly.", MessageType.Warning);
            }
        }
        
        #endregion

        #region Cap tab
        if (_showCapTab)
        {
            GUI.skin.label.fontStyle = FontStyle.Bold;
            GUILayout.Label("Cap", GUI.skin.label);
            GUI.skin.label.fontStyle = FontStyle.Normal;

            _myTerrainEditor2D.GenerateCap = EditorGUILayout.Toggle("Enabled", _myTerrainEditor2D.GenerateCap);

            EditorGUI.BeginDisabledGroup(!_myTerrainEditor2D.GenerateCap);

            _myTerrainEditor2D.CapMaterial = (Material)EditorGUILayout.ObjectField("Material", _myTerrainEditor2D.CapMaterial, typeof(Material), false);
            CheckTexture(_myTerrainEditor2D.CapMaterial);
            _myTerrainEditor2D.CapColor = EditorGUILayout.ColorField(new GUIContent("Color", "Color of cap mesh. Works only if shader supports vertex colors."), _myTerrainEditor2D.CapColor);

            _myTerrainEditor2D.CapHeight = EditorGUILayout.FloatField(new GUIContent("Height", "Cap height in units"), _myTerrainEditor2D.CapHeight);
            _myTerrainEditor2D.CapOffset = EditorGUILayout.FloatField(new GUIContent("Offset", "Offset relative to base terrain path"), _myTerrainEditor2D.CapOffset);

            EditorGUI.BeginDisabledGroup(_myTerrainEditor2D.SmartCap);

            _myTerrainEditor2D.CapTextureTiling = EditorGUILayout.IntField(new GUIContent("Texture tiling", "Number of cap texture tiles for whole 2D terrain (does not affect to cap if smart cap enabled)"), _myTerrainEditor2D.CapTextureTiling);
            EditorGUI.EndDisabledGroup();

            _myTerrainEditor2D.CapSortingLayerId = EditorGUILayout.Popup("Sorting layer", _myTerrainEditor2D.CapSortingLayerId, sortingLayers);
            _myTerrainEditor2D.CapSortingLayerName = sortingLayers[_myTerrainEditor2D.CapSortingLayerId];

            _myTerrainEditor2D.CapOrderInLayer = EditorGUILayout.IntField("Order in layer", _myTerrainEditor2D.CapOrderInLayer);

            EditorGUILayout.Space();

            GUI.skin.label.fontStyle = FontStyle.Bold;
            GUILayout.Label("Smart cap", GUI.skin.label);
            GUI.skin.label.fontStyle = FontStyle.Normal;

            _myTerrainEditor2D.SmartCap = EditorGUILayout.Toggle("Enabled", _myTerrainEditor2D.SmartCap);

            EditorGUI.BeginDisabledGroup(!_myTerrainEditor2D.SmartCap);
            _myTerrainEditor2D.SmartCapCutHeight = EditorGUILayout.FloatField(new GUIContent("Cut height", "Min height in units for splitting cap into different paths"), _myTerrainEditor2D.SmartCapCutHeight);
            _myTerrainEditor2D.SmartCapSegmentsUv = EditorGUILayout.Vector2Field(new GUIContent("Segments UV", "Horizontal UV for smart cap texture. It's like separator which divides cap texture into 3 parts."), _myTerrainEditor2D.SmartCapSegmentsUv);
            _myTerrainEditor2D.SmartCapSideSegmentsWidth = EditorGUILayout.FloatField("Side segments width", _myTerrainEditor2D.SmartCapSideSegmentsWidth);

            EditorGUI.EndDisabledGroup();

            EditorGUI.EndDisabledGroup();
        }
        #endregion

        #region Deform tab

        if (_showDeformTab)
        {
            GUI.skin.label.fontStyle = FontStyle.Bold;
            GUILayout.Label("Realtime deformation", GUI.skin.label);
            GUI.skin.label.fontStyle = FontStyle.Normal;

            _myTerrainEditor2D.RealtimeDeformEnabled = EditorGUILayout.Toggle("Enabled", _myTerrainEditor2D.RealtimeDeformEnabled);
            
            EditorGUI.BeginDisabledGroup(!_myTerrainEditor2D.RealtimeDeformEnabled);

            _myTerrainEditor2D.RealtimeDeformUpdateColliders = EditorGUILayout.Toggle(new GUIContent("Update colliders", 
                "Is colliders should be updated on each iteration of deformation?"), _myTerrainEditor2D.RealtimeDeformUpdateColliders);
            _myTerrainEditor2D.RealtimeDeformUpdateUv = EditorGUILayout.Toggle(new GUIContent("Update UV",
                "Is UV texture coordinates should be updated on each iteration of deformation?"), _myTerrainEditor2D.RealtimeDeformUpdateUv);

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.HelpBox("Read the documentation to leran how realtime deformation works", MessageType.None);
            EditorGUILayout.HelpBox("Note that updating 2D collider is very expensive operation due to 2D physics specificity", MessageType.None);
        }
        #endregion

        #region Main tab

        if (_showMainTab)
        {
            EditorGUI.BeginChangeCheck();

            //Main
            GUI.skin.label.fontStyle = FontStyle.Bold;
            GUILayout.Label("Main", GUI.skin.label);
            GUI.skin.label.fontStyle = FontStyle.Normal;

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.IntField("Mesh ID", _myTerrainEditor2D.InstanceId);
            EditorGUI.EndDisabledGroup();

            _myTerrainEditor2D.MainMaterial = (Material)EditorGUILayout.ObjectField("Material", _myTerrainEditor2D.MainMaterial, typeof(Material), false);
            CheckTexture(_myTerrainEditor2D.MainMaterial);
            _myTerrainEditor2D.MainColor = EditorGUILayout.ColorField(new GUIContent("Color", "Color of 2D terrain mesh. Works only if shader supports vertex colors."), _myTerrainEditor2D.MainColor);

            _myTerrainEditor2D.MainTextureSize = EditorGUILayout.IntField("Texture size", _myTerrainEditor2D.MainTextureSize);

            _myTerrainEditor2D.SortingLayerId = EditorGUILayout.Popup("Sorting layer", _myTerrainEditor2D.SortingLayerId, sortingLayers);
            _myTerrainEditor2D.SortingLayerName = sortingLayers[_myTerrainEditor2D.SortingLayerId];

            _myTerrainEditor2D.OrderInLayer = EditorGUILayout.IntField("Order in layer", _myTerrainEditor2D.OrderInLayer);

            //Additionally
            EditorGUILayout.Space();

            GUI.skin.label.fontStyle = FontStyle.Bold;
            GUILayout.Label("Additionally", GUI.skin.label);
            GUI.skin.label.fontStyle = FontStyle.Normal;
            
            _myTerrainEditor2D.FixSides = EditorGUILayout.Toggle(new GUIContent("Fix sides", 
                "Is side end points of terrain 2D path needs to be fixed and protected from changes? "), _myTerrainEditor2D.FixSides);
            EditorGUI.BeginDisabledGroup(!_myTerrainEditor2D.FixSides);
            _myTerrainEditor2D.LeftFixedPoint = EditorGUILayout.FloatField("Left fixed point", _myTerrainEditor2D.LeftFixedPoint);
            _myTerrainEditor2D.RightFixedPoint = EditorGUILayout.FloatField("Right fixed point", _myTerrainEditor2D.RightFixedPoint);
            EditorGUI.EndDisabledGroup();

            _myTerrainEditor2D.Generate3DCollider = EditorGUILayout.Toggle("3D collider", _myTerrainEditor2D.Generate3DCollider);
            EditorGUI.BeginDisabledGroup(!_myTerrainEditor2D.Generate3DCollider);
            _myTerrainEditor2D.Collider3DWidth = EditorGUILayout.FloatField("3D collider width", _myTerrainEditor2D.Collider3DWidth);
            EditorGUI.EndDisabledGroup();
            
            //Resolution
            EditorGUILayout.Space();

            GUI.skin.label.fontStyle = FontStyle.Bold;
            GUILayout.Label("Size and resolution", GUI.skin.label);
            GUI.skin.label.fontStyle = FontStyle.Normal;

            _myTerrainEditor2D.Width = EditorGUILayout.IntField("Width", _myTerrainEditor2D.Width);
            _myTerrainEditor2D.Height = EditorGUILayout.IntField("Height", _myTerrainEditor2D.Height);
            _myTerrainEditor2D.Resolution = EditorGUILayout.IntField("Resolution", _myTerrainEditor2D.Resolution);

            if (_myTerrainEditor2D.Resolution > 8)
                EditorGUILayout.HelpBox("Resolution is responsible for the number of polygons. High resolution value may affect performance.", MessageType.Warning);

            if (GUILayout.Button("Apply"))
            {
                _recordedVerts.Clear();
                _curUndoRecordedVertsId = 0;
                _myTerrainEditor2D.Create();
                _recordedVerts.Add(new RecordedTerrainVerts(_myTerrainEditor2D.GetVertsPos()));
            }
            EditorGUILayout.HelpBox("Applying will clear all changes in geometry", MessageType.None);
            
        }
        #endregion

        #region Check colliders
        if (EditorApplication.isPlaying && !_playMode)
        {
            _myTerrainEditor2D.UpdateCollider2D();

            if (_myTerrainEditor2D.Generate3DCollider)
            {
                _myTerrainEditor2D.UpdateCollider3D(false);
            }

            _playMode = true;
        }
        else _playMode = false;
        #endregion

        if (GUI.changed)
        {
            CheckValues();
            
            if (!EditorApplication.isPlaying)
                _myTerrainEditor2D.EditTerrain(_myTerrainEditor2D.GetVertsPos(), true);
            else _myTerrainEditor2D.EditTerrain(_myTerrainEditor2D.GetVertsPos(), false);

            EditorUtility.SetDirty(target);
        }
    }

    void OnSceneGUI()
    {
        if (PrefabUtility.GetPrefabType(_myTerrainEditor2D.gameObject) == PrefabType.Prefab || _myTerrainEditor2D == null)
            return;

        #region Scene view events
        //Switch tab
        if (Event.current.type == EventType.KeyUp)
        {
            if (Event.current.keyCode == _hotKeys["ShowEditTab"])
                SwitchTab("Edit settings");
            if (Event.current.keyCode == _hotKeys["ShowCapTab"])
                SwitchTab("Cap settings");
            if (Event.current.keyCode == _hotKeys["ShowMainTab"])
                SwitchTab("Main settings");
        }
        

        if (Event.current.Equals(Event.KeyboardEvent("#" + _hotKeys["SwitchEditMode"])))
            SwitchEditMode();

        //Undo & redo (values)
        if (Event.current.commandName == "UndoRedoPerformed")
        {
            if (!EditorApplication.isPlaying)
                _myTerrainEditor2D.EditTerrain(_myTerrainEditor2D.GetVertsPos(), true);
            else _myTerrainEditor2D.EditTerrain(_myTerrainEditor2D.GetVertsPos(), false);
        }

        #endregion

        if (!_editMode)
            return;

        #region Draw handles
        if (GetSceneViewCamera() != null)
        {
            _mousePos = Camera.current.ScreenToWorldPoint(new Vector2(Event.current.mousePosition.x, (Camera.current.pixelHeight - Event.current.mousePosition.y)));
            _handleLocalPos = _mousePos - new Vector2(_myTerrainEditor2D.transform.position.x, _myTerrainEditor2D.transform.position.y);
            if (_brushLockMode)
                _handleLocalPos.y = _brushHandleLockPos.y;

            Vector2 finalHandlePos = _handleLocalPos + (Vector2)_myTerrainEditor2D.transform.position;
            Handles.color = Color.green;
            Handles.CircleCap(0, finalHandlePos, Quaternion.identity, BrushSize);

            if (_brushLockMode)
            {
                Handles.DrawLine(finalHandlePos + new Vector2(-BrushSize, BrushSize), finalHandlePos + new Vector2(BrushSize, BrushSize));
                Handles.DrawLine(finalHandlePos + new Vector2(-BrushSize, -BrushSize), finalHandlePos + new Vector2(BrushSize, -BrushSize));
            }

            //Draw cap tile errors
            if (_myTerrainEditor2D.GenerateCap && _myTerrainEditor2D.SmartCap)
            {
                if (_myTerrainEditor2D.SmartCapAreas != null)
                {
                    foreach (var smartCapArea in _myTerrainEditor2D.SmartCapAreas)
                    {
                        Handles.color = Color.red;
                        if (smartCapArea.CorruptedTilesVertsPoints.Count >= 2)
                        {
                            Handles.DrawLine(smartCapArea.CorruptedTilesVertsPoints[0] + _myTerrainEditor2D.transform.position, smartCapArea.CorruptedTilesVertsPoints[1] + _myTerrainEditor2D.transform.position);
                        }
                            
                        Handles.color = Color.green;
                    }
                }
            }

        }
        #endregion

        #region Draw terrain borders
        Vector3 terrainPos = _myTerrainEditor2D.transform.position;
        Handles.color = new Color(1, 1, 1, 0.5f);
        Handles.DrawLine(terrainPos, terrainPos + new Vector3(0, _myTerrainEditor2D.Height));
        Handles.DrawLine(terrainPos, terrainPos + new Vector3(_myTerrainEditor2D.Width, 0));
        Handles.DrawLine(terrainPos + new Vector3(0, _myTerrainEditor2D.Height), terrainPos + new Vector3(0, _myTerrainEditor2D.Height) + new Vector3(_myTerrainEditor2D.Width, 0));
        Handles.DrawLine(terrainPos + new Vector3(_myTerrainEditor2D.Width, 0), terrainPos + new Vector3(0, _myTerrainEditor2D.Height) + new Vector3(_myTerrainEditor2D.Width, 0));

        if (_myTerrainEditor2D.FixSides)
        {
            Vector3 leftFixedPoint = terrainPos + new Vector3(0, _myTerrainEditor2D.LeftFixedPoint);
            Vector3 rightFixedPoint = terrainPos + new Vector3(_myTerrainEditor2D.Width, _myTerrainEditor2D.RightFixedPoint);
            Handles.DrawLine(leftFixedPoint, leftFixedPoint - new Vector3(1, 0));
            Handles.DrawLine(rightFixedPoint, rightFixedPoint + new Vector3(1, 0));
        }

        #endregion

        #region Edit mode events
        //Start edit
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            _startEdit = true;

        //End edit
        if (Event.current.type == EventType.MouseUp)
        {
            _startEdit = false;

            //Register custom undo
            _lastRecordedVerts = new RecordedTerrainVerts(_myTerrainEditor2D.GetVertsPos());
            if (_recordedVerts.Count > 0)
            {
                if (!IsRecordedTerrainVertsEquals(_recordedVerts[_recordedVerts.Count - 1], _lastRecordedVerts))
                {
                    if (_curUndoRecordedVertsId < _recordedVerts.Count - 1)
                        _recordedVerts.RemoveRange(_curUndoRecordedVertsId + 1, (_recordedVerts.Count - _curUndoRecordedVertsId - 1));

                    if (_recordedVerts.Count > 75)
                        _recordedVerts.RemoveAt(0);

                    _recordedVerts.Add(_lastRecordedVerts);
                    _curUndoRecordedVertsId = _recordedVerts.Count - 1;
                }
            }
            else _recordedVerts.Add(_lastRecordedVerts);
        }

        //Dig mode
        if (Event.current.keyCode == _hotKeys["DigMode"])
        {
            if (Event.current.type == EventType.KeyDown)
                _digMode = true;
            if (Event.current.type == EventType.keyUp)
                _digMode = false;
        }

        //Brush lock
        if (Event.current.shift)
        {
            if (!_brushLockMode)
            {
                _brushHandleLockPos = _handleLocalPos;
                _brushLockMode = true;
            }
        }
        else _brushLockMode = false;

        //Brush size
        if (Event.current.alt)
        {
            if (Event.current.type == EventType.ScrollWheel)
            {
                float sizeDelta = 0.2f;
                if (Event.current.control)
                    sizeDelta += 0.8f;

                if (Event.current.delta.y > 0)
                    BrushSize -= sizeDelta;
                if (Event.current.delta.y < 0)
                    BrushSize += sizeDelta;

                Event.current.Use();
            }
            if (BrushSize <= 0.1f)
                BrushSize = 0.1f;
        }

        //Undo & redo (edit Mode)
        if (Event.current.shift)
        {
            if (Event.current.type == EventType.KeyUp)
            {
                //Undo
                if (Event.current.keyCode == _hotKeys["EditModeUndo"])
                {
                    if (_recordedVerts.Count > 0 && _curUndoRecordedVertsId > 0)
                    {
                        _curUndoRecordedVertsId--;
                        _myTerrainEditor2D.EditTerrain(_recordedVerts[_curUndoRecordedVertsId].TerrainVerts, true);
                    }
                }

                //Redo
                if (Event.current.keyCode == _hotKeys["EditModeRedo"])
                {
                    if (_curUndoRecordedVertsId < _recordedVerts.Count - 1)
                    {
                        _curUndoRecordedVertsId++;
                        _myTerrainEditor2D.EditTerrain(_recordedVerts[_curUndoRecordedVertsId].TerrainVerts, true);
                    }
                }
            }
        }

        #endregion

        #region Start edit mesh
        if (_startEdit)
        {
            Vector3[] vertsPos = _myTerrainEditor2D.GetVertsPos();

            for (int i = 0; i < vertsPos.Length; i += 2)
            {
                if (Vector2.Distance(vertsPos[i], _handleLocalPos) <= BrushSize)
                {
                    float vertOffset = BrushSize - Vector2.Distance(vertsPos[i], _handleLocalPos);

                    if (_digMode)
                        vertsPos[i] -= new Vector3(0, vertOffset * (BrushHardness * 0.1f));
                    else vertsPos[i] += new Vector3(0, vertOffset * (BrushHardness * 0.1f));

                    if (BrushNoise > 0f)
                        vertsPos[i] += new Vector3(0, Random.Range(-BrushNoise * 0.25f, BrushNoise * 0.25f));

                    if (BrushHeightLimitEnabled)
                        if (vertsPos[i].y > BrushHeightLimit)
                            vertsPos[i].y = BrushHeightLimit;

                    if (vertsPos[i].y < 0)
                        vertsPos[i].y = 0;
                    if (vertsPos[i].y > _myTerrainEditor2D.Height)
                        vertsPos[i].y = _myTerrainEditor2D.Height;

                }
            }

            if (!EditorApplication.isPlaying)
                _myTerrainEditor2D.EditTerrain(vertsPos, true);
            else _myTerrainEditor2D.EditTerrain(vertsPos, false);

            Selection.activeGameObject = _myTerrainEditor2D.gameObject;
        }
        #endregion

        #region Configure handles

        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        if (_myTerrainEditor2D.GetComponent<Renderer>() != null)
            EditorUtility.SetSelectedWireframeHidden(_myTerrainEditor2D.GetComponent<Renderer>(), true);
        if (_myTerrainEditor2D.CapObj != null)
        {
            if (_myTerrainEditor2D.CapObj.GetComponent<Renderer>() != null)
                EditorUtility.SetSelectedWireframeHidden(_myTerrainEditor2D.CapObj.GetComponent<Renderer>(), true);
        }

        #endregion

        SceneView.RepaintAll();
        EditorUtility.SetDirty(_myTerrainEditor2D);
    }

    void OnDisable()
    {
        #region Save prefs
        EditorPrefs.SetString("BrushSize", BrushSize.ToString());
        EditorPrefs.SetString("BrushHardness", BrushHardness.ToString());
        EditorPrefs.SetString("BrushNoise", BrushNoise.ToString());
        EditorPrefs.SetString("BrushHeightLimit", BrushHeightLimit.ToString());
        EditorPrefs.SetBool("BrushHeightLimitEnabled", BrushHeightLimitEnabled);

        EditorPrefs.SetBool("ShowEditSettings", _showEditTab);
        EditorPrefs.SetBool("ShowCapSettings", _showCapTab);
        EditorPrefs.SetBool("ShowDeformSettings", _showDeformTab);
        EditorPrefs.SetBool("ShowMainSettings", _showMainTab);
        #endregion

        if (_myTerrainEditor2D == null)
            return;

        if (PrefabUtility.GetPrefabType(_myTerrainEditor2D.gameObject) == PrefabType.Prefab)
            return;

        if (_editMode)
            SwitchEditMode();

        _myTerrainEditor2D.UpdateCollider2D();

        if (_myTerrainEditor2D.Collider3DObj != null)
            InternalEditorUtility.SetIsInspectorExpanded(_myTerrainEditor2D.Collider3DObj.GetComponent<MeshCollider>(), true);
        if (_myTerrainEditor2D.GetComponent<EdgeCollider2D>())
            InternalEditorUtility.SetIsInspectorExpanded(_myTerrainEditor2D.GetComponent<EdgeCollider2D>(), true);
        if (_myTerrainEditor2D.GetComponent<PolygonCollider2D>())
            InternalEditorUtility.SetIsInspectorExpanded(_myTerrainEditor2D.GetComponent<PolygonCollider2D>(), true);

        #region Save meshes
        if (!EditorApplication.isPlaying)
        {
            SaveMesh(_myTerrainEditor2D.GetComponent<MeshFilter>().sharedMesh);
            if (_myTerrainEditor2D.GenerateCap)
            {
                if (_myTerrainEditor2D.CapObj != null)
                    SaveMesh(_myTerrainEditor2D.CapObj.GetComponent<MeshFilter>().sharedMesh);
                else _myTerrainEditor2D.GenerateCap = false;
            }

            if (_myTerrainEditor2D.Generate3DCollider)
            {
                if (_myTerrainEditor2D.Collider3DObj != null)
                {
                    SaveMesh(_myTerrainEditor2D.Collider3DObj.GetComponent<MeshFilter>().sharedMesh);
                    _myTerrainEditor2D.Collider3DObj.GetComponent<MeshCollider>().enabled = false;
                    _myTerrainEditor2D.Collider3DObj.GetComponent<MeshCollider>().enabled = true;
                }
                else _myTerrainEditor2D.Generate3DCollider = false;
            }
        }
        #endregion

        
    }

    void SwitchTab(string tabName)
    {
        _showEditTab = false;
        _showCapTab = false;
        _showDeformTab = false;
        _showMainTab = false;

        switch (tabName)
        {
            case "Edit settings":
                _showEditTab = true;
                break;

            case "Cap settings":
                _showCapTab = true;
                break;

            case "Deform settings":
                _showDeformTab = true;
                break;

            case "Main settings":
                _showMainTab = true;
                break;
        }

        Repaint();
    }

    void SwitchEditMode()
    {
        if (GetSceneViewCamera() != null)
        {
            if (!_editMode && !GetSceneViewCamera().orthographic)
                return;
        }

        _editMode = !_editMode;

        if (_editMode)
        {
            if (_myTerrainEditor2D.Collider3DObj != null)
                InternalEditorUtility.SetIsInspectorExpanded(_myTerrainEditor2D.Collider3DObj.GetComponent<MeshCollider>(), false);
            if (_myTerrainEditor2D.GetComponent<EdgeCollider2D>())
                InternalEditorUtility.SetIsInspectorExpanded(_myTerrainEditor2D.GetComponent<EdgeCollider2D>(), false);
            if (_myTerrainEditor2D.GetComponent<PolygonCollider2D>())
                InternalEditorUtility.SetIsInspectorExpanded(_myTerrainEditor2D.GetComponent<PolygonCollider2D>(), false);
            _previousUsingTool = Tools.current;
            Tools.current = Tool.None;
        }
        else
        {
            Tools.current = _previousUsingTool;
        }
    }

    void CheckTexture(Material mat)
    {
        if (mat == null)
            return;

        mat.mainTexture = (Texture)EditorGUILayout.ObjectField("Material texture", mat.mainTexture, typeof(Texture), false, GUILayout.MaxHeight(15));

        if (mat.mainTexture != null)
            if (mat.mainTexture.wrapMode != TextureWrapMode.Repeat)
                EditorGUILayout.HelpBox("Material texture wrap mode must be set to Repeat for correct tiling", MessageType.Warning);
        
    }

    void CheckValues()
    {
        if (_myTerrainEditor2D.Width < 10)
            _myTerrainEditor2D.Width = 10;
        if (_myTerrainEditor2D.Height < 10)
            _myTerrainEditor2D.Height = 10;
        if (_myTerrainEditor2D.Resolution < 1)
            _myTerrainEditor2D.Resolution = 1;
        if (_myTerrainEditor2D.CapHeight < 0.1f)
            _myTerrainEditor2D.CapHeight = 0.1f;
        if (_myTerrainEditor2D.LeftFixedPoint < 0)
            _myTerrainEditor2D.LeftFixedPoint = 0;
        if (_myTerrainEditor2D.LeftFixedPoint > _myTerrainEditor2D.Height)
            _myTerrainEditor2D.LeftFixedPoint = _myTerrainEditor2D.Height;
        if (_myTerrainEditor2D.RightFixedPoint < 0)
            _myTerrainEditor2D.RightFixedPoint = 0;
        if (_myTerrainEditor2D.RightFixedPoint > _myTerrainEditor2D.Height)
            _myTerrainEditor2D.RightFixedPoint = _myTerrainEditor2D.Height;
        if (_myTerrainEditor2D.SmartCapCutHeight < 0.01f)
            _myTerrainEditor2D.SmartCapCutHeight = 0.01f;
        if (_myTerrainEditor2D.SmartCapSideSegmentsWidth < 0)
            _myTerrainEditor2D.SmartCapSideSegmentsWidth = 0;

        _myTerrainEditor2D.GetComponent<Renderer>().material = _myTerrainEditor2D.MainMaterial;
        if (_myTerrainEditor2D.GenerateCap)
            if (_myTerrainEditor2D.CapObj != null)
                _myTerrainEditor2D.CapObj.GetComponent<Renderer>().material = _myTerrainEditor2D.CapMaterial;
    }

    void LoadInspector()
    {
        #region Load prefs
        if (EditorPrefs.HasKey("BrushSize"))
            BrushSize = Single.Parse(EditorPrefs.GetString("BrushSize"));
        if (EditorPrefs.HasKey("BrushHardness"))
            BrushHardness = Single.Parse(EditorPrefs.GetString("BrushHardness"));
        if (EditorPrefs.HasKey("BrushNoise"))
            BrushNoise = Single.Parse(EditorPrefs.GetString("BrushNoise"));
        if (EditorPrefs.HasKey("BrushHeightLimit"))
            BrushHeightLimit = Single.Parse(EditorPrefs.GetString("BrushHeightLimit"));
        if (EditorPrefs.HasKey("BrushHeightLimitEnabled"))
            BrushHeightLimitEnabled = EditorPrefs.GetBool("BrushHeightLimitEnabled");

        if (EditorPrefs.HasKey("ShowEditSettings"))
            _showEditTab = EditorPrefs.GetBool("ShowEditSettings");
        if (EditorPrefs.HasKey("ShowCapSettings"))
            _showCapTab = EditorPrefs.GetBool("ShowCapSettings");
        if (EditorPrefs.HasKey("ShowDeformSettings"))
            _showDeformTab = EditorPrefs.GetBool("ShowDeformSettings");
        if (EditorPrefs.HasKey("ShowMainSettings"))
            _showMainTab = EditorPrefs.GetBool("ShowMainSettings");
        #endregion

        string path = GetFolderPath("2DTerrainInspectorRes");
        path = path.Replace('\\', '/');

        _inspectorTextures["Edit settings"] = (Texture)AssetDatabase.LoadAssetAtPath(path + "2dterrain_inspector_edit_settings_button.png", typeof(Texture));
        _inspectorTextures["Cap settings"] = (Texture)AssetDatabase.LoadAssetAtPath(path + "2dterrain_inspector_cap_settings_button.png", typeof(Texture));
        _inspectorTextures["Deform settings"] = (Texture)AssetDatabase.LoadAssetAtPath(path + "2dterrain_inspector_deform_settings_button.png", typeof(Texture));
        _inspectorTextures["Main settings"] = (Texture)AssetDatabase.LoadAssetAtPath(path + "2dterrain_inspector_main_settings_button.png", typeof(Texture));
    }

    void CopySharedMesh(MeshFilter meshFilter)
    {
        Mesh sharedMesh = meshFilter.sharedMesh;
        Mesh newMeshInstance = new Mesh();

        newMeshInstance.vertices = sharedMesh.vertices;
        newMeshInstance.triangles = sharedMesh.triangles;
        newMeshInstance.normals = sharedMesh.normals;
        newMeshInstance.uv = sharedMesh.uv;
        newMeshInstance.RecalculateBounds();

        newMeshInstance.name = sharedMesh.name.Remove(sharedMesh.name.LastIndexOf('_') + 1, (sharedMesh.name.Length - sharedMesh.name.LastIndexOf('_') - 1));
        newMeshInstance.name += _myTerrainEditor2D.GetInstanceID();

        meshFilter.sharedMesh = newMeshInstance;
    }

    void SaveMesh(Mesh mesh)
    {
        if (PrefabUtility.GetPrefabType(_myTerrainEditor2D.gameObject) == PrefabType.Prefab)
            return;

        string path = GetFolderPath("2DTerrainSavedMeshes");

        if (String.IsNullOrEmpty(path))
        {
            Debug.LogError("'SavedTerrain2DMeshes' folder is missing. Please, create folder with this name to save 2D terrain mesh.");
            return;
        }

        AssetDatabase.Refresh();

        if (!AssetDatabase.Contains(mesh))
            AssetDatabase.CreateAsset(mesh, path + mesh.name + ".asset");
        

        AssetDatabase.SaveAssets();
    }
    
    string[] GetSortingLayerNames()
    {
        Type internalEditorUtilityType = typeof(InternalEditorUtility);
        
        PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
        return (string[])sortingLayersProperty.GetValue(null, new object[0]);
    }

    string GetFolderPath(string folderName)
    {
        string[] targetDirs = Directory.GetDirectories(Application.dataPath, folderName, SearchOption.AllDirectories);
        foreach (var savedMeshesPath in targetDirs)
        {
            return savedMeshesPath.Remove(0, savedMeshesPath.IndexOf("/Assets", StringComparison.Ordinal) + 1) + "\\";
        }

        return null;
    }

    Camera GetSceneViewCamera()
    {
        if (SceneView.lastActiveSceneView != null)
            if (SceneView.lastActiveSceneView.camera != null)
                return SceneView.lastActiveSceneView.camera;

        return null;
    }

    bool IsRecordedTerrainVertsEquals(RecordedTerrainVerts rtv1, RecordedTerrainVerts rtv2)
    {
        if (rtv1.TerrainVerts.Length != rtv2.TerrainVerts.Length)
            return false;

        for (int i = 0; i < rtv1.TerrainVerts.Length; i++)
        {
            if (rtv1.TerrainVerts[i] != rtv2.TerrainVerts[i])
                return false;
        }
        return true;
    }

    class RecordedTerrainVerts
    {
        public Vector3[] TerrainVerts;

        public RecordedTerrainVerts(Vector3[] terrainVerts)
        {
            TerrainVerts = terrainVerts;
        }
    }
    
}