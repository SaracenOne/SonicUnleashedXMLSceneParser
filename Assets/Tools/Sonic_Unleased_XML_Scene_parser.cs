#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

// TODO:
// Multiset Param

public class SonicUnleasedXMLSceneParserScriptableWizard : UnityEditor.ScriptableWizard
{
    public List<TextAsset> textAssets = new List<TextAsset>();

    private void SetGameObjectParent(GameObject child, GameObject parent)
    {
        if (parent == null)
        {
            child.transform.parent = null;
        }
        else
        {
            child.transform.parent = parent.transform;
        }
    }

    private Transform FindGameObjectChildTransform(GameObject root, string gameObjectName)
    {
        Transform childTransform = null;

        if (root != null)
        {
            childTransform = root.transform.Find(gameObjectName);
        }
        else
        {
            GameObject[] rootGameObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (GameObject currentGameObject in rootGameObjects)
            {
                if (currentGameObject.name == gameObjectName)
                {
                    childTransform = currentGameObject.transform;
                    break;
                }
            }
        }

        return childTransform;
    }

    private GameObject GetOrCreateNamedGameObject(string gameObjectName, GameObject gameObjectParent)
    {
        Transform childTransform = null;

        childTransform = FindGameObjectChildTransform(gameObjectParent, gameObjectName);

        if (childTransform)
        {
            return childTransform.gameObject;
        }
        else
        {
            GameObject emptyGameObject = new GameObject(gameObjectName);
            SetGameObjectParent(emptyGameObject, gameObjectParent);

			// Zero out all relative transforms
            emptyGameObject.transform.localPosition = new Vector3();
            emptyGameObject.transform.localRotation = new Quaternion();
            emptyGameObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

            return emptyGameObject;
        }
    }

    private Vector4 ParseComponent(XElement componentItem)
    {
        Vector4 component = new Vector4();

        foreach (XElement item in componentItem.Elements())
        {
            switch (item.Name.ToString())
            {
                case "x":
                    component.x = float.Parse(item.Value);
                    break;
                case "y":
                    component.y = float.Parse(item.Value);
                    break;
                case "z":
                    component.z = float.Parse(item.Value);
                    break;
                case "w":
                    component.w = float.Parse(item.Value);
                    break;
            }
        }

        return component;
    }

    private Vector3 ParsePosition(XElement positionItem)
    {
        Vector4 pos = ParseComponent(positionItem);

        return new Vector3(pos.x, pos.y, pos.z);
    }

    private Quaternion ParseRotation(XElement rotationItem)
    {
        Vector4 rot = ParseComponent(rotationItem);

        return new Quaternion(rot.x, rot.y, rot.z, rot.w);
    }

    private void ParseObjectPhysics(GameObject itemGameObject, XElement bodyItem)
    {
        foreach (XElement propertyItem in bodyItem.Elements())
        {
            switch (propertyItem.Name.ToString())
            {
                case "Position":
                    itemGameObject.transform.localPosition = ParsePosition(propertyItem);
                    break;
                case "Rotation":
                    itemGameObject.transform.localRotation = ParseRotation(propertyItem);
                    break;
                case "AddRange":
                    int addRange = int.Parse(propertyItem.Value);
                    break;
                case "GroundOffset":
                    float groundOffset = float.Parse(propertyItem.Value);
                    break;
                case "IsDynamic":
                    bool isDynamic = bool.Parse(propertyItem.Value);
                    break;
                case "IsReset":
                    bool isReset = bool.Parse(propertyItem.Value);
                    break;
                case "SetObjectID":
                    int setObjectID = int.Parse(propertyItem.Value);
                    break;
                // Dunno how to interpret these, so just make them child GameObjects for now
                case "Type":
                    string type = propertyItem.Value;
                    GetOrCreateNamedGameObject(type, itemGameObject);
                    break;
                //
            }
        }
    }

    private void ParseObjSoundPoint(GameObject itemGameObject, XElement bodyItem)
    {
        foreach (XElement propertyItem in bodyItem.Elements())
        {
            switch (propertyItem.Name.ToString())
            {
                case "Position":
                    itemGameObject.transform.localPosition = ParsePosition(propertyItem);
                    break;
                case "Rotation":
                    itemGameObject.transform.localRotation = ParseRotation(propertyItem);
                    break;
                // Dunno how to interpret these, so just make them child GameObjects for now
                case "CueName":
                    string cueName = propertyItem.Value;
                    GameObject cueNameGameObject = GetOrCreateNamedGameObject(cueName, itemGameObject);
                    break;
                case "FileName":
                    string fileName = propertyItem.Value;
                    GetOrCreateNamedGameObject(fileName, itemGameObject);
                    break;
                //
                case "BaseVolume":
                    float baseVolume = float.Parse(propertyItem.Value);
                    break;
                case "InsideRadius":
                    float insideRadius = float.Parse(propertyItem.Value);
                    break;
                case "Radius":
                    float radius = float.Parse(propertyItem.Value);
                    break;
                case "IntervalTime":
                    float intervalTime = float.Parse(propertyItem.Value);
                    break;
                case "IsRegularly":
                    bool isRegularly = bool.Parse(propertyItem.Value);
                    break;
                case "GroundOffset":
                    float groundOffset = float.Parse(propertyItem.Value);
                    break;
                case "SetObjectID":
                    int setObjectID = int.Parse(propertyItem.Value);
                    break;
            }
        }
    }

    private void ParseStageEffect(GameObject itemGameObject, XElement bodyItem)
    {
        foreach (XElement propertyItem in bodyItem.Elements())
        {
            switch (propertyItem.Name.ToString())
            {
                case "Position":
                    itemGameObject.transform.localPosition = ParsePosition(propertyItem);
                    break;
                case "Rotation":
                    itemGameObject.transform.localRotation = ParseRotation(propertyItem);
                    break;
                // Dunno how to interpret these, so just make them child GameObjects for now
                case "EffectType":
                    string cueName = propertyItem.Value;
                    GetOrCreateNamedGameObject(cueName, itemGameObject);
                    break;
                //
                // Color Scale
                case "ColorScale_A":
                    float colorScale_A = float.Parse(propertyItem.Value);
                    break;
                case "ColorScale_R":
                    float colorScale_R = float.Parse(propertyItem.Value);
                    break;
                case "ColorScale_G":
                    float colorScale_G = float.Parse(propertyItem.Value);
                    break;
                case "ColorScale_B":
                    float colorScale_B = float.Parse(propertyItem.Value);
                    break;
                //
                case "ScaleX":
                    float scaleX = float.Parse(propertyItem.Value);
                    break;
                case "ScaleY":
                    float scaleY = float.Parse(propertyItem.Value);
                    break;
                case "scaleZ":
                    float scaleZ = float.Parse(propertyItem.Value);
                    break;
                case "EditColorFlag":
                    bool editColorFlag = bool.Parse(propertyItem.Value);
                    break;
                case "Loop":
                    bool loop = bool.Parse(propertyItem.Value);
                    break;
                case "VolumeScale":
                    float volumeScale = float.Parse(propertyItem.Value);
                    break;
                case "GroundOffset":
                    float groundOffset = float.Parse(propertyItem.Value);
                    break;
                case "SetObjectID":
                    int setObjectID = int.Parse(propertyItem.Value);
                    break;
            }
        }
    }

    private void ParseRoot(GameObject rootGameObject, XElement rootItem) {
        int objectPhysicsCount = 0;
        int objSoundPointCount = 0;
        int stageEffectCount = 0;
        int otherEntityCount = 0;

        foreach (XElement bodyItem in rootItem.Elements())
        {
            switch (bodyItem.Name.ToString())
            {
                case "ObjectPhysics":
                    GameObject objectPhysics = GetOrCreateNamedGameObject("ObjectPhysics" + "_" + objectPhysicsCount, rootGameObject);
                    ParseObjectPhysics(objectPhysics, bodyItem);
                    objectPhysicsCount++;
                    break;
                case "ObjSoundPoint":
                    GameObject objSoundPoint = GetOrCreateNamedGameObject("ObjSoundPoint" + "_" + objSoundPointCount, rootGameObject);
                    ParseObjSoundPoint(objSoundPoint, bodyItem);
                    objSoundPointCount++;
                    break;
                case "StageEffect":
                    GameObject stageEffect = GetOrCreateNamedGameObject("StageEffect" + "_" + stageEffectCount, rootGameObject);
                    ParseStageEffect(stageEffect, bodyItem);
                    stageEffectCount++;
                    break;
                case "LayerDefine":
                    break;
                case "SetRigidBody":
                    break;
                case "EventCollision":
                    break;
                case "EventCollision2":
                    break;
                case "Hint":
                    break;
                case "SequenceChangeCollision":
                    break;
                default:
                    // Lazy, just parse every other unique object as an ObjectPhysics for now
                    // This mostly seems to cover the NPCs, but we don't really care about them.
                    // There is the EU gate though
                    GameObject otherEntity = GetOrCreateNamedGameObject("OtherEntity" + "_" + otherEntityCount + "_" + bodyItem.Name.ToString(), rootGameObject);
                    ParseObjectPhysics(otherEntity, bodyItem);
                    break;
            }
        }
    }

    private void ParseTextAssets()
    {
        foreach (TextAsset textAsset in textAssets) {
            if (textAsset) {
                XDocument xmlDoc;
                xmlDoc = XDocument.Parse(textAsset.ToString());

                GameObject rootGameObject = GetOrCreateNamedGameObject(textAsset.name, null);

                foreach (XElement rootItem in xmlDoc.Elements()) {
                    if (rootItem.Name.ToString() == "SetObject") {
                        ParseRoot(rootGameObject, rootItem);
                    }
                }
            }
        }
    }

    [UnityEditor.MenuItem("Tools/Sonic Unleased XML Scene Parser/Parse XML File")]
    static void CreateWizard() {
        UnityEditor.ScriptableWizard.DisplayWizard<SonicUnleasedXMLSceneParserScriptableWizard>("Parse XML File", "Apply");
    }

    void OnWizardCreate() {
        ParseTextAssets();
    }

    void OnWizardUpdate() {

    }
};

#endif